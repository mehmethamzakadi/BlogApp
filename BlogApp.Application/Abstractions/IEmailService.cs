using BlogApp.Domain.DTOs;

namespace BlogApp.Application.Abstractions;

public interface IEmailService
{
    Task SendEmailAsync(EMailRequest email);
}
