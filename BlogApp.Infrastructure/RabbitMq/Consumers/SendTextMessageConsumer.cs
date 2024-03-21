using BlogApp.Application.Abstractions;
using BlogApp.Domain.Events.Telegram;
using MassTransit;

namespace BlogApp.Infrastructure.RabbitMq.Consumers;

public class SendTextMessageConsumer(ITelegramService telegramService) : IConsumer<SendTextMessageEvent>
{
    public async Task Consume(ConsumeContext<SendTextMessageEvent> context)
    {
        await telegramService.SendTextMessage(context.Message.message, context.Message.chatId);
    }
}
