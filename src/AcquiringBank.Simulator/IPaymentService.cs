using AcquiringBank.Simulator.Models;

namespace AcquiringBank.Simulator
{
    public interface IPaymentService
    {
        PaymentResponse ProcessPayment(PaymentRequest request);
    }
}
