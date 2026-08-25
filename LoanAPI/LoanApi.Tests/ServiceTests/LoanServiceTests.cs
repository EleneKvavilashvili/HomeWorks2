using LoanAPI.Context;
using LoanAPI.Models;
using LoanAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LoanAPI.Tests.ServiceTests
{
    public class LoanServiceTests
    {
        private readonly Mock<ILogService> _logServiceMock;

        public LoanServiceTests()
        {
            _logServiceMock = new Mock<ILogService>();
        }

        private LoanAPIContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<LoanAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new LoanAPIContext(options);
        }

        [Fact]
        public async Task CreateLoan_WithNonExistentUser_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            var service = new LoanService(context, _logServiceMock.Object);
            var newLoan = new Loan { Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan };

            var result = await service.CreateLoan(newLoan, "nonexistentuser");

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.CreateLoan), Times.Once);
        }

        [Fact]
        public async Task CreateLoan_WhenUserIsBlocked_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "blockedUser", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash", IsBlocked = true });
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);
            var loan = new Loan { Amount = 1000, Currency = LoanCurrency.USD, PeriodMonths = 12 };

            var result = await service.CreateLoan(loan, "blockedUser");

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.CreateLoan), Times.Once);
        }

        [Fact]
        public async Task CreateLoan_WithValidUser_ReturnsTrue()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "activeUser", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash", IsBlocked = false });
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);
            var loan = new Loan { Amount = 1000, Currency = LoanCurrency.USD, PeriodMonths = 12 };

            var result = await service.CreateLoan(loan, "activeUser");

            Assert.True(result);
            Assert.Equal(LoanStatus.InProcess, loan.Status);
            Assert.Equal(1, loan.UserId);
            _logServiceMock.Verify(x => x.LogAction(ActionType.LoanCreated, 1, loan.Id), Times.Once);
        }
        
        [Fact]
        public async Task UpdateLoan_WithNonExistentLoan_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            var service = new LoanService(context, _logServiceMock.Object);
            var updateData = new Loan { Amount = 2000, PeriodMonths = 24, Type = LoanType.AutoLoan, Currency = LoanCurrency.GEL };

            var result = await service.UpdateLoan(999, updateData, "john", isAccountant: false);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.UpdateLoan), Times.Once);
        }

        [Fact]
        public async Task UpdateLoan_WithNonExistentUser_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            context.Loans.Add(new Loan { Id = 1, UserId = 10, Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan, Currency = LoanCurrency.USD, Status = LoanStatus.InProcess });
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);
            var updateData = new Loan { Amount = 2000, PeriodMonths = 24, Type = LoanType.AutoLoan, Currency = LoanCurrency.GEL };

            var result = await service.UpdateLoan(1, updateData, "ghost", isAccountant: false);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.UpdateLoan), Times.Once);
        }

        [Fact]
        public async Task UpdateLoan_WhenUnauthorizedUser_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" });
            context.Users.Add(new User { Id = 2, Username = "hacker", FirstName = "Hacker", LastName = "Man", Email = "hacker@example.com", PasswordHash = "hash" });
            context.Loans.Add(new Loan { Id = 1, UserId = 1, Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan, Currency = LoanCurrency.USD, Status = LoanStatus.InProcess });
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);
            var updateData = new Loan { Amount = 2000, PeriodMonths = 24, Type = LoanType.AutoLoan, Currency = LoanCurrency.USD };

            var result = await service.UpdateLoan(1, updateData, "hacker", isAccountant: false);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.UpdateLoan), Times.Once);
        }

        [Fact]
        public async Task UpdateLoan_WhenLoanAlreadyProcessed_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" });
            context.Loans.Add(new Loan { Id = 1, UserId = 1, Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan, Currency = LoanCurrency.GEL, Status = LoanStatus.Approved });
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);
            var updateData = new Loan { Amount = 2000, PeriodMonths = 24, Type = LoanType.AutoLoan, Currency = LoanCurrency.GEL };

            var result = await service.UpdateLoan(1, updateData, "john", isAccountant: false);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.UpdateLoan), Times.Once);
        }

        [Fact]
        public async Task UpdateLoan_ValidData_ReturnsTrue()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" });
            context.Loans.Add(new Loan { Id = 1, UserId = 1, Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan, Currency = LoanCurrency.USD, Status = LoanStatus.InProcess });
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);
            var updateData = new Loan { Amount = 5000, PeriodMonths = 36, Type = LoanType.AutoLoan, Currency = LoanCurrency.EUR };

            var result = await service.UpdateLoan(1, updateData, "john", isAccountant: false);

            var dbLoan = await context.Loans.FindAsync(1);
            Assert.True(result);
            Assert.NotNull(dbLoan);
            Assert.Equal(5000, dbLoan.Amount);
            Assert.Equal(36, dbLoan.PeriodMonths);
            Assert.Equal(LoanType.AutoLoan, dbLoan.Type);
            Assert.Equal(LoanCurrency.EUR, dbLoan.Currency);
            _logServiceMock.Verify(x => x.LogAction(ActionType.LoanUpdated, 1, 1), Times.Once);
        }

        [Fact]
        public async Task UpdateLoan_AsAccountant_ReturnsTrue()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" });
            context.Users.Add(new User { Id = 2, Username = "admin", FirstName = "Admin", LastName = "User", Email = "admin@example.com", PasswordHash = "hash" });
            context.Loans.Add(new Loan { Id = 1, UserId = 1, Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan, Currency = LoanCurrency.USD, Status = LoanStatus.InProcess });
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);
            var updateData = new Loan { Amount = 3000, PeriodMonths = 18, Type = LoanType.AutoLoan, Currency = LoanCurrency.USD };

            var result = await service.UpdateLoan(1, updateData, "admin", isAccountant: true);

            var dbLoan = await context.Loans.FindAsync(1);
            Assert.True(result);
            Assert.Equal(3000, dbLoan!.Amount);
            _logServiceMock.Verify(x => x.LogAction(ActionType.LoanUpdated, 2, 1), Times.Once);
        }

        [Fact]
        public async Task DeleteLoan_WithNonExistentLoan_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            var service = new LoanService(context, _logServiceMock.Object);

            var result = await service.DeleteLoan(999, "john", isAccountant: false);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.DeleteLoan), Times.Once);
        }

        [Fact]
        public async Task DeleteLoan_WithNonExistentUser_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            context.Loans.Add(new Loan { Id = 1, UserId = 10, Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan, Currency = LoanCurrency.USD, Status = LoanStatus.InProcess });
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);

            var result = await service.DeleteLoan(1, "ghost", isAccountant: false);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.DeleteLoan), Times.Once);
        }

        [Fact]
        public async Task DeleteLoan_WhenUnauthorizedUser_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" });
            context.Users.Add(new User { Id = 2, Username = "hacker", FirstName = "Hacker", LastName = "Man", Email = "hacker@example.com", PasswordHash = "hash" });
            context.Loans.Add(new Loan { Id = 1, UserId = 1, Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan, Currency = LoanCurrency.USD, Status = LoanStatus.InProcess });
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);

            var result = await service.DeleteLoan(1, "hacker", isAccountant: false);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.DeleteLoan), Times.Once);
        }

        [Fact]
        public async Task DeleteLoan_WhenLoanAlreadyProcessed_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" });
            context.Loans.Add(new Loan { Id = 1, UserId = 1, Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan, Currency = LoanCurrency.USD, Status = LoanStatus.Approved });
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);

            var result = await service.DeleteLoan(1, "john", isAccountant: false);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.DeleteLoan), Times.Once);
        }

        [Fact]
        public async Task DeleteLoan_ValidData_ReturnsTrue()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" });
            context.Loans.Add(new Loan { Id = 1, UserId = 1, Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan, Currency = LoanCurrency.USD, Status = LoanStatus.InProcess });
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);

            var result = await service.DeleteLoan(1, "john", isAccountant: false);

            var dbLoan = await context.Loans.FindAsync(1);
            Assert.True(result);
            Assert.Null(dbLoan);
            _logServiceMock.Verify(x => x.LogAction(ActionType.LoanDeleted, 1, 1), Times.Once);
        }

        [Fact]
        public async Task DeleteLoan_AsAccountant_ReturnsTrue()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" });
            context.Users.Add(new User { Id = 2, Username = "admin", FirstName = "Admin", LastName = "User", Email = "admin@example.com", PasswordHash = "hash" });
            context.Loans.Add(new Loan { Id = 1, UserId = 1, Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan, Currency = LoanCurrency.USD, Status = LoanStatus.InProcess });
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);

            var result = await service.DeleteLoan(1, "admin", isAccountant: true);

            var dbLoan = await context.Loans.FindAsync(1);
            Assert.True(result);
            Assert.Null(dbLoan);
            _logServiceMock.Verify(x => x.LogAction(ActionType.LoanDeleted, 2, 1), Times.Once);
        }

        [Fact]
        public async Task GetLoansByUsername_WithNonExistentUser_ReturnsNull()
        {
            using var context = GetInMemoryDbContext();
            var service = new LoanService(context, _logServiceMock.Object);

            var result = await service.GetLoansByUsername("nonexistentuser", "john", isAccountant: false, null, null, null);

            Assert.Null(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.GetLoan), Times.Once);
        }

        [Fact]
        public async Task GetLoansByUsername_WhenUnauthorizedUser_ReturnsNull()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" });
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);

            var result = await service.GetLoansByUsername("john", "hacker", isAccountant: false, null, null, null);

            Assert.Null(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.GetLoan), Times.Once);
        }

        [Fact]
        public async Task GetLoansByUsername_OwnProfile_ReturnsSuccessfully()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" });
            context.Loans.AddRange(
                new Loan { Id = 10, UserId = 1, Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan, Currency = LoanCurrency.USD, Status = LoanStatus.InProcess },
                new Loan { Id = 11, UserId = 1, Amount = 5000, PeriodMonths = 24, Type = LoanType.AutoLoan, Currency = LoanCurrency.USD, Status = LoanStatus.Approved },
                new Loan { Id = 12, UserId = 2, Amount = 3000, PeriodMonths = 12, Type = LoanType.QuickLoan, Currency = LoanCurrency.USD, Status = LoanStatus.InProcess }
            );
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);

            var result = await service.GetLoansByUsername("john", "john", isAccountant: false, null, null, null);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, l => Assert.Equal(1, l.UserId));
        }

        [Fact]
        public async Task GetLoansByUsername_AsAccountant_ReturnsSuccessfully()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" });
            context.Loans.Add(new Loan { Id = 10, UserId = 1, Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan, Currency = LoanCurrency.USD, Status = LoanStatus.InProcess });
            await context.SaveChangesAsync();

            var service = new LoanService(context, _logServiceMock.Object);

            var result = await service.GetLoansByUsername("john", "admin", isAccountant: true, null, null, null);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(10, result[0].Id);
        }
    }
}