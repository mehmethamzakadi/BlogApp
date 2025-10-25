namespace BlogApp.Application.Abstractions;

public interface ITelegramService
{
    Task SendTextMessage(string message, long chatId);
}
