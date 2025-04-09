namespace MortgageLoanProcessing.Model
{
    public class InterestRate
    {
        public int Id { get; set; }
        public decimal Rate { get; set; }
        public DateTime ValidFrom { get; set; }
    }
}
