using Microsoft.Extensions.Caching.Memory;
using Checkout.PaymentGateway.Domain.Entities;
using Checkout.PaymentGateway.Domain.Exceptions;

namespace Checkout.PaymentGateway.Infrastructure.Repositories
{
    public class TransactionRepository(IMemoryCache cache) : ITransactionRepository
    {
        /// <summary>
        /// Retrieves a transaction by its ID.
        /// </summary>
        /// <param name="id">The ID of the transaction.</param>
        /// <returns>The transaction if found; otherwise, null.</returns>
        public ValueTask<Payment?> GetTransactionAsync(Guid id)
        {
            bool paymentFound = cache.TryGetValue<Payment>(id, out var payment);
            return new ValueTask<Payment?>(paymentFound ? payment : null);
        }

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        /// <param name="payment">The transaction to create.</param>
        /// <returns>The created transaction.</returns>
        /// <exception cref="DuplicateTransactionException">Thrown if a transaction with the same ID already exists.</exception>
        public ValueTask<Payment> CreateTransactionAsync(Payment payment)
        {
            if (cache.TryGetValue<Payment>(payment.PaymentId, out _))
                throw new DuplicateTransactionException($"Duplicate transaction with id {payment.PaymentId}");

            cache.Set(payment.PaymentId, payment);
            return new ValueTask<Payment>(payment);
        }

        /// <summary>
        /// Updates an existing transaction.
        /// </summary>
        /// <param name="payment">The transaction to update.</param>
        /// <returns>The updated transaction.</returns>
        /// <exception cref="ArgumentException">Thrown if no transaction with the given ID is found.</exception>
        public ValueTask<Payment> UpdateTransactionAsync(Payment payment)
        {
            if (!cache.TryGetValue<Payment>(payment.PaymentId, out _))
                throw new ArgumentException($"No payment found with id {payment.PaymentId}");

            cache.Set(payment.PaymentId, payment);
            return new ValueTask<Payment>(payment);
        }

    }
}
