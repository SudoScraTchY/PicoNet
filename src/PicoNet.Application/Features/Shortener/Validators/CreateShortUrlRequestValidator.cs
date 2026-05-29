using FluentValidation;
using PicoNet.Contracts.DTOs.Requests;
using PicoNet.Contracts.DTOs.Requests.Shortener;

namespace PicoNet.Application.Features.Shortener.Validators;

public class CreateShortUrlRequestValidator : AbstractValidator<CreateShortUrlRequest>
{
    public CreateShortUrlRequestValidator()
    {
        RuleFor(x => x.OriginalUrl)
            .NotEmpty()
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("A valid absolute URL is required.");

        RuleFor(x => x.CustomAlias)
            .Matches(@"^[a-zA-Z0-9_-]{4,20}$")
            .When(x => !string.IsNullOrEmpty(x.CustomAlias))
            .WithMessage("Alias must be 4–20 alphanumeric, dash, or underscore characters.");
    }
}