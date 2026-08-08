using Homework21.Context;
using Homework21.Models;
using Humanizer.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace Homework21.Services
{
    public interface IUserService
    {
        Task<User>? LoginAsync(UserLoginModel model);
        Task<User>? GetByIdAsync(int id);
        string GenerateJwtToken(User user);
    }
    public class UserService : IUserService
    {
        private readonly UserContext _context;
        private readonly IConfiguration _configuration;

        public UserService(UserContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User>? GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User>? LoginAsync(UserLoginModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                return null;

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == model.Username && x.Password == model.Password);

            return user;
        }
        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("SuperSecretKeyThatIsAtLeast32BytesLong12345!");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username ?? ""),
            new Claim(ClaimTypes.Role, "Admin")
        }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
