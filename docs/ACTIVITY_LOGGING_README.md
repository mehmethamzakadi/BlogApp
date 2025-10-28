# BlogApp - Activity Logging Guide

_Son Güncelleme: 28 Ekim 2025_

## 🎯 Amaç
Activity logging, kullanıcı ve sistem aksiyonlarını kalıcı olarak kaydederek audit trail, güvenlik soruşturmaları ve operasyonel raporlamayı destekler. BlogApp bu akışı domain event → outbox → RabbitMQ → consumer zinciri ile uygular; böylece ana işlem tamamlandıktan sonra yan etkiler güvenilir biçimde işlenir.

## 🧱 Mimarinin Özeti

### Akış Diyagramı
```
┌─────────────────────┐
│  Command Handler    │
│  (Application)      │
└──────────┬──────────┘
           │ entity.AddDomainEvent(...)
           ↓
┌─────────────────────┐
│  Domain Entity      │
│  + DomainEvents[]   │
└──────────┬──────────┘
           │ UnitOfWork.SaveChangesAsync()
           ↓
┌─────────────────────┐
│  OutboxMessages     │
│  (PostgreSQL)       │  [StoreInOutbox] attribute ile
└──────────┬──────────┘  JSON olarak kaydedilir
           │
           │ OutboxProcessorService
           │ (Background Service, 5sn interval)
           ↓
┌─────────────────────┐
│  Converter          │
│  (Strategy Pattern) │
└──────────┬──────────┘
           │ ActivityLogCreatedIntegrationEvent
           ↓
┌─────────────────────┐
│  RabbitMQ           │
│  (MassTransit)      │
└──────────┬──────────┘
           │ Publish/Subscribe
           ↓
┌─────────────────────┐
│  ActivityLog        │
│  Consumer           │
└──────────┬──────────┘
           │ Persist to DB
           ↓
┌─────────────────────┐
│  ActivityLogs       │
│  (PostgreSQL)       │
└─────────────────────┘
```

### İşlem Adımları
1. **Command Handler** domain entity üzerinde değişiklik yapar ve `entity.AddDomainEvent(...)` çağırır.
2. **UnitOfWork.SaveChangesAsync** çağrısı entity üzerindeki `[StoreInOutbox]` ile işaretli event'leri `OutboxMessages` tablosuna JSON olarak kaydeder.
3. **OutboxProcessorService** (5 sn aralıklarla, batch=50) mesajları okur, uygun `ActivityLogIntegrationEventConverter` ile `ActivityLogCreatedIntegrationEvent` üretir ve MassTransit ile RabbitMQ'ya yayınlar.
4. **ActivityLogConsumer** mesajı tüketir ve `ActivityLogs` tablosuna kalıcı kayıt ekler.

### Avantajlar
✅ **Güvenilirlik:** Outbox pattern ile at-least-once delivery garantisi  
✅ **Performans:** Ana transaction'ı bloklamaz, asenkron işleme  
✅ **Ölçeklenebilirlik:** Message broker sayesinde horizontal scaling  
✅ **Hata Yönetimi:** Retry mekanizması ve dead-letter queue desteği  
✅ **Audit Trail:** Tüm önemli işlemler kayıt altına alınır

## 💾 Veritabanı Şeması
```sql
CREATE TABLE "ActivityLogs" (
  "Id" UUID PRIMARY KEY,
  "ActivityType" VARCHAR(50) NOT NULL,
  "EntityType" VARCHAR(50) NOT NULL,
  "EntityId" UUID NULL,
  "Title" VARCHAR(500) NOT NULL,
  "Details" VARCHAR(2000) NULL,
  "UserId" UUID NULL,
  "Timestamp" TIMESTAMP WITH TIME ZONE NOT NULL,
  CONSTRAINT "FK_ActivityLogs_Users" FOREIGN KEY ("UserId")
    REFERENCES "Users"("Id") ON DELETE SET NULL
);

CREATE INDEX "IX_ActivityLogs_Timestamp" ON "ActivityLogs"("Timestamp");
CREATE INDEX "IX_ActivityLogs_Entity" ON "ActivityLogs"("EntityType", "EntityId");
```

> `Users` tablosu BlogApp'in custom kimlik sistemi tarafından yönetilir; Identity'nin `AspNetUsers` şemasına bağımlılık yoktur.

