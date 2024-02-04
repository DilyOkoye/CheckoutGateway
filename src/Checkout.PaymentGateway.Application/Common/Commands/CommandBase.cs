using MediatR;

namespace Checkout.PaymentGateway.Application.Common.Commands
{
    public interface CommandBase : IRequest
    {

    }

    public interface CommandBase<out TResult> : IRequest<TResult>
    {
    }
}
