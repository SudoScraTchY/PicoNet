using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PicoNet.Domain.Entities;
using PicoNet.Domain.Entities.Common.Interfaces;
using PicoNet.Infrastructure.Identity;
using PicoNet.Infrastructure.Identity.Entities;

namespace PicoNet.Infrastructure.Data;

public class PicoNetDbContext : IdentityDbContext<ApplicationUser,IdentityRole<Guid>, Guid>
{
    public PicoNetDbContext(DbContextOptions<PicoNetDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PicoNetDbContext).Assembly);

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
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}