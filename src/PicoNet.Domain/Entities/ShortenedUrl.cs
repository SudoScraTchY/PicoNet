using PicoNet.Domain.Entities.Common.Concrete;
using PicoNet.Domain.Enums;
using PicoNet.Domain.Events;
using PicoNet.Domain.IServices;
using PicoNet.Domain.ValueObjects;

namespace PicoNet.Domain.Entities;

public class ShortenedUrl : SoftDeletableAggregateRoot<Guid>
{
    // Core identifiers
    // For full-text search (EF Core will manage this)
    public ShortCode NanoId { get; private set; }
    public string OriginalUrl { get; private set; } = string.Empty;
    public string? CustomAlias { get; private set; } // User-defined custom short codes
    
    // Status & Configuration
    public UrlStatus Status { get; private set; } = UrlStatus.Active;
    public bool IsPermanent { get; private set; }
    public DateTime? ExpiryTime { get; private set; }
    public int MaxClicks { get; private set; } // Rate limiting per link
    public string? Password { get; private set; } // Optional password protection
    
    // Tracking
    public long ClickCount { get; private set; }
    public DateTime? LastAccessedAt { get; private set; }
    
    // Ownership & Organization
    public Guid? UserId { get; private set; } // Nullable for anonymous links
    public string? Tags { get; private set; } // Comma-separated tags for filtering
    public string? Campaign { get; private set; } // UTM campaign tracking
    
    // Security
    public string? AllowedDomains { get; private set; } // Restrict redirect domains
    
    // Navigation properties (for EF Core)
    private readonly List<UrlVisit> _visits = [];
    public IReadOnlyCollection<UrlVisit> Visits => _visits.AsReadOnly();
    
    // Private constructor for EF Core
    private ShortenedUrl() { }
    
    // Factory method
    public static ShortenedUrl Create(
        string originalUrl,
        IShortCodeGenerator codeGenerator,
        Guid? userId = null,
        string? customAlias = null,
        bool isPermanent = false,
        int maxClicks = 0,
        string? password = null,
        string? campaign = null,
        string? tags = null)
    {
        var url = new ShortenedUrl
        {
            Id = Guid.NewGuid(),
            NanoId = customAlias is not null 
                ? new ShortCode(customAlias) 
                : codeGenerator.Generate(),
            OriginalUrl = originalUrl,
            CustomAlias = customAlias,
            UserId = userId,
            ExpiryTime = isPermanent ? DateTime.UtcNow.AddDays(31) : null,
            IsPermanent = isPermanent,
            MaxClicks = maxClicks,
            Password = password,
            Campaign = campaign,
            Tags = tags,
            Status = UrlStatus.Active,
            CreatedBy = userId ?? Guid.Empty
        };
        url.AddDomainEvent(new UrlCreatedDomainEvent(url.Id, url.NanoId, originalUrl, userId));
        
        return url;
    }
    
    public static ShortenedUrl CreateWithShortCode(
        string originalUrl,
        ShortCode shortCode,
        Guid? userId = null,
        string? customAlias = null,
        bool isPermanent = false,
        int maxClicks = 0,
        string? password = null,
        string? campaign = null,
        string? tags = null)
    {
        var url = new ShortenedUrl
        {
            Id = Guid.NewGuid(),
            NanoId = customAlias is not null ? new ShortCode(customAlias) :  shortCode,
            OriginalUrl = originalUrl,
            CustomAlias = customAlias,
            UserId = userId,
            ExpiryTime = isPermanent ? DateTime.UtcNow.AddDays(31) : null,
            IsPermanent = isPermanent,
            MaxClicks = maxClicks,
            Password = password,
            Campaign = campaign,
            Tags = tags,
            Status = UrlStatus.Active,
            CreatedBy = userId ?? Guid.Empty
        };
        url.AddDomainEvent(new UrlCreatedDomainEvent(url.Id, url.NanoId, originalUrl, userId));
        
        return url;
    }
    
    // Domain behaviors
    public void RecordVisit(string? ipAddress, string? userAgent, string? referrer, string? country)
    {
        if (Status != UrlStatus.Active)
            throw new InvalidOperationException("Cannot record visit for inactive URL");
            
        if (ExpiryTime.HasValue && DateTime.UtcNow > ExpiryTime.Value)
        {
            Deactivate();
            throw new InvalidOperationException("URL has expired");
        }
        
        if (MaxClicks > 0 && ClickCount >= MaxClicks)
        {
            Deactivate();
            throw new InvalidOperationException("URL has reached maximum clicks");
        }
        
        ClickCount++;
        LastAccessedAt = DateTime.UtcNow;
        
        var visit = UrlVisit.Create(Id, ipAddress, userAgent, referrer, country);
        _visits.Add(visit);
        
        AddDomainEvent(new UrlVisitedDomainEvent(Id, NanoId, ipAddress, userAgent, DateTime.UtcNow));
    }
    
    public void Deactivate()
    {
        Status = UrlStatus.Inactive;
        AddDomainEvent(new UrlDeactivatedDomainEvent(Id, NanoId));
    }
    
    public void UpdateOriginalUrl(string newUrl)
    {
        if (string.IsNullOrWhiteSpace(newUrl))
            throw new ArgumentException("URL cannot be empty", nameof(newUrl));
            
        OriginalUrl = newUrl;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetPassword(string? password)
    {
        Password = password;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public bool IsExpired()
    {
        return ExpiryTime.HasValue && DateTime.UtcNow > ExpiryTime.Value;
    }
    
    public bool HasReachedMaxClicks()
    {
        return MaxClicks > 0 && ClickCount >= MaxClicks;
    }
    
    public void TogglePermanent()
    {
        IsPermanent = !IsPermanent;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void RenewalUrl(IShortCodeGenerator codeGenerator)
    {
        if (!IsPermanent)
        {
            NanoId = codeGenerator.Generate();
            ExpiryTime = DateTime.UtcNow.AddDays(31);
        }
        UpdatedAt = DateTime.UtcNow;
    }
}