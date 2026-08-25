using AutoMapper;
using LoanAPI.Controllers;
using LoanAPI.DTOs;
using LoanAPI.Models;
using LoanAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace LoanApi.Tests.ControllerTests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new UserController(_userServiceMock.Object, _mapperMock.Object);
        }

        private void SetUserClaims(string username, string role = "User")
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }


        [Fact]
        public async Task Register_Success_ReturnsOk()
        {
            var dto = new RegisterDTO { Username = "john", Password = "pass123" };
            var user = new User { Username = "john" };

            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
            _userServiceMock.Setup(s => s.Register(user, dto.Password)).ReturnsAsync(true);

            var result = await _controller.Register(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully.", okResult.Value);
        }

        [Fact]
        public async Task Register_Failure_ReturnsBadRequest()
        {
            var dto = new RegisterDTO { Username = "john", Password = "pass123" };
            var user = new User { Username = "john" };

            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
            _userServiceMock.Setup(s => s.Register(user, dto.Password)).ReturnsAsync(false);

            var result = await _controller.Register(dto);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Registration failed. Check logs for details.", badResult.Value);
        }


        [Fact]
        public async Task Login_Success_ReturnsOk()
        {
            _userServiceMock.Setup(s => s.Login("john", "pass123")).ReturnsAsync("jwt_token_string");

            var result = await _controller.Login("john", "pass123");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Login_Failure_ReturnsBadRequest()
        {
            _userServiceMock.Setup(s => s.Login("john", "wrongpass")).ReturnsAsync((string?)null);

            var result = await _controller.Login("john", "wrongpass");

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid credentials.", badResult.Value);
        }


        [Fact]
        public async Task GetUserByUsername_Success_ReturnsOk()
        {
            SetUserClaims("john");
            var user = new User { Id = 1, Username = "john" };
            _userServiceMock.Setup(s => s.GetUserByUsername("john", "john", false)).ReturnsAsync(user);

            var result = await _controller.GetUserByUsername("john");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task GetUserByUsername_Failure_ReturnsBadRequest()
        {
            SetUserClaims("john");
            _userServiceMock.Setup(s => s.GetUserByUsername("unknown", "john", false)).ReturnsAsync((User?)null);

            var result = await _controller.GetUserByUsername("unknown");

            var badResult = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("Invalid username. Check logs for details.", badResult.Value);
        }


        [Fact]
        public async Task UpdateUser_Success_ReturnsOk()
        {
            SetUserClaims("john");
            var dto = new UpdateDTO { FirstName = "John", LastName = "Doe" };
            var updatedUser = new User { Username = "john", FirstName = "John", LastName = "Doe" };

            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(updatedUser);
            _userServiceMock.Setup(s => s.UpdateUser("john", updatedUser, "john", false)).ReturnsAsync(true);

            var result = await _controller.UpdateUser("john", dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User updated.", okResult.Value);
        }

        [Fact]
        public async Task UpdateUser_Failure_ReturnsBadRequest()
        {
            SetUserClaims("john");
            var dto = new UpdateDTO { FirstName = "John", LastName = "Doe" };
            var updatedUser = new User { Username = "john", FirstName = "John", LastName = "Doe" };

            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(updatedUser);
            _userServiceMock.Setup(s => s.UpdateUser("john", updatedUser, "john", false)).ReturnsAsync(false);

            var result = await _controller.UpdateUser("john", dto);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User update failed. Check logs for details.", badResult.Value);
        }


        [Fact]
        public async Task DeleteUser_Success_ReturnsOk()
        {
            SetUserClaims("john");
            _userServiceMock.Setup(s => s.DeleteUser("john", "john", false)).ReturnsAsync(true);

            var result = await _controller.DeleteUser("john");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User deleted.", okResult.Value);
        }

        [Fact]
        public async Task DeleteUser_Failure_ReturnsBadRequest()
        {
            SetUserClaims("john");
            _userServiceMock.Setup(s => s.DeleteUser("john", "john", false)).ReturnsAsync(false);

            var result = await _controller.DeleteUser("john");

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User deletion failed. Check logs for details.", badResult.Value);
        }

        [Fact]
        public async Task BlockUser_Success_ReturnsOk()
        {
            _userServiceMock.Setup(s => s.BlockUser("john", true)).ReturnsAsync(true);

            var result = await _controller.BlockUser("john", true);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User blocked.", okResult.Value);
        }

        [Fact]
        public async Task BlockUser_Failure_ReturnsBadRequest()
        {
            _userServiceMock.Setup(s => s.BlockUser("john", true)).ReturnsAsync(false);
              
            var result = await _controller.BlockUser("john", true);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Block status update failed. Couldn't find user.", badResult.Value);
        }


        [Fact]
        public async Task ChangePassword_Success_ReturnsOk()
        {
            SetUserClaims("john");
            _userServiceMock.Setup(s => s.UpdatePassword("john", "oldpass", "newpass")).ReturnsAsync(true);

            var result = await _controller.ChangePassword("oldpass", "newpass");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Password updated successfully.", okResult.Value);
        }

        [Fact]
        public async Task ChangePassword_Failure_ReturnsBadRequest()
        {
            SetUserClaims("john");
            _userServiceMock.Setup(s => s.UpdatePassword("john", "wrongpass", "newpass")).ReturnsAsync(false);

            var result = await _controller.ChangePassword("wrongpass", "newpass");

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Password update failed. Check logs for details.", badResult.Value);
        }
    }
}