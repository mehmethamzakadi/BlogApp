namespace BlogApp.Application.Features.AppUsers.Queries.GetById;

public sealed record GetByIdAppUserResponse(int Id, string UserName, string Email, DateTimeOffset? LockoutEnd, bool LockoutEnabled, int AccessFailedCount);

