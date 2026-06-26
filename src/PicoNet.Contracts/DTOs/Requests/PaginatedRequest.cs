namespace PicoNet.Contracts.DTOs.Requests;

public class OffsetPaginatedRequestDto(int pageNumber, int pageSize)
{
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
}