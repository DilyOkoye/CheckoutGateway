using Checkout.PaymentGateway.Application.Commands;
using Checkout.PaymentGateway.Domain.Entities;
using Checkout.PaymentGateway.Domain.Exceptions;

namespace Checkout.PaymentGateway.Application.Validators
{
    public static class ValidatePaymentCommand
    {
        public static CreatePaymentCommand ValidatePayment(CreatePaymentCommand createPaymentCommand)
        {

            if (!IsCardNumberValid(createPaymentCommand.Card?.Number!))
                throw new PaymentValidationException("Card number is not valid");

            if (!IsValidAmount(createPaymentCommand.Amount))
                throw new PaymentValidationException("Amount is not valid");

            var currency = Currencies.SupportedCurrencies.Contains(createPaymentCommand.Currency, StringComparer.OrdinalIgnoreCase);
            if (!currency)
                throw new PaymentValidationException("Currency is not valid");

            var expireAt = ParseExpiryDate(createPaymentCommand.Card?.ExpirationDate!);
            if (!expireAt)
                throw new PaymentValidationException("Expiration Date is not valid");

            if (!IsCvvNumberValid(createPaymentCommand.Card?.Cvv!))
                throw new PaymentValidationException("CVV is not valid");

            var amountValue = createPaymentCommand.Amount / 100;

            return new CreatePaymentCommand()
            {
                Card = createPaymentCommand.Card,
                Amount = amountValue,
                Currency = createPaymentCommand.Currency,
                Merchant = createPaymentCommand.Merchant,
                IdempotencyKeyId = createPaymentCommand.IdempotencyKeyId
            };
        }

        public static bool IsValidAmount(decimal amount)
        {
            const decimal maxAmount = 1000000.00M;

            return amount is >= 0 and <= maxAmount;
        }

        public static bool IsCvvNumberValid(string cvv)
        {
            if (string.IsNullOrWhiteSpace(cvv) || (cvv.Length != 3 && cvv.Length != 4))
            {
                return false;
            }
            return int.TryParse(cvv, out _);
        }

        public static bool IsCardNumberValid(string cardNumber)
        {
            var sum = 0;
            var alternate = false;
            for (var i = cardNumber.Length - 1; i >= 0; i--)
            {
                var digit = int.Parse(cardNumber[i].ToString());
                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9)
                    {
                        digit -= 9;
                    }
                }
                sum += digit;
                alternate = !alternate;
            }
            return sum % 10 == 0;
        }

        public static bool ParseExpiryDate(string expirationDate)
        {
            if (string.IsNullOrWhiteSpace(expirationDate) || expirationDate.Length != 7)
            {
                return false;
            }

            var parts = expirationDate.Split('/');
            if (parts.Length != 2)
            {
                return false;
            }

            if (!int.TryParse(parts[0], out var month) || !int.TryParse(parts[1], out var year))
            {
                return false;
            }

            if (month is < 1 or > 12)
            {
                return false;
            }

            var lastDayOfMonth = DateTime.DaysInMonth(year, month);
            var expiration = new DateTime(year, month, lastDayOfMonth, 23, 59, 59);

            return DateTime.Now <= expiration;
        }
    }
}
