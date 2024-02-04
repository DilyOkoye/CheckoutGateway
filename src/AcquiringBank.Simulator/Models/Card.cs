namespace AcquiringBank.Simulator.Models
{
    public class Card
    {
        public string Number { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public int Cvv { get; set; } 
        public string HolderName { get; set; } = string.Empty;
    }
}
