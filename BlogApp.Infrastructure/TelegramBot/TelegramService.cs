using BlogApp.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BlogApp.Infrastructure.TelegramBot;

public sealed class TelegramService(IConfiguration configuration) : ITelegramService
{
    private readonly TelegramBotClient TelegramBot = new(configuration["TelegramBotOptions:TelegramBotToken"] ?? string.Empty);

    public async Task SendTextMessage(string message, long chatId)
    {
        await TelegramBot.SendTextMessageAsync(new ChatId(chatId), message);
    }
}
