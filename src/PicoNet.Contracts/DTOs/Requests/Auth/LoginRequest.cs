using System.ComponentModel.DataAnnotations;

namespace PicoNet.Contracts.DTOs.Requests.Auth;

public record LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}