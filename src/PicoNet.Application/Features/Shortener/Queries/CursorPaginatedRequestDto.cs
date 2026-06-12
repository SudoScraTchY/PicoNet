namespace PicoNet.Application.Features.Shortener.Queries;

public record CursorPaginatedCommand(int PageSize, string? Cursor = null);