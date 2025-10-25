# Outbox Pattern Implementation

## 📚 Overview

Bu proje **Outbox Pattern** kullanarak reliable message delivery (güvenilir mesaj teslimi) sağlar. Domain events veritabanına kaydedilir ve daha sonra bir background service tarafından RabbitMQ'ya yayınlanır.

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                          OUTBOX PATTERN FLOW                            │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  1. DOMAIN EVENT RAISED                                                 │
│     ├─ User creates a category                                          │
│     └─ Category entity raises CategoryCreatedEvent                      │
│                                                                         │
│  2. UNIT OF WORK INTERCEPTS                                             │
│     ├─ UnitOfWork.SaveChangesAsync() is called                          │
│     ├─ Gets all domain events from entities                             │
│     ├─ Serializes events to JSON                                        │
│     └─ Stores in OutboxMessages table (same transaction)                │
│                                                                         │
│  3. BACKGROUND SERVICE PROCESSES                                        │
│     ├─ OutboxProcessorService runs every 5 seconds                      │
│     ├─ Queries unprocessed messages from DB                             │
│     ├─ Deserializes JSON to domain events                               │
│     ├─ Converts to integration events                                   │
│     └─ Publishes to RabbitMQ                                            │
│                                                                         │
│  4. RABBITMQ CONSUMER HANDLES                                           │
│     ├─ ActivityLogConsumer receives message                             │
│     ├─ Creates ActivityLog entity                                       │
│     └─ Saves to database                                                │
│                                                                         │
│  5. CLEANUP                                                             │
│     └─ Old processed messages deleted after 7 days                      │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

## 🎯 Benefits

### ✅ **ACID Guarantees**
- Domain events ve business data aynı transaction'da saklanır
- Event kaybı riski yoktur
- Database rollback olursa event'ler de rollback olur

### ✅ **Eventual Consistency**
- Integration events asenkron olarak işlenir
- Ana transaction performansını etkilemez
- Retry mekanizması ile güvenilirlik

### ✅ **Decoupling**
- Domain layer RabbitMQ'dan bağımsız
- Event handler'lar basitleştirildi
- Infrastructure concerns ayrıldı

### ✅ **Fault Tolerance**
- Message broker çökerse event'ler DB'de güvende
- Exponential backoff ile akıllı retry
- Dead letter queue ile hata yönetimi

### ✅ **Scalability**
- Background service bağımsız scale edilebilir
- RabbitMQ consumer'lar horizontal scale olur
- Batch processing için optimize edilmiş

## 📦 Components

### 1. **OutboxMessage Entity**
```csharp
public class OutboxMessage : BaseEntity
{
    public string EventType { get; set; }        // "CategoryCreatedEvent"
    public string Payload { get; set; }          // JSON serialized event
    public DateTime CreatedAt { get; set; }      // When event was raised
    public DateTime? ProcessedAt { get; set; }   // When published to RabbitMQ
    public int RetryCount { get; set; }          // Number of retry attempts
    public string? Error { get; set; }           // Last error message
    public DateTime? NextRetryAt { get; set; }   // When to retry next
}
```

### 2. **UnitOfWork Integration**
```csharp
public async Task<int> SaveChangesAsync(CancellationToken ct)
{
    // Get domain events
    var domainEvents = GetDomainEvents().ToList();
    
    // Store events in outbox table
    foreach (var domainEvent in domainEvents)
    {
        if (ShouldStoreInOutbox(domainEvent))
        {
            var outboxMessage = new OutboxMessage
            {
                EventType = domainEvent.GetType().Name,
                Payload = JsonSerializer.Serialize(domainEvent),
                CreatedAt = DateTime.UtcNow
            };
            await _context.OutboxMessages.AddAsync(outboxMessage, ct);
        }
    }
    
    // Save everything in one transaction
    var result = await _context.SaveChangesAsync(ct);
    ClearDomainEvents();
    
    return result;
}
```

