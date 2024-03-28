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
        public DbSet<Accountant> Accountant { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Loan>()
           .HasOne(l => l.User)
           .WithMany(x=>x.Loans) 
           .HasForeignKey(l => l.UserId);
            modelBuilder.Entity<User>()
                .HasOne(x => x.Accountant)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.AccId);
        }
    }

}

