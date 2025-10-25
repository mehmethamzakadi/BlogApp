namespace BlogApp.Application.Features.AppUsers.Commands.BulkDelete;

public class BulkDeleteUsersResponse
{
    public int DeletedCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
