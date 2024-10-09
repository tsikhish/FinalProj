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
                return BadRequest(ex.Message);
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
                return StatusCode(500, $"An error occurred while updating loan: {ex.Message}");
            }
        }
        [HttpDelete("/deleteloan")]
        public async Task<ActionResult<IEnumerable<User>>> DeleteLoanByUserId(int userId,int loanId)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int IdOfUser))
                {
                    return Unauthorized();
                }
                if (IdOfUser != userId)
                {
                    return Forbid(); 
                }
                var loan = await _personcontext.Loans.FirstOrDefaultAsync(x => x.Id == loanId);
                if (loan == null)
                {
                    return BadRequest($"{loanId} cant be deleted");
                }
                if (User.IsInRole("Admin") || loan.Status == LoanStatus.Proccessing)
                {
                    _personcontext.Loans.Remove(loan);
                    _personcontext.SaveChanges();
                }
                return Ok($"{loan} successfully been deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting user: {ex.Message}");
            }
        }

    }
}
