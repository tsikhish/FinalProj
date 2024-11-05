using Data;
using Domain.Post;
using Domain;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Final.Services.ServiceHelper
{
    
    public class LoanValidationService
    {
        private readonly PersonContext _personContext;
        private readonly ILogger<LoanValidationService> _logger;

        public LoanValidationService(PersonContext personContext, ILogger<LoanValidationService> logger)
        {
            _personContext = personContext;
            _logger = logger;
        }

        public async Task<PaymentHistory> ValidateNonExistingPaymentAsync(int loanId)
        {
            _logger.LogInformation($"Validating existing payment for Loan ID: {loanId}"); 
            var payment = await _personContext.Payment.FirstOrDefaultAsync(x => x.LoanId == loanId);
            if (payment == null)
            {
                _logger.LogWarning($"No payment record exists for Loan ID: {loanId}");
                throw new Exception("No payment record exists for this loan.");
            }
            return payment;
        }
        public async Task ValidateExistingPaymentAsync(PaymentForLoan paymentForLoan)
        {
            _logger.LogInformation($"Validating non-existing payment for Loan ID: {paymentForLoan.loanId}");
            var existingPayment = await _personContext.Payment
                    .FirstOrDefaultAsync(x => x.LoanId == paymentForLoan.loanId);
            if (existingPayment != null)
            {
                _logger.LogWarning($"Payment already exists for Loan ID: {paymentForLoan.loanId}");
                throw new Exception("Payment already exists for this loan.");
            }
        }

        public async Task<Loan> ValidateLoanAndUserAsync(PaymentForLoan paymentForLoan, int idOfUser)
        {
            _logger.LogInformation($"Validating loan and user for Loan ID: {paymentForLoan.loanId} and User ID: {idOfUser}");

            var existingLoan = await _personContext.Loans
                     .FirstOrDefaultAsync(x => x.Id == paymentForLoan.loanId);
            if (existingLoan.UserId != idOfUser) throw new Exception("You are not authorized to make payments on this loan.");
            if (existingLoan == null) throw new Exception("The specified loan does not exist in loan records.");
            if (existingLoan.LoanPeriod == 0) throw new Exception("Loan period cannot be zero.");
            return existingLoan;
        }
        public async Task CheckUsersLoan(int userId, Loan loans)
        {
            _logger.LogInformation($"Checking user's loan for User ID: {userId} and Loan ID: {loans.Id}");
            var existingLoans = await _personContext.Loans.FirstOrDefaultAsync(x => x.Id == loans.Id);
            if (existingLoans.UserId != userId)
            {
                _logger.LogWarning($"User ID: {userId} is not authorized to change Loan ID: {loans.Id}");
                throw new Exception("You are not authorized to make changes on this loan.");
            }
        }
        public async Task<User> ValidateUser(int userId)
        {
            _logger.LogInformation($"Validating user for User ID: {userId}");
            var user = await _personContext.AppUsers.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                _logger.LogWarning($"User not found for User ID: {userId}");
                throw new KeyNotFoundException("User not found");
            }
            return user;
        }
        public async Task<Loan> ValidateLoan(int loanId)
        {
            _logger.LogInformation($"Validating loan for Loan ID: {loanId}");

            var loan = await _personContext.Loans.FirstOrDefaultAsync(x => x.Id == loanId);
            if (loan == null)
            {
                _logger.LogWarning($"Loan not found for Loan ID: {loanId}");
                throw new KeyNotFoundException("Loan not found");
            }
            return loan;
        }
    }

}
