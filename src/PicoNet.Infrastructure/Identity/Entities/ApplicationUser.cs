// PicoNet.Infrastructure/Identity/ApplicationUser.cs

using Microsoft.AspNetCore.Identity;

namespace PicoNet.Infrastructure.Identity.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}