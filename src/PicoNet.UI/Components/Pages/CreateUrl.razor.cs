using Microsoft.AspNetCore.Components;
using PicoNet.Contracts.DTOs.Requests.Shortener;

namespace PicoNet.UI.Components.Pages;

public partial class CreateUrl : ComponentBase
{
    private readonly CreateShortUrlRequest _model = new CreateShortUrlRequest();
    private string _tagsRaw = string.Empty; // tags as raw string, parsed on submit
    private bool _submitting;
    private string? _error;

    private async Task HandleSubmitAsync()
    {
        // Guard against double-submit (button disabled but defensive here too)
        if (_submitting) return;

        _submitting = true;
        _error = null;

        try
        {
            // Parse tags from the raw comma-separated string
            _model.Tags = _tagsRaw
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            var result = await ApiClient.CreateAsync(_model);

            if (result is null)
            {
                _error = "Failed to create URL. Please try again.";
                return;
            }

            // Success — navigate to dashboard
            Nav.NavigateTo("/dashboard");
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            // Always re-enable the button, even if something throws
            _submitting = false;
        }
    }
}