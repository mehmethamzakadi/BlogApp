using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BlogApp.Infrastructure.TelegramBot
{
    public class TelegramBotManager(IConfiguration configuration) : ITelegramBotManager
    {
        private readonly TelegramBotClient TelegramBot = new TelegramBotClient(configuration["TelegramBotConfiguration:TelegramBotToken"] ?? string.Empty);

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
