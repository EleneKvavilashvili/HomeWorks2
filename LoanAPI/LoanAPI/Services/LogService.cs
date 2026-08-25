using LoanAPI.Context;
using LoanAPI.Models;

namespace LoanAPI.Services
{
    public class LogService : ILogService
    {
        private readonly LoanAPIContext _context;

        public LogService(LoanAPIContext context)
        {
            _context = context;
        }
        public async Task LogAction(ActionType action, int? userId = null, int? loanId = null)
        {
            _context.ActionLogs.Add(new ActionLog
            {
                ActionName = action,
                UserId = userId,
                LoanId = loanId,
                Timestamp = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }

        public async Task LogError(string error, ErrorType errorType)
        {
            _context.ErrorLogs.Add(new ErrorLog
            {
                Message = error,
                Type = errorType,
                Timestamp = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }
    }
}
