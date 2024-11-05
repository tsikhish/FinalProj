using Data;
using Domain;
using Domain.Post;
using Final.Services.ServiceHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Domain.Post.AddLoans;

namespace Final.Services
{
    public interface ILoanService
    {
        Task AddingLoan(int idOfUser, AddLoans loan);
        Task UpdatingLoan(HttpContext httpContext, int idOfUser, int loanId, AddLoans updateLoan);
        Task AddingPayment(HttpContext httpContext, PaymentForLoan paymentForLoan, int idOfUser);
        Task UpdateLoanPayment(HttpContext httpContext, PaymentForLoan paymentHistory, int idOfUser);
        public class LoanService : ILoanService
        {
            private readonly PersonContext _personContext;
            private readonly ILogger _logger;
            private readonly LoanValidationService _loanValidationService;
            public LoanService(PersonContext personContext, ILogger<LoanService> logger, LoanValidationService loanValidationService)
            {
                _personContext = personContext;
                _logger = logger;
                _loanValidationService = loanValidationService;
            }
            public async Task AddingLoan(int idOfUser, AddLoans loan)
            {
                var existingUser = _personContext.AppUsers.FirstOrDefault(x => x.Id == idOfUser);
                if (existingUser == null)
                {
                    _logger.LogWarning($"User not found for User ID: {idOfUser}");
                    throw new KeyNotFoundException($"User with ID {idOfUser} not found.");
                }
                if (existingUser.IsBlocked == true)
                {
                    _logger.LogWarning("User is blocked");
                    throw new Exception("Cant be added, user is Blocked");
                }
                await CreateLoanAsync(idOfUser,loan);
            }
            public async Task UpdatingLoan(HttpContext httpContext, int idOfUser,
                int loanId, AddLoans updateLoan)
            {
                var userRoles = GetUserRoles(httpContext);
                var loans = await _loanValidationService.ValidateLoan(loanId);
                //Admin can change everyone's loan
                if (userRoles.Contains(Role.Admin.ToString()))
                {
                    _logger.LogInformation($"{userRoles} is admin");
                    await UpdateLoanInDbAsync(loans, updateLoan);
                    return;
                }
                else if (userRoles.Contains(Role.User.ToString()))
                {
                    await UpdateLoanAsUser(loans, updateLoan, idOfUser);
                    return;
                }
                else
                {
                    _logger.LogWarning($"Unauthorized access attempt by user {idOfUser} with roles {userRoles}.");
                    throw new UnauthorizedAccessException("You don't have the required permissions.");
                }
            }
            public async Task AddingPayment(HttpContext httpContext, PaymentForLoan paymentForLoan, int idOfUser)
            {
                await _loanValidationService.ValidateExistingPaymentAsync(paymentForLoan);
                var existingLoan = await _loanValidationService.ValidateLoanAndUserAsync(paymentForLoan, idOfUser);
                decimal monthlyPayment = existingLoan.Ammount / existingLoan.LoanPeriod;
                int completedMonths = LoanPaymentService.CalculateCompletedMonths(paymentForLoan.PaidAmount, monthlyPayment);
                //if you remain negative number,you should cover this before the deadline,
                //else you have paid more than needed
                var monthlyremainPayment = paymentForLoan.PaidAmount > monthlyPayment
                    ? paymentForLoan.PaidAmount - monthlyPayment * completedMonths
                    : paymentForLoan.PaidAmount - monthlyPayment;
                var paymentRecord = CreatePaymentRecordAsync(idOfUser,paymentForLoan,existingLoan,
                    monthlyremainPayment,completedMonths,monthlyPayment);
                await SavePaymentAndUpdateUserStatusAsync(paymentRecord, existingLoan, idOfUser);
            }
            public async Task UpdateLoanPayment(HttpContext httpContext, PaymentForLoan paymentHistory, int idOfUser)
            {
                var existingPayment =await  _loanValidationService.ValidateNonExistingPaymentAsync(idOfUser);
                await _loanValidationService.ValidateLoanAndUserAsync(paymentHistory, idOfUser);
                bool isUserBlocked =  ShouldBlockUser(existingPayment);
                int completedMonths = LoanPaymentService.CalculateCompletedMonths(paymentHistory.PaidAmount, existingPayment.MonthlyPayment);
                var remainPaymentForMonth = existingPayment.RemainMonthlyPayment + paymentHistory.PaidAmount - existingPayment.MonthlyPayment;
                await UpdateToDB(isUserBlocked,existingPayment, remainPaymentForMonth, paymentHistory, completedMonths);
            }
            private async Task UpdateToDB(bool isUserBlocked,PaymentHistory existingPayment,decimal remainPaymentForMonth,
                PaymentForLoan paymentHistory,int completedMonths)
            {
                existingPayment.PaidDate = DateTime.Now;
                existingPayment.RemainedAmount -= paymentHistory.PaidAmount;
                existingPayment.PaidAmountForThisTime = paymentHistory.PaidAmount;
                existingPayment.DeadLine = DateTime.Now.AddMonths(completedMonths);
                existingPayment.RemainMonthlyPayment = remainPaymentForMonth;
                existingPayment.RemainedAmount -= paymentHistory.PaidAmount;
                if (isUserBlocked)
                {
                    var user = await _personContext.AppUsers.FirstOrDefaultAsync(x => x.Id == existingPayment.UserId);
                    if (user != null)
                    {
                        user.IsBlocked = true;
                        _personContext.AppUsers.Update(user);
                    }
                }
                _personContext.Payment.Update(existingPayment);
                await _personContext.SaveChangesAsync();
            }
            private bool ShouldBlockUser(PaymentHistory existingPayment)
            {
                return existingPayment.PaidDate > existingPayment.DeadLine && existingPayment.RemainMonthlyPayment < 0;
            }
            private PaymentHistory CreatePaymentRecordAsync(int idOfUser, PaymentForLoan paymentForLoan, Loan existingLoan,
               decimal monthlyremainPayment, int completedMonths, decimal monthlyPayment)
            {
                return new PaymentHistory()
                {
                    UserId = idOfUser,
                    LoanId = paymentForLoan.loanId,
                    DeadLine = existingLoan.StartDate.AddMonths(completedMonths),
                    PaidDate = DateTime.Now,
                    FullPayment = existingLoan.Ammount,
                    MonthlyPayment = monthlyPayment,
                    PaidAmountForThisTime = paymentForLoan.PaidAmount,
                    RemainMonthlyPayment = monthlyremainPayment,
                    RemainedAmount = existingLoan.Ammount - paymentForLoan.PaidAmount
                };
            }

