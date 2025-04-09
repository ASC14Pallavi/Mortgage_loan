using Microsoft.AspNetCore.Identity;

namespace MortgageLoanProcessing.Model
{
    public class User : IdentityUser
    {
        public ICollection<Loan> Loans { get; set; }
    }
}
