using Homework21.Context;
using Homework21.Models;
using Homework21.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Homework21.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IUserService _userService;
        public UserController(UserContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var updatedList = await _context.Users.Include(p => p.UserAddress).ToListAsync();
            return Ok(updatedList);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Users.Include(p => p.UserAddress).ToListAsync();
            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users.Include(p => p.UserAddress).FirstOrDefaultAsync(p => p.Id == id);
            if (user == null)
                return BadRequest("Can't find requested index.");

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpGet("filter")]
        public async Task<IActionResult> Search([FromQuery] double salary)
        {
            var query = _context.Users.Include(p => p.UserAddress).Where(p => p.Salary > salary);
            var list = await query.ToListAsync();

            return Ok(list);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.Include(p => p.UserAddress).FirstOrDefaultAsync(p => p.Id == id);
            if (user == null)
                return BadRequest("Can't find requested index.");

            _context.Users.Remove(user);
            if (user.UserAddress != null)
            {
                _context.Addresses.Remove(user.UserAddress);
            }
            await _context.SaveChangesAsync();

            var updatedList = await _context.Users.Include(p => p.UserAddress).ToListAsync();
            return Ok(updatedList);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User updateduser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existinguser = await _context.Users.Include(p => p.UserAddress).FirstOrDefaultAsync(p => p.Id == id);
            if (existinguser == null)
                return BadRequest("Can't find requested index.");

            existinguser.CreateDate = updateduser.CreateDate;
            existinguser.Firstname = updateduser.Firstname;
            existinguser.Lastname = updateduser.Lastname;
            existinguser.JobPosition = updateduser.JobPosition;
            existinguser.Salary = updateduser.Salary;
            existinguser.WorkExperience = updateduser.WorkExperience;

            if (existinguser.UserAddress != null && updateduser.UserAddress != null)
            {
                existinguser.UserAddress.Country = updateduser.UserAddress.Country;
                existinguser.UserAddress.City = updateduser.UserAddress.City;
                existinguser.UserAddress.HomeNumber = updateduser.UserAddress.HomeNumber;
            }

            await _context.SaveChangesAsync();

            var updatedList = await _context.Users.Include(p => p.UserAddress).ToListAsync();
            return Ok(updatedList);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            var user = await _userService.LoginAsync(model);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect." });
            var token = _userService.GenerateJwtToken(user);
            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                Token = token
            });
        }
    }
}