            private async Task SavePaymentAndUpdateUserStatusAsync(PaymentHistory paymentLoan,Loan existingLoan,int idOfUser)
            {
                var user = _personContext.AppUsers.FirstOrDefault(x => x.Id == idOfUser);
                _personContext.Payment.Add(paymentLoan);
                await _personContext.SaveChangesAsync();
                if (paymentLoan.PaidDate > existingLoan.StartDate.AddMonths(1))
                {
                    user.IsBlocked = true;
                    _personContext.AppUsers.Update(user);
                }
                await _personContext.SaveChangesAsync();
            }
            private async Task CreateLoanAsync(int userId, AddLoans loan)
            {
                var bankLoan = new Loan()
                {
                    UserId = userId,
                    LoanPeriod = loan.LoanPeriod,
                    Ammount = loan.Ammount,
                    Status = loan.Status,
                    Loantype = loan.LoanType,
                    Currency = loan.Currency,
                    StartDate = DateTime.Now,
                };
                _personContext.Loans.Add(bankLoan);
                await _personContext.SaveChangesAsync();
            }
            private async Task UpdateLoanInDbAsync(Loan loans, AddLoans updateLoan)
            {
                loans.LoanPeriod = updateLoan.LoanPeriod;
                loans.Ammount = updateLoan.Ammount;
                loans.Loantype = updateLoan.LoanType;
                loans.Currency = updateLoan.Currency;
                loans.Status = updateLoan.Status;
                _personContext.Update(loans);
                await _personContext.SaveChangesAsync();
            }

            private async Task UpdateLoanAsUser(Loan loans, AddLoans updateLoan, int idOfUser)
            {
                if (loans.Status != LoanStatus.Proccessing)
                {
                    _logger.LogWarning("User can update only if Loanstatus is in a processing status");
                    throw new InvalidOperationException("User cannot update loans that are not in 'Processing' status.");
                }
                await _loanValidationService.CheckUsersLoan(idOfUser, loans);
                await UpdateLoanInDbAsync(loans, updateLoan);
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