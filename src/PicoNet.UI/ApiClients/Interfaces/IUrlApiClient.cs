using ErrorOr;
using PicoNet.Contracts.DTOs.Requests.Shortener;
using PicoNet.Contracts.DTOs.Responses;
using PicoNet.Contracts.DTOs.Responses.Shortener;

namespace PicoNet.UI.ApiClients.Interfaces;

public interface IUrlApiClient
{
    Task<ErrorOr<ShortUrlResponse>> CreateAsync(CreateShortUrlRequest command, CancellationToken ct = default);
    Task<ErrorOr<CursorPaginatedResult<ShortUrlResponse>>> GetUrlsAsync(int pageSize, string? cursor = null,
        CancellationToken ct = default);
    Task<ErrorOr<ShortUrlResponse?>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ErrorOr<ShortUrlResponse>> EditAsync(Guid id, EditShortUrlRequest request, CancellationToken ct = default);
    Task<ErrorOr<bool>> DeleteAsync(Guid id, CancellationToken ct = default);
}