using Checkout.PaymentGateway.Application.Interfaces;
using Checkout.PaymentGateway.Domain.Responses;

namespace Checkout.PaymentGateway.Application.Queries
{
    public class GetPaymentQuery(Guid id) : IQuery<GetPaymentResponse?>
    {
        public Guid Id { get; set; } = id;
    }
}
