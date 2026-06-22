using System.ComponentModel.DataAnnotations;

namespace PicoNet.Contracts.DTOs.Requests.Auth;

public record RegisterRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;
    
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;
}