using Domain;
using Domain.Post;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Loan>()
           .HasOne(l => l.User)
           .WithMany(x => x.Loans)
           .HasForeignKey(l => l.UserId);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
        .Property(u => u.Role)
        .HasConversion<int>() 
        .IsRequired();
        }
    }
}


