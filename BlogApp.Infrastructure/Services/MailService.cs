using BlogApp.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace BlogApp.Infrastructure.Services;

public sealed class MailService(IConfiguration configuration) : IMailService
{
    public async Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true)
    {
        MailMessage mail = new();
        mail.IsBodyHtml = isBodyHtml;
        mail.To.Add(to);
        mail.Subject = subject;
        mail.Body = body;
        mail.From = new(configuration["EmailOptions:Username"] ?? throw new InvalidOperationException());

        using var smtpClient = new SmtpClient();
        smtpClient.Credentials = new NetworkCredential(configuration["EmailOptions:Username"],
            configuration["EmailOptions:Password"]);
        smtpClient.Port = Convert.ToInt32(configuration["EmailOptions:Port"]);
        smtpClient.EnableSsl = true;
        smtpClient.Host = configuration["EmailOptions:Host"] ?? string.Empty;
        await smtpClient.SendMailAsync(mail);

    }

    public async Task SendPasswordResetMailAsync(string to, int userId, string resetToken)
    {
        StringBuilder mail = new();
        mail.AppendLine(
            "<br>Merhaba <br> Yeni şifre talebinde bulunduysanız aşağıdaki linkten şifrenizi yenileyebilirsiniz.<br><a target=\"_blank\" href=\"");
        mail.AppendLine("http://localhost:5001");
        mail.AppendLine("/UpdatePassword/");
        mail.AppendLine(userId.ToString());
        mail.AppendLine("/");
        mail.AppendLine(resetToken);
        mail.AppendLine("\"> Yeni şifre talebi için tıklayınız...</a></strong><br><br><small>NOT: Eğer bu talep tarafınızca gerçekleştirilmemişse lütfen bu maili ciddiye almayınız. </small><br><br><hr><br>BLOG APP<br> ");

        await SendMailAsync(to, "Şİfre Yenileme Talebi", mail.ToString(), true);
    }
}
