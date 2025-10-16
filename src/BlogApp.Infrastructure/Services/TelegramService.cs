
using System;
using BlogApp.Application.Abstractions;
using BlogApp.Domain.Options;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BlogApp.Infrastructure.Services;

public sealed class TelegramService : ITelegramService
{
    private readonly TelegramBotClient? telegramBotClient;
    private readonly TelegramOptions options;

    public TelegramService(IOptions<TelegramOptions> options)
    {
        this.options = options.Value;
        if (!string.IsNullOrWhiteSpace(this.options.TelegramBotToken))
        {
            telegramBotClient = new TelegramBotClient(this.options.TelegramBotToken);
        }
    }

    public async Task SendTextMessage(string message, long chatId)
    {
        if (telegramBotClient is null)
        {
            throw new InvalidOperationException("Telegram bot yapılandırması eksik.");
        }

        long targetChatId = chatId == 0 ? options.ChatId : chatId;
        if (targetChatId == 0)
        {
            throw new InvalidOperationException("Geçerli bir Telegram chat kimliği bulunamadı.");
        }

        await telegramBotClient.SendTextMessageAsync(new ChatId(targetChatId), message);
    }
}
