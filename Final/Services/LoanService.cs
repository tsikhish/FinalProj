using Data;
using Domain;
using Domain.Post;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Domain.Post.AddLoans;

namespace Final.Services
{
    public interface ILoanService
    {
        Task AddingLoan(int userId, AddLoans loan);
        Task UpdatingLoan(HttpContext httpContext, int userId, int loanId, AddLoans updateLoan, int idOfUser);
        public class LoanService : ILoanService
        {
            private readonly PersonContext _personContext;
            public LoanService(PersonContext personContext)
            {
                _personContext = personContext;
            }
           public async Task AddingLoan(int userId, AddLoans loan)
            {
                var user = await _personContext.AppUsers.FirstOrDefaultAsync(x => x.Id == userId);
                if  (user == null)
                {
                    throw new Exception($"{user} Not Found");
                }
                if (user.IsBlocked == true)
                {
                    throw new Exception("Cant be added, user is Blocked");

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
                _personContext.Loans.Add(bankLoan);
                _personContext.SaveChanges();
            }
            public async Task UpdatingLoan(HttpContext httpContext,int userId, int loanId, AddLoans updateLoan, int idOfUser)
            {
                var users = await _personContext.AppUsers.FirstOrDefaultAsync(x => x.Id == userId);
                var loans = await _personContext.Loans.FirstOrDefaultAsync(x => x.Id == loanId);
                if (users == null || loans == null)
                {
                    throw new Exception($"{users} {loans} Not Found");
                }
                var userRoles = httpContext.User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                if (userRoles.Contains(Role.Admin.ToString()))
                {
                    await LoansUpdated(loans, updateLoan);
                    loans.Status = updateLoan.Status;
                    _personContext.Update(loans);
                    await _personContext.SaveChangesAsync();
                }
               else if (idOfUser != userId)
                {
                    throw new Exception($"{userId} cant see other User's informations");
                }
                else if (idOfUser == userId && loans.Status == LoanStatus.Proccessing)
                {
                    await LoansUpdated(loans, updateLoan);
                }
                else
                {
                    throw new Exception("User cant be updated");
                }
            }
            public async Task LoansUpdated(Loan loans,AddLoans updateLoan)
            {
                loans.LoanPeriod = updateLoan.LoanPeriod;
                loans.Ammount = updateLoan.Ammount;
                loans.Loantype = updateLoan.LoanType;
                loans.Currency = updateLoan.Currency;
                _personContext.Update(loans);
                await _personContext.SaveChangesAsync();
            }
        }
    }
}
