using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Booking entity
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            entity.HasOne(e => e.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Room entity
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.RoomNumber).IsUnique();
            entity.Property(e => e.PricePerNight).HasPrecision(18, 2);
        });

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}
