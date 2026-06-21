using PicoNet.Contracts.DTOs.Requests;

namespace PicoNet.Application.Features.Shortener.Queries;

public record OffsetPaginatedCommand(UserContext UserContext,int PageNumber, int PageSize);