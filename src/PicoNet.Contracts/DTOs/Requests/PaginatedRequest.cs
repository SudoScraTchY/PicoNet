using Microsoft.AspNetCore.Http;

namespace PicoNet.Contracts.DTOs.Requests;

public class PaginatedRequestDto(int pageNumber, int pageSize,HttpContext? httpContext = null)
{
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
    public HttpContext? HttpContext { get; set; }
}