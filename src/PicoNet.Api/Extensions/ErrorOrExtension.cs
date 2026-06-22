using ErrorOr;

namespace PicoNet.Api.Extensions;

public static class ErrorOrExtension
{
    public static IResult ToProblemResult(this List<Error> errors)
    {
        var first = errors[0];

        if (first.Type == ErrorType.Validation)
        {
            var grouped = errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

            return Results.ValidationProblem(grouped);
        }

        return Results.Problem(
            title: first.Code,
            detail: first.Description,
            statusCode: first.Type switch
            {
                ErrorType.NotFound => 404,
                ErrorType.Conflict => 409,
                ErrorType.Unauthorized => 401,
                _ => 500
            });
    }
}