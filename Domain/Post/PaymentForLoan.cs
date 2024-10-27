using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Post
{
    public class PaymentForLoan
    {
        public int loanId { get; set; } //for which Loan will be paid
        [Required(ErrorMessage ="You should fill the day of payment")]
        public int PaidAmount { get; set; }
    }
}
