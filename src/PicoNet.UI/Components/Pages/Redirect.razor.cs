using ErrorOr;
using Microsoft.AspNetCore.Components;
using PicoNet.Contracts.DTOs.Responses.Redirect;

namespace PicoNet.UI.Components.Pages;

public partial class Redirect
{
    [Parameter]
    public string ShortCode { get; set; } = "";

    private bool _loading = true;
    private bool _hasPassword = false;
    
    [Parameter]
    public string Password { get; set; } = "";
    
    protected override async Task OnInitializedAsync()
    {
        await RedirectUrl();
    }

    private async Task RedirectUrl()
    {
        ShortCode = "HasPassword";
        var result =
            await RedirectClient.ResolveAsync(ShortCode, Password);

        if (result.IsError)
        {
            if (result.Errors.Any(x => x.Type == ErrorType.Unauthorized))
            {
                _hasPassword = true;
                _loading = false;
                return;
            }
            else
            {
                _loading = false;
                return;
            }
            // Nav.NavigateTo("/not-found");
            // return;
        }

        Nav.NavigateTo(
            result.Value.OriginalUrl,
            forceLoad:true);

        _loading=false;
    }
}