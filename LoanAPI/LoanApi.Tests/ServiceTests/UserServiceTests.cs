using LoanAPI.Context;
using LoanAPI.Models;
using LoanAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using Xunit;

namespace LoanAPI.Tests.ServiceTests
{
    public class UserServiceTests
    {
        private readonly Mock<ILogService> _logServiceMock;
        private readonly Mock<IConfiguration> _configMock;

        public UserServiceTests()
        {
            _logServiceMock = new Mock<ILogService>();
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(c => c["AppSettings:Secret"]).Returns("SuperSecretKeyThatIsAtLeast32BytesLong!");
        }

        private LoanAPIContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<LoanAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new LoanAPIContext(options);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsSuccessfully()
        {
            using var context = GetInMemoryDbContext();
            context.Users.AddRange(
                new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash1" },
                new User { Id = 2, Username = "jane", FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", PasswordHash = "hash2" },
                new User { Id = 3, Username = "adam", FirstName = "Adam", LastName = "Smith", Email = "adam@example.com", PasswordHash = "hash3" }
            );
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.GetAllUsers();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, u => u.Username == "john");
            Assert.Contains(result, u => u.Username == "jane");
            Assert.Contains(result, u => u.Username == "adam");
        }

        [Fact]
        public async Task Register_WithDuplicateUsername_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "existinguser", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" });
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);
            var newUser = new User { Username = "existinguser" };


