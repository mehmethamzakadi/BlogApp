using BlogApp.Domain.Events.Telegram;
using BlogApp.Infrastructure.TelegramBot;
using MassTransit;

namespace BlogApp.Infrastructure.RabbitMq.Consumers
{
    public class SendTextMessageConsumer(ITelegramBotManager telegramBotManager) : IConsumer<SendTextMessageEvent>
    {
        public async Task Consume(ConsumeContext<SendTextMessageEvent> context)
        {
            await telegramBotManager.SendTextMessage(context.Message.message, context.Message.chatId);
        }
    }
}
