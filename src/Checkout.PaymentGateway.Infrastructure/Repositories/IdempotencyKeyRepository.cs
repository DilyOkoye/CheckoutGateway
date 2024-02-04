using Checkout.PaymentGateway.Domain.Entities;
using Checkout.PaymentGateway.Domain.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace Checkout.PaymentGateway.Infrastructure.Repositories
{
    public class IdempotencyKeyRepository : IIdempotencyKeyRepository
    {
        private readonly IMemoryCache _cache;

        public IdempotencyKeyRepository(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Retrieves a transaction by its ID.
        /// </summary>
        /// <param name="idempotencyKey">The ID of the transaction.</param>
        /// <returns>The transaction if found; otherwise, null.</returns>
        public ValueTask<IdempotencyKey?> GetIdempotencyKeyAsync(Guid idempotencyKey)
        {
            bool idempotencyKeyFound = _cache.TryGetValue<IdempotencyKey>(idempotencyKey, out var idempotency);
            return new ValueTask<IdempotencyKey?>(idempotencyKeyFound ? idempotency : null);
        }


        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        /// <param name="idempotencyKey">The transaction to create.</param>
        /// <returns>The created transaction.</returns>
        /// <exception cref="DuplicateTransactionException">Thrown if a transaction with the same ID already exists.</exception>
        public ValueTask<IdempotencyKey> CreateTransactionAsync(IdempotencyKey idempotencyKey)
        {
            _cache.Set(idempotencyKey.Id, idempotencyKey);
            return new ValueTask<IdempotencyKey>(idempotencyKey);
        }
    }
}
