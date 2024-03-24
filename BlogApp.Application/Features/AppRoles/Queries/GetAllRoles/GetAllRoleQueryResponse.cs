namespace BlogApp.Application.Features.AppRoles.Queries.GetAllRoles;

public sealed record GetAllRoleQueryResponse(IDictionary<int, string?>? Datas);