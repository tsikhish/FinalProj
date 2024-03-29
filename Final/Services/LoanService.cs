using Data;
using Domain;
using Domain.Post;
using Final.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using static Domain.Post.AddLoans;

namespace Final.Services
{
    public interface ILoanService
    {
        Task AddingLoan(int userId, AddLoans loan);
        Task ValidateAddLoan(AddLoans loan);
        Task UpdatingLoan(int userId, int loanId, AddLoans loan, int idOfUser);
        public class LoanService : ILoanService
        {
            private readonly PersonContext _personContext;
            public LoanService(PersonContext personContext)
            {
                _personContext = personContext;
            }
           public async Task AddingLoan(int userId, AddLoans loan)
            {
                await ValidateAddLoan(loan);
                var user = await _personContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
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
            public async Task UpdatingLoan(int userId, int loanId, AddLoans updateLoan, int idOfUser)
            {
                var users = await _personContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                var loans = await _personContext.Loans.FirstOrDefaultAsync(x => x.Id == loanId);
                if (users == null || loans == null)
                {
                    throw new Exception($"{users} {loans} Not Found");
                }
                if (idOfUser != userId)
                {
                    throw new Exception($"{userId} cant see other User's informations");
                }
                if (users.Role == "Accountant")
                {
                    await LoansUpdated(loans,updateLoan);
                    loans.Status=updateLoan.Status;
                    _personContext.Update(loans);
                    await _personContext.SaveChangesAsync(); 
                }
                else if (idOfUser == userId && loans.Status == LoanStatus.Proccessing)
                {
                    await LoansUpdated(loans, updateLoan);
                }
                throw new Exception("User cant be updated");
            }
            public async Task ValidateAddLoan(AddLoans loan)
            {
                var validator = new LoanValidation();
                var validationResult = await validator.ValidateAsync(loan);
                var errorMessage = "";
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        errorMessage += item.ErrorMessage + " , ";
                    }
                    throw new System.Exception(errorMessage);
                }
            }
            private async Task LoansUpdated(Loan loans,AddLoans updateLoan)
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
