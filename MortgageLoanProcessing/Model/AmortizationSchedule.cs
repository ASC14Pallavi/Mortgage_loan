using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageLoanProcessing.Model
{
   
        public class AmortizationSchedule
        {
            public int Id { get; set; }
            public int LoanId { get; set; }
            [ForeignKey("LoanId")]
            public Loan Loan { get; set; }
        public int PaymentNumber { get; set; }
            public DateTime PaymentDate { get; set; }
            public decimal PrincipalPayment { get; set; }
            public decimal InterestPayment { get; set; }
            public decimal RemainingBalance { get; set; }
        }
    }
