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
using Microsoft.AspNetCore.Http;
using Final.helper;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Final.Controllers
{
    [Route("api/[controller]")]

    public class LoanController : Controller
    {
        private readonly PersonContext _personcontext;
        private readonly ILoanService _loanService;
        private readonly ILogger<LoanController> _logger;
        public LoanController(PersonContext personcontext,ILoanService loanService,ILogger<LoanController> logger)
        {
            _personcontext = personcontext; 
            _loanService= loanService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("addloan")]
        public async Task<ActionResult> AddLoan(AddLoans loan)
        {
            try
            {

                var userRoles = HttpContext.User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
                if (!userRoles.Contains(Role.Admin.ToString()))
                {
                    _logger.LogWarning("Non-admin user attempted to add a loan.");
                    return BadRequest("Only admin users are allowed to add loans.");
                }
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int idOfUser))
                {
                    _logger.LogWarning("Unauthorized");
                    return Unauthorized();
                }
                _logger.LogDebug($"Starting Addloan method");
                await _loanService.AddingLoan(idOfUser, loan);
                _logger.LogInformation($"Loan was added for user {idOfUser}", loan, idOfUser);
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
        public async Task<ActionResult<IEnumerable<Loan>>> UpdateLoanByUserId(int loanId, AddLoans updateLoan)
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
                await _loanService.UpdatingLoan(HttpContext, idOfUser, loanId, updateLoan);
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
                if (await _personcontext.Loans.FirstOrDefaultAsync(x => x.Id == loanId) == null)
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
        [Authorize]
        [HttpPost("Payment")]
        public async Task<IActionResult> AddPayment(PaymentForLoan paymentForLoan)
        {
            try
            {
                _logger.LogDebug("Started AddPayment method.");
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim ==null || !int.TryParse(userIdClaim.Value, out int idOfUser))
                {
                    _logger.LogWarning("Unauthorized");
                    return Unauthorized();
                }
                await _loanService.AddingPayment(HttpContext,paymentForLoan, idOfUser);
                _logger.LogDebug("Finished AddPayment method.");
                return Ok("Payment has added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during adding payment.");
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPut("UpdatePayment")]
        public async Task<IActionResult> UpdatePayment(PaymentForLoan paymentHistory)
        {
            try
            {
                _logger.LogDebug("Started UpdatePayment method.");

                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int idOfUser))
                {
                    _logger.LogWarning("Unauthorized");
                    return Unauthorized();
                }
                await _loanService.UpdateLoanPayment(HttpContext, paymentHistory, idOfUser);
                _logger.LogDebug("Finished UpdatePayment method.");
                return Ok("Payment has updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during payment update.");
                return BadRequest(ex.Message);
            }
        }
    }
}
