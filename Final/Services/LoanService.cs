using Data;
using Domain;
using Domain.Post;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
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
                if (user == null)
                {
                    throw new Exception($"{user} Not Found");
                }
                if (user.IsBlocked == true)
                {
                    throw new Exception("Cant be added, user is Blocked");

                }
                await AddLoan(userId,loan);
            }
            public async Task UpdatingLoan(HttpContext httpContext, int userId, int loanId, AddLoans updateLoan, int idOfUser)
            {
                var users = await ValidateUser(userId);
                var loans = await ValidateLoan(loanId);
                //Admin can change everyone's loan
                var userRoles = GetUserRoles(httpContext);
                if (userRoles.Contains(Role.Admin.ToString()))
                {
                    await LoansUpdated(loans, updateLoan);
                }
                else if (userRoles.Contains(Role.User.ToString()))
                {
                    await UpdateLoanAsUser(loans, updateLoan, userId, idOfUser);
                }
                else
                {
                    throw new UnauthorizedAccessException("You don't have the required permissions.");
                }
            }
            
            private async Task AddLoan(int userId, AddLoans loan)
            {
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
            public async Task LoansUpdated(Loan loans, AddLoans updateLoan)
            {
                loans.LoanPeriod = updateLoan.LoanPeriod;
                loans.Ammount = updateLoan.Ammount;
                loans.Loantype = updateLoan.LoanType;
                loans.Currency = updateLoan.Currency;
                loans.Status = updateLoan.Status;
                _personContext.Update(loans);
                await _personContext.SaveChangesAsync();
            }
           
            private async Task UpdateLoanAsUser(Loan loans, AddLoans updateLoan, int userId, int idOfUser)
            {
                //Only the authorised person whose loanStatusis is processing can change only its loan, not other's.
                if (idOfUser != userId)
                {
                    throw new UnauthorizedAccessException("You can't update another user's loan.");
                }
                if (loans.Status != LoanStatus.Proccessing)
                {
                    throw new InvalidOperationException("User cannot update loans that are not in 'Processing' status.");
                }
                await CheckUsersLoan(userId, loans);
                await LoansUpdated(loans, updateLoan);
            }
            private async Task CheckUsersLoan(int userId,Loan loans)
            {
                var groupUsers = await _personContext.Loans.ToListAsync();
                var groupsKey = groupUsers.GroupBy(x => x.UserId).FirstOrDefault(x => x.Key == userId);
                if (groupsKey != null)
                {
                    var loanItem = groupsKey.Any(x => x.Id == loans.Id);
                    if (!loanItem)
                    {
                        throw new Exception($"{userId} doesnt have this {loans.Id}");
                    }
                }
            }
            private async Task<Loan> ValidateLoan(int loanId)
            {
                var loan = await _personContext.Loans.FirstOrDefaultAsync(x => x.Id == loanId);
                if (loan == null)
                {
                    throw new KeyNotFoundException("Loan not found");
                }
                return loan;
            }
            private async Task<User> ValidateUser(int userId)
            {
                var user = await _personContext.AppUsers.FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                {
                    throw new KeyNotFoundException("User not found");
                }
                return user;
            }
            private List<string> GetUserRoles(HttpContext httpContext)
            {
                return httpContext.User.Claims
                       .Where(c => c.Type == ClaimTypes.Role)
                       .Select(c => c.Value)
                       .ToList();
            }

            
        }
    }
}