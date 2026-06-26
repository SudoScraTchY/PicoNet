using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PicoNet.Contracts.DTOs.Responses.Shortener;
using PicoNet.UI.ApiClients.Interfaces;

namespace PicoNet.UI.Components.Shared;

public partial class UrlTable
{
    [Parameter]
    public List<ShortUrlResponse> Urls { get; set; } = new();

    [Parameter]
    public string? EmptyActionLabel { get; set; }

    [Parameter]
    public EventCallback OnEmptyAction { get; set; }

    [Parameter]
    public EventCallback<Guid> OnUrlDeleted { get; set; }

    [Inject]
    private IUrlApiClient ApiClient { get; set; } = null!;

    [Inject]
    private NavigationManager Nav { get; set; } = null!;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    private DeleteConfirmationModal? _deleteModal;
    private Guid _deleteUrlId;
    private string _deleteShortCode = string.Empty;

    private async Task CopyShortCode(string shortCode)
    {
        await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", $"https://NimMas.ir/u/{shortCode}");
    }

    private void HandleEdit(Guid urlId)
    {
        Nav.NavigateTo($"/edit/{urlId}");
    }

    private void HandleDelete(Guid urlId)
    {
        var url = Urls.FirstOrDefault(u => u.Id == urlId);
        if (url is not null)
        {
            _deleteUrlId = urlId;
            _deleteShortCode = url.ShortCode;
            _deleteModal?.Show();
        }
    }

    private async Task ConfirmDelete()
    {
        var result = await ApiClient.DeleteAsync(_deleteUrlId);
        if (!result.IsError)
        {
            Urls.RemoveAll(u => u.Id == _deleteUrlId);
            await OnUrlDeleted.InvokeAsync(_deleteUrlId);
        }
    }
}