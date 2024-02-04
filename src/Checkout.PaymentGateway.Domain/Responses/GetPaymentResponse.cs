using Checkout.PaymentGateway.Domain.Entities;

namespace Checkout.PaymentGateway.Domain.Responses
{
    public class GetPaymentResponse
    {
        public Guid PaymentId { get; set; }
        public Merchant Merchant { get; set; }
        public Card Card { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;

        public string TransactionStatus { get; set; } = string.Empty;
    }
}
