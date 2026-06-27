using System.ComponentModel.DataAnnotations;

namespace PicoNet.Contracts.DTOs.Requests.Users;

public record UpdateUserRequest
{
    [StringLength(256)]
    public string? Username { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public bool? IsActive { get; set; }
}

