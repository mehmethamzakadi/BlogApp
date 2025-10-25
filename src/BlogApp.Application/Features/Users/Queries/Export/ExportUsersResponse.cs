namespace BlogApp.Application.Features.Users.Queries.Export;

public class ExportUsersResponse
{
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "text/csv";
}
