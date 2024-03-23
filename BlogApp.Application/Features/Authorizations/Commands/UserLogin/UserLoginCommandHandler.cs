using BlogApp.Application.Abstractions;
using BlogApp.Domain.AppSettingsOptions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.Telegram;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BlogApp.Application.Features.Authorizations.Commands.UserLogin;

public sealed class UserLoginCommandHandler(
         ITokenService tokenService,
         UserManager<AppUser> userManager,
         IOptions<TelegramOptions> telegramOptions,
         SignInManager<AppUser> signInManager,
         IPublishEndpoint publishEndpoint) : IRequestHandler<UserLoginCommand, IDataResult<TokenResponse>>
{
    public async Task<IDataResult<TokenResponse>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = await userManager.FindByEmailAsync(request.Email);

        if (user is not null)
        {
            bool checkPassword = await userManager.CheckPasswordAsync(user, request.Password);
            if (!checkPassword)
                return new ErrorDataResult<TokenResponse>("E-Mail veya şifre hatalı!");

            var authClaims = await tokenService.GetAuthClaims(user);
            var tokenResponse = tokenService.GenerateAccessToken(authClaims, user);
            await signInManager.SignInWithClaimsAsync(user, false, authClaims);

            await userManager.RemoveAuthenticationTokenAsync(user, "BlogApp", "RefreshToken");
            await userManager.SetAuthenticationTokenAsync(user, "BlogApp", "RefreshToken", tokenResponse.RefreshToken);
            await SendTelegramMessage(user);

            return new SuccessDataResult<TokenResponse>(tokenResponse, "Giriş Başarılı");
        }
        return new ErrorDataResult<TokenResponse>("E-Mail veya şifre hatalı!");
    }

    private async Task SendTelegramMessage(AppUser user)
    {
        var message = $"{user.UserName} Kullanıcısı Sisteme Giriş Yaptı.";
        await publishEndpoint.Publish(
            new SendTextMessageEvent(message: message, chatId: telegramOptions.Value.ChatId));
    }
}
