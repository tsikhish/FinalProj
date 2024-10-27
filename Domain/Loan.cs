using Domain.Post;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Domain.Post.AddLoans;
namespace Domain
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Ammount { get; set; }
        public int LoanPeriod { get; set; }
        public User User { get; set; }
        public TypeOfLoan Loantype { get; set; }
        public CurrencyType Currency { get; set; }
        public LoanStatus Status { get; set; }
        public DateTime StartDate { get; set; }
    }
}
