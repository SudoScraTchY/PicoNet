using PicoNet.Contracts.DTOs.Requests;
using PicoNet.Contracts.DTOs.Requests.Auth;

namespace PicoNet.Application.Features.Shortener.Queries;

public record CursorPaginatedCommand(UserContext UserContext,int PageSize, string? Cursor = null);