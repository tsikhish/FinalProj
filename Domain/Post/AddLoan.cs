using System.ComponentModel.DataAnnotations;

namespace Domain.Post
{
    public class AddLoans
    {
        [EnumDataType(typeof(TypeOfLoan))]
        [Required(ErrorMessage ="LoanType should be filled")]
        public TypeOfLoan LoanType { get; set; } = TypeOfLoan.QuickLoan;
        [Required(ErrorMessage = "Ammount should be filled")]
        public int Ammount { get; set; }
        [Required(ErrorMessage = "LoanPeriod should be filled")]

        public int LoanPeriod { get; set; }
        [EnumDataType(typeof(LoanStatus))]
        [Required(ErrorMessage = "LoanStatus should be filled")]
        public LoanStatus Status { get; set; } = LoanStatus.Proccessing;
        [EnumDataType(typeof(CurrencyType))]
        [Required(ErrorMessage = "CurrencyType should be filled")]
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