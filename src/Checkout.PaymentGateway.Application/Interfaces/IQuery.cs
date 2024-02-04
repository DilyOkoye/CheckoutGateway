using MediatR;

namespace Checkout.PaymentGateway.Application.Interfaces
{
    public interface IQuery<out TResult> : IRequest<TResult>;
}
