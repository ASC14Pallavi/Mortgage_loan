using MortgageLoanProcessing.Model;

namespace MortgageLoanProcessing.Repositories
{
    public interface IAmortizationRepository
    {
        Task<IEnumerable<AmortizationSchedule>> GetScheduleByLoanIdAsync(int loanId, int pageNumber, int pageSize);
        Task<int> GetTotalCountByLoanIdAsync(int loanId);
        Task SaveAmortizationScheduleAsync(List<AmortizationSchedule> schedule);

    }
}
