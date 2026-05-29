using Microsoft.AspNetCore.Http;

namespace PicoNet.Contracts.DTOs.Requests;

public class OffsetPaginatedRequestDto(int pageNumber, int pageSize)
{
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
    public HttpContext? HttpContext { get; set; }
}

public class CursorPaginatedRequestDto(int pageSize, string? Cursor = null)
{
    public string? Cursor { get; set; }
    public int PageSize { get; set; } = pageSize;
}