## 🗂️ Domain Event → Activity Type Eşlemeleri
`ActivityLogIntegrationEventConverters.cs` dosyası aşağıdaki eşlemeleri üretir:

| Domain Event | ActivityType | EntityType | Açıklama |
|--------------|--------------|------------|----------|
| `CategoryCreatedEvent` | `category_created` | Category | Yeni kategori oluşturuldu |
| `CategoryUpdatedEvent` | `category_updated` | Category | Kategori güncellendi |
| `CategoryDeletedEvent` | `category_deleted` | Category | Kategori silindi |
| `BookshelfItemCreatedEvent` | `bookshelf_item_created` | BookshelfItem | Kitap kitaplığa eklendi |
| `BookshelfItemUpdatedEvent` | `bookshelf_item_updated` | BookshelfItem | Kitap bilgileri güncellendi |
| `BookshelfItemDeletedEvent` | `bookshelf_item_deleted` | BookshelfItem | Kitap kitaplıktan silindi |
| `PostCreatedEvent` | `post_created` | Post | Yeni blog yazısı oluşturuldu |
| `PostUpdatedEvent` | `post_updated` | Post | Blog yazısı güncellendi |
| `PostDeletedEvent` | `post_deleted` | Post | Blog yazısı silindi |
| `UserCreatedEvent` | `user_created` | User | Kullanıcı oluşturuldu |
| `UserUpdatedEvent` | `user_updated` | User | Kullanıcı bilgileri güncellendi |
| `UserDeletedEvent` | `user_deleted` | User | Kullanıcı silindi |
| `UserRolesAssignedEvent` | `user_roles_assigned` | User | Kullanıcı rolleri güncellendi |
| `RoleCreatedEvent` | `role_created` | Role | Rol oluşturuldu |
| `RoleUpdatedEvent` | `role_updated` | Role | Rol güncellendi |
| `RoleDeletedEvent` | `role_deleted` | Role | Rol silindi |
| `PermissionsAssignedToRoleEvent` | `permissions_assigned_to_role` | Role | Role'e izin atandı |

Yeni bir domain event eklediğinizde uygun converter ekleyerek tabloyu genişletebilirsiniz.

## 🔍 API Kullanımı
- **Endpoint:** `POST /api/activitylogs/search`
- **Request Body:** `DataGridRequest`
- **Yanıt:** `PaginatedListResponse<GetPaginatedActivityLogsResponse>`
- **Yetki:** `Permissions.ActivityLogsView`
- **Controller:** `ActivityLogsController`
- **Query Handler:** `GetPaginatedActivityLogsQueryHandler`

**Örnek Request:**
```json
{
  "pageIndex": 0,
  "pageSize": 20,
  "filters": [
    {
      "field": "activityType",
      "operator": "eq",
      "value": "post_created"
    }
  ],
  "sortBy": "timestamp",
  "sortDirection": "desc"
}
```

**Örnek Response:**
```json
{
  "pageIndex": 0,
  "pageSize": 20,
  "totalCount": 145,
  "totalPages": 8,
  "hasPreviousPage": false,
  "hasNextPage": true,
  "items": [
    {
      "id": "3a9d...",
      "activityType": "post_created",
      "entityType": "Post",
      "entityId": "7b2c...",
      "title": "\"Intro to CQRS\" oluşturuldu",
      "details": "Kategori ID: 5f3e...",
      "userId": "1a8b...",
      "userName": "admin",
      "timestamp": "2025-10-28T14:30:00Z"
    }
  ]
}
```

## 🎨 Frontend Kullanımı
Activity logs için tam özellikli bir admin paneli mevcuttur:

- **Sayfa:** `/admin/activity-logs`
- **Component:** `activity-logs-page.tsx`
- **API Service:** `features/activity-logs/api.ts`
- **Types:** `features/activity-logs/types.ts`
- **Yetki:** `Permissions.ActivityLogsView` ile route guard korumalı

### Özellikler
- ✅ Sayfalama desteği
- ✅ Activity type filtreleme
- ✅ Entity type filtreleme  
- ✅ Arama (search)
- ✅ Gerçek zamanlı yenileme
- ✅ İkon ve badge gösterimi
- ✅ Kullanıcı bilgileri gösterimi
- ✅ Zaman damgası (relative format)
- ✅ Responsive tasarım

