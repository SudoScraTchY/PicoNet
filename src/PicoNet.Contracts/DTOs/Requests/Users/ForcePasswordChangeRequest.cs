using System.ComponentModel.DataAnnotations;

namespace PicoNet.Contracts.DTOs.Requests.Users;

public record ForcePasswordChangeRequest
{
    [Required]
    [StringLength(256, MinimumLength = 8)]
    public string NewPassword { get; set; } = string.Empty;

    public bool? SendEmailNotification { get; set; } = true;
}

