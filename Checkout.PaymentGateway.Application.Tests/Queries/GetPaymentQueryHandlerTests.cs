using Checkout.PaymentGateway.Application.Queries;
using Checkout.PaymentGateway.Domain.Entities;
using Checkout.PaymentGateway.Infrastructure.Repositories;
using Moq;

namespace Checkout.PaymentGateway.Application.Tests.Queries;

[TestFixture]
public class GetPaymentQueryHandlerTests
{
    private Mock<ITransactionRepository> _mockTransactionRepository = new Mock<ITransactionRepository>();
    private GetPaymentQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _handler = new GetPaymentQueryHandler(_mockTransactionRepository.Object);
    }

    [Test]
    public async Task Handle_WhenTransactionFound_ReturnsGetPaymentResponse()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var transaction = new Payment
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

        _mockTransactionRepository.Setup(repo => repo.GetTransactionAsync(It.IsAny<Guid>()))
            .ReturnsAsync(transaction);

        var query = new GetPaymentQuery(paymentId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Card.Number, Is.EqualTo("**** **** **** 5100"));
            Assert.That(result?.TransactionStatus, Is.EqualTo(transaction.TransactionStatus.ToString()));
            Assert.That(result?.Amount, Is.EqualTo(transaction.Amount * 100)); 
        });
    }

    [Test]
    public async Task Handle_WhenTransactionNotFound_ReturnsNull()
    {
        // Arrange
        _mockTransactionRepository.Setup(repo => repo.GetTransactionAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Payment)null);

        var query = new GetPaymentQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }
}