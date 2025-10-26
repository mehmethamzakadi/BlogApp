dotnet ef database update --context BlogAppDbContext --startup-project ../BlogApp.API
docker-compose up --build
# Outbox Pattern - Kurulum Özeti

## 1. Tamamlanan İşler

### Domain
- `Entities/OutboxMessage.cs` (GUID tabanlı, retry + error alanları)
- `[StoreInOutbox]` attribute ile seçimli event kaydı
- `ActivityLogCreatedIntegrationEvent` ve `EventConstants.ActivityLogQueue`

### Persistence
- `OutboxMessageRepository` + EF Core config (index’ler, retry hesaplaması)
- `UnitOfWork.SaveChangesAsync` → domain event to outbox JSON (tip bilgisiyle)
- `BlogAppDbContext` ve DI kayıtları güncellendi

### Infrastructure
- `OutboxProcessorService` (5 sn döngü, batch=50, max retry=5)
- `ActivityLogConsumer` (MassTransit, retry + concurrency)
- `IIntegrationEventConverterStrategy` implementasyonları ve DI kayıtları

### Documentation
- `OUTBOX_PATTERN_IMPLEMENTATION.md` ve `ACTIVITY_LOGGING_README.md` güncel
- Bu özet dosyası revize edildi

## 2. Mimari Akış

```
Domain Event (StoreInOutbox) → UnitOfWork → OutboxMessages
  → OutboxProcessorService → Converter → RabbitMQ
    → Consumer → Hedef tablo (örn. ActivityLogs)
```

## 3. Operasyonel Parametreler

- Batch: 50 mesaj
- Döngü: 5 saniye
- Retry: 5 deneme, 1→2→4→8→16 dk
- Cleanup: İşlenmiş kayıtlar 7 gün sonra silinir
- Queue: `activity-log-queue` (prefetch 16, concurrency 8)

## 4. İzleme

```sql
-- Bekleyen mesajlar
SELECT "Id", "EventType", "RetryCount" FROM "OutboxMessages"
WHERE "ProcessedAt" IS NULL ORDER BY "CreatedAt";

-- Hatalar
SELECT "Id", "EventType", "Error", "RetryCount"
FROM "OutboxMessages"
WHERE "ProcessedAt" IS NULL AND "Error" IS NOT NULL;

-- Retry bekleyenler
SELECT "Id", "EventType", "NextRetryAt"
FROM "OutboxMessages"
WHERE "ProcessedAt" IS NULL AND "NextRetryAt" > NOW();
```

Prometheus/Dashboard metrikleri: bekleyen kayıt sayısı, maksimum retry’a ulaşan kayıtlar, son başarılı publish zamanı.

## 5. Yapılandırma Notları

- `InfrastructureServicesRegistration` converter’ları `AddSingleton<IIntegrationEventConverterStrategy, …>` ile kaydeder.
- `RabbitMqOptions.RetryLimit` (appsettings) publish katmanında ek inline retry sağlar.
- `OutboxMessageRepository.CalculateNextRetryTime` üstel gecikmeyi yönetir.

## 6. En İyi Uygulamalar

- Payload’ları mümkün olduğunca küçük tut (audit için yeterli alan).
- Yeni domain event eklerken `[StoreInOutbox]`, converter ve DI kaydını birlikte ele al.
- Consumer’ları idempotent yaz; aynı mesaj tekrar gelirse veri tutarlılığını bozmasın.
- Outbox tablosunu düzenli olarak izle; büyüme veya yoğun retry durumunda alarm tanımla.
- Manuel silme yerine `CleanupProcessedMessagesAsync` veya planlı SQL script’i kullan.

## 7. İlgili Belgeler
- `docs/OUTBOX_PATTERN_IMPLEMENTATION.md`
- `docs/ACTIVITY_LOGGING_README.md`
- `docs/DOMAIN_EVENTS_IMPLEMENTATION.md`

Bu özet, outbox pattern kurulumu sonrasındaki yapı taşlarını ve operasyonel aksiyonları hızlıca hatırlatmak için günceldir. Kodda yeni entegrasyonlar eklediğinizde converter + consumer + config üçlüsünü birlikte güncellemeyi unutmayın.
