using Data;
using Domain.Post;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Final.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using static Domain.Post.AddLoans;

namespace Final.Controllers
{
    [Route("api/[controller]")]

    public class LoanController : Controller
    {
        private readonly PersonContext _personcontext;
        private readonly IUserServices _userServices;
        public LoanController(PersonContext personcontext, IUserServices userServices)
        {
            _personcontext = personcontext;
            _userServices = userServices;
        }
        [Authorize(Roles =Role.Accountant)]
        [HttpPost("/addloan")]
        public async Task<ActionResult<Domain.Loan>> AddLoan(int userId, AddLoans loan)
        {
            try
            {
               
                var user = await _personcontext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                {
                    return NotFound();
                }
                if (user.IsBlocked == true)
                {
                    return BadRequest("We're sorry you cant add loan in your bank account");
                }
               var bankLoan = new Domain.Loan()
                {
                    UserId = userId,
                    LoanPeriod = loan.LoanPeriod,
                    Ammount = loan.Ammount,
                    Status = loan.Status,
                    Loantype = loan.LoanType,
                    Currency = loan.Currency
                };

                _personcontext.Loans.Add(bankLoan);
                _personcontext.SaveChanges();
                return Ok(bankLoan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding loan: {ex.Message}");
            }
        }

        //[HttpGet("/getAllUsersLoan")]
        //public async Task<ActionResult<IEnumerable<Domain.Loan>>> GetAllUsersLoan()
        //{
        //    try
        //    {

        //        var loans = await _personcontext.Loans.ToListAsync();
        //        if (!loans.Any())
        //        {
        //            return NotFound();
        //        }
        //        return Ok(loans);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"An error occurred while getting loans: {ex.Message}");
        //    }
        //}
        
        [Authorize]
        [HttpPut("/updateloan")]
        public async Task<ActionResult<IEnumerable<Loan>>> UpdateLoanByUserId(int userId,int loanId, AddLoans updateLoan)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int IdOfUser))
                {
                    return Unauthorized();
                }
                var users=await _personcontext.Users.FirstOrDefaultAsync(x=>x.Id == userId);
                if (users==null)
                {
                    return NotFound();
                }
                var loans=await _personcontext.Loans.FirstOrDefaultAsync(x=>x.Id==loanId);
                if (loans==null)
                {
                    return NotFound($"{loanId} loan doesnt exists and cant update");
                }
                if (IdOfUser != userId)
                {
                    return Forbid();
                }
                if (User.IsInRole(Role.Accountant))
                {
                    loans.LoanPeriod = updateLoan.LoanPeriod;
                    loans.Ammount = updateLoan.Ammount;
                    loans.Status = updateLoan.Status;
                    loans.Loantype = updateLoan.LoanType;
                    loans.Currency = updateLoan.Currency;

                    _personcontext.Update(loans);
                   await _personcontext.SaveChangesAsync();
                    return Ok($"{loanId}  updated by Accountant successfully");
                }
                else if(IdOfUser == userId && loans.Status == LoanStatus.Proccessing)
                {
                    loans.LoanPeriod = updateLoan.LoanPeriod;
                    loans.Ammount = updateLoan.Ammount;
                    loans.Loantype = updateLoan.LoanType;
                    loans.Currency = updateLoan.Currency;
                    _personcontext.Update(loans);
                    await _personcontext.SaveChangesAsync();
                    return Ok($"{loanId} loan updated by user successfully");
                }
                return BadRequest("You can't update the loan.");

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
                if (loan == null || loan.Status != 0)
                {
                    return BadRequest($"{loanId} cant be deleted");
                }
                if (User.IsInRole(Role.Accountant) || loan.Status == LoanStatus.Proccessing)
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
