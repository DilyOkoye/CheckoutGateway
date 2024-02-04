using Checkout.PaymentGateway.Domain.Enums;

namespace Checkout.PaymentGateway.Domain.Entities
{
    public class Payment
    {
        public Guid PaymentId { get; set; }
        public Merchant Merchant { get; set; }
        public Card Card { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;

        public TransactionStatus TransactionStatus { get; set; }
    }
}
