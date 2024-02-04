using MediatR;

namespace Checkout.PaymentGateway.Application.Interfaces
{
    public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult>;
}
