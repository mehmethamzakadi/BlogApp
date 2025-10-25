using MediatR;

namespace BlogApp.Application.Features.AppUsers.Queries.Export;

public class ExportUsersQuery : IRequest<ExportUsersResponse>
{
    public string Format { get; set; } = "csv"; // csv or excel
}
