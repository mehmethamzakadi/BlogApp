namespace BlogApp.Application.Features.Authorizations.Queries.UserLogin
{
    public class TokenResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Expiration { get; set; }
        public string? Token { get; set; }
    }
}
