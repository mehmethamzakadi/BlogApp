using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Requests;
using FluentEmail.Core;

namespace BlogApp.Infrastructure.Email
{
    public class EmailSenderManagerTest(IFluentEmail fluentEmail) : IMailService
    {
        public async Task SendEmailAsync(MailRequest email)
        {
            var result = await fluentEmail
                 .To(email.ToEmail)
                 .Subject(email.Subject)
                 .Body(email.Body)
                 .SendAsync();

        }
    }
}
