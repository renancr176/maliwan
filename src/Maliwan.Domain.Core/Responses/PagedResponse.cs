namespace Maliwan.Domain.Core.Responses;

public class PagedResponse<TData>
{
    public int PageIndex { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<TData> Data { get; set; }

    public PagedResponse()
    {
    }

    public PagedResponse(int pagedIndex, int pageSize, int totalCount, int totalPages, IEnumerable<TData> data)
    {
        PageIndex = pagedIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalPages;
        Data = data;
    }
}