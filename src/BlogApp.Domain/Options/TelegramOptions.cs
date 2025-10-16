
namespace BlogApp.Domain.Options;

public sealed class TelegramOptions
{
    public const string SectionName = "TelegramBotOptions";

    public string TelegramBotToken { get; set; } = string.Empty;
    public long ChatId { get; set; }
}
