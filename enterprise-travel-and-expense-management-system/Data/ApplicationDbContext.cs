using Microsoft.EntityFrameworkCore;
using enterprise_travel_and_expense_management_system.Models.Entities;

namespace enterprise_travel_and_expense_management_system.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<TravelRequest> TravelRequests { get; set; }

    public DbSet<Expense> Expenses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Use SQLite for cross-platform support
            optionsBuilder.UseSqlite("Data Source=TravelExpenseManagement.db");
        }
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Role).IsRequired();
        });

        // TravelRequest entity configuration
        modelBuilder.Entity<TravelRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Destination).IsRequired().HasMaxLength(256);
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.EndDate).IsRequired();
            entity.Property(e => e.Status).IsRequired();

            // Foreign key and relationship
            entity.HasOne(e => e.User)
                .WithMany(u => u.TravelRequests)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many with Expenses
            entity.HasMany(e => e.Expenses)
                .WithOne(ex => ex.TravelRequest)
                .HasForeignKey(ex => ex.TravelRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Expense entity configuration
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(512);

            // Foreign key configuration handled in TravelRequest
        });
    }
}
