# Domain Events İyileştirme Raporu

## 🎯 Sorunun Kökeni
Önceki sürümlerde kimlik yönetimi ASP.NET Identity tabanlı `AppUser`/`AppRole` modellerine dayanıyordu. Bu sınıflar `IdentityUser<int>` ve `IdentityRole<int>`'ten türediği için `BaseEntity` altyapımızdaki domain event listesinden yararlanamıyordu. Sonuç olarak kullanıcı/rol komutlarında domain event raise edilemedi ve handler'lar `OutboxMessageHelper` gibi yardımcı sınıflarla manuel outbox mesajı üretmek zorunda kaldı.

**Yan etkiler:**
- Domain event pattern'i aggregate'ler arasında tutarsız hale geldi.
- Her handler'da tekrar eden outbox kodu oluştu.
- `IOutboxMessageRepository` gereksiz bağımlılıklar ekledi.

## ✅ Çözümün Özeti
1. **Custom Kimlik Modelleri:** `User` ve `Role` entity'leri yeniden yazılarak `BaseEntity`'den türetildi (`src/BlogApp.Domain/Entities/User.cs`, `Role.cs`). Bu sayede domain event koleksiyonu tüm aggregate'lerde ortak hale geldi.
2. **IHasDomainEvents Interface'i:** Tip güvenliği için `IHasDomainEvents` interface'i (`Domain/Common/IHasDomainEvents.cs`) oluşturuldu ve `BaseEntity` tarafından implement edildi.
3. **UnitOfWork Güncellemesi:** `Persistence/Repositories/UnitOfWork.cs` sadece `BaseEntity`'den türeyen entity'lerden domain event'leri toplar (tüm entity'ler `BaseEntity`'den türediği için yeterli); outbox mesajı üretimi merkezi hale geldi.
4. **Handler Temizliği:** Kullanıcı ve rol komutları dahil olmak üzere 20+ handler domain event raise edecek şekilde güncellendi. Manuel outbox üretimi ve `IOutboxMessageRepository` bağımlılıkları kaldırıldı.
5. **OutboxMessageHelper Kaldırıldı:** Artık hiçbir senaryoda kullanılmadığı için helper sınıfı tamamen silindi.

## 🔁 Eski vs Yeni Akış

### ❌ Eski (manuel outbox)
```csharp
// Anti-pattern: Manuel outbox mesajı oluşturma
var domainEvent = new RoleCreatedEvent(role.Id, role.Name!, currentUserId);
var outboxMessage = OutboxMessageHelper.CreateFromDomainEvent(domainEvent);
await _outboxRepository.AddAsync(outboxMessage);
await _unitOfWork.SaveChangesAsync(ct);

// Sorunlar:
// - Tekrar eden kod
// - Ekstra repository bağımlılığı
// - Domain event pattern'i ile tutarsız
// - Test etmesi zor
```

### ✅ Yeni (standart domain event)
```csharp
// Domain-Driven Design uyumlu yaklaşım
role.AddDomainEvent(new RoleCreatedEvent(role.Id, role.Name!, currentUserId));
await _unitOfWork.SaveChangesAsync(ct);

// UnitOfWork otomatik olarak:
// 1. Domain event'leri toplar (BaseEntity üzerinden)
// 2. [StoreInOutbox] attribute'u kontrol eder
// 3. Event'leri OutboxMessages tablosuna yazar
// 4. Business data ile aynı transaction'da kaydeder (ACID)
// 5. Event'leri temizler (bellek koruması)

// Avantajlar:
// ✅ Tüm entity'lerde tutarlı pattern
// ✅ Domain-Driven Design prensiplerine uygun
// ✅ Daha temiz, daha az kod
// ✅ Repository dependency'sine gerek yok
// ✅ Test etmesi kolay
```

## 🏗️ Teknik Detaylar

### BaseEntity Yapısı
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

    // Domain Events
    private readonly List<IDomainEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

