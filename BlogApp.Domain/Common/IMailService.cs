
using BlogApp.Domain.Common.Requests;
using System.Threading.Tasks;

namespace BlogApp.Domain.Common
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest email);
    }
}
