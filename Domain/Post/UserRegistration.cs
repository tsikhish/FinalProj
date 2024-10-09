using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post
{
    public class UserRegistration
    {
        [Required(ErrorMessage ="FirstName is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Age is required")]
        [Range(18, int.MaxValue, ErrorMessage = "Only 18+ is required.")]
        public int Age { get; set; }
        
        [Required(ErrorMessage = "Salary is required")]
        [Range(0,int.MaxValue,ErrorMessage ="Salary can't be negative")]
        public int Salary { get; set; }
        [EmailAddress(ErrorMessage ="Please input right email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100,MinimumLength =6,ErrorMessage ="Password must be at least 6 characters long")]
        [PasswordValidation]
        public string Password { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public Role Role { get; set; }
    }

}

