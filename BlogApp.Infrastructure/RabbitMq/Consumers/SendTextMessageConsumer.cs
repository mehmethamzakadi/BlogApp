using BlogApp.Application.Interfaces.Infrastructure;
using BlogApp.Domain.Events.Telegram;
using MassTransit;

namespace BlogApp.Infrastructure.RabbitMq.Consumers
{
    public class SendTextMessageConsumer : IConsumer<SendTextMessageEvent>
    {
        private readonly ITelegramBotManager _telegramBotManager;

        public SendTextMessageConsumer(ITelegramBotManager telegramBotManager)
        {
            _telegramBotManager = telegramBotManager;
        }

        public async Task Consume(ConsumeContext<SendTextMessageEvent> context)
        {
            await _telegramBotManager.SendTextMessage(context.Message.message, context.Message.chatId);
        }
    }
}
