# Outbox Pattern Implementation

## 1. Amaç
Transactional outbox, domain event’lerin business değişiklikleriyle aynı ACID transaction içinde kaydedilmesini ve RabbitMQ’ya güvenilir biçimde aktarılmasını sağlar. BlogApp’te outbox akışı activity logging, Telegram bildirimleri ve ileride eklenecek diğer entegrasyonlar için tek kaynak olarak kullanılır.

## 2. Akış

```
Command Handler
    ↳ Domain Event (örn. CategoryCreatedEvent)
        ↳ [StoreInOutbox] attribute kontrolü
            ↳ UnitOfWork.SaveChangesAsync
                ↳ OutboxMessages tablosuna JSON payload
                    ↳ OutboxProcessorService (5 sn döngü)
                        ↳ Converter stratejisi (EventType -> IntegrationEvent)
                            ↳ RabbitMQ publish (MassTransit)
                                ↳ Consumer (örn. ActivityLogConsumer)
                                    ↳ Hedef tabloya kayıt
```

### Neden Attribute?
`StoreInOutboxAttribute` sadece kritik domain event’lerin outbox’a yazılmasına izin verir. Böylece synchronous iş mantığı ile audit/entegrasyon akışları ayrılır, gereksiz satır oluşması engellenir.

## 3. Temel Bileşenler

### 3.1 OutboxMessage

```csharp
public class OutboxMessage : BaseEntity
{
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public int RetryCount { get; set; } = 0;
    public string? Error { get; set; }
    public DateTime? NextRetryAt { get; set; }
}
```

- `BaseEntity` mirası sayesinde `Id`, `CreatedDate`, `CreatedById` vb. audit alanları otomatik gelir.
- `EventType` string olarak saklanır (ör. `CategoryCreatedEvent`).
- `Payload` domain event’in JSON karşılığıdır (tip bilgisi serileştirme sırasında korunur).

### 3.2 UnitOfWork
- `SaveChangesAsync`, tracked entity’lerden domain event’leri çıkarır.
- Event tipi `[StoreInOutbox]` ile işaretliyse `OutboxMessages` tablosuna ekler.
- `JsonSerializer.Serialize(domainEvent, domainEvent.GetType())` ile tip bilgisi kaybedilmez.
- Transaction başarısız olsa bile `finally` bloğunda domain event listeleri temizlenir.

### 3.3 Converter Stratejileri
- `ActivityLogIntegrationEventConverter<T>` soyut sınıfı event payload’ını `ActivityLogCreatedIntegrationEvent`’e dönüştürür.
- `InfrastructureServicesRegistration` tüm converter implementasyonlarını `IIntegrationEventConverterStrategy` olarak singleton kaydeder.
- Yeni event için sadece converter eklemek yeterlidir.

### 3.4 OutboxProcessorService
- BackgroundService, her 5 sn’de bir (konfigüre edilmiş `_processingInterval`) çalışır.
- `GetUnprocessedMessagesAsync` ile `ProcessedAt IS NULL` kayıtlarını, `NextRetryAt` süresi dolanları dahil, `BatchSize=50` olacak şekilde çeker.
- `EventType` için uygun converter yoksa kayıt `MarkAsFailedAsync` ile hata durumuna alınır.
- Başarılı publish sonrası `MarkAsProcessedAsync` çağrılır ve durum değişikliği hemen `unitOfWork.SaveChangesAsync` ile persist edilir.
- Her tur sonunda 7 günden eski işlenmiş mesajlar temizlenir (`CleanupProcessedMessagesAsync(7)`).

### 3.5 RabbitMQ Consumers
- `ActivityLogConsumer`, `ActivityLogCreatedIntegrationEvent` aldığında yeni `ActivityLog` kaydı oluşturur ve `IUnitOfWork.SaveChangesAsync` çağırır.
- Retry MassTransit tarafından yönetilir; kalıcı hata durumunda kayıt tekrar outbox tarafından işleme alınır.

## 4. Konfigürasyon

### 4.1 DI Kayıtları

```csharp
services.AddHostedService<OutboxProcessorService>();
services.AddSingleton<IIntegrationEventConverterStrategy, CategoryCreatedIntegrationEventConverter>();
// ... (tüm Category/Post/User/Role/Permission converter’ları)
services.AddScoped<ActivityLogConsumer>();
```

### 4.2 MassTransit / RabbitMQ

```csharp
cfg.ReceiveEndpoint(EventConstants.ActivityLogQueue, endpoint =>
{
    endpoint.ConfigureConsumer<ActivityLogConsumer>(context);

    endpoint.UseMessageRetry(retry => retry.Exponential(
        retryLimit: 5,
        minInterval: TimeSpan.FromSeconds(1),
        maxInterval: TimeSpan.FromMinutes(5),
        intervalDelta: TimeSpan.FromSeconds(2)));

    endpoint.PrefetchCount = 16;
    endpoint.ConcurrentMessageLimit = 8;
});
```

### 4.3 Outbox Ayarları
- `BatchSize`: 50
- `MaxRetryCount`: 5
- `Retry` zamanlaması: 1, 2, 4, 8, 16 dakika (üstel artış)
- `Cleanup` süresi: 7 gün

## 5. Veritabanı Şeması

