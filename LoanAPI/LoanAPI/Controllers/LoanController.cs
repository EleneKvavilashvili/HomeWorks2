using LoanAPI.Models;
using LoanAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanAPI.Controllers
{
    [ApiController]
    [Route("api/loans")]
    [Authorize]
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [Authorize(Roles = "Accountant")]
        [HttpGet("Accountant/FilterLoans")]
        public async Task<IActionResult> GetLoans([FromQuery] LoanType? type, [FromQuery] LoanStatus? status)
        {
            return Ok(await _loanService.GetLoans(type, status));
        }

        
        [HttpGet("{requestedUsername}")]
        public async Task<IActionResult> GetLoansByUsername(
            string requestedUsername,
            [FromQuery] int? loanId,
            [FromQuery] LoanType? type,
            [FromQuery] LoanStatus? status)
        {
            string authorizedUsername = User.Identity!.Name!;
            bool isAccountant = User.IsInRole("Accountant");

            var loans = await _loanService.GetLoansByUsername(requestedUsername, authorizedUsername, isAccountant, loanId, type, status);
            if (loans == null)
            {
                return BadRequest("Loans not found. Check logs for details.");
            }

            return Ok(loans);
        }

        
        [Authorize(Roles = "User")]
        [HttpPost("Request")]
        public async Task<IActionResult> CreateLoan([FromBody] Loan loan)
        {
            string authorizedUsername = User.Identity!.Name!;

            bool result = await _loanService.CreateLoan(loan, authorizedUsername);
            if (!result)
            {
                return BadRequest("Failed to request loan. Check logs for details." );
            }

            return Ok("Loan requested successfully.");
        }

        
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateLoan(int id, [FromBody] Loan updatedLoan)
        {
            string authorizedUsername = User.Identity!.Name!;
            bool isAccountant = User.IsInRole("Accountant");

            bool result = await _loanService.UpdateLoan(id, updatedLoan, authorizedUsername, isAccountant);
            if (!result)
            {
                return BadRequest("Loan update failed. Check logs for details.");
            }

            return Ok("Loan updated successfully.");
        }

        
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            string authorizedUsername = User.Identity!.Name!;
            bool isAccountant = User.IsInRole("Accountant");

            bool result = await _loanService.DeleteLoan(id, authorizedUsername, isAccountant);
            if (!result)
            {
                return BadRequest("Loan deletion failed. Check logs for details.");
            }

            return Ok("Loan deleted successfully.");
        }
    }
}
