using Domain;
using Microsoft.EntityFrameworkCore;
namespace Data
{
    public class PersonContext : DbContext
    {
        public PersonContext(DbContextOptions<PersonContext> options)
        : base(options)
        {

        }
        public DbSet<User> AppUsers { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<PaymentHistory> Payment {get;set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}


