namespace MortgageLoanProcessing.DTOs
{
    public class LoanDto
    {
        public int Id { get; set; }
        public decimal LoanAmount { get; set; }
        public int LoanTermYears { get; set; }
        public decimal InterestRate { get; set; } 
        public int InterestRateId { get; set; } 
        public DateTime ApplicationDate { get; set; }
        //public string UserId { get; set; }
    }
}
