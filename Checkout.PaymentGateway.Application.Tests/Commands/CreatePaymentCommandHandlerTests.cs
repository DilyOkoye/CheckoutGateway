using Checkout.PaymentGateway.Application.Clients;
using Checkout.PaymentGateway.Application.Clients.Response;
using Checkout.PaymentGateway.Application.Commands;
using Checkout.PaymentGateway.Domain.Entities;
using Checkout.PaymentGateway.Domain.Enums;
using Checkout.PaymentGateway.Domain.Responses;
using Checkout.PaymentGateway.Infrastructure.Repositories;
using Moq;
using Microsoft.Extensions.Logging;
using Checkout.PaymentGateway.Application.Clients.Request;
using Checkout.PaymentGateway.Domain.Exceptions;

namespace Checkout.PaymentGateway.Application.Tests.Commands
{
    [TestFixture]
    public class CreatePaymentCommandHandlerTests
    {
        private Mock<ITransactionRepository> _mockTransactionRepository;
        private Mock<IIdempotencyKeyRepository> _mockIdempotencyKeyRepository;
        private Mock<IAcquirerClient> _mockAcquirerClient;
        private Mock<ILogger<CreatePaymentCommandHandler>> _mockLogger;
        private CreatePaymentCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockIdempotencyKeyRepository = new Mock<IIdempotencyKeyRepository>();
            _mockAcquirerClient = new Mock<IAcquirerClient>();
            _mockLogger = new Mock<ILogger<CreatePaymentCommandHandler>>();

            _handler = new CreatePaymentCommandHandler(
                _mockTransactionRepository.Object,
                _mockIdempotencyKeyRepository.Object,
                _mockAcquirerClient.Object,
                _mockLogger.Object);
        }

        [Test]
        public async Task Handle_WhenPaymentIsNew_ShouldProcessPayment()
        {
            // Arrange
            var request = new CreatePaymentCommand
            {
                Currency = "GBP",
                Card = new Card()
                {
                    ExpirationDate = "01/2027",
                    Cvv = "322",
                    HolderName = "Dili Okoye",
                    Number = "5105105105105100"
                },
                Amount = 10,
                IdempotencyKeyId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
                Merchant = new Merchant()
                {
                    Id = Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae7")
                }
            };
            var payment = new Payment
            {
                Currency = "GBP",
                Card = new Card()
                {
                    ExpirationDate = "01/2027",
                    Cvv = "322",
                    HolderName = "Dili Okoye",
                    Number = "5105105105105100"
                },
                Amount = 10,
                Merchant = new Merchant()
                {
                    Id = Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae7")
                }
            };
            var acquirerResponse = new ClientResponse { TransactionStatus = TransactionStatus.Approved };

            _mockIdempotencyKeyRepository
                .Setup(x => x.GetIdempotencyKeyAsync(It.IsAny<Guid>()))
                .ReturnsAsync((IdempotencyKey)null);

            _mockTransactionRepository
                .Setup(x => x.CreateTransactionAsync(It.IsAny<Payment>()))
                .ReturnsAsync(payment);

            _mockAcquirerClient
                .Setup(x => x.AcquirerPayment(It.IsAny<ClientRequest>()))
                .ReturnsAsync(acquirerResponse);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.TransactionCode, Is.EqualTo(TransactionStatus.Approved.ToString()));
        }

        [Test]
        public async Task Handle_WhenPaymentExists_ShouldReturnExistingPayment()
        {
            // Arrange
            var idempotencyKeyId = Guid.NewGuid();
            var existingPaymentResponse = new CreatePaymentResponse { PaymentReference = Guid.NewGuid(), TransactionCode = TransactionStatus.Approved.ToString() };

            _mockIdempotencyKeyRepository
                .Setup(x => x.GetIdempotencyKeyAsync(idempotencyKeyId))
                .ReturnsAsync(new IdempotencyKey(Guid.Parse("5c9e6679-7425-40de-944b-e07fc1f90ae7"), Guid.Parse("2c9e6679-7425-40de-944b-e07fc1f90ae7")));

            var request = new CreatePaymentCommand { IdempotencyKeyId = idempotencyKeyId };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.TransactionCode, Is.EqualTo(existingPaymentResponse.TransactionCode));
            _mockTransactionRepository.Verify(x => x.CreateTransactionAsync(It.IsAny<Payment>()), Times.Never);
        }


        [Test]
        public void Handle_WhenPaymentValidationFails_ShouldThrowPaymentValidationException()
        {
            // Arrange
            var request = new CreatePaymentCommand
            {
                Currency = "GBP",
                Card = new Card()
                {
                    ExpirationDate = "01/2027",
                    Cvv = "322",
                    HolderName = "Dili Okoye",
                    Number = "510510510510"
                },
                Amount = 10,
                IdempotencyKeyId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
                Merchant = new Merchant()
                {
                    Id = Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae7")
                }
            };

            var acquirerResponse = new ClientResponse { TransactionStatus = TransactionStatus.Approved };

            _mockAcquirerClient
                .Setup(x => x.AcquirerPayment(It.IsAny<ClientRequest>()))
                .ReturnsAsync(acquirerResponse);

            var ex = Assert.ThrowsAsync<PaymentValidationException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.That(ex!.Message, Is.Not.Null);
            _mockTransactionRepository.Verify(x => x.CreateTransactionAsync(It.IsAny<Payment>()), Times.Never, "A transaction should not be created for an invalid payment");
        }


        [Test]
        public async Task Handle_WhenAcquirerClientFails_ShouldReturnFailedTransaction()
        {
            // Arrange
            var request = new CreatePaymentCommand
            {
                Currency = "GBP",
                Card = new Card()
                {
                    ExpirationDate = "01/2027",
                    Cvv = "322",
                    HolderName = "Dili Okoye",
                    Number = "5105105105105100"
                },
                Amount = 10,
                IdempotencyKeyId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
                Merchant = new Merchant()
                {
                    Id = Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae7")
                }
            };
            var payment = new Payment
            {
                Currency = "GBP",
                Card = new Card()
                {
                    ExpirationDate = "01/2027",
                    Cvv = "322",
                    HolderName = "Dili Okoye",
                    Number = "5105105105105100"
                },
                Amount = 10,
                Merchant = new Merchant()
                {
                    Id = Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae7")
                }
            };
            var acquirerResponse = new ClientResponse { TransactionStatus = TransactionStatus.Declined };

            _mockIdempotencyKeyRepository
                .Setup(x => x.GetIdempotencyKeyAsync(It.IsAny<Guid>()))
                .ReturnsAsync((IdempotencyKey)null);

            _mockTransactionRepository
                .Setup(x => x.CreateTransactionAsync(It.IsAny<Payment>()))
                .ReturnsAsync(payment);

            _mockAcquirerClient
                .Setup(x => x.AcquirerPayment(It.IsAny<ClientRequest>()))
                .ReturnsAsync(acquirerResponse);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result.TransactionCode, Is.EqualTo(TransactionStatus.Declined.ToString()), "The transaction should be marked as declined");
        }

    }
}