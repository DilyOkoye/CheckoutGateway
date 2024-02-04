using System.ComponentModel.DataAnnotations;

namespace Checkout.PaymentGateway.Domain.Entities
{
    public class Card
    {
        [Required(ErrorMessage = "Card number is required.")]
        public string Number { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expiration Date is required.")]
        public string ExpirationDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cvv is required.")]
        public string Cvv { get; set; }

        [Required(ErrorMessage = "Holder Name is required.")]
        public string HolderName { get; set; } = string.Empty;
    }
}
