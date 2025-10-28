# Domain Events Pattern - Implementation Guide

## Hızlı Özet

| Özellik | Değer |
|---------|-------|
| **Pattern** | Domain Events + Outbox Pattern |
| **Event Sayısı** | 17 farklı domain event |
| **Aggregate Sayısı** | 6 (Category, Post, User, Role, Permission, BookshelfItem) |
| **İşleme Sıklığı** | 5 saniye |
| **Batch Size** | 50 mesaj/işlem |
| **Max Retry** | 5 deneme |
| **Message Broker** | RabbitMQ + MassTransit |
| **Transaction Güvencesi** | ACID (event + business data) |
| **Bellek Koruması** | Finally bloğunda otomatik temizlik |

## 1. Amaç
Domain event'ler, aggregate'lerin önemli değişimlerini ifade eder ve BlogApp'te Outbox Pattern ile birlikte kullanılarak güvenilir audit + entegrasyon akışını besler. Command handler'lar sadece iş mantığını yürütür; event'ler `UnitOfWork.SaveChangesAsync` sırasında Outbox tablosuna kaydedilir ve background servisler tarafından işlenir.

## 2. Temel Bileşenler

### 2.1 BaseEntity & DomainEvent
`BaseEntity`, tüm entity'lerde domain event listesini tutar:

```csharp
public abstract class BaseEntity : IEntityTimestamps, IHasDomainEvents
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid? UpdatedById { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    // Domain Events - Entity üzerindeki önemli olayları takip eder
    private readonly List<IDomainEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    
    public void RemoveDomainEvent(IDomainEvent domainEvent) => _domainEvents.Remove(domainEvent);
    
    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

**DomainEvent Soyut Sınıfı:**
```csharp
public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
```

**IDomainEvent Interface:**
```csharp
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
```

`IDomainEvent : INotification` marker'ı, MediatR uyumluluğunu korur (ileride tekrar publish etmek istersek hazır).

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
public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    try
    {
        // Kaydetmeden önce takip edilen entity'lerden domain event'leri al
        var domainEvents = GetDomainEvents().ToList();

        // Domain event'leri outbox mesajlarına dönüştür
        foreach (var domainEvent in domainEvents)
        {
            if (ShouldStoreInOutbox(domainEvent))
            {
                var outboxMessage = new OutboxMessage
                {
                    EventType = domainEvent.GetType().Name,
                    Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                    CreatedAt = DateTime.UtcNow,
                    RetryCount = 0
                };
                await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
            }
        }

        // Business data ve outbox mesajları tek transaction'da kaydedilir
        var result = await _context.SaveChangesAsync(cancellationToken);

        // Domain event'leri temizle
        ClearDomainEvents();

        return result;
    }
    finally
    {
        // Başarısız olsa bile event'leri temizle (bellek sızıntısını önle)
        ClearDomainEvents();
    }
}

private static bool ShouldStoreInOutbox(IDomainEvent domainEvent)
{
    // [StoreInOutbox] attribute'una sahip olup olmadığını kontrol et
    var eventType = domainEvent.GetType();
    return eventType.GetCustomAttributes(typeof(StoreInOutboxAttribute), false).Any();
}
```

**GetDomainEvents ve ClearDomainEvents:**
```csharp
public IEnumerable<IDomainEvent> GetDomainEvents()
{
    // BaseEntity'den türeyen tüm entity'lerin event'lerini al
    return _context.ChangeTracker
        .Entries<BaseEntity>()
        .Where(e => e.Entity.DomainEvents.Any())
        .SelectMany(e => e.Entity.DomainEvents)
        .ToList();
}

public void ClearDomainEvents()
{
    // BaseEntity'den türeyen tüm entity'lerin event'lerini temizle
    var entities = _context.ChangeTracker
        .Entries<BaseEntity>()
        .Where(e => e.Entity.DomainEvents.Any())
        .Select(e => e.Entity)
        .ToList();

    foreach (var entity in entities)
    {
        entity.ClearDomainEvents();
    }
}
```

