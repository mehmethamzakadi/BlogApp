namespace BlogApp.Application.DTOs.AppUsers
{
    public class GetByIdAppUserResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }
    }
}
