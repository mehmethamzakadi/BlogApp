using System.Linq;

namespace BlogApp.Domain.Common;

public interface IQuery<T>
{
    IQueryable<T> Query();
}
