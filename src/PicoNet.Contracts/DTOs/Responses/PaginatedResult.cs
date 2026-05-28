namespace PicoNet.Contracts.DTOs.Responses;

public class PaginatedResult<TItem>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IList<TItem>? Items { get; set; }
}