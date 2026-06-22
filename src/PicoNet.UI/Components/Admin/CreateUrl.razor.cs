using ErrorOr;
using Microsoft.AspNetCore.Components;
using PicoNet.Contracts.DTOs.Requests.Shortener;

namespace PicoNet.UI.Components.Admin;

public partial class CreateUrl : ComponentBase
{
    private readonly CreateShortUrlRequest _model = new CreateShortUrlRequest();
    private string _tagsRaw = string.Empty;
    private bool _submitting;
    private IList<Error> _errors = [];

    private async Task HandleSubmitAsync()
    {
        if (_submitting) return;

        _submitting = true;
        _errors = [];

        try
        {
            _model.Tags = _tagsRaw
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            var result = await ApiClient.CreateAsync(_model);

            if (result.IsError)
            {
                _errors = result.Errors;
                return;
            }

            Nav.NavigateTo("/dashboard");
        }
        catch (Exception ex)
        {
            _errors.Add(Error.Unexpected("CreateUrl.Unexpected", ex.Message));
        }
        finally
        {
            _submitting = false;
        }
    }
}