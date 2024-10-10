using BlogApp.Domain.Common.Dynamic;

namespace BlogApp.Domain.Common.Requests;

public class DataGridRequest
{
    public PaginatedRequest PaginatedRequest { get; set; }
    public DynamicQuery? DynamicQuery { get; set; }

    public DataGridRequest(PaginatedRequest paginatedRequest, DynamicQuery dynamicQuery)
    {
        PaginatedRequest = paginatedRequest;
        DynamicQuery = dynamicQuery;
    }
}
