using ErrorOr;

namespace PicoNet.Api.Extensions;

public static class ErrorOrExtension
{
    // PicoNet.Api/Extensions/ErrorOrExtensions.cs
    public static IResult ToProblemResult(this List<Error> errors)
    {
        var first = errors.First();
        return Results.Problem(
            title: first.Code,
            detail: first.Description,
            statusCode: first.Type switch
            {
                ErrorType.NotFound   => 404,
                ErrorType.Conflict   => 409,
                ErrorType.Validation => 400,
                ErrorType.Unauthorized => 401,
                _ => 500
            });
    }
}