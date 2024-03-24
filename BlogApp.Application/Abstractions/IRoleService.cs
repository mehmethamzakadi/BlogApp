namespace BlogApp.Application.Abstractions;

public interface IRoleService
{
    IDictionary<int, string?>? GetAllRoles();
    Task<(int id, string name)> GetRoleById(int id);
    Task<bool> CreateRole(string name);
    Task<bool> DeleteRole(string name);
    Task<bool> UpdateRole(int id, string name);
}
