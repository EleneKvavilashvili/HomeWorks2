using LoanAPI.Models;

namespace LoanAPI.Services
{
    public interface ILogService
    {
        Task LogAction(ActionType action, int? userId = null, int? loanId = null);
        Task LogError(string error, ErrorType errorType);
    }
}
