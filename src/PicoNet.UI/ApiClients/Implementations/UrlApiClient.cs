using PicoNet.Contracts.DTOs.Requests.Shortener;
using PicoNet.Contracts.DTOs.Responses;
using PicoNet.Contracts.DTOs.Responses.Shortener;
using PicoNet.UI.ApiClients.Interfaces;
using ErrorOr;

namespace PicoNet.UI.ApiClients.Implementations;

public sealed class UrlApiClient : IUrlApiClient
{
    private readonly HttpClient _http;

    public UrlApiClient(HttpClient http) => _http = http;

    public async Task<ErrorOr<CursorPaginatedResult<ShortUrlResponse>>> GetUrlsAsync(int pageSize,
        string? cursor = null, CancellationToken ct = default)
    {
        var url = cursor is null
            ? $"/api/shortener?pageSize={pageSize}"
            : $"/api/shortener?pageSize={pageSize}&cursor={Uri.EscapeDataString(cursor)}";

        return await _http.GetFromJsonAsync<CursorPaginatedResult<ShortUrlResponse>>(url, ct) ??
               (ErrorOr<CursorPaginatedResult<ShortUrlResponse>>)Error.Unexpected("Url.Unexpected","Server returned an empty response.");
    }

    public async Task<ErrorOr<ShortUrlResponse?>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<ShortUrlResponse>($"/api/shortener/{id}", ct);
    }

    public async Task<ErrorOr<ShortUrlResponse>> EditAsync(Guid urlId, EditShortUrlRequest request,
        CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync($"/api/shortener/{urlId}", request, ct);
        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);
        
        return await response.Content.ReadFromJsonAsync<ShortUrlResponse>(ct) ??
               (ErrorOr<ShortUrlResponse>)Error.Unexpected("Url.Unexpected","Server returned an empty response.");
    }

    public async Task<ErrorOr<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"/api/shortener/{id}", ct);
        
        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);
        
        return await response.Content.ReadFromJsonAsync<bool?>(ct) 
               ?? (ErrorOr<bool>)Error.Unexpected("Url.Unexpected","Server returned an empty response.");
    }

    public async Task<ErrorOr<ShortUrlResponse>> CreateAsync(CreateShortUrlRequest command,
        CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/api/shortener", command, ct);
        
        if (!response.IsSuccessStatusCode)
            return await response.ToErrorListAsync(ct);
        
        return await response.Content.ReadFromJsonAsync<ShortUrlResponse>(cancellationToken: ct) ??
               (ErrorOr<ShortUrlResponse>)Error.Unexpected("Url.Unexpected","Server returned an empty response.");
    }
}