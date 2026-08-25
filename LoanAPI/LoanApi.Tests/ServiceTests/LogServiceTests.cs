using LoanAPI.Context;
using LoanAPI.Models;
using LoanAPI.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LoanAPI.Tests.ServiceTests
{
    public class LogServiceTests
    {
        private LoanAPIContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<LoanAPIContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new LoanAPIContext(options);
        }


        [Fact]
        public async Task LogAction_AddsActionLogToDatabase()
        {
            using var context = GetDbContext();
            var service = new LogService(context);

            await service.LogAction(ActionType.UserRegistered, userId: 1, loanId: null);

            var log = await context.ActionLogs.FirstOrDefaultAsync();
            Assert.NotNull(log);
            Assert.Equal(ActionType.UserRegistered, log.ActionName);
            Assert.Equal(1, log.UserId);
            Assert.Null(log.LoanId);
            Assert.True(log.Timestamp <= DateTime.Now);
        }

        [Fact]
        public async Task LogAction_WithLoanId_AddsActionLogWithLoan()
        {
            using var context = GetDbContext();
            var service = new LogService(context);

            await service.LogAction(ActionType.LoanCreated, userId: 1, loanId: 10);

            var log = await context.ActionLogs.FirstOrDefaultAsync();
            Assert.NotNull(log);
            Assert.Equal(ActionType.LoanCreated, log.ActionName);
            Assert.Equal(1, log.UserId);
            Assert.Equal(10, log.LoanId);
        }


        [Fact]
        public async Task LogError_AddsErrorLogToDatabase()
        {
            using var context = GetDbContext();
            var service = new LogService(context);

            await service.LogError("User not found.", ErrorType.GetUserById);

            var log = await context.ErrorLogs.FirstOrDefaultAsync();
            Assert.NotNull(log);
            Assert.Equal("User not found.", log.Message);
            Assert.Equal(ErrorType.GetUserById, log.Type);
            Assert.True(log.Timestamp <= DateTime.Now);
        }
    }
}
