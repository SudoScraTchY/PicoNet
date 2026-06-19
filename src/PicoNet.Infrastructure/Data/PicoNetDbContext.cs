using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PicoNet.Domain.Entities;
using PicoNet.Domain.Entities.Common.Interfaces;
using PicoNet.Infrastructure.Identity;

namespace PicoNet.Infrastructure.Data;

public class PicoNetDbContext : IdentityDbContext<ApplicationUser,IdentityRole<Guid>, Guid>
{
    public PicoNetDbContext(DbContextOptions<PicoNetDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PicoNetDbContext).Assembly);
        
        // Global query filter for soft delete
        modelBuilder.Entity<ShortenedUrl>()
            .HasQueryFilter(u => !u.IsDeleted);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // Automatically set audit fields
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                    break;
            }
        }
        
        return await base.SaveChangesAsync(ct);
    }
    
    public DbSet<ShortenedUrl> Urls => Set<ShortenedUrl>();
    public DbSet<UrlVisit> Visits => Set<UrlVisit>();
    
    // Add this when you create User entity:
    // public DbSet<User> Users => Set<User>();
}