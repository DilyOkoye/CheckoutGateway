using Checkout.PaymentGateway.Application.Clients.Request;
using Checkout.PaymentGateway.Application.Clients.Response;

namespace Checkout.PaymentGateway.Application.Clients
{
    public interface IAcquirerClient
    {
        Task<ClientResponse?> AcquirerPayment(ClientRequest clientRequest);
    }
}
