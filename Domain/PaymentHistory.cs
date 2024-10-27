using System;
using System.ComponentModel.DataAnnotations;
namespace Domain
{
    public class PaymentHistory
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; } 
        public int LoanId { get; set; }
        public int FullPayment { get; set; }
        public int PaidAmountForThisTime { get; set; } 
        public decimal MonthlyPayment { get; set; }
        public decimal RemainMonthlyPayment { get; set; }
        public DateTime PaidDate { get; set; }
        public DateTime DeadLine { get; set; }
        public decimal RemainedAmount { get; set; }

    }
}