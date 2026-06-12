using Microsoft.AspNetCore.Components;

namespace PicoNet.UI.Components.Pages;

public partial class Redirect
{
    [Parameter]
    public string ShortCode { get; set; } = "";

    bool _loading=true;

    protected override async Task OnInitializedAsync()
    {
        var result =
            await RedirectClient.ResolveAsync(ShortCode);

        if (result.IsError)
        {
            Nav.NavigateTo("/not-found");
            return;
        }

        Nav.NavigateTo(
            result.Value.OriginalUrl,
            forceLoad:true);

        _loading=false;
    }
}