using Microsoft.AspNetCore.Components;
using PicoNet.Contracts.DTOs.Responses.Shortener;

namespace PicoNet.UI.Components.Pages;

public partial class Dashboard : ComponentBase
{
    // ─── State ────────────────────────────────────────────────
    private readonly List<ShortUrlResponse> _items = [];
    private string? _nextCursor;
    private bool _hasMore;

    private bool _initialLoading = true;
    private bool _loadingMore;
    private string? _error;

    private const int PageSize = 20;

    // ─── Lifecycle ────────────────────────────────────────────
    
    // OnInitializedAsync fires exactly once when the component mounts.
    // Right place for initial data fetching that doesn't depend on route params.
    protected override async Task OnInitializedAsync()
    {
        await LoadAsync(cursor: null);
        _initialLoading = false;
    }

    // ─── Actions ──────────────────────────────────────────────

    private async Task LoadMoreAsync()
    {
        // Guard: don't allow concurrent fetches
        if (_loadingMore || !_hasMore) return;

        _loadingMore = true;
        await LoadAsync(_nextCursor);
        _loadingMore = false;
    }

    // ─── Core fetch — used by both initial load and load more ─
    
    private async Task LoadAsync(string? cursor)
    {
        try
        {
            var result = await ApiClient.GetUrlsAsync(PageSize, cursor);

            if (result is null)
            {
                _error = "Failed to load URLs. Please try again.";
                return;
            }

            // Append — never replace — so Load More accumulates
            _items.AddRange(result.Items ?? []);
            _nextCursor = result.NextCursor;
            _hasMore = result.HasMore;
        }
        catch (Exception ex)
        {
            _error = $"Something went wrong: {ex.Message}";
        }
    }
}