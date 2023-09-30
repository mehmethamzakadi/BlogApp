using BlogApp.Application.Interfaces.Persistence.Paging;

namespace BlogApp.Application.Utilities.Responses;

public class GetListResponse<T> : BasePageableModel
{
    private IList<T> _items;

    public IList<T> Items
    {
        get => _items ??= new List<T>();
        set => _items = value;
    }
}