using BlogApp.Application.Interfaces.Infrastructure;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BlogApp.Infrastructure.TelegramBot
{
    public class TelegramBotManager : ITelegramBotManager
    {
        private readonly TelegramBotClient TelegramBot;
        public TelegramBotManager(IConfiguration configuration)
        {
            TelegramBot = new TelegramBotClient(configuration["TelegramBotConfiguration:TelegramBotToken"] ?? string.Empty);
        }
        public async Task SendTextMessage(string message, long chatId)
        {
            try
            {
                await TelegramBot.SendTextMessageAsync(new ChatId(chatId), message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
