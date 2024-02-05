using Checkout.PaymentGateway.Application.Commands;
using Checkout.PaymentGateway.Application.Mapper;
using Checkout.PaymentGateway.Domain.Entities;
using Checkout.PaymentGateway.Domain.Enums;

namespace Checkout.PaymentGateway.Application.Tests.Mapper;

[TestFixture]
public class PaymentMapperTests
{
    [Test]
    public void CommandToEntity_ShouldCorrectlyMapFields()
    {
        // Arrange
        var command = new CreatePaymentCommand
        {
            Currency = "GBP",
            Card = new Card
            {
                ExpirationDate = "01/2027",
                Cvv = "322",
                HolderName = "Dili Okoye",
                Number = "5105105105105100"
            },
            Amount = 10,
            Merchant = new Merchant
            {
                Id = Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae7")
            }
        };

        // Act
        var result = PaymentMapper.CommandToEntity(command);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.PaymentId, Is.EqualTo(command.PaymentId));
            Assert.That(result.Card, Is.EqualTo(command.Card));
            Assert.That(result.Amount, Is.EqualTo(command.Amount));
            Assert.That(result.Currency, Is.EqualTo(command.Currency));
            Assert.That(result.Merchant, Is.EqualTo(command.Merchant));
            Assert.That(result.TransactionStatus, Is.EqualTo(TransactionStatus.Processing));
        });
    }

    [Test]
    public void PaymentToAcquirerRequest_ShouldCorrectlyMapFields()
    {
        // Arrange
        var payment = new Payment
        {
            Currency = "GBP",
            Card = new Card
            {
                ExpirationDate = "01/2027",
                Cvv = "322",
                HolderName = "Dili Okoye",
                Number = "5105105105105100"
            },
            Amount = 10,
            Merchant = new Merchant
            {
                Id = Guid.Parse("1c9e6679-7425-40de-944b-e07fc1f90ae7")
            }
        };

        // Act
        var result = payment.PaymentToAcquirerRequest();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Amount, Is.EqualTo(payment.Amount));
            Assert.That(result.CardNumber, Is.EqualTo(payment.Card.Number));
            Assert.That(result.Currency, Is.EqualTo(payment.Currency));
            Assert.That(result.Cvv, Is.EqualTo(payment.Card.Cvv));
            Assert.That(result.ExpirationDate, Is.EqualTo(payment.Card.ExpirationDate));
            Assert.That(result.HolderName, Is.EqualTo(payment.Card.HolderName));
        });
    }

    [Test]
    public void MapToIdempotencyKey_ShouldReturnCorrectIdempotencyKey()
    {
        // Arrange
        var idempotencyKeyId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();

        // Act
        var result = PaymentMapper.MapToIdempotencyKey(idempotencyKeyId, paymentId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(idempotencyKeyId));
            Assert.That(result.PaymentId, Is.EqualTo(paymentId));
        });
    }
}