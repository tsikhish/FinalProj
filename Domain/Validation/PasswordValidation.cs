using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
namespace Domain
{
    public class PasswordValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = (string)value;
            if (!char.IsUpper(password[0]))
            {
                return new ValidationResult($"{password} must start with a capital letter");
            }
            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':;[\]{}|<>+-=_]"))
            {
                return new ValidationResult($"{password} must contain at least one symbol");
            }
            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                return new ValidationResult($"{password} must contain at least one number");
            }
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                return new ValidationResult("Password must contain at least one lowercase letter.");
            }
            return ValidationResult.Success;
        }
    }
}
