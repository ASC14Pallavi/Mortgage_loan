
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageLoanProcessing.Model
{
    public class Loan
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public decimal LoanAmount { get; set; }
        public int LoanTermYears { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int InterestRateId { get; set; }
        public ICollection<AmortizationSchedule> AmortizationSchedules { get; set; }
    }
}