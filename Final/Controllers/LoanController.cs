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

namespace Final.Controllers
{
    [Route("api/[controller]")]

    public class LoanController : Controller
    {
        private readonly PersonContext _personcontext;
        private readonly ILoanService _loanService;
        public LoanController(PersonContext personcontext,ILoanService loanService)
        {
            _personcontext = personcontext; 
            _loanService= loanService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/addloan")]
        public async Task<ActionResult<Domain.Loan>> AddLoan(int userId, AddLoans loan)
        {
            try
            {
                await _loanService.AddingLoan(userId, loan);
                return Ok("Added loan successfully");
            }
            catch (Exception ex)
{
    var errorResponse = new ErrorResponse
    {
        Message = "An error occurred while processing your request.",
        Detail = ex.Message
    };
    return BadRequest(errorResponse);
}

        }
        [Authorize]
        [HttpPut("/updateloan")]
        public async Task<ActionResult<IEnumerable<Loan>>> UpdateLoanByUserId(int userId,int loanId, AddLoans updateLoan)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int idOfUser))
                {
                    return Unauthorized();
                }
                await _loanService.UpdatingLoan(HttpContext,userId, loanId, updateLoan, idOfUser);
                return Ok($"{updateLoan} updated successfully");
            }
            catch (Exception ex)
            {
                var errorResponse = new ErrorResponse
                {
                    Message = "An error occurred while processing your request.",
                    Detail = ex.Message
                };
                return BadRequest(errorResponse);
            }

        }
        [Authorize(Roles = nameof(Role.Admin))]
        [HttpDelete("/deleteloan")]
        public async Task<ActionResult<IEnumerable<User>>> DeleteLoanByUserId(int loanId)
        {
            try
            {
                if (_personcontext.Loans.FirstOrDefaultAsync(x => x.Id == loanId) == null)
                {
                    return Unauthorized($"{loanId} doesnt exists");
                }
                return Ok($"{loanId} successfully been deleted");
            }
            catch (Exception ex)
            {
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
