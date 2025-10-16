namespace BlogApp.Domain.Common.Requests;

public class PaginatedRequest
{
    public const int DefaultPageIndex = 0;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    private int pageIndex = DefaultPageIndex;
    private int pageSize = DefaultPageSize;

    public int PageIndex
    {
        get => pageIndex;
        set => pageIndex = value > 0 ? value : DefaultPageIndex;
    }

    public int PageSize
    {
        get => pageSize;
        set
        {
            if (value <= 0)
            {
                pageSize = DefaultPageSize;
                return;
            }

            pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
