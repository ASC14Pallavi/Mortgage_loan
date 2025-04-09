using MortgageLoanProcessing.Model;

namespace MortgageLoanProcessing.Repositories
{
    public interface IInterestRateRepository
    {
        
    Task<IEnumerable<InterestRate>> GetAllInterestRatesAsync();
    Task<InterestRate> GetInterestRateByIdAsync(int id);
    }
}

