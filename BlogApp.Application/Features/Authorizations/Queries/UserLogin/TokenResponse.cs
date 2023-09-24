namespace BlogApp.Application.Features.Authorizations.Queries.UserLogin
{
    public class TokenResponse
    {
        public DateTime Expiration { get; set; }
        public string? Token { get; set; }
    }
}
