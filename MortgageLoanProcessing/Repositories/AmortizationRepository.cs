using Microsoft.EntityFrameworkCore;
using MortgageLoanProcessing.Data;
using MortgageLoanProcessing.Model;

namespace MortgageLoanProcessing.Repositories
{
    public class AmortizationRepository : IAmortizationRepository
    {
        private readonly ApplicationDbContext _context;

        public AmortizationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AmortizationSchedule>> GetScheduleByLoanIdAsync(int loanId, int pageNumber, int pageSize)
        {
            return await _context.AmortizationSchedules
                                 .Where(a => a.LoanId == loanId)
                                 .OrderBy(a => a.PaymentNumber)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }

        public async Task<int> GetTotalCountByLoanIdAsync(int loanId)
        {
            return await _context.AmortizationSchedules.Where(a => a.LoanId == loanId).CountAsync();
        }

        public async Task SaveAmortizationScheduleAsync(List<AmortizationSchedule> schedule)
        {
            if (schedule == null || !schedule.Any())
            {
                throw new ArgumentException("Schedule cannot be null or empty.");
            }

            _context.AmortizationSchedules.AddRange(schedule);
            await _context.SaveChangesAsync();
        }

    }
}
