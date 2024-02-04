using Checkout.PaymentGateway.Domain.Entities;

namespace Checkout.PaymentGateway.Infrastructure.Repositories
{
    public interface ITransactionRepository
    {
        ValueTask<Payment?> GetTransactionAsync(Guid id);

        ValueTask<Payment> CreateTransactionAsync(Payment payment);

        ValueTask<Payment> UpdateTransactionAsync(Payment payment);

    }
}
