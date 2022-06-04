using BlogApp.Application.Interfaces.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BlogApp.Infrastructure.TelegramBot
{
    public class TelegramBotManager : ITelegramBotManager
    {
        private readonly TelegramBotClient TelegramBot;
        public TelegramBotManager(IConfiguration configuration)
        {
            TelegramBot = new TelegramBotClient(configuration["TelegramBotConfiguration:TelegramBotToken"]);
        }
        public async Task SendTextMessage(string message, long chatId)
        {
            try
            {
                await TelegramBot.SendTextMessageAsync(new ChatId(chatId), message);
            }
            catch (Exception ex)
            {
                await TelegramBot.SendTextMessageAsync(new ChatId(chatId), $"Hata: {ex.Message}");
                throw;
            }
        }
    }
}
