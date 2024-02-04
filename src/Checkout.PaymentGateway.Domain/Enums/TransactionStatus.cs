using System.ComponentModel;

namespace Checkout.PaymentGateway.Domain.Enums
{
    public enum TransactionStatus
    {
        [Description("Approved Transaction")]
        Approved,

        [Description("Transaction Declined")]
        Declined,

        [Description("Processing Error")]
        ProcessingError,

        [Description("Suspected fraud")]
        SuspectedFraud,

        [Description("Internal Error while processing")]
        InternalError,

        [Description("Transaction processing")]
        Processing,
    }
}
