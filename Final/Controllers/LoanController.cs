using Data;
using Domain.Post;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Final.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using static Domain.Post.AddLoans;
using Microsoft.AspNetCore.Http;
using Final.helper;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace Final.Controllers
{
    [Route("api/[controller]")]

    public class LoanController : Controller
    {
        private readonly PersonContext _personcontext;
        private readonly ILoanService _loanService;
        private readonly ILogger _logger;
        public LoanController(PersonContext personcontext,ILoanService loanService,ILogger<LoanController> logger)
        {
            _personcontext = personcontext; 
            _loanService= loanService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("addloan")]
        public async Task<ActionResult<Domain.Loan>> AddLoan(int userId, AddLoans loan)
        {
            try
            {
                _logger.LogDebug($"Starting Addloan method");
                await _loanService.AddingLoan(userId, loan);
                _logger.LogInformation("Loan {LoanId} was added for user {UserId}", loan, userId);
                return Ok("Added loan successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during user update.");
                var errorResponse = new ErrorResponse
                {
                    Message = "An error occurred while processing your request.",
                    Detail = ex.Message
                };
                return BadRequest(errorResponse);
            }

        }
        [Authorize]
        [HttpPut("updateloan")]
        public async Task<ActionResult<IEnumerable<Loan>>> UpdateLoanByUserId(int userId,int loanId, AddLoans updateLoan)
        {
            try
            {
                _logger.LogDebug("Starting UpdateLoanByUserId method");
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int idOfUser))
                {
                    _logger.LogWarning("Unauthorized");
                    return Unauthorized();
                }
                await _loanService.UpdatingLoan(HttpContext,userId, loanId, updateLoan, idOfUser);
                _logger.LogDebug("Finished UpdateLoanByUserId method.");
                return Ok($"{updateLoan} updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during user update.");
                var errorResponse = new ErrorResponse
                {
                    Message = "An error occurred while processing your request.",
                    Detail = ex.Message
                };
                return BadRequest(errorResponse);
            }

        }
        [Authorize(Roles = nameof(Role.Admin))]
        [HttpDelete("deleteloan")]
        public async Task<ActionResult<IEnumerable<User>>> DeleteLoanByUserId(int loanId)
        {
            try
            {
                _logger.LogDebug("Started DeleteLoanByUserId method.");
                if (_personcontext.Loans.FirstOrDefaultAsync(x => x.Id == loanId) == null)
                {
                    _logger.LogWarning("Loan not found");
                    return Unauthorized($"{loanId} doesnt exists");

                }
                _logger.LogDebug("Finished DeleteLoanByUserId method.");
                return Ok($"{loanId} successfully been deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during user update.");
                var errorResponse = new ErrorResponse
                {
                    Message = "An error occurred while processing your request.",
                    Detail = ex.Message
                };
                return BadRequest(errorResponse);
            }

        }

    }
}
