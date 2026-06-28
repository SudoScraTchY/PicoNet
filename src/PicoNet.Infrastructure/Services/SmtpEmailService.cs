using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace PicoNet.Infrastructure.Services;

public sealed class SmtpEmailService : IEmailService
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string? _username;
    private readonly string? _password;
    private readonly bool _enableSsl;

    public SmtpEmailService(IConfiguration config)
    {
        var section = config.GetSection("Smtp");
        _host = section["Host"]!;
        _port = int.Parse(section["Port"]!);
        _fromEmail = section["FromEmail"]!;
        _fromName = section["FromName"]!;
        _username = section["Username"];
        _password = section["Password"];
        _enableSsl = bool.Parse(section["EnableSsl"] ?? "true");
    }

    public async Task SendAsync(string to, string subject, string body, bool isHtml = true)
    {
        using var client = new SmtpClient(_host, _port);
        client.EnableSsl = _enableSsl;
        client.Credentials = new NetworkCredential(_username, _password);
        client.DeliveryMethod = SmtpDeliveryMethod.Network;

        var message = new MailMessage
        {
            From = new MailAddress(_fromEmail, _fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };
        message.To.Add(to);

        await client.SendMailAsync(message);
    }
}