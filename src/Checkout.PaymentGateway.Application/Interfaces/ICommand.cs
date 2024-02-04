using MediatR;

namespace Checkout.PaymentGateway.Application.Interfaces
{
    public interface ICommand : IRequest;

    public interface ICommand<out TResult> : IRequest<TResult>;
}