## ➕ Yeni Aktivite Tipi Eklemek
1. **Domain Event Oluşturma:**
   - `Domain/Events/<Aggregate>` altında yeni `[StoreInOutbox]` ile işaretli event sınıfını oluşturun
   - Event'in gerekli tüm bilgileri içerdiğinden emin olun (EntityId, UserId, Title bilgileri vb.)

2. **Event'i Tetikleme:**
   - İlgili command handler'da işlem tamamlandıktan sonra `entity.AddDomainEvent(new ...)` çağırın
   - Event, UnitOfWork.SaveChangesAsync() çağrısı ile OutboxMessages tablosuna kaydedilecektir

3. **Converter Ekleme:**
   - `Infrastructure/Services/BackgroundServices/Outbox/Converters/ActivityLogIntegrationEventConverters.cs` dosyasına yeni converter sınıfı ekleyin
   - `ActivityLogIntegrationEventConverter<TDomainEvent>` base class'ından türetin
   - `InfrastructureServicesRegistration.cs` içinde converter'ı DI container'a kaydedin:
     ```csharp
     services.AddSingleton<IIntegrationEventConverterStrategy, YourNewConverter>();
     ```

4. **Frontend Güncellemeleri:**
   - `clients/blogapp-client/src/pages/admin/activity-logs-page.tsx` içindeki `ACTIVITY_TYPES` dizisine yeni tip ekleyin
   - İkon eşlemelerini `getActivityIcon` fonksiyonuna ekleyin
   - Gerekirse `EntityType` filtreleme seçeneklerini güncelleyin

5. **Test Ekleme:**
   - Unit test'ler için converter'ın doğru çalıştığını test edin
   - Integration test'ler için end-to-end akışı test edin

## ⚙️ Operasyonel Notlar

### Background Services
- **OutboxProcessorService:**
  - Her 5 saniyede bir çalışır (`_processingInterval = TimeSpan.FromSeconds(5)`)
  - Her batch'te maksimum 50 mesaj işler (`BatchSize = 50`)
  - Başarısız mesajlar için 5 denemeye kadar retry yapar (`MaxRetryCount = 5`)
  - Converter bulunamayan event'ler için `MarkAsFailedAsync` çağırır
  - Hosted service olarak otomatik başlatılır

### Message Consumer
- **ActivityLogConsumer:**
  - MassTransit ile RabbitMQ'dan mesaj tüketir
  - `ActivityLogCreatedIntegrationEvent` mesajlarını işler
  - Retry politikası ile hata yönetimi sağlanır
  - Başarılı/başarısız işlemler Serilog ile loglanır
  - Kalıcı hatalar Seq'e düşer

### Bağımlılıklar
- **RabbitMQ:** Message broker olarak kullanılır (MassTransit ile)
- **PostgreSQL:** ActivityLogs ve OutboxMessages tablolarını barındırır
- **Docker Compose:** Development ortamında tüm servisleri orkestre eder

### Performans ve Optimizasyon
- **Indexler:** Timestamp ve (EntityType, EntityId) üzerinde indexler performansı artırır
- **Batch Processing:** OutboxProcessorService batch halinde işleyerek verimlilik sağlar
- **Async Processing:** Ana transaction'ı bloklamaz, yan etkiler asenkron işlenir
- **Retry Mechanism:** Geçici hatalar otomatik tekrar deneme ile tolere edilir

### Veri Yönetimi
- **Retention:** ActivityLogs tablosunda süresiz saklama (compliance için)
- **Cleanup:** Gerekirse custom cleanup service eklenebilir
- **Archiving:** Uzun vadeli saklama için partitioning veya arşivleme stratejisi uygulanabilir
- **GDPR:** Kullanıcı silindiğinde UserId SET NULL olur (DeleteBehavior.SetNull)

## 🔧 Troubleshooting

### Activity Loglar Oluşmuyor
1. **OutboxProcessorService Kontrol:**
   ```bash
   # Logları kontrol et
   docker-compose logs -f blogapp-api | grep "Outbox"
   ```
   - Service başlatıldı mı? → "Outbox İşleyici Servisi başlatıldı" mesajı görünmeli
   - Mesajlar işleniyor mu? → "X adet outbox mesajı işleniyor" mesajı görünmeli

2. **OutboxMessages Tablosu Kontrol:**
   ```sql
   SELECT * FROM "OutboxMessages" 
   WHERE "ProcessedAt" IS NULL 
   ORDER BY "CreatedAt" DESC;
   ```
   - Eğer bekleyen mesajlar varsa → Converter veya RabbitMQ problemi olabilir
   - Eğer hiç mesaj yoksa → Domain event'ler StoreInOutbox attribute'una sahip mi kontrol edin

