namespace BlogApp.Application.Abstractions;

public interface ITelegramBotManager
{
    Task SendTextMessage(string message, long chatId);
}
