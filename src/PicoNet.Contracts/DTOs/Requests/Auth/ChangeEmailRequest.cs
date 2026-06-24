using System.ComponentModel.DataAnnotations;

namespace PicoNet.Contracts.DTOs.Requests.Auth;

public record ChangeEmailRequest()
{
    [Required,EmailAddress]
    public string NewEmail { get; set; }
}