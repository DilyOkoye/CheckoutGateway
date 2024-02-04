using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Checkout.PaymentGateway.Application.Interfaces;
using Checkout.PaymentGateway.Domain.Entities;
using Checkout.PaymentGateway.Domain.Responses;

namespace Checkout.PaymentGateway.Application.Commands
{
    public class CreatePaymentCommand : ICommand<CreatePaymentResponse>
    {
        [JsonIgnore] public Guid IdempotencyKeyId { get; set; }

        [JsonIgnore]
        public Guid PaymentId { get; set; } = Guid.NewGuid();
        public Merchant? Merchant { get; set; }
        public Card? Card { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Currency is required.")]
        public string Currency { get; set; } = string.Empty;
    }
}
