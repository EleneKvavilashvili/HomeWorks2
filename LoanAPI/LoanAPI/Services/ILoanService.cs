using LoanAPI.Models;

namespace LoanAPI.Services
{
    public interface ILoanService
    {
        Task<List<Loan>> GetLoans(LoanType? type, LoanStatus? status);
        Task<List<Loan>?> GetLoansByUsername(string requestedUsername, string authorizedUsername, bool isAccountant, int? loanId, LoanType? type, LoanStatus? status);
        Task<bool> CreateLoan(Loan loan, string authorizedUsername);
        Task<bool> UpdateLoan(int requestedId, Loan updatedLoan, string authorizedUsername, bool isAccountant);
        Task<bool> DeleteLoan(int requestedId, string authorizedUsername, bool isAccountant);
    }
}
