using PicoNet.Contracts.DTOs.Requests;

namespace PicoNet.Application.Features.Shortener.Queries;

public record CursorPaginatedCommand(UserContext UserContext,int PageSize, string? Cursor = null);