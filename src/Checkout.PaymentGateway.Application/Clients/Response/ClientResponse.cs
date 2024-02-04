using Checkout.PaymentGateway.Domain.Enums;

namespace Checkout.PaymentGateway.Application.Clients.Response
{
    public class ClientResponse
    {
        public Guid PaymentReference { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public string TransactionMessage { get; set; } = string.Empty;
    }
}
