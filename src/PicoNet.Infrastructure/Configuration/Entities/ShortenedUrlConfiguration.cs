using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;
using PicoNet.Domain.Entities;
using PicoNet.Domain.ValueObjects;

namespace PicoNet.Infrastructure.Configuration.Entities;

public class ShortenedUrlConfiguration : IEntityTypeConfiguration<ShortenedUrl>
{
    public void Configure(EntityTypeBuilder<ShortenedUrl> builder)
    {
        builder.ToTable("ShortenedUrls");
        
        // Primary Key
        builder.HasKey(u => u.Id);
        
        // Properties
        // Value Object mapping for NanoId
        builder.Property(u => u.NanoId)
            .HasConversion(
                code => code.Value,           
                value => new ShortCode(value))
            .HasColumnName("NanoId")
            .IsRequired()
            .HasMaxLength(20)
            .IsUnicode(false);

        
        builder.Property(u => u.OriginalUrl)
            .IsRequired()
            .HasMaxLength(2048);
        
        builder.Property(u => u.CustomAlias)
            .HasMaxLength(50)
            .IsUnicode(false);
        
        builder.Property(u => u.Tags)
            .HasMaxLength(500);
        
        builder.Property(u => u.Campaign)
            .HasMaxLength(200);
        
        builder.Property(u => u.Password)
            .HasMaxLength(100);
        
        builder.Property(u => u.AllowedDomains)
            .HasMaxLength(1000);
        
        // Auditable shadow properties (already in entity, but we configure them)
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");
        
        builder.Property(u => u.CreatedBy)
            .IsRequired();
        
        builder.Property(u => u.UpdatedAt);
        builder.Property(u => u.UpdatedBy);
        
        // Soft delete
        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(u => u.DeletedAt);
        
        // Indexes
        builder.HasIndex(u => u.NanoId)
            .IsUnique()
            .HasDatabaseName("IX_shortened_urls_nano_id");
        
        builder.HasIndex(u => u.CustomAlias)
            .IsUnique()
            .HasFilter("\"CustomAlias\" IS NOT NULL")
            .HasDatabaseName("IX_shortened_urls_custom_alias");
        
        builder.HasIndex(u => u.UserId)
            .HasDatabaseName("IX_shortened_urls_user_id");
        
        builder.HasIndex(u => u.Status)
            .HasDatabaseName("IX_shortened_urls_status");
        
        builder.HasIndex(u => u.ExpiryTime)
            .HasDatabaseName("IX_shortened_urls_expiry_time");
        
        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("IX_shortened_urls_created_at");
        
        // Composite index for common queries
        builder.HasIndex(u => new { u.UserId, u.Status, u.IsDeleted })
            .HasDatabaseName("IX_shortened_urls_user_status_deleted");
        
        ConfigureFullTextSearch(builder);
    }
    
    private void ConfigureFullTextSearch(EntityTypeBuilder<ShortenedUrl> builder)
    {
        // Step 1: Define the shadow property with NpgsqlTsVector type
        builder.Property<NpgsqlTsVector>("SearchVector")
            .HasColumnType("tsvector")
            .HasComputedColumnSql(
                """to_tsvector('english', coalesce("OriginalUrl", '') || ' ' || coalesce("CustomAlias", '') || ' ' || coalesce("Tags", ''))""",
                stored: true); // Stored generated column for better performance
        
        // Step 2: Create GIN index on the tsvector column
        builder.HasIndex("SearchVector")
            .HasMethod("GIN")
            .HasDatabaseName("IX_shortened_urls_search");
    }
    
}