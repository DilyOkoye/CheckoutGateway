using Checkout.PaymentGateway.Application.Commands;
using Checkout.PaymentGateway.Application.Validators;
using Checkout.PaymentGateway.Domain.Exceptions;
using Checkout.PaymentGateway.Domain.Entities;

namespace Checkout.PaymentGateway.Application.Tests.Validators
{
    [TestFixture]
    public class ValidatePaymentCommandTests
    {
        private CreatePaymentCommand CreateValidCommand()
        {
            return new CreatePaymentCommand
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
        }

        [TestCase("1234567890123456", "Card number is not valid")]
        [TestCase("5105105105105100", "Amount is not valid", -10)]
        [TestCase("5105105105105100", "Amount is not valid", 100000000)]
        [TestCase("5105105105105100", "Currency is not valid", 10, "XYZ")]
        [TestCase("5105105105105100", "Expiration Date is not valid", 10, "GBP", "01/2000")]
        [TestCase("5105105105105100", "CVV is not valid", 10, "GBP", "01/2027", "22")]
        public void ValidatePayment_WithInvalidInput_ShouldThrowPaymentValidationException(
            string cardNumber,
            string expectedMessage,
            int amount = 10,
            string currency = "GBP",
            string expirationDate = "01/2027",
            string cvv = "322")
        {
            // Arrange
            var command = CreateValidCommand();
            command.Card!.Number = cardNumber;
            command.Amount = amount;
            command.Currency = currency;
            command.Card.ExpirationDate = expirationDate;
            command.Card.Cvv = cvv;

            // Act & Assert
            var ex = Assert.Throws<PaymentValidationException>(() => ValidatePaymentCommand.ValidatePayment(command));
            Assert.That(ex!.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void ValidatePayment_WithValidInput_ShouldReturnModifiedCommand()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = ValidatePaymentCommand.ValidatePayment(command);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Amount, Is.EqualTo(command.Amount / 100));
            });
        }
    }
}
