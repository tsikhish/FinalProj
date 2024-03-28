using Domain;
using FluentValidation;

namespace Final.Validation
{
    public class LoanValidation : AbstractValidator<Loan>
    {
        public LoanValidation()
        {
            RuleFor(x => x.Ammount).NotEmpty().WithMessage("Amount should be filled");
            //RuleFor(x => x.Status).IsInEnum().WithName("status").WithMessage("invalid");
        }
    }
}
