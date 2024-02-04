namespace Checkout.PaymentGateway.Domain.Exceptions
{
    public class PaymentValidationException : Exception
    {
        public PaymentValidationException()
        {
        }

        public PaymentValidationException(string error) : base(error)
        {

        }

        public PaymentValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
