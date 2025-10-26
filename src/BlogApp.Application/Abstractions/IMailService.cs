namespace BlogApp.Application.Abstractions;

public interface IMailService
{
    Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true);
    Task SendPasswordResetMailAsync(string to, Guid userId, string resetToken);
}
