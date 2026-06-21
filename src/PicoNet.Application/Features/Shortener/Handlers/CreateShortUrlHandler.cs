using System.Security.Cryptography;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using PicoNet.Application.Features.Shortener.Commands;
using PicoNet.Application.Mappings;
using PicoNet.Contracts.DTOs.Requests.Shortener;
using PicoNet.Contracts.DTOs.Responses;
using PicoNet.Contracts.DTOs.Responses.Shortener;
using PicoNet.Domain.Entities;
using PicoNet.Domain.Enums;
using PicoNet.Domain.ValueObjects;
using PicoNet.Infrastructure.Data;
using PicoNet.Infrastructure.IServices;

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
        var shortCode = _codeGenerator.Generate();
        if (!string.IsNullOrWhiteSpace(command.CustomAlias))
        {
            var aliasExists = await _db.Urls.AnyAsync(u => u.CustomAlias == command.CustomAlias && !u.IsDeleted);
            if (aliasExists)
                return Error.Conflict("CustomAlias.AlreadyExists", $"Alias '{command.CustomAlias}' is already in use.");
            shortCode = new ShortCode(command.CustomAlias);
        }
        else
        {
            const int maxAttempts = 5;
            for (var i = 0; i < maxAttempts; i++)
            {
                shortCode = _codeGenerator.Generate();
                if (!await _db.Urls.AnyAsync(u => u.NanoId == shortCode &&  !u.IsDeleted))
                    break;
                if (i == maxAttempts - 1)
                    return Error.Unexpected("ShortCode.GenerationFailed", "Could not generate a unique code.");
            }
        }

        // temporary until we create Password Hasher Service
        string passwordHash;
        if (command.Password != null)
        {
            passwordHash = command.Password;
        }

        // 2. Create the aggregate
        var shortenedUrl = ShortenedUrl.Create(
            command.OriginalUrl,
            shortCode,
            userId: command.UserContext.UserId,
            password: command.Password,
            customAlias: command.CustomAlias,
            tags: command.Tags != null ? string.Join(", ", command.Tags) : null 
        );

        try
        {
            await _db.Urls.AddAsync(shortenedUrl);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == "23505")
        {
            return Error.Conflict("ShortCode.AlreadyExists", "This alias is already taken.");
        }

        _logger.LogInformation("Created short URL {Code}", shortCode.Value);

        // 3. Map to response DTO (use Mapperly)
        return shortenedUrl.ToShortUrlResponse();
    }
}