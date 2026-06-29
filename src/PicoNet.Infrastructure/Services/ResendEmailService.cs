using Microsoft.Extensions.Configuration;
using Resend;

namespace PicoNet.Infrastructure.Services;

public sealed class ResendEmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public ResendEmailService(IConfiguration config)
    {
        var apiKey = config["Resend:ApiKey"] 
                     ?? throw new InvalidOperationException("Resend:ApiKey is not configured.");
        
        _fromEmail = config["Resend:FromEmail"] 
                     ?? "onboarding@nimmas.ir";
        _fromName = config["Resend:FromName"] 
                    ?? "PicoNet";

        _resend = ResendClient.Create(apiKey);
    }

    public async Task SendAsync(string to, string subject, string body, bool isHtml = true)
    {
        var message = new EmailMessage
        {
            From = $"{_fromName} <{_fromEmail}>",
            To = to,
            Subject = subject,
        };

        if (isHtml)
            message.HtmlBody = body;
        else
            message.TextBody = body;

        await _resend.EmailSendAsync(message);
    }
}