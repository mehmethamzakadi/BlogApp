namespace BlogApp.Infrastructure.TelegramBot
{
    public interface ITelegramBotManager
    {
        Task SendTextMessage(string message, long chatId);
    }
}
