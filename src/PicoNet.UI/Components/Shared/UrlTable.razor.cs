using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PicoNet.Contracts.DTOs.Responses.Shortener;

namespace PicoNet.UI.Components.Shared;

public partial class UrlTable
{
    [Parameter]
    public IReadOnlyList<ShortUrlResponse> Urls { get; set; }
        = Array.Empty<ShortUrlResponse>();

    [Parameter]
    public string BaseUrl { get; set; } = "https://nimmas.ir";

    [Parameter]
    public string? EmptyActionLabel { get; set; }

    [Parameter]
    public EventCallback OnEmptyAction { get; set; }
    
    [Parameter]
    public EventCallback<string> OnUrlCopied { get; set; }
        
    private async Task CopyShortCode(string shortCode)
    {
        var fullUrl = $"nimmas.ir/{shortCode}";
            
        try
        {
            await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", fullUrl);
                
            // Optional: Notify parent component
            await OnUrlCopied.InvokeAsync(shortCode);
                
            // Optional: Show toast notification
            // await ShowNotification("Copied to clipboard!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to copy: {ex.Message}");
        }
    }
}