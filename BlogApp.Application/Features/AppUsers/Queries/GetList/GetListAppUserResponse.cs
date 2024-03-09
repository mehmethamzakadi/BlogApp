namespace BlogApp.Application.Features.AppUsers.Queries.GetList
{
    public class GetListAppUserResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTimeOffset? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }
    }
}
