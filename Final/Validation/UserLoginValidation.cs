using Domain;
using Domain.Post;
using FluentValidation;

namespace Final.Validation
{
    public class UserLoginValidation : AbstractValidator<LoginUser>
    {
        public UserLoginValidation()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName should be filled");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password should be filled");
        }
    }
}
