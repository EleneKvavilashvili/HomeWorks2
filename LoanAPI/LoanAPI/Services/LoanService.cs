using LoanAPI.Context;
using LoanAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LoanAPI.Services
{
    public class LoanService : ILoanService
    {
        private readonly LoanAPIContext _context;
        private readonly ILogService _logger;

        public LoanService(LoanAPIContext context, ILogService logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<List<Loan>> GetLoans(LoanType? type, LoanStatus? status)
        {
            var query = _context.Loans.AsQueryable();

            if (type.HasValue)
            {
                query = query.Where(l => l.Type == type.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(l => l.Status == status.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<List<Loan>?> GetLoansByUsername(string requestedUsername, string authorizedUsername, bool isAccountant, int? loanId, LoanType? type, LoanStatus? status)
        {
            var requestedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == requestedUsername);
            if (requestedUser == null) 
            {
                await _logger.LogError("User not found.", ErrorType.GetLoan);
                return null;
            }

            if (!isAccountant && requestedUser.Username != authorizedUsername)
            {
                await _logger.LogError("Can only view your loans.", ErrorType.GetLoan);
                return null;
            }

            var query = _context.Loans.Where(l => l.UserId == requestedUser.Id).AsQueryable();

            if (loanId.HasValue)
            {
                query = query.Where(l => l.Id == loanId.Value);
            }

            if (type.HasValue)
            {
                query = query.Where(l => l.Type == type.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(l => l.Status == status.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> CreateLoan(Loan loan, string authorizedUsername)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == authorizedUsername);
            if (user == null)
            {
                await _logger.LogError("User not found.", ErrorType.CreateLoan);
                return false;
            }

            if (user.IsBlocked)
            {
                await _logger.LogError("User blocked.", ErrorType.CreateLoan);
                return false;
            }

            loan.UserId = user.Id;
            loan.Status = LoanStatus.InProcess;

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            await _logger.LogAction(ActionType.LoanCreated, user.Id, loan.Id);
            return true;
        }

        public async Task<bool> UpdateLoan(int requestedId, Loan updatedLoan, string authorizedUsername, bool isAccountant)
        {
            var oldLoan = await _context.Loans.FindAsync(requestedId);
            if (oldLoan == null)
            {
                await _logger.LogError("Loan not found.", ErrorType.UpdateLoan);
                return false;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == authorizedUsername);

            if (user == null)
            {
                await _logger.LogError("User not found.", ErrorType.UpdateLoan);
                return false;
            }

            if (!isAccountant && oldLoan.UserId != user.Id)
            {
                await _logger.LogError("Can't access other's loans.", ErrorType.UpdateLoan);
                return false;
            }

            if (oldLoan.Status != LoanStatus.InProcess)
            {
                await _logger.LogError("Cannot update loan that's been processed.", ErrorType.UpdateLoan);
                return false;
            }

            oldLoan.Amount = updatedLoan.Amount;
            oldLoan.Currency = updatedLoan.Currency;
            oldLoan.PeriodMonths = updatedLoan.PeriodMonths;
            oldLoan.Type = updatedLoan.Type;

            await _context.SaveChangesAsync();
            await _logger.LogAction(ActionType.LoanUpdated, user.Id, oldLoan.Id);
            return true;
        }

        public async Task<bool> DeleteLoan(int requestedId, string authorizedUsername, bool isAccountant)
        {
            var loan = await _context.Loans.FindAsync(requestedId);
            if (loan == null)
            {
                await _logger.LogError("Loan not found.", ErrorType.DeleteLoan);
                return false;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == authorizedUsername);

            if (user == null)
            {
                await _logger.LogError("User not found.", ErrorType.DeleteLoan);
                return false;
            }

            if (!isAccountant && loan.UserId != user.Id)
            {
                await _logger.LogError("Can't access other's loans.", ErrorType.DeleteLoan);
                return false;
            }

            if (loan.Status != LoanStatus.InProcess)
            {
                await _logger.LogError("Cannot delete loan that's been processed.", ErrorType.DeleteLoan);
                return false;
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            await _logger.LogAction(ActionType.LoanDeleted, user.Id, requestedId);
            return true;
        }
    }
}
