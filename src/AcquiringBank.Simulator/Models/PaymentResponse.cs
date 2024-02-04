namespace AcquiringBank.Simulator.Models
{
    public class PaymentResponse
    {
        public Guid PaymentReference { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public string TransactionMessage { get; set; } = string.Empty;
    }
}
