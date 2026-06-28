using System.ComponentModel.DataAnnotations;

namespace PicoNet.Contracts.DTOs.Requests.Auth;

public record ValidateRegistrationRequest
{
    public ValidateRegistrationRequest(string id, string token)
    {
        Id = id;
        Token = token;
    }

    [Required]
    public string Id { get; set; }

    [Required]
    public string Token { get; set; }
}