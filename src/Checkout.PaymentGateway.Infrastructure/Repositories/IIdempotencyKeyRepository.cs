using Checkout.PaymentGateway.Domain.Entities;

namespace Checkout.PaymentGateway.Infrastructure.Repositories
{
    public interface IIdempotencyKeyRepository
    {
        ValueTask<IdempotencyKey?> GetIdempotencyKeyAsync(Guid idempotencyKey);

        ValueTask<IdempotencyKey> CreateTransactionAsync(IdempotencyKey idempotencyKey);
    }
}
