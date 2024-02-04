using System.Threading.RateLimiting;

namespace Checkout.PaymentGateway.Api.RateLimits
{
    public class RateLimitOptions
    {
        public int Window { get; set; }
        public int PermitLimit { get; set; }
        public int QueueLimit { get; set; }
        public QueueProcessingOrder QueueProcessingOrder { get; set; } = QueueProcessingOrder.OldestFirst;
    }
}
