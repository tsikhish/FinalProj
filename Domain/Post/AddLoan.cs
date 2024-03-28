using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post
{
    public class AddLoans
    {
        public TypeOfLoan LoanType { get; set; } = TypeOfLoan.QuickLoan;
        public int Ammount { get; set; }
        public int LoanPeriod { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Proccessing;
        public CurrencyType Currency { get; set; } = CurrencyType.USD;


        public enum CurrencyType
        {
            USD = 0,
            EUR = 1,
            Geo = 2
        }
        public enum TypeOfLoan
        {
            QuickLoan = 0,
            CarLoan = 1,
            Installment = 2
        }

        public enum LoanStatus
        {
            Proccessing = 0,
            Confirmed = 1,
            Rejected = 2
        }
    }
}