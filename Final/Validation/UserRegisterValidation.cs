using Domain;
using Domain.Post;
using FluentValidation;

namespace Final.Validation
{
    public class UserRegisterValidation : AbstractValidator<UserRegistration>
    {
        public UserRegisterValidation()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Name Should be filled");
            RuleFor(x => x.Email).EmailAddress().NotEmpty().WithMessage("Email Should be filled and Email should have email type");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Surname Should be filled");
            RuleFor(x => x.Salary).NotEmpty().WithMessage("Salary Should be filled");
            RuleFor(x => x.Age).NotEmpty().WithMessage("Age Should be filled");
        }
    }
}
