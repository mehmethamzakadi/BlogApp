﻿using BlogApp.Application.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System.Text;

namespace BlogApp.Infrastructure.Services;

public sealed class MailService(IConfiguration configuration) : IMailService
{
    public async Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true)
    {
        var host = configuration["EmailOptions:Host"];
        var port = Convert.ToInt32(configuration["EmailOptions:Port"]);
        var from = configuration["EmailOptions:Username"];
        var pass = configuration["EmailOptions:Password"];

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(from));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(from, pass);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
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
