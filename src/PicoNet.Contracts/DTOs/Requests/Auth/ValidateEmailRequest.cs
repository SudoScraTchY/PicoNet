using System.ComponentModel.DataAnnotations;

namespace PicoNet.Contracts.DTOs.Requests.Auth;

public record ValidateRegistrationRequest
{
    [Required,EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Token { get; set; }
}