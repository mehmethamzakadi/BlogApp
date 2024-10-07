namespace BlogApp.Domain.Options;
public sealed class TelegramOptions
{
    public string TelegramBotToken { get; set; } = default!;
    public int ChatId { get; set; }
}