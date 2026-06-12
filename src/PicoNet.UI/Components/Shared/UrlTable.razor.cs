using Microsoft.AspNetCore.Components;
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
}