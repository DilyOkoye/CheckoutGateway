namespace Checkout.PaymentGateway.Domain.Exceptions
{
    public class CreatePaymentCommandException : Exception
    {
        public CreatePaymentCommandException()
        {
        }

        public CreatePaymentCommandException(string error) : base(error)
        {

        }

        public CreatePaymentCommandException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
