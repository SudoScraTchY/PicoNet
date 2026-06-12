namespace PicoNet.Application.Features.Shortener.Queries;

public record OffsetPaginatedCommand(int PageNumber, int PageSize);