3. **RabbitMQ Kontrol:**
   ```bash
   # RabbitMQ Management UI
   http://localhost:15672
   # Username: guest, Password: guest
   ```
   - Queue'lar oluşturulmuş mu?
   - Message rate normal mi?
   - Dead letter queue'da mesaj var mı?

4. **ActivityLogConsumer Kontrol:**
   ```bash
   docker-compose logs -f blogapp-api | grep "ActivityLog"
   ```
   - "Processing ActivityLog" mesajları görünüyor mu?
   - Hata mesajları var mı?

### Converter Bulunamıyor
```
Event tipi için converter bulunamadı: YourEventType
```
**Çözüm:**
- Converter sınıfını `ActivityLogIntegrationEventConverters.cs` dosyasına ekleyin
- `InfrastructureServicesRegistration.cs` içinde DI kaydını yapın:
  ```csharp
  services.AddSingleton<IIntegrationEventConverterStrategy, YourNewConverter>();
  ```

### Performance Sorunları
- **Index Eksikliği:** Sorgularda kullanılan alanlara index ekleyin
- **Batch Size:** OutboxProcessorService batch size'ı artırın (dikkatli kullanın)
- **Processing Interval:** Interval'i azaltın (RabbitMQ yükünü artırabilir)
- **Database Load:** ActivityLogs tablosunu ayrı bir database'e taşıyın

## 📎 İlgili Dokümanlar
- [`DOMAIN_EVENTS_IMPLEMENTATION.md`](DOMAIN_EVENTS_IMPLEMENTATION.md) - Domain event pattern implementasyonu
- [`OUTBOX_PATTERN_IMPLEMENTATION.md`](OUTBOX_PATTERN_IMPLEMENTATION.md) - Outbox pattern detayları
- [`LOGGING_ARCHITECTURE.md`](LOGGING_ARCHITECTURE.md) - Genel logging mimarisi (3-tier)
- [`TRANSACTION_MANAGEMENT_STRATEGY.md`](TRANSACTION_MANAGEMENT_STRATEGY.md) - Transaction yönetimi
- [`EVENT_HANDLERS_REMOVAL_REPORT.md`](EVENT_HANDLERS_REMOVAL_REPORT.md) - Event handler'ların neden kaldırıldığı
- [`ADVANCED_FEATURES_IMPLEMENTATION.md`](ADVANCED_FEATURES_IMPLEMENTATION.md) - Genel özellikler ve izinler

## 📊 Dosya Konumları

### Backend
```
src/
├── BlogApp.Domain/
│   ├── Entities/ActivityLog.cs
│   ├── Events/
│   │   ├── CategoryEvents/
│   │   ├── PostEvents/
│   │   ├── UserEvents/
│   │   ├── RoleEvents/
│   │   ├── PermissionEvents/
│   │   ├── BookshelfItemEvents/
│   │   └── IntegrationEvents/ActivityLogCreatedIntegrationEvent.cs
│   └── Repositories/IActivityLogRepository.cs
├── BlogApp.Application/
│   └── Features/ActivityLogs/
│       └── Queries/GetPaginatedList/
│           ├── GetPaginatedActivityLogsQuery.cs
│           ├── GetPaginatedActivityLogsQueryHandler.cs
│           └── GetPaginatedActivityLogsResponse.cs
├── BlogApp.Infrastructure/
│   ├── Consumers/ActivityLogConsumer.cs
│   └── Services/BackgroundServices/
│       ├── OutboxProcessorService.cs
│       └── Outbox/Converters/ActivityLogIntegrationEventConverters.cs
├── BlogApp.Persistence/
│   ├── Configurations/ActivityLogConfiguration.cs
│   └── Repositories/ActivityLogRepository.cs
└── BlogApp.API/
    └── Controllers/ActivityLogsController.cs
```

### Frontend
```
clients/blogapp-client/src/
├── features/activity-logs/
│   ├── api.ts
│   └── types.ts
└── pages/admin/
    └── activity-logs-page.tsx
```

---

Bu doküman BlogApp'in güncel domain event tabanlı activity logging altyapısını özetler. Yeni aggregate veya event eklerken aynı outbox zincirini takip etmek yeterlidir.
