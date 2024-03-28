//using Data;
//using Domain.Post;
//using Domain;
//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System;
//using Final.Services;
//using Microsoft.EntityFrameworkCore;
//using System.Linq;
//using Microsoft.AspNetCore.Authorization;
//using System.Security.Claims;
//using static Domain.Post.AddLoans;

//namespace Final.Controllers
//{
//    [Route("api/[controller]")]

//    public class LoanController : Controller
//    {
//        private readonly PersonContext _personcontext;
//        private readonly IUserServices _userServices;
//        public LoanController(PersonContext personcontext,IUserServices userServices)
//        {
//            _personcontext=personcontext;
//            _userServices=userServices;
//        }
//        [Authorize]
//        [HttpPost("/addloan")]
//        public async Task<ActionResult<Domain.Loan>> AddLoan(int userId, AddLoans loan)
//        {
//            try
//            {
//                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
//                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int IdOfUser))
//                {
//                    return Unauthorized(); 
//                }

//                if (IdOfUser != userId)
//                {
//                    return Forbid();
//                }
//                var user = await _personcontext.Users.FindAsync(userId);
//                var existingLoan = _personcontext.Loans.FirstOrDefaultAsync(x => x.UserId == userId);

//                if (user == null)
//                {
//                    return NotFound("User not found");
//                }
//                if (user.IsBlocked == true)
//                {
//                    return BadRequest("We're sorry you cant add loan in your bank account");
//                }

//                var bankLoan = new Domain.Loan()
//                {
//                    UserId = userId,
//                    LoanPeriod = loan.LoanPeriod,
//                    Ammount = loan.Ammount,
//                    Status = loan.Status,
//                    Loantype = loan.LoanType,
//                    Currency = loan.Currency
//                };

//                _personcontext.Loans.Add(bankLoan);
//                await _personcontext.SaveChangesAsync();

//                return Ok(bankLoan);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"An error occurred while adding loan: {ex.Message}");
//            }
//        }

//        //[HttpGet("/getAllUsersLoan")]
//        //public async Task<ActionResult<IEnumerable<Domain.Loan>>> GetAllUsersLoan()
//        //{
//        //    try
//        //    {
                
//        //        var loans = await _personcontext.Loans.ToListAsync();
//        //        if (!loans.Any())
//        //        {
//        //            return NotFound();
//        //        }
//        //        return Ok(loans);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return StatusCode(500, $"An error occurred while getting loans: {ex.Message}");
//        //    }
//        //}
//        [HttpPut("/updateloan")]
//        public async Task<ActionResult<IEnumerable<Loan>>> UpdateLoanByUserId(int id, AddLoans updateLoan)
//        {
//            try
//            {

//                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
//                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int IdOfUser))
//                {
//                    return Unauthorized();
//                }

//                if (IdOfUser != id)
//                {
//                    return Forbid(); // User is not authorized to access other users' information
//                }
//                var loan = _personcontext.Loans.FirstOrDefault(x => x.UserId == id);
//                //if (loan == null || loan.Status != 0)
//                //{
//                //    return BadRequest("You cant update loan");
//                //}
//                if (loan.Status == LoanStatus.Proccessing)
//                {
//                    loan.LoanPeriod = updateLoan.LoanPeriod;
//                    loan.Ammount = updateLoan.Ammount;
//                    _personcontext.Update(loan);
//                    await _personcontext.SaveChangesAsync();
//                }
//                return Ok(updateLoan);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"An error occurred while updating loan: {ex.Message}");
//            }
//        }
//        [HttpDelete("/deleteloan")]
//        public async Task<ActionResult<IEnumerable<User>>> DeleteLoanByUserId(int id)
//        {
//            try
//            {
//                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
//                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int IdOfUser))
//                {
//                    return Unauthorized();
//                }

//                if (IdOfUser != id)
//                {
//                    return Forbid(); // User is not authorized to access other users' information
//                }
//                var loan = await _personcontext.Loans.FirstOrDefaultAsync(x => x.UserId == id);
//                //if (loan == null || _personcontext.Loans.Count() < id || loan.Status != 0)
//                //{
//                //    return BadRequest($"{id} cant be deleted");
//                //}
//                if (loan.Status == LoanStatus.Proccessing)
//                {
//                    _personcontext.Loans.Remove(loan);
//                    _personcontext.SaveChanges();
//                }
//                return Ok($"{loan} successfully been deleted");
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"An error occurred while deleting user: {ex.Message}");
//            }
//        }

//    }
//}
