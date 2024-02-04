using System.ComponentModel;

namespace AcquiringBank.Simulator.Models
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
    }
}