### 2.4 Outbox İşleme Zinciri

```
Command Handler
    → entity.AddDomainEvent(new ...Event(...))
        → UnitOfWork.SaveChangesAsync (event → outbox payload)
            → OutboxProcessorService (5sn interval, batch=50, max retry=5)
                → IIntegrationEventConverterStrategy (event tipi → ActivityLogCreatedIntegrationEvent)
                    → MassTransit Publish → RabbitMQ (activity-log-queue)
                        → ActivityLogConsumer → ActivityLogs tablosu
```

**OutboxProcessorService Detayları:**
- **İşleme Sıklığı:** 5 saniyede bir
- **Batch Size:** Her seferde 50 mesaj işlenir
- **Max Retry:** Başarısız mesajlar 5 kereye kadar tekrar denenir
- **Converter Desteği:** Event tiplerine göre dinamik converter stratejisi

Konvertörler `src/BlogApp.Infrastructure/Services/BackgroundServices/Outbox/Converters/ActivityLogIntegrationEventConverters.cs` dosyasında bulunur. Her converter, spesifik bir domain event tipini `ActivityLogCreatedIntegrationEvent`'e dönüştürür. DI kaydı `InfrastructureServicesRegistration` içinde yapılır.

## 3. Domain Event Envanteri

| Aggregate | Event | Açıklama |
|-----------|-------|----------|
| Category | `CategoryCreatedEvent`, `CategoryUpdatedEvent`, `CategoryDeletedEvent` | Kategori yaşam döngüsü |
| Post | `PostCreatedEvent`, `PostUpdatedEvent`, `PostDeletedEvent` | Blog yazısı operasyonları |
| User | `UserCreatedEvent`, `UserUpdatedEvent`, `UserDeletedEvent`, `UserRolesAssignedEvent` | Kullanıcı yönetimi |
| Role | `RoleCreatedEvent`, `RoleUpdatedEvent`, `RoleDeletedEvent` | Rol işlemleri |
| Permission | `PermissionsAssignedToRoleEvent` | İzin atama işlemleri |
| BookshelfItem | `BookshelfItemCreatedEvent`, `BookshelfItemUpdatedEvent`, `BookshelfItemDeletedEvent` | Kitaplık öğeleri yönetimi |

Tüm event dosyaları `src/BlogApp.Domain/Events/*` altında tutulur ve tamamı `[StoreInOutbox]` ile işaretlidir.

## 4. Komutlarda Kullanım Deseni

```csharp
public async Task<IResult> Handle(CreatePostCommand request, CancellationToken cancellationToken)
{
    var actorId = currentUserService.GetCurrentUserId() ?? SystemUsers.SystemUserId;

    var post = new Post
    {
        CategoryId = request.CategoryId,
        Title = request.Title,
        Body = request.Body,
        Summary = request.Summary,
        Thumbnail = request.Thumbnail,
        IsPublished = request.IsPublished
    };

    await postRepository.AddAsync(post);

    // ✅ Outbox Pattern için SaveChanges'dan ÖNCE domain event'i tetikle
    post.AddDomainEvent(new PostCreatedEvent(post.Id, post.Title, post.CategoryId, actorId));

    await unitOfWork.SaveChangesAsync(cancellationToken);

    return new SuccessResult("Post bilgisi başarıyla eklendi.");
}
```

**Önemli Notlar:**
- Domain event `SaveChangesAsync`'den **önce** eklenir
- Event raise etmek için ekstra DI kaydına gerek yoktur
- UnitOfWork otomatik olarak event'leri toplar ve Outbox'a kaydeder
- `actorId` genellikle mevcut kullanıcı veya sistem kullanıcısı olarak belirlenir

## 5. Avantajlar

