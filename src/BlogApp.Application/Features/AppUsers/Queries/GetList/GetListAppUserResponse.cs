namespace BlogApp.Application.Features.AppUsers.Queries.GetList;

public sealed record GetListAppUserResponse(int Id, string UserName, string Email, DateTimeOffset? LockoutEnd, bool LockoutEnabled, int AccessFailedCount);
