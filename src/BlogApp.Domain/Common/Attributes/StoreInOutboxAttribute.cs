namespace BlogApp.Domain.Common.Attributes;

/// <summary>
/// Bir domain event'in RabbitMQ üzerinden asenkron işleme için Outbox tablosunda saklanmasını işaretler.
/// Bu attribute'a sahip event'ler:
/// 1. OutboxMessages tablosunda saklanır (business data ile aynı transaction içinde)
/// 2. OutboxProcessorService (background service) tarafından alınır
/// 3. RabbitMQ'ya yayınlanır
/// 4. Uygun consumer'lar tarafından tüketilir
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class StoreInOutboxAttribute : Attribute
{
}
