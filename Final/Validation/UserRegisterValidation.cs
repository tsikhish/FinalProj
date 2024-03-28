using Domain;
using Domain.Post;
using FluentValidation;

namespace Final.Validation
{
    public class UserRegisterValidation : AbstractValidator<UserRegistration>
    {
        public UserRegisterValidation()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName Should be filled");
            RuleFor(x => x.Email).EmailAddress().NotEmpty().WithMessage("Email Should be filled and Email should have email type");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Surname Should be filled");
            RuleFor(x => x.Salary).NotEmpty().WithMessage("Salary Should be filled");
            RuleFor(x => x.Age).GreaterThan(18).WithMessage("Age should be above 18").NotEmpty().WithMessage("Age Should be filled");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password should be filled");
        }
    }
}
