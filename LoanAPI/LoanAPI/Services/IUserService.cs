using LoanAPI.Models;

namespace LoanAPI.Services
{
    public interface IUserService
    {
        Task<bool> Register(User user, string plainPassword);
        Task<string> Login(string username, string password);
        Task<List<User>> GetAllUsers();
        Task<User?> GetUserByUsername(string requestedUSername, string authorizedUsername, bool isAccountant);
        Task<bool> UpdateUser(string requestedUsername, User updatedUser, string authorizedUsername, bool isAccountant);
        Task<bool> UpdatePassword(string username, string oldPassword, string newPassword);
        Task<bool> DeleteUser(string requestedUsername, string authorizedUsername, bool isAccountant);
        Task<bool> BlockUser(string requestedUsername, bool isBlocked);
    }
}
