# Outbox Pattern - Kurulum Özeti

## ✅ Tamamlanan İşlemler

### 1. **Domain Layer**
- ✅ `OutboxMessage` entity oluşturuldu
- ✅ `IOutboxMessageRepository` interface oluşturuldu
- ✅ `ActivityLogCreatedIntegrationEvent` integration event oluşturuldu
- ✅ `EventConstants.ActivityLogQueue` constant eklendi
- ✅ Global usings güncellendi

### 2. **Persistence Layer**
- ✅ `OutboxMessageRepository` implementasyonu
- ✅ `OutboxMessageConfiguration` (EF Core)
- ✅ `BlogAppDbContext.OutboxMessages` DbSet eklendi
- ✅ Repository DI kayıtları yapıldı
- ✅ `UnitOfWork.SaveChangesAsync` güncellendi (domain events → outbox)
- ✅ Database migration oluşturuldu: `AddOutboxPattern`

### 3. **Infrastructure Layer**
- ✅ `ActivityLogConsumer` RabbitMQ consumer'ı
- ✅ `OutboxProcessorService` background service
- ✅ MassTransit configuration güncellendi
- ✅ ActivityLog queue yapılandırıldı (retry, concurrency)
- ✅ Background service DI kayıtları

### 4. **Application Layer**
- ✅ Event handler'lar güncellendi (3 dosya - Category events)
- ✅ Logger injection eklendi
- ✅ Artık direkt DB yazmıyor, outbox pattern kullanıyor

### 5. **Documentation**
- ✅ Kapsamlı `OUTBOX_PATTERN_IMPLEMENTATION.md` dökümanı
- ✅ Architecture diagram
- ✅ Usage examples
- ✅ Configuration guide

## 🏗️ Mimari Değişiklikler

### Öncesi (Direkt DB Write)
```
Domain Event → Event Handler → DB Write (ActivityLog)
```

### Sonrası (Outbox Pattern)
```
Domain Event → OutboxMessage (DB) → Background Service → RabbitMQ → Consumer → ActivityLog (DB)
```

## 🔄 Veri Akışı

1. **User Action**: Kategori oluşturulur
2. **Domain Event**: `CategoryCreatedEvent` raise edilir
3. **UnitOfWork**: Event'i serialize edip `OutboxMessages` tablosuna yazar (aynı transaction)
4. **Background Service** (her 5 saniyede): Outbox'taki mesajları okur
5. **Serialization**: JSON → Domain Event → Integration Event
6. **RabbitMQ**: Integration event publish edilir
7. **Consumer**: Message consume edilir, ActivityLog oluşturulur
8. **Cleanup**: İşlenen mesajlar 7 gün sonra silinir

## 📊 Performans Özellikleri

- **Batch Size**: 50 mesaj/işlem
- **Processing Interval**: 5 saniye
- **Max Throughput**: ~600 mesaj/dakika
- **Retry**: Exponential backoff (5 deneme)
- **Concurrency**: 8 paralel consumer

## 🎯 Faydalar

### ✅ ACID Garantisi
- Domain events ve iş verisi tek transaction'da
- Event kaybı riski yok
- Rollback güvenliği

### ✅ Performance
- Ana transaction asenkron event'lerden etkilenmiyor
- ActivityLog yazma işlemi non-blocking
- RabbitMQ retry mekanizması

### ✅ Scalability
- Background service bağımsız scale edilebilir
- RabbitMQ consumer'lar horizontal scale
- Batch processing optimization

### ✅ Reliability
- Message broker çökse bile event'ler güvende
- Exponential backoff retry
- Dead letter queue desteği

## 🚀 Çalıştırma Adımları

### 1. Migration Uygula
```bash
cd src/BlogApp.Persistence
dotnet ef database update --context BlogAppDbContext --startup-project ../BlogApp.API
```

### 2. Docker Compose
```bash
docker-compose up --build
```

### 3. Monitoring
```sql
-- İşlenmemiş mesajlar
SELECT * FROM "OutboxMessages" WHERE "ProcessedAt" IS NULL;

-- Hatalı mesajlar
SELECT * FROM "OutboxMessages" WHERE "Error" IS NOT NULL;

-- İstatistikler
SELECT 
    "EventType",
    COUNT(*) as Total,
    COUNT(CASE WHEN "ProcessedAt" IS NOT NULL THEN 1 END) as Processed,
    COUNT(CASE WHEN "Error" IS NOT NULL THEN 1 END) as Failed
FROM "OutboxMessages"
GROUP BY "EventType";
```

## 📝 Yapılandırma

