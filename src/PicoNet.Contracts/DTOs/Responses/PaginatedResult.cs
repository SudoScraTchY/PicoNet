namespace PicoNet.Contracts.DTOs.Responses;

public class OffsetPaginatedResult<TItem>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IList<TItem>? Items { get; set; }
}

public class CursorPaginatedResult<TItem>
{
    public IList<TItem>? Items { get; set; }
    public string? NextCursor { get; set; }
    public string? PreviousCursor { get; set; }
    public bool HasMore { get; set; }
}