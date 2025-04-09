
using MortgageLoanProcessing.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MortgageLoanProcessing.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<Loan> Loans { get; set; }
            public DbSet<InterestRate> InterestRates { get; set; }
            public DbSet<AmortizationSchedule> AmortizationSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cascade Delete: If User is deleted, Loans will be deleted
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cascade Delete: If Loan is deleted, AmortizationSchedule will be deleted
            modelBuilder.Entity<AmortizationSchedule>()
                .HasOne(a => a.Loan)
                .WithMany(l => l.AmortizationSchedules)
                .HasForeignKey(a => a.LoanId)
                .OnDelete(DeleteBehavior.Cascade);

            //// Enforce ON UPDATE CASCADE in SQL
            //modelBuilder.Entity<Loan>()
            //    .HasOne(l => l.User)
            //    .WithMany(u => u.Loans)
            //    .HasForeignKey(l => l.UserId)
            //    .OnUpdate(DeleteBehavior.Cascade);

            //modelBuilder.Entity<AmortizationSchedule>()
            //    .HasOne(a => a.Loan)
            //    .WithMany(l => l.AmortizationSchedules)
            //    .HasForeignKey(a => a.LoanId)
            //    .OnUpdate(DeleteBehavior.Cascade);
        }

    }
}