- **Separation of Concerns:** Handler'lar sadece aggregate mutasyonundan sorumlu; yan etkiler (activity log, bildirim) outbox tüketicileriyle yönetiliyor
- **Testability:** Komut/handler testleri domain event raise edildiğini doğrular; converter/consumer testleri yan etkileri kapsar
- **Eventual Consistency:** Activity log gibi operasyonlar ana transaction'ı bloklamaz, retry + dead-letter desteğiyle güvenilirlik artar
- **ACID Garantisi:** Domain event'ler business data ile aynı transaction'da kaydedilir, veri tutarlılığı sağlanır
- **Bellek Yönetimi:** Finally bloğunda event'lerin temizlenmesi, bellek sızıntısını önler
- **Future-Proof:** `IDomainEvent`, `INotification` implement eder; gerekirse tekrar MediatR publish pipeline'ı eklenebilir
- **Tip Güvenliği:** `[StoreInOutbox]` attribute ile derleme zamanı güvenliği sağlanır

## 6. Yeni Domain Event Eklemek İçin Adımlar

1. **Event Sınıfı Oluştur:**
   - `src/BlogApp.Domain/Events/<Aggregate>` altında yeni sınıf oluştur
   - `DomainEvent` soyut sınıfından türet
   - `[StoreInOutbox]` attribute'unu ekle
   
   ```csharp
   using BlogApp.Domain.Common;
   using BlogApp.Domain.Common.Attributes;

   namespace BlogApp.Domain.Events.BookshelfItemEvents;

   [StoreInOutbox]
   public sealed class BookshelfItemCreatedEvent : DomainEvent
   {
       public Guid ItemId { get; }
       public string Title { get; }
       public Guid ActorId { get; }

       public BookshelfItemCreatedEvent(Guid itemId, string title, Guid actorId)
       {
           ItemId = itemId;
           Title = title;
           ActorId = actorId;
       }
   }
   ```

2. **Command Handler'da Event Raise Et:**
   ```csharp
   // Entity oluştur/güncelle
   var item = new BookshelfItem { /* ... */ };
   
   // Domain event ekle (SaveChanges'dan ÖNCE)
   item.AddDomainEvent(new BookshelfItemCreatedEvent(item.Id, item.Title, actorId));
   
   // Kaydet (UnitOfWork otomatik olarak event'i outbox'a yazar)
   await unitOfWork.SaveChangesAsync(cancellationToken);
   ```

3. **Activity Log Üretimi İçin Converter Ekle:**
   `ActivityLogIntegrationEventConverters.cs` içinde yeni converter tanımla:
   ```csharp
   internal sealed class BookshelfItemCreatedIntegrationEventConverter 
       : ActivityLogIntegrationEventConverter<BookshelfItemCreatedEvent>
   {
       public override string EventType => nameof(BookshelfItemCreatedEvent);

       protected override ActivityLogCreatedIntegrationEvent Convert(BookshelfItemCreatedEvent domainEvent) => new(
           ActivityType: "bookshelf_item_created",
           EntityType: "BookshelfItem",
           EntityId: domainEvent.ItemId,
           Title: $"\"{domainEvent.Title}\" kitaplığa eklendi",
           Details: null,
           UserId: domainEvent.ActorId,
           Timestamp: DateTime.UtcNow
       );
   }
   ```

4. **DI Kaydı Yap:**
   `InfrastructureServicesRegistration.cs` dosyasında converter'ı kaydet:
   ```csharp
   services.AddSingleton<IIntegrationEventConverterStrategy, BookshelfItemCreatedIntegrationEventConverter>();
   ```

5. **Test Ekle:**
   - Command handler testlerinde event raise edildiğini doğrula
   - Converter testlerinde JSON dönüşümü kontrol et
   - Consumer testlerinde veri tabanına yazımı test et

## 7. Test Önerileri

### 7.1 Command Handler Testleri
Domain event raise edildiğini kontrol et:
```csharp
[Fact]
public async Task Handle_WhenPostCreated_ShouldRaiseDomainEvent()
{
    // Arrange
    var command = new CreatePostCommand { /* ... */ };
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    var post = await postRepository.GetByIdAsync(postId);
    post.DomainEvents.Should().ContainSingle(e => e is PostCreatedEvent);
    
    var domainEvent = post.DomainEvents.OfType<PostCreatedEvent>().First();
    domainEvent.PostId.Should().Be(post.Id);
    domainEvent.Title.Should().Be(command.Title);
}
```

