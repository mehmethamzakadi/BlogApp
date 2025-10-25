using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Entities;
using MediatR;
using System.Text;

namespace BlogApp.Application.Features.AppUsers.Queries.Export;

public class ExportUsersQueryHandler : IRequestHandler<ExportUsersQuery, ExportUsersResponse>
{
    private readonly IUserService _userService;

    public ExportUsersQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ExportUsersResponse> Handle(ExportUsersQuery request, CancellationToken cancellationToken)
    {
        var usersResult = await _userService.GetUsers(0, int.MaxValue, cancellationToken);
        var users = usersResult.Items.OrderBy(u => u.Id).ToList();

        var csv = GenerateCsv(users);
        var bytes = Encoding.UTF8.GetBytes(csv);

        return new ExportUsersResponse
        {
            FileContent = bytes,
            FileName = $"users_{DateTime.UtcNow:yyyyMMddHHmmss}.csv",
            ContentType = "text/csv"
        };
    }

    private string GenerateCsv(List<AppUser> users)
    {
        var sb = new StringBuilder();

        // Header
        sb.AppendLine("Id,UserName,Email,PhoneNumber,EmailConfirmed");

        // Rows
        foreach (var user in users)
        {
            sb.AppendLine($"{user.Id},{EscapeCsv(user.UserName!)},{EscapeCsv(user.Email!)},{EscapeCsv(user.PhoneNumber)},{user.EmailConfirmed}");
        }

        return sb.ToString();
    }

    private string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}
