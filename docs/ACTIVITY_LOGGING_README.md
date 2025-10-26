# BlogApp - Activity Logging Guide

## 🎯 Amaç
Activity logging, kullanıcı ve sistem aksiyonlarını kalıcı olarak kaydederek audit trail, güvenlik soruşturmaları ve operasyonel raporlamayı destekler. BlogApp bu akışı domain event → outbox → RabbitMQ → consumer zinciri ile uygular; böylece ana işlem tamamlandıktan sonra yan etkiler güvenilir biçimde işlenir.

## 🧱 Mimarinin Özeti
1. **Command Handler** domain entity üzerinde değişiklik yapar ve `entity.AddDomainEvent(...)` çağırır.
2. **UnitOfWork.SaveChangesAsync** çağrısı entity üzerindeki `[StoreInOutbox]` ile işaretli event'leri `OutboxMessages` tablosuna JSON olarak kaydeder.
3. **OutboxProcessorService** (5 sn aralıklarla, batch=50) mesajları okur, uygun `ActivityLogIntegrationEventConverter` ile `ActivityLogCreatedIntegrationEvent` üretir ve MassTransit ile RabbitMQ'ya yayınlar.
4. **ActivityLogConsumer** mesajı tüketir ve `ActivityLogs` tablosuna kalıcı kayıt ekler.

Bu sayede activity log oluşturma ana transaction'ı bloklamaz, tekrar deneyen (retry) ve dead-letter politikaları ile güvenilirlik sağlanır.

## 💾 Veritabanı Şeması
```sql
CREATE TABLE "ActivityLogs" (
  "Id" UUID PRIMARY KEY,
  "ActivityType" VARCHAR(64) NOT NULL,
  "EntityType" VARCHAR(64) NOT NULL,
  "EntityId" UUID NULL,
  "Title" VARCHAR(500) NOT NULL,
  "Details" VARCHAR(2000) NULL,
  "UserId" UUID NULL,
  "Timestamp" TIMESTAMP WITH TIME ZONE NOT NULL,
  CONSTRAINT "FK_ActivityLogs_Users" FOREIGN KEY ("UserId")
    REFERENCES "Users"("Id") ON DELETE SET NULL
);

CREATE INDEX "IX_ActivityLogs_Timestamp" ON "ActivityLogs"("Timestamp");
CREATE INDEX "IX_ActivityLogs_EntityType" ON "ActivityLogs"("EntityType");
CREATE INDEX "IX_ActivityLogs_UserId" ON "ActivityLogs"("UserId");
```

> `Users` tablosu BlogApp'in custom kimlik sistemi tarafından yönetilir; Identity'nin `AspNetUsers` şemasına bağımlılık yoktur.

## 🗂️ Domain Event → Activity Type Eşlemeleri
`ActivityLogIntegrationEventConverters.cs` dosyası aşağıdaki eşlemeleri üretir:

| Domain Event | ActivityType | EntityType | Açıklama |
|--------------|--------------|------------|----------|
| `CategoryCreatedEvent` | `category_created` | Category | Yeni kategori oluşturuldu |
| `CategoryUpdatedEvent` | `category_updated` | Category | Kategori güncellendi |
| `CategoryDeletedEvent` | `category_deleted` | Category | Kategori silindi |
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

```json
{
  "pageIndex": 0,
  "pageSize": 20,
  "items": [
    {
      "activityType": "post_created",
      "entityType": "Post",
      "entityId": "3a9d...",
      "title": "\"Intro to CQRS\" oluşturuldu",
      "userName": "admin",
      "timestamp": "2025-10-25T14:30:00Z"
    }
  ]
}
```

## ➕ Yeni Aktivite Tipi Eklemek
1. `Domain/Events/<Aggregate>` altında yeni `[StoreInOutbox]` event sınıfını oluşturun.
2. İlgili command handler'da `entity.AddDomainEvent(new ...)` çağırın.
3. `ActivityLogIntegrationEventConverters` dosyasında yeni converter sınıfı ekleyin ve DI kaydını `InfrastructureServicesRegistration` içinde yapın.
4. Gerekirse frontend filtrelerini/ikon eşlemelerini güncelleyin.
5. Unit/integration testlerine yeni senaryoyu ekleyin.

## ⚙️ Operasyonel Notlar
- **OutboxProcessorService** her 5 saniyede bir maksimum 50 mesaj işler; başarısız mesajlar için 5 denemeye kadar retry yapar.
- Activity log tüketimi `ActivityLogConsumer` üzerinden MassTransit retry politikası ile yönetilir; kalıcı hata durumunda loglar Serilog üzerinden Seq'e düşer.
- Docker Compose kullanırken RabbitMQ (`activity-log-queue`) ve PostgreSQL servislerinin ayakta olması gerekir.
- Uzun vadeli saklama politikası ihtiyaç duyulursa `ActivityLogs` tablosu partitioning veya arşivleme ile genişletilebilir.

## 📎 İlgili Dokümanlar
- `docs/DOMAIN_EVENTS_IMPLEMENTATION.md`
- `docs/OUTBOX_PATTERN_IMPLEMENTATION.md`
- `docs/LOGGING_ARCHITECTURE.md`
- `docs/TRANSACTION_MANAGEMENT_STRATEGY.md`

Bu doküman BlogApp'in güncel domain event tabanlı activity logging altyapısını özetler. Yeni aggregate veya event eklerken aynı outbox zincirini takip etmek yeterlidir.