### UnitOfWork Domain Event İşleme
```csharp
public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    try
    {
        // 1. Domain event'leri topla
        var domainEvents = GetDomainEvents().ToList();

        // 2. [StoreInOutbox] attribute'una sahip event'leri outbox'a kaydet
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

        // 3. Business data + outbox mesajlarını tek transaction'da kaydet
        var result = await _context.SaveChangesAsync(cancellationToken);

        // 4. Event'leri temizle
        ClearDomainEvents();

        return result;
    }
    finally
    {
        // Hata olsa bile event'leri temizle (bellek koruması)
        ClearDomainEvents();
    }
}

public IEnumerable<IDomainEvent> GetDomainEvents()
{
    // BaseEntity'den türeyen tüm entity'lerin event'lerini al
    return _context.ChangeTracker
        .Entries<BaseEntity>()
        .Where(e => e.Entity.DomainEvents.Any())
        .SelectMany(e => e.Entity.DomainEvents)
        .ToList();
}

private static bool ShouldStoreInOutbox(IDomainEvent domainEvent)
{
    // [StoreInOutbox] attribute kontrolü
    var eventType = domainEvent.GetType();
    return eventType.GetCustomAttributes(typeof(StoreInOutboxAttribute), false).Any();
}
```

### Domain Event Örneği
```csharp
[StoreInOutbox]  // Bu attribute outbox'a kaydedilmesini sağlar
public sealed class UserCreatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string UserName { get; }
    public string Email { get; }
    public Guid ActorId { get; }

    public UserCreatedEvent(Guid userId, string userName, string email, Guid actorId)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
        ActorId = actorId;
    }
}
```

### Handler Kullanım Örneği
```csharp
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var currentUserId = _currentUserService.GetCurrentUserId() ?? SystemUsers.SystemUserId;

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            // ... diğer özellikler
        };

        await _userRepository.AddAsync(user, ct);

        // Domain event ekle
        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.UserName!, user.Email!, currentUserId));

        // UnitOfWork otomatik olarak event'i outbox'a kaydeder
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<UserDto>.Success(userDto);
    }
}
```

## 📦 Güncellenen Başlıca Dosyalar

### Domain Katmanı
- `src/BlogApp.Domain/Common/BaseEntity.cs` - Domain event listesi ve `IHasDomainEvents` implementasyonu
- `src/BlogApp.Domain/Common/IHasDomainEvents.cs` - Domain event desteği için interface
- `src/BlogApp.Domain/Common/DomainEvent.cs` - Domain event'ler için temel sınıf
- `src/BlogApp.Domain/Common/IDomainEvent.cs` - Domain event marker interface
- `src/BlogApp.Domain/Common/Attributes/StoreInOutboxAttribute.cs` - Outbox'a kaydedilecek event'leri işaretler
- `src/BlogApp.Domain/Entities/User.cs` - `BaseEntity`'den türetildi
- `src/BlogApp.Domain/Entities/Role.cs` - `BaseEntity`'den türetildi

### Event Tanımları (17 Domain Event)
**User Events (4):**
- `UserCreatedEvent`, `UserUpdatedEvent`, `UserDeletedEvent`, `UserRolesAssignedEvent`

**Role Events (3):**
- `RoleCreatedEvent`, `RoleUpdatedEvent`, `RoleDeletedEvent`

**Permission Events (1):**
- `PermissionsAssignedToRoleEvent`

**Category Events (3):**
- `CategoryCreatedEvent`, `CategoryUpdatedEvent`, `CategoryDeletedEvent`

**Post Events (3):**
- `PostCreatedEvent`, `PostUpdatedEvent`, `PostDeletedEvent`

**BookshelfItem Events (3):**
- `BookshelfItemCreatedEvent`, `BookshelfItemUpdatedEvent`, `BookshelfItemDeletedEvent`

### Persistence Katmanı
- `src/BlogApp.Persistence/Repositories/UnitOfWork.cs` - Domain event toplama ve outbox entegrasyonu

### Application Katmanı (20+ Handler)
**User Handlers (5):**
- `CreateUserCommandHandler`
- `UpdateUserCommandHandler`
- `DeleteUserCommandHandler`
- `BulkDeleteUsersCommandHandler`
- `AssignRolesToUserCommandHandler`

