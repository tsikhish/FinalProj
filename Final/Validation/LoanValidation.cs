using Domain;
using Domain.Post;
using FluentValidation;

namespace Final.Validation
{
    public class LoanValidation : AbstractValidator<AddLoans>
    {
        public LoanValidation()
        {
            RuleFor(x => x.Ammount).NotEmpty().WithMessage("Amount should be filled").LessThan(20000).GreaterThan(100).WithMessage("Amount should be between 100-20000");
            RuleFor(x => x.Status).IsInEnum().WithName("status").WithMessage("invalid Status").NotEmpty().WithMessage("Status should be filled");
            RuleFor(x => x.LoanPeriod).NotEmpty().WithMessage("LoanPeriod should be filled");
            RuleFor(x => x.Currency).IsInEnum().WithName("Currency").WithMessage("invalid Currency").NotEmpty().WithMessage("Currency should be filled");
            RuleFor(x => x.LoanType).IsInEnum().WithName("LoanType").WithMessage("invalid LoanType").NotEmpty().WithMessage("LoanType should be filled");
        }
    }
}
