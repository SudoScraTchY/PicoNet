using PicoNet.Contracts.DTOs.Requests;
using PicoNet.Contracts.DTOs.Requests.Auth;

namespace PicoNet.Application.Features.Shortener.Queries;

public record OffsetPaginatedCommand(UserContext UserContext,int PageNumber, int PageSize);