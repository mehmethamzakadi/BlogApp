using BlogApp.Application.Abstractions;
using BlogApp.Domain.DTOs;
using FluentEmail.Core;

namespace BlogApp.Infrastructure.Email;

public class EmailSenderService(IFluentEmail fluentEmail) : IEmailService
{
    public async Task SendEmailAsync(EMailRequest email)
    {
        var result = await fluentEmail
             .To(email.ToEmail)
             .Subject(email.Subject)
             .Body(email.Body)
             .SendAsync();

    }
}
