using System.ComponentModel.DataAnnotations;

namespace Checkout.PaymentGateway.Domain.Entities
{
    public class IdempotencyKey
    {
        public IdempotencyKey(Guid id, Guid paymentId)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("id can't be empty");

            if (paymentId == Guid.Empty)
                throw new ArgumentException("Payment id can't be empty");

            Id = id;
            PaymentId = paymentId;
        }

        [Key]
        public Guid Id { get; private set; }

        public Guid PaymentId { get; private set; }
    }
}
