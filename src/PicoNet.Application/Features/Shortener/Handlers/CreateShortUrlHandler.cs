using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PicoNet.Application.Features.Shortener.Commands;
using PicoNet.Contracts.DTOs.Responses;
using PicoNet.Domain.Entities;
using PicoNet.Domain.Enums;
using PicoNet.Domain.IServices;
using PicoNet.Domain.ValueObjects;
using PicoNet.Infrastructure.Data;

namespace PicoNet.Application.Features.Shortener.Handlers;

public class CreateShortUrlHandler
{
    private readonly PicoNetDbContext _db;
    private readonly IShortCodeGenerator _codeGenerator;
    private readonly ILogger<CreateShortUrlHandler> _logger;

    public CreateShortUrlHandler(PicoNetDbContext db, IShortCodeGenerator codeGenerator, ILogger<CreateShortUrlHandler> logger)
    {
        _db = db;
        _codeGenerator = codeGenerator;
        _logger = logger;
    }

    public async Task<ErrorOr<ShortUrlResponse>> Handle(CreateShortUrlCommand command)
    {
        // 1. Generate or use custom alias
        ShortCode shortCode;
        if (!string.IsNullOrWhiteSpace(command.CustomAlias))
        {
            bool aliasExists = await _db.Urls.AnyAsync(u => u.CustomAlias == command.CustomAlias);
            if (aliasExists)
                return Error.Conflict("CustomAlias.AlreadyExists", $"Alias '{command.CustomAlias}' is already in use.");
            shortCode = new ShortCode(command.CustomAlias);
        }
        else
        {
            string code;
            do
            {
                code = _codeGenerator.Generate();
            } while (await _db.Urls.AnyAsync(u => u.NanoId.Value == code));
            shortCode = new ShortCode(code);
        }

        // 2. Create the aggregate
        var shortenedUrl = ShortenedUrl.Create(
            command.OriginalUrl,
            _codeGenerator,
            userId: null,             // no user yet
            customAlias: command.CustomAlias,
            tags: command.Tags != null ? string.Join(", ", command.Tags) : null 
        );

        _db.Urls.Add(shortenedUrl);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Created short URL {Code}", shortCode.Value);

        // 3. Map to response DTO (use Mapperly)
        return new ShortUrlResponse(
            shortenedUrl.Id,
            shortCode.Value,
            shortenedUrl.OriginalUrl,
            shortenedUrl.CustomAlias,
            shortenedUrl.CreatedAt,
            shortenedUrl.ExpiryTime,
            shortenedUrl.Status == UrlStatus.Active,
            shortenedUrl.Tags?.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList(),
            0 // visit count is 0 initially
        );
    }
}