```sql
CREATE TABLE "OutboxMessages" (
    "Id" uuid PRIMARY KEY,
    "EventType" varchar(256) NOT NULL,
    "Payload" text NOT NULL,
    "CreatedAt" timestamp NOT NULL,
    "ProcessedAt" timestamp NULL,
    "RetryCount" int NOT NULL DEFAULT 0,
    "Error" varchar(2000) NULL,
    "NextRetryAt" timestamp NULL,
    "CreatedDate" timestamp NOT NULL,
    "CreatedById" uuid NOT NULL,
    "UpdatedDate" timestamp NULL,
    "UpdatedById" uuid NULL,
    "IsDeleted" boolean NOT NULL,
    "DeletedDate" timestamp NULL
);

CREATE INDEX "IX_OutboxMessages_ProcessedAt" ON "OutboxMessages" ("ProcessedAt");
CREATE INDEX "IX_OutboxMessages_CreatedAt" ON "OutboxMessages" ("CreatedAt");
CREATE INDEX "IX_OutboxMessages_ProcessedAt_NextRetryAt" ON "OutboxMessages" ("ProcessedAt", "NextRetryAt");
```

> Not: EF Core migration’ları `Guid` tipindeki `Id` ve audit kolonlarını `BaseEntity` üzerinden yaratır.

## 6. İzleme ve Operasyon

### 6.1 SQL Sorguları

```sql
-- İşlenmemiş mesajlar
SELECT "Id", "EventType", "RetryCount", "NextRetryAt"
FROM "OutboxMessages"
WHERE "ProcessedAt" IS NULL
ORDER BY "CreatedAt";

-- Hata alan mesajlar
SELECT "Id", "EventType", "Error", "RetryCount"
FROM "OutboxMessages"
WHERE "Error" IS NOT NULL AND "ProcessedAt" IS NULL;

-- Retry bekleyenler
SELECT "Id", "EventType", "NextRetryAt"
FROM "OutboxMessages"
WHERE "ProcessedAt" IS NULL AND "RetryCount" > 0 AND "NextRetryAt" > NOW();
```

### 6.2 Prometheus / Healthcheck Önerileri
- Outbox kuyruğundaki kayıt sayısı (işlenmemiş + hatalı)
- Son başarılı publish zamanı
- Maksimum retry sayısını aşmış kayıtlar (manuel müdahale gerektirir)

## 7. Desteklenen Domain Event’ler

| Event | Açıklama | ActivityLog Converter |
|-------|----------|-----------------------|
| `CategoryCreatedEvent` | Yeni kategori | ✅ |
| `CategoryUpdatedEvent` | Güncelleme | ✅ |
| `CategoryDeletedEvent` | Silme | ✅ |
| `PostCreatedEvent` | Yeni yazı | ✅ |
| `PostUpdatedEvent` | Yazı güncelleme | ✅ |
| `PostDeletedEvent` | Yazı silme | ✅ |
| `UserCreatedEvent` | Kullanıcı oluşturma | ✅ |
| `UserUpdatedEvent` | Güncelleme | ✅ |
| `UserDeletedEvent` | Silme | ✅ |
| `UserRolesAssignedEvent` | Rol ataması | ✅ |
| `RoleCreatedEvent` | Rol oluşturma | ✅ |
| `RoleUpdatedEvent` | Rol güncelleme | ✅ |
| `RoleDeletedEvent` | Rol silme | ✅ |
| `PermissionsAssignedToRoleEvent` | Yetki ataması | ✅ |

Yeni bir event eklemek için `[StoreInOutbox]` attribute’u, uygun converter ve gerekirse consumer implementasyonu eklenmelidir.

## 8. Yeni Event Ekleme Adımları

1. Domain event sınıfını oluştur ve `[StoreInOutbox]` ile işaretle.
2. Event’i raise eden handler’da `entity.AddDomainEvent(new Event(...))` çağır.
3. `ActivityLogIntegrationEventConverters` içerisine yeni converter ekle veya farklı bir integration event oluştur.
4. Converter’ı `InfrastructureServicesRegistration`’da `IIntegrationEventConverterStrategy` olarak kaydet.
5. Gerekirse yeni RabbitMQ consumer yaz ve MassTransit receive endpoint’ini tanımla.

## 9. Retry ve Cleanup Mekanizması

- `MarkAsFailedAsync`, `RetryCount`’ı artırır ve `NextRetryAt` değerini üstel olarak ayarlar.
- `GetUnprocessedMessagesAsync`, `NextRetryAt <= UtcNow` şartıyla tekrar denemeye hazır mesajları sıraya alır.
- Maximum denemeye (5) ulaşan kayıtlar outbox’ta kalır; manuel inceleme önerilir.
- Her döngünün sonunda `CleanupProcessedMessagesAsync(7)` ile 7 günden eski başarılı kayıtlar soft delete yerine kalıcı silinir.

## 10. Best Practices

- Production’da outbox tablosu büyüklüğünü ve retry metriklerini gözlemle.
- `Payload` boyutunu <1 MB tutacak şekilde event içeriklerini minimal tasarla.
- Yeni consumer eklerken idempotent davranışı garanti et (ör. aynı activity log tekrar yazılmamalı).
- Sık gerçekleşen event’ler için converter’larda gereksiz string formatlamasından kaçın.
- Gereksiz event’leri `[StoreInOutbox]` ile işaretleme; synchronous işlem yeterliyse domain handler’ı kullan.

## 11. İlgili Dokümanlar
- `docs/DOMAIN_EVENTS_IMPLEMENTATION.md`
- `docs/ACTIVITY_LOGGING_README.md`
- `docs/TRANSACTION_MANAGEMENT_STRATEGY.md`

Bu doküman, outbox pattern’inin güncel .NET 9 implementasyonunu özetler. Kodda değişiklik yaptığınızda converter kayıtları, MassTransit endpoint ayarları ve UnitOfWork davranışını senkron tutmayı unutmayın.
