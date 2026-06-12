namespace PicoNet.Contracts.DTOs.Requests;

public class CursorPaginatedRequestDto(int pageSize, string? Cursor = null)
{
    public string? Cursor { get; set; }
    public int PageSize { get; set; } = pageSize;
}