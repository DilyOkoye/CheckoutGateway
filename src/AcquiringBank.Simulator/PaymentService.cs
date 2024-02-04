using AcquiringBank.Simulator.Helpers;
using AcquiringBank.Simulator.Models;

namespace AcquiringBank.Simulator
{
    public class PaymentService : IPaymentService
    {
        private readonly Random _random = new();

        public PaymentResponse ProcessPayment(PaymentRequest request)
        {
            if (CheckBlacklistedCard(request.CardNumber))
                return new PaymentResponse()
                {
                    TransactionMessage = "Suspected Fraud",
                    TransactionStatus = TransactionStatus.SuspectedFraud,
                    PaymentReference = Guid.Empty
                };
            
            var number = _random.Next(0, 1);
            return number switch
            {
                0 => new PaymentResponse
                {
                    PaymentReference = Guid.NewGuid(),
                    TransactionStatus = TransactionStatus.Approved,
                    TransactionMessage = "Transaction successful"
                },
                1 => new PaymentResponse
                {
                    TransactionStatus = TransactionStatus.Declined,
                    TransactionMessage =
                        $"Transaction failed"
                },
                _ => throw new InvalidOperationException("Unexpected failure")
            };
        }

        public bool CheckBlacklistedCard(string card)
        {
            return (BlacklistedCards.ExemptedCards.Contains(card));
        }
    }
}
