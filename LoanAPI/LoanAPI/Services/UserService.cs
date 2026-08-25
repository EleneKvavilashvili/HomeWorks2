
using LoanAPI.Context;
using LoanAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoanAPI.Services
{
    public class UserService : IUserService
    {
        private readonly LoanAPIContext _context;
        private readonly IConfiguration _config;
        private readonly ILogService _logger;

        public UserService(LoanAPIContext db, IConfiguration config, ILogService logger)
        {
            _context = db;
            _config = config;
            _logger = logger;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            string secretKey = _config["AppSettings:Secret"]!;
            var key = Encoding.UTF8.GetBytes(secretKey);
            string role = user.IsAccountant ? "Accountant" : "User";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username ?? ""),
            new Claim(ClaimTypes.Role, role)
        }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> Register(User user, string plainPassword)
        {
            if (await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username) != null)
            {
                _logger.LogError("Username already exists.", ErrorType.Register);
                return false;
            }

            if (string.IsNullOrWhiteSpace(plainPassword))
            {
                _logger.LogError("Invalid password", ErrorType.Register);
                return false;
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            user.PasswordHash = hashedPassword;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogAction(ActionType.UserRegistered, user.Id);
            return true;
        }

        public async Task<string> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                _logger.LogError("Couldn't find user.", ErrorType.LogIn);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogError("Invalid password.", ErrorType.LogIn);
                return null;
            }

            string token = GenerateJwtToken(user);
            _logger.LogAction(ActionType.UserLoggedIn, user.Id);
            return token;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByUsername(string requestedUsername, string authorizedUsername, bool isAccountant)
        {
            var requestedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == requestedUsername);
            if (requestedUser == null)
            {
                await _logger.LogError("User not found.", ErrorType.GetUserById);
                return null;
            }
            if (!isAccountant && requestedUser.Username != authorizedUsername)
            {
                await _logger.LogError("Can only view your info.", ErrorType.GetUserById);
                return null;
            }
            return requestedUser;
        }

        public async Task<bool> UpdateUser(string requestedUsername, User updatedUser, string authorizedUsername, bool isAccountant)
        {
            var requestedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == requestedUsername);
            if (requestedUser == null)
            {
                _logger.LogError("Couldn't find user.", ErrorType.UpdateUser);
                return false;
            }
            if (!isAccountant && requestedUser.Username != authorizedUsername)
            {
                await _logger.LogError("Can only update your info.", ErrorType.UpdateUser);
                return false;
            }
            if (await _context.Users.FirstOrDefaultAsync(u => u.Username == updatedUser.Username) != null && updatedUser.Username != requestedUser.Username)
            {
                _logger.LogError("Username already exists.", ErrorType.UpdateUser);
                return false;
            }
            requestedUser.FirstName = updatedUser.FirstName;
            requestedUser.LastName = updatedUser.LastName;
            requestedUser.Username = updatedUser.Username;
            requestedUser.Age = updatedUser.Age;
            requestedUser.Email = updatedUser.Email;
            requestedUser.MonthlyIncome = updatedUser.MonthlyIncome;
            requestedUser.IsAccountant = updatedUser.IsAccountant;

            await _context.SaveChangesAsync();
            _logger.LogAction(ActionType.UserUpdated, requestedUser.Id);
            return true;
        }

        public async Task<bool> UpdatePassword(string authorizedUsername, string oldPassword, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == authorizedUsername);

            if (user == null)
            {
                await _logger.LogError("User not found.", ErrorType.UpdatePassword);
                return false;
            }


            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            {
                _logger.LogError("Incorrect old password. Can only change your passcode.", ErrorType.UpdatePassword);
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _context.SaveChangesAsync();
            _logger.LogAction(ActionType.PasswordUpdated, user.Id);
            return true;
        }

        public async Task<bool> DeleteUser(string requestedUsername, string authorizedUsername, bool isAccountant)
        {
            var requestedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == requestedUsername);
            if (requestedUser == null)
            {
                _logger.LogError("Couldn't find user.", ErrorType.DeleteUser);
                return false;
            }
            if (!isAccountant && requestedUser.Username != authorizedUsername)
            {
                await _logger.LogError("Can only delete your profile.", ErrorType.DeleteUser);
                return false;
            }


            var hasActiveLoan = await _context.Loans.AnyAsync(l => l.UserId == requestedUser.Id && l.Status != LoanStatus.Rejected);

            if (hasActiveLoan)
            {
                await _logger.LogError("Cannot delete user with active loans.", ErrorType.DeleteUser);
                return false;
            }

            var userLoans = _context.Loans.Where(l => l.UserId == requestedUser.Id);
            _context.Loans.RemoveRange(userLoans);

            _context.Users.Remove(requestedUser);
            await _context.SaveChangesAsync();
            _logger.LogAction(ActionType.UserDeleted, requestedUser.Id);
            return true;
        }

        public async Task<bool> BlockUser(string requestedUsername, bool isBlocked)
        {
            var requestedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == requestedUsername);
            if (requestedUser == null)
            {
                _logger.LogError("Couldn't find user.", ErrorType.BlockUser);
                return false;
            }
            if (requestedUser.IsAccountant)
            {
                _logger.LogError("Cannot block an accountant.", ErrorType.BlockUser);
                return false;
            }

            requestedUser.IsBlocked = isBlocked;
            await _context.SaveChangesAsync();
            _logger.LogAction(ActionType.UserBlockedStatusChanged, requestedUser.Id);
            return true;
        }
    }
}