### appsettings.json
```json
{
  "RabbitMQOptions": {
    "HostName": "rabbitmq",
    "UserName": "blogapp",
    "Password": "supersecret",
    "RetryLimit": 10
  }
}
```

### EventConstants.cs
```csharp
public static class EventConstants
{
    public static string SendTelegramTextMessageQueue = "send-telegram-text-message-quee";
    public static string ActivityLogQueue = "activity-log-queue";
}
```

## 🔧 Troubleshooting

### Mesajlar işlenmiyor
1. Background service çalışıyor mu kontrol et
2. RabbitMQ bağlantısını doğrula
3. Outbox tablosunda mesaj var mı bak
4. Log dosyalarını incele

### Yüksek retry count
1. Consumer'da hata oluyor olabilir
2. Database connection sorunlu olabilir
3. Error kolonunu kontrol et

### Performans sorunları
1. Batch size'ı artır
2. Processing interval'ı azalt
3. Consumer concurrency'yi artır

## 📦 Oluşturulan Dosyalar

### Domain Layer
- `Entities/OutboxMessage.cs`
- `Repositories/IOutboxMessageRepository.cs`
- `Events/IntegrationEvents/ActivityLogCreatedIntegrationEvent.cs`
- `Constants/EventConstants.cs` (updated)
- `GlobalUsings.cs` (updated)

### Persistence Layer
- `Repositories/OutboxMessageRepository.cs`
- `Configurations/OutboxMessageConfiguration.cs`
- `Contexts/BlogAppDbContext.cs` (updated)
- `PersistenceServicesRegistration.cs` (updated)
- `Repositories/UnitOfWork.cs` (updated)
- `Migrations/PostgreSql/20251025192521_AddOutboxPattern.cs`

### Infrastructure Layer
- `Consumers/ActivityLogConsumer.cs`
- `Services/BackgroundServices/OutboxProcessorService.cs`
- `InfrastructureServicesRegistration.cs` (updated)

### Application Layer
- `Features/Categories/EventHandlers/*.cs` (3 files updated)

### Documentation
- `docs/OUTBOX_PATTERN_IMPLEMENTATION.md`
- `docs/OUTBOX_PATTERN_SETUP_SUMMARY.md` (this file)

## 🎓 Best Practices

### ✅ Yapılması Gerekenler
- Outbox table boyutunu düzenli izle
- Yüksek retry count için alert kur
- Dead letter mesajları takip et
- Regular cleanup schedule belirle

### ❌ Yapılmaması Gerekenler
- Outbox mesajlarını manuel silme
- Critical event'leri outbox'tan bypass etme
- İşlenmiş mesajları değiştirme
- Retry interval'ları çok düşük ayarlama

## 📈 Monitoring Metrikleri

### Takip Edilmesi Gerekenler
- Outbox table size
- Unprocessed message count
- Average processing latency
- Retry rate
- Error rate
- Consumer throughput

### Dashboard Queries
```sql
-- Outbox durumu
SELECT 
    COUNT(*) FILTER (WHERE "ProcessedAt" IS NULL) as Pending,
    COUNT(*) FILTER (WHERE "ProcessedAt" IS NOT NULL) as Processed,
    COUNT(*) FILTER (WHERE "Error" IS NOT NULL) as Failed,
    COUNT(*) FILTER (WHERE "RetryCount" >= 5) as DeadLetter
FROM "OutboxMessages";

-- Processing hızı (son 1 saat)
SELECT 
    DATE_TRUNC('minute', "ProcessedAt") as Minute,
    COUNT(*) as MessagesProcessed
FROM "OutboxMessages"
WHERE "ProcessedAt" > NOW() - INTERVAL '1 hour'
GROUP BY Minute
ORDER BY Minute;
```

## 🔗 İlgili Kaynaklar

- [Outbox Pattern - Microservices.io](https://microservices.io/patterns/data/transactional-outbox.html)
- [MassTransit Documentation](https://masstransit.io/)
- [Domain Events Implementation](./DOMAIN_EVENTS_IMPLEMENTATION.md)
- [Activity Logging Guide](./ACTIVITY_LOGGING_README.md)

## ✨ Özet

**Outbox Pattern** başarıyla kuruldu! Artık:
- ✅ Domain events güvenli bir şekilde saklanıyor
- ✅ Event loss riski ortadan kalktı
- ✅ Asenkron processing ile performans arttı
- ✅ Retry ve fault tolerance eklendi
- ✅ Scalable ve maintainable bir yapı kuruldu

**Migration'ı çalıştırdıktan sonra sistem production-ready!** 🚀
