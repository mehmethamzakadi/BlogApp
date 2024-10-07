using BlogApp.Domain.Common.Paging;
using System.Collections.Generic;

namespace BlogApp.Domain.Common.Responses;

public class GetListResponse<T> : BasePageableModel
{
    private IList<T> _items;

    public IList<T> Items
    {
        get => _items ??= new List<T>();
        set => _items = value;
    }
}