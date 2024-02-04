using Checkout.PaymentGateway.Application.Clients.Request;
using Checkout.PaymentGateway.Application.Commands;
using Checkout.PaymentGateway.Domain.Entities;
using Checkout.PaymentGateway.Domain.Enums;

namespace Checkout.PaymentGateway.Application.Mapper
{
    public static class PaymentMapper
    {
        public static Payment CommandToEntity(CreatePaymentCommand createPaymentCommand)
        {
            return new Payment()
            {
                PaymentId = createPaymentCommand.PaymentId,
                Card = createPaymentCommand.Card!,
                Amount = createPaymentCommand.Amount,
                Currency = createPaymentCommand.Currency,
                Merchant = createPaymentCommand.Merchant!,
                TransactionStatus = TransactionStatus.Processing
            };
        }

        public static ClientRequest PaymentToAcquirerRequest(this Payment payment)
        {
            return payment == null
                ? throw new ArgumentNullException(nameof(payment))
                : new ClientRequest
            {
                Amount = payment.Amount,
                CardNumber = payment.Card.Number,
                Currency = payment.Currency,
                Cvv = payment.Card.Cvv,
                ExpirationDate = payment.Card.ExpirationDate,
                HolderName = payment.Card.HolderName
            };
        }

        public static IdempotencyKey MapToIdempotencyKey(Guid idempotencyKey, Guid paymentId) => new(idempotencyKey, paymentId);
    }
}
