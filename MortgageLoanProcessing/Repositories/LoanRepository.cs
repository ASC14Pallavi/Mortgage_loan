using Microsoft.EntityFrameworkCore;
using MortgageLoanProcessing.Data;
using MortgageLoanProcessing.Model;

namespace MortgageLoanProcessing.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly ApplicationDbContext _context;

        public LoanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Loan>> GetAllLoansAsync()
        {
            return await _context.Loans.ToListAsync();
        }

        public async Task<Loan> GetLoanByIdAsync(int id)
        {
            return await _context.Loans.FindAsync(id);
        }

        public async Task AddLoanAsync(Loan loan)
        {
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLoanAsync(Loan loan)
        {
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLoanAsync(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan != null)
            {
                _context.Loans.Remove(loan);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Loan>> GetLoansByUserIdAsync(string userId)
        {
            return await _context.Loans.Where(l => l.UserId == userId).ToListAsync();
        }

    }
}
