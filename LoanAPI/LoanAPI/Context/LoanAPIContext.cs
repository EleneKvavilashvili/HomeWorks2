using LoanAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LoanAPI.Context
{
    public class LoanAPIContext : DbContext
    {
        public LoanAPIContext(DbContextOptions<LoanAPIContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<ActionLog> ActionLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ActionLog>()
                .Property(a => a.ActionName)
                .HasConversion<string>();

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Loan>()
                .Property(l => l.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Loan>()
                .Property(l => l.Currency)
                .HasConversion<string>();

            modelBuilder.Entity<Loan>()
                .Property(l => l.Status)
                .HasConversion<string>();
        }
    }
}
