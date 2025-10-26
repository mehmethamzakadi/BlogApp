namespace BlogApp.Application.Features.Users.Queries.GetUserRoles;

public class GetUserRolesResponse
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<UserRoleDto> Roles { get; set; } = new();
}

public class UserRoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
