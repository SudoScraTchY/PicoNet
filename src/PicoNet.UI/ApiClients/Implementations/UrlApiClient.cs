using PicoNet.Contracts.DTOs.Requests.Shortener;
using PicoNet.Contracts.DTOs.Responses;
using PicoNet.Contracts.DTOs.Responses.Shortener;
using PicoNet.UI.ApiClients.Interfaces;

namespace PicoNet.UI.ApiClients.Implementations;

public sealed class UrlApiClient : IUrlApiClient
{
    private readonly HttpClient _http;

    public UrlApiClient(HttpClient http) => _http = http;

    public async Task<CursorPaginatedResult<ShortUrlResponse>?> GetUrlsAsync(
        int pageSize, string? cursor = null, CancellationToken ct = default)
    {
        var url = cursor is null
            ? $"/api/shortener?pageSize={pageSize}"
            : $"/api/shortener?pageSize={pageSize}&cursor={Uri.EscapeDataString(cursor)}";

        return await _http.GetFromJsonAsync<CursorPaginatedResult<ShortUrlResponse>>(url, ct);
    }

    public async Task<ShortUrlResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<ShortUrlResponse>($"/api/shortener/{id}", ct);
    }

    public async Task<ShortUrlResponse?> EditAsync(Guid urlId, EditShortUrlRequest request, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync($"/api/shortener/{urlId}", request, ct);
        if (!response.IsSuccessStatusCode) return null; // or throw, or return ErrorOr
        return await response.Content.ReadFromJsonAsync<ShortUrlResponse>(ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"/api/shortener/{id}", ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<ShortUrlResponse?> CreateAsync(CreateShortUrlRequest command, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/api/shortener", command, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ShortUrlResponse>(cancellationToken: ct);
    }
}