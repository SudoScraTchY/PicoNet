namespace PicoNet.UI.Models;

public class UserSession
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    // Add whatever you need from AuthResponse.User
}