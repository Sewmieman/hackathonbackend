using FixMyCampus.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FixMyCampus.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<TicketStatusHistory> TicketStatusHistories => Set<TicketStatusHistory>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ticket>()
            .Property(x => x.Status)
            .HasConversion<string>();

        modelBuilder.Entity<TicketStatusHistory>()
            .Property(x => x.FromStatus)
            .HasConversion<string>();

        modelBuilder.Entity<TicketStatusHistory>()
            .Property(x => x.ToStatus)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .Property(x => x.Role)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Ticket>()
            .HasMany(x => x.History)
            .WithOne(x => x.Ticket)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}