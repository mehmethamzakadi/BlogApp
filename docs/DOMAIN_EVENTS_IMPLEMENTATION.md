# Domain Events Pattern - Implementation Guide

## 1. Amaç
Domain event'ler, aggregate'lerin önemli değişimlerini ifade eder ve BlogApp'te Outbox Pattern ile birlikte kullanılarak güvenilir audit + entegrasyon akışını besler. Command handler'lar sadece iş mantığını yürütür; event'ler `UnitOfWork.SaveChangesAsync` sırasında Outbox tablosuna kaydedilir ve background servisler tarafından işlenir.

## 2. Temel Bileşenler

### 2.1 BaseEntity & DomainEvent
`BaseEntity`, tüm entity'lerde domain event listesini tutar:

```csharp
public abstract class BaseEntity : IEntityTimestamps, IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = new();
    [NotMapped] public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

`DomainEvent` soyut sınıfı ve `IDomainEvent : INotification` marker'ı, MediatR uyumluluğunu korur (ileride tekrar publish etmek istersek hazır).

### 2.2 StoreInOutbox Attribute
`[StoreInOutbox]` sadece outbox'a gitmesi gereken event'leri işaretler. Örnek:

```csharp
[StoreInOutbox]
public class PostCreatedEvent : DomainEvent
{
    public Guid PostId { get; }
    public string Title { get; }
    public Guid CategoryId { get; }
    public Guid CreatedById { get; }
    // ctor...
}
```

### 2.3 UnitOfWork Entegrasyonu
`UnitOfWork.SaveChangesAsync` tracked entity'lerden event'leri toplar, attribute denetimine tabi tutar ve OutboxMessages tablosuna JSON payload olarak yazar:

```csharp
var domainEvents = GetDomainEvents().ToList();
foreach (var domainEvent in domainEvents)
{
    if (ShouldStoreInOutbox(domainEvent))
    {
        var outboxMessage = new OutboxMessage
        {
            EventType = domainEvent.GetType().Name,
            Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
            CreatedAt = DateTime.UtcNow
        };
        await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    }
}
await _context.SaveChangesAsync(cancellationToken);
ClearDomainEvents();
```

### 2.4 Outbox İşleme Zinciri

```
Command Handler
    → entity.AddDomainEvent(new ...Event(...))
        → UnitOfWork.SaveChangesAsync (event → outbox payload)
            → OutboxProcessorService (5sn interval, batch=50)
                → IIntegrationEventConverterStrategy (event tipi → ActivityLogCreatedIntegrationEvent)
                    → MassTransit Publish → RabbitMQ (activity-log-queue)
                        → ActivityLogConsumer → ActivityLogs tablosu
```

Konvertörler `src/BlogApp.Infrastructure/Services/BackgroundServices/Outbox/Converters/ActivityLogIntegrationEventConverters.cs` dosyasında; DI kaydı `InfrastructureServicesRegistration` içinde.

## 3. Domain Event Envanteri

| Aggregate | Event | Açıklama |
|-----------|-------|----------|
| Category | `CategoryCreatedEvent`, `CategoryUpdatedEvent`, `CategoryDeletedEvent` | Kategori yaşam döngüsü |
| Post | `PostCreatedEvent`, `PostUpdatedEvent`, `PostDeletedEvent` | Blog yazısı operasyonları |
| User | `UserCreatedEvent`, `UserUpdatedEvent`, `UserDeletedEvent`, `UserRolesAssignedEvent` | Kullanıcı yönetimi |
| Role | `RoleCreatedEvent`, `RoleUpdatedEvent`, `RoleDeletedEvent`, `PermissionsAssignedToRoleEvent` | Rol ve izin işlemleri |

Tüm event dosyaları `src/BlogApp.Domain/Events/*` altında tutulur ve tamamı `[StoreInOutbox]` ile işaretlidir.

## 4. Komutlarda Kullanım Deseni

```csharp
public async Task<Guid> Handle(CreatePostCommand request, CancellationToken ct)
{
    var post = new Post(request.Title, request.Content, request.CategoryId, currentUserId);
    post.AddDomainEvent(new PostCreatedEvent(post.Id, post.Title, post.CategoryId, currentUserId));

    await _postRepository.AddAsync(post, ct);
    await _unitOfWork.SaveChangesAsync(ct);

    return post.Id;
}
```

Event raise etmek için ekstra DI kaydına gerek yoktur; Outbox işleyicisi event'i otomatik yakalar.

## 5. Avantajlar

- **Separation of Concerns:** Handler'lar sadece aggregate mutasyonundan sorumlu, yan etkiler (activity log, bildirim) outbox tüketicileriyle yönetiliyor.
- **Testability:** Komut/handler testleri domain event raise edildiğini doğrular; converter/consumer testleri yan etkileri kapsar.
- **Eventual Consistency:** Activity log gibi operasyonlar ana transaction'ı bloklamaz, retry + dead-letter desteğiyle güvenilirlik artar.
- **Future-Proof:** `IDomainEvent` hâlâ `INotification`; gerekirse tekrar MediatR publish pipeline'ı eklenebilir.

## 6. Yeni Domain Event Eklemek İçin Adımlar

1. `Domain/Events/<Aggregate>` altında yeni sınıf oluştur, `DomainEvent`ten türet ve `[StoreInOutbox]` ekle.
2. Command handler'da uygun noktada `entity.AddDomainEvent(new ...)` çağır.
3. Eğer activity log üretilecekse `ActivityLogIntegrationEventConverters` içinde yeni converter yaz ve DI'a ekle.
4. Gerekirse yeni integration event/consumer oluştur (`ActivityLog` dışındaki senaryolar için).
5. Unit/integration testlerini güncelle.

## 7. Test Önerileri

- **Command Testi:** Event raise edildi mi?
  ```csharp
  post.DomainEvents.Should().ContainSingle(e => e is PostCreatedEvent);
  ```
- **Converter Testi:** JSON payload doğru integration event'e dönüşüyor mu?
- **Consumer Testi:** Mesaj işlendiğinde beklenen veri tabanına yazılıyor mu?

## 8. Checklist (Güncel Durum)

- [x] BaseEntity domain event koleksiyonu
- [x] `[StoreInOutbox]` attribute
- [x] UnitOfWork → Outbox entegrasyonu
- [x] OutboxProcessorService + MassTransit publish
- [x] ActivityLog converter'ları
- [x] ActivityLogConsumer
- [ ] Domain event raise eden komutlar için unit testler (eklenmeli)
- [ ] Outbox converter/consumer senaryoları için entegrasyon testleri

## 9. İlgili Dokümanlar
- `docs/OUTBOX_PATTERN_IMPLEMENTATION.md`
- `docs/OUTBOX_PATTERN_SETUP_SUMMARY.md`
- `docs/ACTIVITY_LOGGING_README.md`
- `docs/TRANSACTION_MANAGEMENT_STRATEGY.md`

Domain events sayesinde BlogApp’teki yan etkiler tamamen outbox pipeline'ına devredildi. Yeni aggregate'ler eklerken aynı yaklaşımı izlediğinizde hem audit akışı hem de entegrasyon süreci otomatik olarak genişleyecektir.