            var result = await service.Register(newUser, "Password123!");

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.Register), Times.Once);
        }

        [Fact]
        public async Task Register_WithEmptyPassword_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);
            var newUser = new User { Username = "newuser" };

            var result = await service.Register(newUser, " ");
            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.Register), Times.Once);
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsTrue()
        {
            using var context = GetInMemoryDbContext();
            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);
            var newUser = new User { Id = 1, Username = "newuser", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" };
            
            var result = await service.Register(newUser, "Password123!");

            Assert.True(result);
            Assert.Single(context.Users);
            Assert.False(context.Users.Single().IsBlocked);
            _logServiceMock.Verify(x => x.LogAction(ActionType.UserRegistered, newUser.Id, null), Times.Once);
        }

        [Fact]
        public async Task Login_WithNonExistentUser_ReturnsNull()
        {
            using var context = GetInMemoryDbContext();
            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var token = await service.Login("nonexistent", "Password123!");

            Assert.Null(token);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.LogIn), Times.Once);
        }

        [Fact]
        public async Task Login_WithIncorrectPassword_ReturnsNull()
        {
            using var context = GetInMemoryDbContext();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = hashedPassword });
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var token = await service.Login("john", "WrongPassword");

            Assert.Null(token);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.LogIn), Times.Once);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsJwtToken()
        {
            using var context = GetInMemoryDbContext();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
            var user = new User { Id = 10, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = hashedPassword, IsAccountant = false };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var token = await service.Login("john", "Password123!");

            Assert.NotNull(token);
            Assert.NotEmpty(token);
            _logServiceMock.Verify(x => x.LogAction(ActionType.UserLoggedIn, user.Id, null), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_WithNonExistentUser_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.DeleteUser("nonexistentuser", "requesterUser", isAccountant: false);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.DeleteUser), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_WithActiveLoan_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            var user = new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "new@example.com", PasswordHash = "hash" };
            context.Users.Add(user);
            context.Loans.Add(new Loan { Id = 1, UserId = 1, Status = LoanStatus.InProcess, Type=LoanType.QuickLoan, Amount=1000, Currency=LoanCurrency.GEL, PeriodMonths=5 });
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.DeleteUser("john", "john", false);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.DeleteUser), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_WhenUnauthorizedUser_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "new@example.com", PasswordHash = "hash" });
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.DeleteUser("john", "otherUser", isAccountant: false);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.DeleteUser), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_OwnProfile_ReturnsTrue()
        {
            using var context = GetInMemoryDbContext();
            var user = new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "new@example.com", PasswordHash = "hash" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.DeleteUser("john", "john", isAccountant: false);

            Assert.True(result);
            Assert.Equal(0, await context.Users.CountAsync());
            _logServiceMock.Verify(x => x.LogAction(ActionType.UserDeleted, 1, null), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_AsAccountant_ReturnsTrue()
        {
            using var context = GetInMemoryDbContext();
            var user = new User { Id = 2, Username = "requestedUser", FirstName = "John", LastName = "Doe", Email = "new@example.com", PasswordHash = "hash" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.DeleteUser("requestedUser", "accountantAdmin", isAccountant: true);

            Assert.True(result);
            Assert.Null(await context.Users.FindAsync(2));
            _logServiceMock.Verify(x => x.LogAction(ActionType.UserDeleted, 2, null), Times.Once);
        }

        [Fact]
        public async Task BlockUser_WithNonExistentUser_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.BlockUser("nonexistenUser", isBlocked: true);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.BlockUser), Times.Once);
        }

        [Fact]
        public async Task BlockUser_SetToTrue_ReturnsTrue()
        {
            using var context = GetInMemoryDbContext();
            var user = new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash", IsBlocked = false };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.BlockUser("john", isBlocked: true);

            var dbUser = await context.Users.FindAsync(1);
            Assert.True(result);
            Assert.NotNull(dbUser);
            Assert.True(dbUser.IsBlocked);
            _logServiceMock.Verify(x => x.LogAction(ActionType.UserBlockedStatusChanged, 1, null), Times.Once);
        }

        [Fact]
        public async Task GetUserByUsername_WithNonExistentUser_ReturnsNull()
        {
            using var context = GetInMemoryDbContext();
            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.GetUserByUsername("nonexistentUser", "john", isAccountant: false);

            Assert.Null(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.GetUserById), Times.Once);
        }

        [Fact]
        public async Task GetUserByUsername_WhenUnauthorizedUser_ReturnsNull()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" });
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.GetUserByUsername("john", "otherUser", isAccountant: false);

            Assert.Null(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.GetUserById), Times.Once);
        }

        [Fact]
        public async Task GetUserByUsername_OwnProfile_ReturnsUserSuccessfully()
        {
            using var context = GetInMemoryDbContext();
            var user = new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.GetUserByUsername("john", "john", isAccountant: false);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("john", result.Username);
        }

        [Fact]
        public async Task GetUserByUsername_AsAccountant_ReturnsOtherUserSuccessfully()
        {
            using var context = GetInMemoryDbContext();
            var user = new User { Id = 2, Username = "jane", FirstName = "jane", LastName = "Doe", Email = "jane@example.com", PasswordHash = "hash" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.GetUserByUsername("jane", "adminAccountant", isAccountant: true);

            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("jane", result.Username);
        }

        [Fact]
        public async Task UpdateUser_WithNonExistentUser_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);
            var updatedData = new User { Id = 999, Username = "nonexistent" };

            var result = await service.UpdateUser("nonexistent", updatedData, "nonexistent", isAccountant: false);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.UpdateUser), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WhenUnauthorizedUser_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            context.Users.Add(new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "old@example.com", PasswordHash = "hash" });
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);
            var updatedData = new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "new@example.com", PasswordHash = "hash" };

            var result = await service.UpdateUser("john", updatedData, "hacker", isAccountant: false);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.UpdateUser), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WithDuplicateUsername_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();

            context.Users.AddRange(
                new User { Id = 1, Username = "currentUser", FirstName = "John1", LastName = "Doe1", Email = "user1@example.com", PasswordHash = "hash" },
                new User { Id = 2, Username = "takenUsername", FirstName = "John2", LastName = "Doe2", Email = "user2@example.com", PasswordHash = "hash" }
            );
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);
            var updatedData = new User { Id = 1, Username = "takenUsername", Email = "user1@example.com" };

            var result = await service.UpdateUser("currentUser", updatedData, "currentUser", isAccountant: false);

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.UpdateUser), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_OwnProfile_ReturnsTrue()
        {
            using var context = GetInMemoryDbContext();
            var user = new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "old@example.com", PasswordHash = "hash", MonthlyIncome = 5000 }; 
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);
            var updatedData = new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "new@example.com", PasswordHash = "hash", MonthlyIncome = 6000 };

            var result = await service.UpdateUser("john", updatedData, "john", isAccountant: false);

            var dbUser = await context.Users.FindAsync(1);
            Assert.True(result);
            Assert.NotNull(dbUser);
            Assert.Equal("new@example.com", dbUser.Email);
            Assert.Equal(6000, dbUser.MonthlyIncome);
            _logServiceMock.Verify(x => x.LogAction(ActionType.UserUpdated, 1, null), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_AsAccountant_ReturnsTrue()
        {
            using var context = GetInMemoryDbContext();
            var user = new User { Id = 2, Username = "jane", FirstName = "jane", LastName = "Doe", Email = "jane@example.com", PasswordHash = "hash", MonthlyIncome = 4000 };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);
            var updatedData = new User { Id = 2, Username = "jane", FirstName = "jane", LastName = "Doe", Email = "jane@example.com", PasswordHash = "hash", MonthlyIncome = 4500 };

            var result = await service.UpdateUser("jane", updatedData, "adminAccountant", isAccountant: true);

            var dbUser = await context.Users.FindAsync(2);
            Assert.True(result);
            Assert.NotNull(dbUser);
            Assert.Equal(4500, dbUser.MonthlyIncome);
            _logServiceMock.Verify(x => x.LogAction(ActionType.UserUpdated, 2, null), Times.Once);
        }

        [Fact]
        public async Task UpdatePassword_WithNonExistentUser_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.UpdatePassword("nonexistentuser", "OldPass123!", "NewPass123!");

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.UpdatePassword), Times.Once);
        }

        [Fact]
        public async Task UpdatePassword_WithIncorrectOldPassword_ReturnsFalse()
        {
            using var context = GetInMemoryDbContext();
            var oldPasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectOldPassword");
            var user = new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "new@example.com", PasswordHash = oldPasswordHash };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.UpdatePassword("john", "WrongOldPassword", "NewSecretPassword123!");

            Assert.False(result);
            _logServiceMock.Verify(x => x.LogError(It.IsAny<string>(), ErrorType.UpdatePassword), Times.Once);
        }

        [Fact]
        public async Task UpdatePassword_WithCorrectOldPassword_ReturnsTrue()
        {
            using var context = GetInMemoryDbContext();
            var oldPasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectOldPassword");
            var user = new User { Id = 1, Username = "john", FirstName = "John", LastName = "Doe", Email = "new@example.com", PasswordHash = oldPasswordHash };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context, _configMock.Object, _logServiceMock.Object);

            var result = await service.UpdatePassword("john", "CorrectOldPassword", "NewSecretPassword123!");

            var dbUser = await context.Users.FindAsync(1);
            Assert.True(result);
            Assert.NotNull(dbUser);
            Assert.True(BCrypt.Net.BCrypt.Verify("NewSecretPassword123!", dbUser.PasswordHash));
            _logServiceMock.Verify(x => x.LogAction(ActionType.PasswordUpdated, 1, null), Times.Once);
        }
    }
}