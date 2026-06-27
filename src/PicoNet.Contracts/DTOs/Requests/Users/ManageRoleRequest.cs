using System.ComponentModel.DataAnnotations;

namespace PicoNet.Contracts.DTOs.Requests.Users;

public record ManageRoleRequest
{
    [Required]
    [StringLength(256)]
    public string RoleName { get; set; } = string.Empty;
}

