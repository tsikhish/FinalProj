using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace Domain
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public int Salary { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsBlocked { get; set; } = false;
        public ICollection<Loan> Loans { get; set; }
        public int FailedLoggingAttempts { get; set; } = 0;
        public DateTime? LockoutEndTime { get; set; } 
        public bool IsBlockedForFailedAttempts { get; set; } = false;
        [EnumDataType(typeof(Role))]
        public Role Role { get; set; }

    }
}
public enum Role
{
    Admin=1,
    User=2
}

