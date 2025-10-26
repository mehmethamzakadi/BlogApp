namespace BlogApp.Application.Features.Users.Queries.GetById;

public sealed record GetByIdUserResponse(Guid Id, string UserName, string Email, DateTimeOffset? LockoutEnd, bool LockoutEnabled, int AccessFailedCount);

