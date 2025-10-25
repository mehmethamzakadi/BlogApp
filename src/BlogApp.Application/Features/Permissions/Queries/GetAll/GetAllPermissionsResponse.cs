namespace BlogApp.Application.Features.Permissions.Queries.GetAll;

public class GetAllPermissionsResponse
{
    public List<PermissionModuleDto> Modules { get; set; } = new();
}

public class PermissionModuleDto
{
    public string ModuleName { get; set; } = string.Empty;
    public List<PermissionDto> Permissions { get; set; } = new();
}

public class PermissionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