### 7.2 Converter Testleri
JSON payload doğru integration event'e dönüşüyor mu kontrol et:
```csharp
[Fact]
public void Convert_PostCreatedEvent_ShouldReturnActivityLogEvent()
{
    // Arrange
    var converter = new PostCreatedIntegrationEventConverter();
    var domainEvent = new PostCreatedEvent(postId, "Test Post", categoryId, userId);
    var payload = JsonSerializer.Serialize(domainEvent);
    
    // Act
    var result = converter.Convert(payload) as ActivityLogCreatedIntegrationEvent;
    
    // Assert
    result.Should().NotBeNull();
    result.ActivityType.Should().Be("post_created");
    result.EntityType.Should().Be("Post");
    result.EntityId.Should().Be(postId);
}
```

### 7.3 Integration Testleri
Mesaj işlendiğinde beklenen veritabanına yazılıyor mu kontrol et:
```csharp
[Fact]
public async Task Consume_ActivityLogEvent_ShouldSaveToDatabase()
{
    // Arrange
    var integrationEvent = new ActivityLogCreatedIntegrationEvent(/* ... */);
    
    // Act
    await consumer.Consume(CreateConsumeContext(integrationEvent));
    
    // Assert
    var activityLog = await dbContext.ActivityLogs
        .FirstOrDefaultAsync(x => x.EntityId == integrationEvent.EntityId);
    
    activityLog.Should().NotBeNull();
    activityLog.ActivityType.Should().Be(integrationEvent.ActivityType);
}
```

### 7.4 OutboxProcessor Testleri
Outbox mesajlarının doğru işlendiğini kontrol et:
```csharp
[Fact]
public async Task ProcessOutboxMessages_ShouldPublishToRabbitMQ()
{
    // Arrange
    var outboxMessage = new OutboxMessage
    {
        EventType = "PostCreatedEvent",
        Payload = JsonSerializer.Serialize(new PostCreatedEvent(/* ... */)),
        ProcessedAt = null
    };
    await dbContext.OutboxMessages.AddAsync(outboxMessage);
    await dbContext.SaveChangesAsync();
    
    // Act
    await outboxProcessor.ProcessOutboxMessagesAsync(CancellationToken.None);
    
    // Assert
    var processed = await dbContext.OutboxMessages.FindAsync(outboxMessage.Id);
    processed.ProcessedAt.Should().NotBeNull();
    processed.Error.Should().BeNull();
}
```

## 8. En İyi Pratikler ve Yaygın Hatalar

### 8.1 En İyi Pratikler

✅ **Event'leri SaveChanges'dan önce ekle**
```csharp
entity.AddDomainEvent(new EntityCreatedEvent(...));
await unitOfWork.SaveChangesAsync(cancellationToken); // ✅ Doğru
```

✅ **Actor/User bilgisini event'e dahil et**
```csharp
var actorId = currentUserService.GetCurrentUserId() ?? SystemUsers.SystemUserId;
entity.AddDomainEvent(new EntityCreatedEvent(..., actorId)); // ✅ Doğru
```

✅ **Event isimlerini geçmiş zaman kullan**
```csharp
public class PostCreatedEvent : DomainEvent { } // ✅ Doğru
public class CreatePostEvent : DomainEvent { } // ❌ Yanlış
```

✅ **Event'leri immutable yap**
```csharp
public class PostCreatedEvent : DomainEvent
{
    public Guid PostId { get; } // ✅ Doğru - sadece getter
    
    public PostCreatedEvent(Guid postId) => PostId = postId;
}
```

✅ **[StoreInOutbox] attribute'unu unutma**
```csharp
[StoreInOutbox] // ✅ Bunu eklemeyi unutma
public class PostCreatedEvent : DomainEvent { }
```

### 8.2 Yaygın Hatalar

