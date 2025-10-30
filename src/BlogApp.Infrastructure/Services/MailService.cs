using BlogApp.Application.Abstractions;
using BlogApp.Infrastructure.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Net;
using System.Text;

namespace BlogApp.Infrastructure.Services;

public sealed class MailService : IMailService
{
    private readonly EmailOptions emailOptions;
    private readonly PasswordResetOptions passwordResetOptions;

    public MailService(IOptions<EmailOptions> emailOptions, IOptions<PasswordResetOptions> passwordResetOptions)
    {
        this.emailOptions = emailOptions.Value;
        this.passwordResetOptions = passwordResetOptions.Value;
    }

    public async Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true)
    {
        if (string.IsNullOrWhiteSpace(emailOptions.Host) ||
            string.IsNullOrWhiteSpace(emailOptions.Username) ||
            string.IsNullOrWhiteSpace(emailOptions.Password) ||
            emailOptions.Port <= 0)
        {
            throw new InvalidOperationException("Email ayarları eksik veya hatalı yapılandırıldı.");
        }

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(emailOptions.Username));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = isBodyHtml ? new TextPart(TextFormat.Html) { Text = body } : new TextPart(TextFormat.Text) { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(emailOptions.Host, emailOptions.Port, SecureSocketOptions.StartTls).ConfigureAwait(false);
        await smtp.AuthenticateAsync(emailOptions.Username, emailOptions.Password).ConfigureAwait(false);
        await smtp.SendAsync(email).ConfigureAwait(false);
        await smtp.DisconnectAsync(true).ConfigureAwait(false);
    }

    public async Task SendPasswordResetMailAsync(string to, Guid userId, string resetToken)
    {
        if (string.IsNullOrWhiteSpace(passwordResetOptions.BaseUrl))
        {
            throw new InvalidOperationException("Şifre sıfırlama bağlantısı için temel adres yapılandırılmadı.");
        }

        if (!Uri.TryCreate(passwordResetOptions.BaseUrl, UriKind.Absolute, out var baseUri))
        {
            throw new InvalidOperationException("Şifre sıfırlama temel adresi geçerli bir URI değil.");
        }

        string encodedToken = WebUtility.UrlEncode(resetToken);
        var resetUri = new Uri(baseUri, $"UpdatePassword/{userId}/{encodedToken}");

        StringBuilder mail = new();
        mail.AppendLine("<br>Merhaba <br> Yeni şifre talebinde bulunduysanız aşağıdaki linkten şifrenizi yenileyebilirsiniz.<br>");
        mail.AppendLine($"<a target='_blank' href='{resetUri}'>Yeni şifre talebi için tıklayınız...</a></strong><br><br><small>NOT: Eğer bu talep tarafınızca gerçekleştirilmemişse lütfen bu maili ciddiye almayınız.</small><br><br><hr><br>BLOG APP<br>");

        await SendMailAsync(to, "Şifre Yenileme Talebi", mail.ToString(), true).ConfigureAwait(false);
    }
}
