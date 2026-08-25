using System.Security.Claims;
using LoanAPI.Controllers;
using LoanAPI.Models;
using LoanAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LoanApi.Tests.ControllerTests
{
    public class LoanControllerTests
    {
        private readonly Mock<ILoanService> _loanServiceMock;
        private readonly LoanController _controller;

        public LoanControllerTests()
        {
            _loanServiceMock = new Mock<ILoanService>();
            _controller = new LoanController(_loanServiceMock.Object);
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
        public async Task GetLoansByUsername_Success_ReturnsOk()
        {
            SetUserClaims("john");
            var loans = new List<Loan>
            {
                new Loan { Id = 1, UserId = 1, Amount = 1000, Type = LoanType.QuickLoan, Status = LoanStatus.InProcess }
            };
            _loanServiceMock.Setup(s => s.GetLoansByUsername("john", "john", false, null, null, null))
                            .ReturnsAsync(loans);

            var result = await _controller.GetLoansByUsername("john", null, null, null);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(loans, okResult.Value);
        }

        [Fact]
        public async Task GetLoansByUsername_Failure_ReturnsBadRequest()
        {
            SetUserClaims("john");
            _loanServiceMock.Setup(s => s.GetLoansByUsername("invalid_user", "john", false, null, null, null))
                            .ReturnsAsync((List<Loan>?)null);

            var result = await _controller.GetLoansByUsername("invalid_user", null, null, null);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Loans not found. Check logs for details.", badResult.Value);
        }


        [Fact]
        public async Task CreateLoan_Success_ReturnsOk()
        {
            SetUserClaims("john");
            var loan = new Loan { Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan };
            _loanServiceMock.Setup(s => s.CreateLoan(loan, "john"))
                            .ReturnsAsync(true);

            var result = await _controller.CreateLoan(loan);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Loan requested successfully.", okResult.Value);
        }

        [Fact]
        public async Task CreateLoan_Failure_ReturnsBadRequest()
        {
            SetUserClaims("john");
            var loan = new Loan { Amount = 1000, PeriodMonths = 12, Type = LoanType.QuickLoan };
            _loanServiceMock.Setup(s => s.CreateLoan(loan, "john"))
                            .ReturnsAsync(false);

            var result = await _controller.CreateLoan(loan);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to request loan. Check logs for details.", badResult.Value);
        }


        [Fact]
        public async Task UpdateLoan_Success_ReturnsOk()
        {
            SetUserClaims("john");
            var loan = new Loan { Amount = 2000, PeriodMonths = 24, Type = LoanType.QuickLoan };
            _loanServiceMock.Setup(s => s.UpdateLoan(1, loan, "john", false))
                            .ReturnsAsync(true);

            var result = await _controller.UpdateLoan(1, loan);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Loan updated successfully.", okResult.Value);
        }

        [Fact]
        public async Task UpdateLoan_Failure_ReturnsBadRequest()
        {
            SetUserClaims("john");
            var loan = new Loan { Amount = 2000, PeriodMonths = 24, Type = LoanType.QuickLoan };
            _loanServiceMock.Setup(s => s.UpdateLoan(1, loan, "john", false))
                            .ReturnsAsync(false);

            var result = await _controller.UpdateLoan(1, loan);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Loan update failed. Check logs for details.", badResult.Value);
        }


        [Fact]
        public async Task DeleteLoan_Success_ReturnsOk()
        {
            SetUserClaims("john");
            _loanServiceMock.Setup(s => s.DeleteLoan(1, "john", false))
                            .ReturnsAsync(true);

            var result = await _controller.DeleteLoan(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Loan deleted successfully.", okResult.Value);
        }

        [Fact]
        public async Task DeleteLoan_Failure_ReturnsBadRequest()
        {
            SetUserClaims("john");
            _loanServiceMock.Setup(s => s.DeleteLoan(1, "john", false))
                            .ReturnsAsync(false);

            var result = await _controller.DeleteLoan(1);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Loan deletion failed. Check logs for details.", badResult.Value);
        }
    }
}
