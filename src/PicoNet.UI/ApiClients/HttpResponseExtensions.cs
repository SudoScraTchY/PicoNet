using System.Net;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace PicoNet.UI.ApiClients;

public static class HttpResponseExtensions
{
    public static async Task<List<Error>> ToErrorListAsync(this HttpResponseMessage response, CancellationToken ct)
    {
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var validation = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: ct);
            if (validation?.Errors is { Count: > 0 })
            {
                return validation.Errors
                    .SelectMany(kvp => kvp.Value.Select(msg => Error.Validation(kvp.Key, msg)))
                    .ToList();
            }
        }

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken: ct);
        Error error = response.StatusCode switch
        {
            HttpStatusCode.NotFound => Error.NotFound(problem?.Title ?? "NotFound", problem?.Detail ?? "Not found."),
            HttpStatusCode.Conflict => Error.Conflict(problem?.Title ?? "Conflict", problem?.Detail ?? "Conflict."),
            HttpStatusCode.Unauthorized => Error.Unauthorized(problem?.Title ?? "Unauthorized", problem?.Detail ?? "Unauthorized."),
            _ => Error.Unexpected(problem?.Title ?? "Unexpected", problem?.Detail ?? "Something went wrong.")
        };
        return [error];
    }
}