**Role Handlers (4):**
- `CreateRoleCommandHandler`
- `UpdateRoleCommandHandler`
- `DeleteRoleCommandHandler`
- `BulkDeleteRolesCommandHandler`

**Permission Handlers (1):**
- `AssignPermissionsToRoleCommandHandler`

**Category Handlers (3):**
- `CreateCategoryCommandHandler`
- `UpdateCategoryCommandHandler`
- `DeleteCategoryCommandHandler`

**Post Handlers (3):**
- `CreatePostCommandHandler`
- `UpdatePostCommandHandler`
- `DeletePostCommandHandler`

**BookshelfItem Handlers (3):**
- `CreateBookshelfItemCommandHandler`
- `UpdateBookshelfItemCommandHandler`
- `DeleteBookshelfItemCommandHandler`

### Infrastructure Katmanı
- `src/BlogApp.Infrastructure/Services/BackgroundServices/OutboxProcessorService.cs` - Outbox mesajlarını RabbitMQ'ya yayınlar
- `src/BlogApp.Infrastructure/Services/BackgroundServices/Outbox/Converters/*` - 17 adet event converter

## 🧪 Sonuçlar ve Kazanımlar
- **Mimari Tutarlılık:** Tüm aggregate'ler (User, Role, Category, Post, BookshelfItem) aynı domain event pattern'ini kullanıyor.
- **Daha Az Kod:** Manuel outbox kodu ve helper sınıfı ortadan kalktı, her handler'da ~5-10 satır kod azaldı.
- **Test Edilebilirlik:** Handler testleri bir domain event raise edildiğini kolayca doğrulayabiliyor.
- **Bakım Kolaylığı:** Outbox mesaj üretimi tek noktada (UnitOfWork) yönetiliyor.
- **ACID Garantisi:** Domain event'ler business data ile aynı transaction'da kaydediliyor.
- **Güvenilir İşleme:** OutboxProcessorService, 5 saniyede bir 50'şer mesaj batch'i olarak işliyor.
- **Retry Mekanizması:** Başarısız mesajlar için maksimum 5 deneme yapılıyor.
- **Bellek Koruması:** Finally bloğunda otomatik event temizleme ile bellek sızıntısı önleniyor.

## 📊 Sistem İstatistikleri
| Özellik | Değer |
|---------|-------|
| **Toplam Domain Event** | 17 adet |
| **Aggregate Sayısı** | 6 (User, Role, Permission, Category, Post, BookshelfItem) |
| **Güncellenen Handler** | 20+ handler |
| **Outbox İşleme Sıklığı** | 5 saniye |
| **Batch Size** | 50 mesaj/işlem |
| **Max Retry** | 5 deneme |
| **Message Broker** | RabbitMQ + MassTransit |

## 🚀 Sonraki Adımlar
- Yeni handler eklerken `entity.AddDomainEvent(...)` çağrısını unutmayın.
- Yeni domain event eklerken mutlaka `[StoreInOutbox]` attribute'unu kullanın.
- Her yeni event için karşılık gelen `IIntegrationEventConverterStrategy` implementasyonu ekleyin.
- Outbox pipeline'ı için entegrasyon testlerini artırın (özellikle kullanıcı/rol senaryoları).
- Domain event raise eden komutlar için unit test yazılmamışsa eklenmesi önerilir.

## 🔗 İlgili Dokümanlar
- `DOMAIN_EVENTS_IMPLEMENTATION.md` - Domain events pattern detaylı implementasyon rehberi
- `OUTBOX_PATTERN_IMPLEMENTATION.md` - Outbox pattern implementasyonu
- `CODE_CENTRALIZATION_REPORT.md` - Kod merkezileştirme ve domain events iyileştirmesi
- `ACTIVITY_LOGGING_README.md` - Activity logging sistemi

---
**Tarih:** 28 Ekim 2025 – Güncel mimari ile uyumlu hale getirildi.
**Son Güncelleme:** Tüm aggregate'ler, handler'lar ve event'ler doğrulandı.
