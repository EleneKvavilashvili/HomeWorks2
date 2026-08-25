using AutoMapper;
using LoanAPI.DTOs;
using LoanAPI.Models;
using LoanAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var user = _mapper.Map<User>(dto);
            bool result = await _userService.Register(user, dto.Password);
            if (!result)
            {
                return BadRequest("Registration failed. Check logs for details.");
            }
            return Ok("User registered successfully.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string password)
        {
            string? token = await _userService.Login(username, password);
            if (token == null)
            {
                return BadRequest("Invalid credentials." );
            }
            return Ok(new { token });
        }


        [Authorize(Roles = "Accountant")]
        [HttpGet("Accountant")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsers());
        }

        [Authorize]
        [HttpGet("User/{requestedUsername}")]
        public async Task<IActionResult> GetUserByUsername(string requestedUsername)
        {
            string authorizedUsername = User.Identity!.Name!;
            bool isAccountant = User.IsInRole("Accountant");
            var requestedUser = await _userService.GetUserByUsername(requestedUsername, authorizedUsername, isAccountant);
            if (requestedUser == null)
            {
                return BadRequest("Invalid username. Check logs for details.");
            }
            return Ok(requestedUser);
        }


        [Authorize]
        [HttpPut("Update/{requestedUsername}")]
        public async Task<IActionResult> UpdateUser(string requestedUsername, [FromBody] UpdateDTO dto)
        {
            string authorizedUsername = User.Identity!.Name!;
            bool isAccountant = User.IsInRole("Accountant");
            var updatedUser = _mapper.Map<User>(dto);

            bool result = await _userService.UpdateUser(requestedUsername, updatedUser, authorizedUsername, isAccountant);

            if (!result)
            {
                return BadRequest("User update failed. Check logs for details.");
            }

            return Ok("User updated.");
        }


        [Authorize]
        [HttpDelete("Delete/{requestedUsername}")]
        public async Task<IActionResult> DeleteUser(string requestedUsername)
        {
            string authorizedUsername = User.Identity!.Name!;
            bool isAccountant = User.IsInRole("Accountant");

            bool result = await _userService.DeleteUser(requestedUsername, authorizedUsername, isAccountant);
            if (!result)
            {
                return BadRequest("User deletion failed. Check logs for details.");
            }
            return Ok("User deleted.");
        }

        [Authorize(Roles = "Accountant")]
        [HttpPut("block/{requestedUsername}")]
        public async Task<IActionResult> BlockUser(string requestedUsername, [FromBody] bool isBlocked)
        {
            bool result = await _userService.BlockUser(requestedUsername, isBlocked);
            if (!result)
            {
                return BadRequest("Block status update failed. Couldn't find user.");
            }
            return Ok("User blocked.");
        }


        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromQuery] string oldPassword, [FromQuery] string newPassword)
        {
            string authorizedUsername = User.Identity!.Name!;
            bool result = await _userService.UpdatePassword(authorizedUsername, oldPassword, newPassword);

            if (!result)
            {
                return BadRequest("Password update failed. Check logs for details.");
            }

            return Ok("Password updated successfully.");
        }

    }
}
