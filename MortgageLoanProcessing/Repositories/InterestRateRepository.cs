using Microsoft.EntityFrameworkCore;
using MortgageLoanProcessing.Data;
using MortgageLoanProcessing.Model;

namespace MortgageLoanProcessing.Repositories
{
    public class InterestRateRepository : IInterestRateRepository
    {
        private readonly ApplicationDbContext _context;

        public InterestRateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InterestRate>> GetAllInterestRatesAsync()
        {
            return await _context.InterestRates.ToListAsync();
        }

        public async Task<InterestRate> GetInterestRateByIdAsync(int id)
        {
            return await _context.InterestRates.FindAsync(id);
        }
    }
}
