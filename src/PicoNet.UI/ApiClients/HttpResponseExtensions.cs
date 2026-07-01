using System.Net;
using System.Text.Json;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace PicoNet.UI.ApiClients;

public static class HttpResponseExtensions
{
    public static async Task<List<Error>> ToErrorListAsync(this HttpResponseMessage response, CancellationToken ct)
    {
        // Read content as string first — safe, doesn't consume the stream twice
        var content = await response.Content.ReadAsStringAsync(ct);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            try
            {
                var validation = JsonSerializer.Deserialize<ValidationProblemDetails>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (validation?.Errors is { Count: > 0 })
                {
                    return validation.Errors
                        .SelectMany(kvp => kvp.Value.Select(msg => Error.Validation(kvp.Key, msg)))
                        .ToList();
                }
            }
            catch { /* fallback to ProblemDetails */ }
        }

        try
        {
            var problem = JsonSerializer.Deserialize<ProblemDetails>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Error error = response.StatusCode switch
            {
                HttpStatusCode.NotFound => Error.NotFound(problem?.Title ?? "NotFound", problem?.Detail ?? "Not found."),
                HttpStatusCode.Conflict => Error.Conflict(problem?.Title ?? "Conflict", problem?.Detail ?? "Conflict."),
                HttpStatusCode.Unauthorized => Error.Unauthorized(problem?.Title ?? "Unauthorized", problem?.Detail ?? "Unauthorized."),
                HttpStatusCode.Forbidden => Error.Forbidden(problem?.Title ?? "Forbidden", problem?.Detail ?? "Forbidden."),
                _ => Error.Unexpected(problem?.Title ?? "Unexpected", problem?.Detail ?? content)
            };
            return [error];
        }
        catch
        {
            // If we can't parse JSON at all, return the raw content
            return [Error.Unexpected("ServerError", string.IsNullOrWhiteSpace(content) ? "Something went wrong." : content)];
        }
    }
}