﻿namespace AcquiringBank.Simulator.Models
{
    public class PaymentRequest
    {
        public string CardNumber { get; set; } = string.Empty;

        public string ExpirationDate { get; set; } = string.Empty;

        public string Cvv { get; set; } = string.Empty;

        public string HolderName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = string.Empty;
    }
}
