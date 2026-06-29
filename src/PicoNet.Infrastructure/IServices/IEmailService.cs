namespace PicoNet.Infrastructure.Services;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, bool isHtml = true);
}