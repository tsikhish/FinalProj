using Domain;
using Domain.Post;
using FluentValidation;
using System;
using System.Threading.Tasks;

namespace Final.Validation
{
    public class UserLoginValidation : AbstractValidator<LoginUser>
    {
        public UserLoginValidation()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName should be filled");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password should be filled");
        }

        internal Task ValidateAsync(UserRegistration userRegistration)
        {
            throw new NotImplementedException();
        }
    }
}
