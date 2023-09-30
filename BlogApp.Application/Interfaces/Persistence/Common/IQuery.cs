namespace BlogApp.Application.Interfaces.Persistence.Common;

public interface IQuery<T>
{
    IQueryable<T> Query();
}
