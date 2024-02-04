namespace Checkout.PaymentGateway.Domain.Responses
{
    public class CreatePaymentResponse
    {
        public Guid PaymentReference { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
    }
}
