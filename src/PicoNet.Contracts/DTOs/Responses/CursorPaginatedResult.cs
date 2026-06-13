namespace PicoNet.Contracts.DTOs.Responses;

public class CursorPaginatedResult<TItem>
{
    public IList<TItem>? Items { get; set; }
    public string? NextCursor { get; set; }
    public string? PreviousCursor { get; set; }
    public bool HasMore { get; set; }
}