❌ **Event'i SaveChanges'dan sonra ekleme**
```csharp
await unitOfWork.SaveChangesAsync(cancellationToken);
entity.AddDomainEvent(new EntityCreatedEvent(...)); // ❌ Yanlış - event kaydedilmez
```

❌ **Event constructor'ında complex logic**
```csharp
public PostCreatedEvent(Post post)
{
    // ❌ Yanlış - constructor'da kompleks işlem yapma
    PostId = post.Id;
    Title = CalculateComplexTitle(post); // ❌ Yan etki
}
```

❌ **Event'leri manuel publish etme**
```csharp
await mediator.Publish(new PostCreatedEvent(...)); // ❌ Yanlış - UnitOfWork otomatik halleder
```

❌ **Aynı event'i birden çok kez ekleme**
```csharp
entity.AddDomainEvent(new PostCreatedEvent(...));
entity.AddDomainEvent(new PostCreatedEvent(...)); // ❌ Yanlış - duplicate event
```

### 8.3 Sorun Giderme

**Problem:** Event outbox'a kaydedilmiyor
- **Çözüm:** `[StoreInOutbox]` attribute'unu kontrol et
- **Çözüm:** Event'in `SaveChanges`'dan önce eklendiğini doğrula

**Problem:** Activity log oluşturulmuyor
- **Çözüm:** Converter'ın DI'a kaydedildiğini kontrol et
- **Çözüm:** OutboxProcessorService'in çalıştığını doğrula
- **Çözüm:** RabbitMQ bağlantısını kontrol et

**Problem:** Outbox mesajları sürekli retry ediliyor
- **Çözüm:** Converter'da JSON deserialize hatası olabilir
- **Çözüm:** Integration event schema'sını kontrol et
- **Çözüm:** Consumer'da exception loglarını incele

**Problem:** Bellek sızıntısı
- **Çözüm:** `finally` bloğunda `ClearDomainEvents()` çağrıldığından emin ol
- **Çözüm:** UnitOfWork'ün doğru dispose edildiğini kontrol et

## 9. Checklist (Güncel Durum)

- [x] BaseEntity domain event koleksiyonu
- [x] `IHasDomainEvents` interface ve implementasyonu
- [x] `[StoreInOutbox]` attribute
- [x] UnitOfWork → Outbox entegrasyonu (ACID garantili)
- [x] OutboxProcessorService + MassTransit publish
- [x] ActivityLog converter'ları (17 adet event için)
- [x] ActivityLogConsumer
- [x] Tüm aggregate'ler için domain event desteği (Category, Post, User, Role, Permission, BookshelfItem)
- [x] Finally bloğunda event temizleme (bellek sızıntısı koruması)
- [x] Retry mekanizması (max 5 deneme)
- [ ] Domain event raise eden komutlar için unit testler (eklenmeli)
- [ ] Outbox converter/consumer senaryoları için entegrasyon testleri

## 10. İlgili Dokümanlar
- `docs/OUTBOX_PATTERN_IMPLEMENTATION.md` - Outbox Pattern detaylı açıklaması
- `docs/OUTBOX_PATTERN_SETUP_SUMMARY.md` - Outbox kurulum özeti
- `docs/ACTIVITY_LOGGING_README.md` - Activity log sistemi
- `docs/TRANSACTION_MANAGEMENT_STRATEGY.md` - Transaction yönetimi stratejisi
- `docs/DOMAIN_EVENTS_IMPROVEMENT.md` - Domain events iyileştirme raporu
- `docs/CODE_CENTRALIZATION_REPORT.md` - Kod merkezileştirme raporu

---

**Son Güncelleme:** 2025-10-28

Domain events sayesinde BlogApp'teki yan etkiler tamamen outbox pipeline'ına devredildi. Yeni aggregate'ler eklerken aynı yaklaşımı izlediğinizde hem audit akışı hem de entegrasyon süreci otomatik olarak genişleyecektir. Sistem, **17 farklı domain event** tipini desteklemekte ve tümü `[StoreInOutbox]` attribute'u ile işaretlenmiştir.
