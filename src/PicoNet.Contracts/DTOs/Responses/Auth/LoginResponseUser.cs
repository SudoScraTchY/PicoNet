namespace PicoNet.Contracts.DTOs.Responses.Auth;

public record AuthResponseUser
{
    public Guid Id { get; set; }   // ← add this
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
}