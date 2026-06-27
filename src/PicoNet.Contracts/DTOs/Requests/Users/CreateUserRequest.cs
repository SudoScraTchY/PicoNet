using System.ComponentModel.DataAnnotations;

namespace PicoNet.Contracts.DTOs.Requests.Users;

public record CreateUserRequest
{
    [Required]
    [StringLength(256)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(256, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    public List<string>? Roles { get; set; }
}

