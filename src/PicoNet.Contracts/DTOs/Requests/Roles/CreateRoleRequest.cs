using System.ComponentModel.DataAnnotations;

namespace PicoNet.Contracts.DTOs.Requests.Roles;

public record CreateRoleRequest
{
    [Required]
    [StringLength(256)]
    public string RoleName { get; set; } = string.Empty;

    public string? Description { get; set; }
}

