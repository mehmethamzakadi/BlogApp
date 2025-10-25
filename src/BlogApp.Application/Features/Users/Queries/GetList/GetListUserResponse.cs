namespace BlogApp.Application.Features.Users.Queries.GetList;

public sealed record GetListUserResponse(int Id, string UserName, string Email, DateTimeOffset? LockoutEnd, bool LockoutEnabled, int AccessFailedCount);
