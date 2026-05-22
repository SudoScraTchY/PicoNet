using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PicoNet.Domain.Entities;

namespace PicoNet.Infrastructure.Configuration.Entities;

public class UrlVisitConfiguration : IEntityTypeConfiguration<UrlVisit>
{
    public void Configure(EntityTypeBuilder<UrlVisit> builder)
    {
        builder.ToTable("UrlVisits");
        
        builder.HasKey(v => v.Id);
        
        builder.Property(v => v.IpAddress)
            .HasMaxLength(45); // IPv6 support
        
        builder.Property(v => v.UserAgent)
            .HasMaxLength(500);
        
        builder.Property(v => v.Referrer)
            .HasMaxLength(2048);
        
        builder.Property(v => v.Country)
            .HasMaxLength(100);
        
        builder.Property(v => v.VisitedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");
        
        // Relationships
        builder.HasOne(v => v.ShortenedUrl)
            .WithMany(u => u.Visits)
            .HasForeignKey(v => v.ShortenedUrlId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes for analytics queries
        builder.HasIndex(v => v.VisitedAt)
            .HasDatabaseName("IX_url_visits_visited_at");
        
        builder.HasIndex(v => v.ShortenedUrlId)
            .HasDatabaseName("IX_url_visits_shortened_url_id");
        
        builder.HasIndex(v => new { v.ShortenedUrlId, v.VisitedAt })
            .HasDatabaseName("IX_url_visits_url_visited");
        
        builder.HasIndex(v => v.Country)
            .HasDatabaseName("IX_url_visits_country");
    }
}