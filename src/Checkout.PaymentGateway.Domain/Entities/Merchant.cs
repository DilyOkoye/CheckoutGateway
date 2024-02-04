using System.ComponentModel.DataAnnotations;

namespace Checkout.PaymentGateway.Domain.Entities
{
    public class Merchant
    {
        [Required(ErrorMessage = "Merchant Id is required.")]
        public Guid Id { get; set; }
    }
}
