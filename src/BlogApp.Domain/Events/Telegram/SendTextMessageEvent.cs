namespace BlogApp.Domain.Events.Telegram
{
    public record SendTextMessageEvent(string message, long chatId);
}
