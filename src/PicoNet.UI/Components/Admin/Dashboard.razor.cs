using ErrorOr;
using Microsoft.AspNetCore.Components;
using PicoNet.Contracts.DTOs.Responses.Shortener;

namespace PicoNet.UI.Components.Admin;

public partial class Dashboard : ComponentBase
{
    private readonly List<ShortUrlResponse>? _urls = [];
    private string? _nextCursor;
    private bool _hasMore;

    private bool _initialLoading = true;
    private bool _loadingMore;
    private IList<Error> _errors = [];

    private const int PageSize = 20;

    // OnInitializedAsync fires exactly once when the component mounts.
    // Right place for initial data fetching that doesn't depend on route params.
    protected override async Task OnInitializedAsync()
    {
        await LoadAsync(cursor: null);
        _initialLoading = false;
    }

    private async Task LoadMoreAsync()
    {
        // Guard: don't allow concurrent fetches
        if (_loadingMore || !_hasMore) return;

        _loadingMore = true;
        await LoadAsync(_nextCursor);
        _loadingMore = false;
    }

    private void GoCreatePage()
    {
        Nav.NavigateTo("/create");
    }

    private async Task LoadAsync(string? cursor)
    {
        try
        {
            var result = await ApiClient.GetUrlsAsync(PageSize, cursor);

            if (result.IsError)
            {
                _errors = result.Errors;
                return;
            }

            // Append — never replace — so Load More accumulates
            _urls.AddRange(result.Value.Items ?? []);
            _nextCursor = result.Value.NextCursor;
            _hasMore = result.Value.HasMore;
        }
        catch (Exception ex)
        {
            _errors.Add(Error.Unexpected("CreateUrl.Unexpected", ex.Message));
        }
    }
}