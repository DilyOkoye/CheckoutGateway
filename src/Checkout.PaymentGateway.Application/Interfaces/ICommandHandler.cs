using Checkout.PaymentGateway.Application.Common.Commands;

namespace Checkout.PaymentGateway.Application.Interfaces
{
    public interface ICommandHandler<in T> where T : CommandBase
    {
        Task<Result> HandleAsync(T command);
    }
}