### 3. **OutboxProcessorService**
Background service that runs every 5 seconds:

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        await ProcessOutboxMessagesAsync(stoppingToken);
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
    }
}
```

**Processing Logic:**
- Batch read unprocessed messages (50 at a time)
- Deserialize JSON to domain events
- Convert to integration events
- Publish to RabbitMQ
- Mark as processed or failed with retry

### 4. **ActivityLogConsumer**
Consumes integration events from RabbitMQ:

```csharp
public async Task Consume(ConsumeContext<ActivityLogCreatedIntegrationEvent> context)
{
    var log = new ActivityLog
    {
        ActivityType = context.Message.ActivityType,
        EntityType = context.Message.EntityType,
        EntityId = context.Message.EntityId,
        Title = context.Message.Title,
        Details = context.Message.Details,
        UserId = context.Message.UserId,
        Timestamp = context.Message.Timestamp
    };
    
    await _repository.AddAsync(log);
    await _unitOfWork.SaveChangesAsync();
}
```

## 🔧 Configuration

### RabbitMQ Queue Setup
```csharp
cfg.ReceiveEndpoint(EventConstants.ActivityLogQueue, e =>
{
    e.ConfigureConsumer<ActivityLogConsumer>(context);
    
    // Exponential retry: 1s, 2s, 4s, 8s, 16s
    e.UseMessageRetry(r => r.Exponential(5,
        TimeSpan.FromSeconds(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromSeconds(2)));
    
    // Concurrency
    e.PrefetchCount = 16;
    e.ConcurrentMessageLimit = 8;
});
```

### Outbox Processor Settings
```csharp
private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(5);
private const int BatchSize = 50;
private const int MaxRetryCount = 5;
```

## 📊 Database Schema

### OutboxMessages Table
```sql
CREATE TABLE "OutboxMessages" (
    "Id" SERIAL PRIMARY KEY,
    "EventType" VARCHAR(256) NOT NULL,
    "Payload" TEXT NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL,
    "ProcessedAt" TIMESTAMP NULL,
    "RetryCount" INTEGER NOT NULL DEFAULT 0,
    "Error" VARCHAR(2000) NULL,
    "NextRetryAt" TIMESTAMP NULL,
    "CreatedDate" TIMESTAMP NOT NULL,
    "CreatedById" INTEGER NOT NULL,
    "UpdatedDate" TIMESTAMP NULL,
    "UpdatedById" INTEGER NULL,
    "IsDeleted" BOOLEAN NOT NULL,
    "DeletedDate" TIMESTAMP NULL
);

CREATE INDEX "IX_OutboxMessages_ProcessedAt" ON "OutboxMessages" ("ProcessedAt");
CREATE INDEX "IX_OutboxMessages_CreatedAt" ON "OutboxMessages" ("CreatedAt");
CREATE INDEX "IX_OutboxMessages_ProcessedAt_NextRetryAt" ON "OutboxMessages" ("ProcessedAt", "NextRetryAt");
```

## 🚀 Usage Example

### 1. Raise Domain Event (Application Layer)
```csharp
// In CreateCategoryCommandHandler
var category = new Category { Name = "Technology" };
await _categoryRepository.AddAsync(category);

// Raise domain event
category.AddDomainEvent(new CategoryCreatedEvent(
    category.Id, 
    category.Name, 
    currentUserId));

// Save changes (events stored in outbox automatically)
await _unitOfWork.SaveChangesAsync(cancellationToken);
```

### 2. Event Processing (Automatic)
- ✅ Event stored in OutboxMessages table
- ✅ Background service picks it up within 5 seconds
- ✅ Publishes to RabbitMQ
- ✅ Consumer creates ActivityLog
- ✅ Message marked as processed

### 3. Monitor Outbox (Optional)
```sql
-- View unprocessed messages
SELECT * FROM "OutboxMessages" WHERE "ProcessedAt" IS NULL;

-- View failed messages
SELECT * FROM "OutboxMessages" WHERE "Error" IS NOT NULL;

-- View messages pending retry
SELECT * FROM "OutboxMessages" 
WHERE "ProcessedAt" IS NULL 
  AND "RetryCount" > 0 
  AND "NextRetryAt" > NOW();
```

## 🔍 Event Types Supported

### Stored in Outbox (Async Processing)
- ✅ CategoryCreatedEvent
- ✅ CategoryUpdatedEvent
- ✅ CategoryDeletedEvent
- ✅ PostCreatedEvent
- ✅ PostUpdatedEvent
- ✅ PostDeletedEvent
- ✅ UserCreatedEvent
- ✅ UserUpdatedEvent
- ✅ UserDeletedEvent
- ✅ UserRolesAssignedEvent
- ✅ RoleCreatedEvent
- ✅ RoleUpdatedEvent
- ✅ RoleDeletedEvent
- ✅ PermissionsAssignedToRoleEvent

### Direct Processing (Sync)
- ❌ Business validation events (handled by domain)

## 📈 Performance Characteristics

### Throughput
- **Batch Size**: 50 messages per iteration
- **Processing Interval**: 5 seconds
- **Max Throughput**: ~600 messages/minute

### Latency
- **Min Latency**: 0-5 seconds (next processing cycle)
- **Avg Latency**: 2.5 seconds
- **Max Latency**: 5 seconds + processing time

### Retry Strategy
- **Exponential Backoff**: 1min, 2min, 4min, 8min, 16min
- **Max Retries**: 5 attempts
- **Total Retry Time**: ~31 minutes

## 🛠️ Maintenance

### Cleanup Old Messages
Automatic cleanup runs during each processing cycle:
```csharp
await outboxRepository.CleanupProcessedMessagesAsync(
    retentionDays: 7, 
    cancellationToken);
```

### Manual Cleanup
```sql
DELETE FROM "OutboxMessages" 
WHERE "ProcessedAt" IS NOT NULL 
  AND "ProcessedAt" < NOW() - INTERVAL '7 days';
```

### Dead Letter Messages
Messages exceeding max retries remain in database for investigation:
```sql
SELECT * FROM "OutboxMessages" 
WHERE "RetryCount" >= 5 
  AND "ProcessedAt" IS NULL;
```

## 📝 Migration

### Apply Migration
```bash
cd src/BlogApp.Persistence
dotnet ef database update --context BlogAppDbContext --startup-project ../BlogApp.API
```

### Rollback Migration
```bash
dotnet ef database update 20251025145645_Init --context BlogAppDbContext --startup-project ../BlogApp.API
```

## 🎓 Best Practices

### ✅ DO
- Monitor outbox table size regularly
- Set up alerts for high retry counts
- Use indexes for performance
- Implement dead letter queue monitoring
- Keep payload size reasonable (<1MB)

### ❌ DON'T
- Don't delete outbox messages manually
- Don't bypass outbox for critical events
- Don't modify processed messages
- Don't decrease retry intervals too much

## 🔗 Related Documentation

- [Domain Events Implementation](./DOMAIN_EVENTS_IMPLEMENTATION.md)
- [Activity Logging](./ACTIVITY_LOGGING_README.md)
- [Transaction Management](./TRANSACTION_MANAGEMENT_STRATEGY.md)

## 📚 References

- [Outbox Pattern - Martin Fowler](https://microservices.io/patterns/data/transactional-outbox.html)
- [Reliable Messaging - Microsoft](https://docs.microsoft.com/en-us/azure/architecture/patterns/publisher-subscriber)
- [MassTransit Outbox](https://masstransit.io/documentation/configuration/middleware/outbox)
