namespace BlogApp.Application.Features.Roles.Commands.BulkDelete;

public class BulkDeleteRolesResponse
{
    public int DeletedCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
