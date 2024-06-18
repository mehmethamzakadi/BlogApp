using BlogApp.Application.Abstractions;
using BlogApp.Domain.AppSettingsOptions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Events.Telegram;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;

namespace BlogApp.Application.Features.AppUsers.Commands.Login;

public sealed class LoginCommandHandler(
         IAuthService authService,
         IOptions<TelegramOptions> telegramOptions,
         IPublishEndpoint publishEndpoint) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request.Email, request.Password);
        if (response.Success)
            await SendTelegramMessage(response.Data.UserName);

        return response;
    }

    private async Task SendTelegramMessage(string userName)
    {
        var message = $"{userName} Kullanıcısı Sisteme Giriş Yaptı.";
        await publishEndpoint.Publish(
            new SendTextMessageEvent(message: message, chatId: telegramOptions.Value.ChatId));
    }
}
