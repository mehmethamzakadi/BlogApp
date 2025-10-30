using BlogApp.Application.Abstractions.Identity;
using BlogApp.Application.Options;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Events.Telegram;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;

namespace BlogApp.Application.Features.Auths.Login;

public sealed class LoginCommandHandler(
         IAuthService authService,
         IOptions<TelegramOptions> telegramOptions,
         IPublishEndpoint publishEndpoint) : IRequestHandler<LoginCommand, IDataResult<LoginResponse>>
{
    public async Task<IDataResult<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request.Email, request.Password, request.DeviceId);
        if (response.Success)
        {
            await SendTelegramMessage(response.Data.UserName, cancellationToken);
        }

        return response;
    }

    private async Task SendTelegramMessage(string userName, CancellationToken cancellationToken)
    {
        TelegramOptions options = telegramOptions.Value;
        if (options.ChatId == 0 || string.IsNullOrWhiteSpace(options.TelegramBotToken))
        {
            return;
        }

        var message = $"{userName} Kullanıcısı Sisteme Giriş Yaptı.";
        await publishEndpoint.Publish(
            new SendTextMessageEvent(message: message, chatId: options.ChatId),
            cancellationToken);
    }
}
