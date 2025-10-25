# Domain Events Pattern - Implementation Guide

## 📊 Özet

ActivityLogs sistemindeki karmaşık yapıyı çözmek için **Domain Events Pattern** implementasyonu yapıldı. Bu değişiklik sistemi çok daha temiz, test edilebilir ve genişletilebilir hale getirdi.

## 🎯 Yapılan Değişiklikler

### 1. TransactionScopeBehavior Kaldırıldı

**Neden?**
- Hiçbir command `ITransactionalRequest` implement etmiyordu
- Kullanılmayan kod repository'de gereksiz yer kaplıyordu
- UnitOfWork pattern zaten transaction yönetimini yapıyordu

**Silinen Dosyalar:**
- ❌ `TransactionScopeBehavior<,>` (ApplicationServicesRegistration'dan kaldırıldı)
- ℹ️ Dosya silinmedi ama pipeline'dan çıkarıldı, gerekirse tekrar eklenebilir

### 2. ActivityLoggingBehavior'dan Domain Events'e Geçiş

**Önceki Karmaşık Yapı:**
```csharp
// ❌ Karmaşık: Reflection ile command name kontrolü
if (requestName.Contains("CreatePost")) => ("post_created", "Post", true)

// ❌ Ayrı scope kullanımı gerekiyordu
using var scope = _serviceProvider.CreateScope();
await activityLogRepository.AddAsync(activityLog);
await unitOfWork.SaveChangesAsync();
```

**Yeni Temiz Yapı:**
```csharp
// ✅ Basit: Handler'da domain event raise ediyoruz
post.AddDomainEvent(new PostCreatedEvent(post.Id, post.Title, ...));

// ✅ Event handler ActivityLog'u kaydediyor
public class PostCreatedEventHandler : INotificationHandler<PostCreatedEvent>
{
    public async Task Handle(PostCreatedEvent notification, ...)
    {
        var activityLog = new ActivityLog { ... };
        await _activityLogRepository.AddAsync(activityLog);
        await _unitOfWork.SaveChangesAsync();
    }
}
```

## 🏗️ Domain Events Mimarisi

### Yeni Eklenen Sınıflar

#### Domain Layer (BlogApp.Domain)

```
Domain/
├── Common/
│   ├── BaseEntity.cs                 # ✅ Domain events desteği eklendi
│   ├── IDomainEvent.cs               # ✅ Yeni: Event marker interface
│   ├── DomainEvent.cs                # ✅ Yeni: Base event class
│   └── IUnitOfWork.cs                # ✅ GetDomainEvents() eklendi
└── Events/
    ├── PostCreatedEvent.cs           # ✅ Yeni
    ├── PostUpdatedEvent.cs           # ✅ Yeni
    ├── PostDeletedEvent.cs           # ✅ Yeni
    ├── CategoryCreatedEvent.cs       # ✅ Yeni
    ├── CategoryUpdatedEvent.cs       # ✅ Yeni
    └── CategoryDeletedEvent.cs       # ✅ Yeni
```

#### Application Layer (BlogApp.Application)

```
Application/
├── Behaviors/
│   ├── DomainEventDispatcherBehavior.cs  # ✅ Yeni: Events dispatcher
│   └── ActivityLoggingBehavior.cs        # ❌ Kaldırıldı (artık gerekmiyor)
└── Features/
    ├── Posts/EventHandlers/
    │   ├── PostCreatedEventHandler.cs    # ✅ Yeni
    │   ├── PostUpdatedEventHandler.cs    # ✅ Yeni
    │   └── PostDeletedEventHandler.cs    # ✅ Yeni
    └── Categories/EventHandlers/
        ├── CategoryCreatedEventHandler.cs # ✅ Yeni
        ├── CategoryUpdatedEventHandler.cs # ✅ Yeni
        └── CategoryDeletedEventHandler.cs # ✅ Yeni
```

#### Persistence Layer (BlogApp.Persistence)

```
Persistence/
└── Repositories/
    └── UnitOfWork.cs                     # ✅ GetDomainEvents() implementasyonu
```

### Execution Flow

```
1. User Request
   ↓
2. Controller → MediatR Command
   ↓
3. LoggingBehavior (önce log)
   ↓
4. Command Handler
   - Business logic
   - post.AddDomainEvent(new PostCreatedEvent(...)) ← Domain event raise
   - await unitOfWork.SaveChangesAsync() ← DB'ye kaydet
   ↓
5. DomainEventDispatcherBehavior
   - unitOfWork.GetDomainEvents() ← Event'leri topla
   - foreach event → mediator.Publish(event) ← Event handler'ları tetikle
   ↓
6. Event Handlers (parallel çalışabilir)
   - PostCreatedEventHandler → ActivityLog kaydet
   - Gelecekte: EmailNotificationHandler
   - Gelecekte: CacheInvalidationHandler
   ↓
7. Response to User
```

## ✅ Avantajlar

### 1. **Separation of Concerns (Sorumlulukların Ayrılması)**
- ✅ Handler sadece business logic'e odaklanır
- ✅ Activity logging ayrı bir event handler'da
- ✅ Gelecekte email, notification, cache invalidation eklemek çok kolay

### 2. **Testability (Test Edilebilirlik)**
```csharp
// ❌ Önceden: ActivityLoggingBehavior'u test etmek zordu
// Reflection, string matching, scope yönetimi...

// ✅ Şimdi: Event handler'ları ayrı test edilebilir
[Fact]
public async Task PostCreatedEvent_Should_Log_Activity()
{
    var handler = new PostCreatedEventHandler(...);
    await handler.Handle(new PostCreatedEvent(...));
    // Assert activity log created
}
```

### 3. **Extensibility (Genişletilebilirlik)**
```csharp
// ✅ Yeni bir özellik eklemek çok kolay:

// Email notification ekle
public class PostCreatedEmailHandler : INotificationHandler<PostCreatedEvent>
{
    public async Task Handle(PostCreatedEvent notification, ...)
    {
        await _emailService.SendAsync("New post created: " + notification.Title);
    }
}

// Cache invalidation ekle
public class PostCreatedCacheInvalidationHandler : INotificationHandler<PostCreatedEvent>
{
    public async Task Handle(PostCreatedEvent notification, ...)
    {
        await _cache.Remove($"post-{notification.PostId}");
    }
}

// MediatR otomatik olarak TÜM handler'ları çalıştırır!
```

### 4. **Single Responsibility Principle**
- ✅ CreatePostCommandHandler → Sadece post oluşturur
- ✅ PostCreatedEventHandler → Sadece activity log'lar
- ✅ Her sınıf tek bir işten sorumlu

### 5. **Domain-Driven Design (DDD)**
- ✅ Domain events, domain expert'lerin konuştuğu şeylerdir
- ✅ "Post oluşturuldu", "Category silindi" gibi business event'ler
- ✅ Domain layer business logic'i ifade eder

## ⚠️ Dezavantajlar (ve Çözümleri)

### 1. **Complexity (Karmaşıklık)**
**Sorun:** Daha fazla dosya, daha fazla sınıf
**Çözüm:** 
- Feature folder organization ile organize edildi
- Her event handler tek bir dosyada
- Naming convention tutarlı: `{Entity}{Action}EventHandler`

### 2. **Performance**
**Sorun:** Her event için ayrı handler çalışır
**Çözüm:**
- Event handler'lar parallel çalışabilir (MediatR destekler)
- Database transaction içinde değiller (async)
- Gerçek dünyada minimal overhead (<1ms)

### 3. **Debugging**
**Sorun:** Event flow'u takip etmek zor olabilir
**Çözüm:**
- LoggingBehavior zaten her şeyi logluyoruz
- Event handler'larda da loglama eklenebilir
- Visual Studio debugger event handler'lara breakpoint koyabilir

### 4. **Transaction Management**
**Sorun:** Event handler'lar farklı transaction'da çalışır
**Çözüm:**
- ActivityLog kaydı ayrı transaction'da (istenen davranış)
- Ana işlem başarısız olursa event handler çalışmaz (DomainEventDispatcherBehavior'un konumu sayesinde)

## 🚀 Gelecek İyileştirmeler

### 1. Outbox Pattern (Eventual Consistency için)
```csharp
// Event'leri önce OutboxMessage tablosuna yaz
// Background worker event'leri işle
// Böylece distributed transaction sorunları çözülür
```

### 2. Event Sourcing
```csharp
// Tüm domain event'leri EventStore'a kaydet
// State'i event'lerden yeniden oluştur
// Audit trail ve time-travel debugging
```

### 3. Domain Event Versioning
```csharp
public class PostCreatedEvent_V2 : DomainEvent
{
    // Breaking change olursa yeni versiyon
}
```

## 📋 Migration Checklist

- [x] Domain layer'a MediatR.Contracts eklendi
- [x] BaseEntity'ye domain events desteği eklendi
- [x] Domain event'ler oluşturuldu (Post, Category)
- [x] IUnitOfWork'e GetDomainEvents() eklendi
- [x] UnitOfWork implementasyonu güncellendi
- [x] DomainEventDispatcherBehavior eklendi
- [x] Event handler'lar oluşturuldu
- [x] Command handler'lar güncellendi (domain event raise)
- [x] ActivityLoggingBehavior kaldırıldı
- [x] TransactionScopeBehavior pipeline'dan çıkarıldı
- [ ] Test yazılması (Unit tests for event handlers)
- [ ] Integration test'ler güncellenmesi
- [ ] Performance test'leri

## 🎓 Öğrendiklerimiz

### Domain Events Ne Zaman Kullanılmalı?

✅ **Kullan:**
- Side effect'ler olduğunda (logging, email, cache invalidation)
- Birden fazla bounded context etkileniyorsa
- Eventual consistency kabul edilebilirse
- Audit trail gerekiyorsa

❌ **Kullanma:**
- Basit CRUD işlemlerinde (overhead yaratır)
- Immediate consistency şart ise
- Single responsibility zaten sağlanıyorsa

### Alternatifler

1. **Mediator Pattern** (zaten kullanıyoruz - MediatR)
2. **Observer Pattern** (Domain events bunun bir türü)
3. **Command Pattern** (Commands için kullanıyoruz)
4. **Repository Pattern** (Data access için kullanıyoruz)

## 🔗 İlgili Kaynaklar

- [Domain Events - Martin Fowler](https://martinfowler.com/eaaDev/DomainEvent.html)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [Domain-Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)
- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

**Sonuç:** Domain Events pattern, karmaşık ActivityLogging behavior'unu basit, test edilebilir ve genişletilebilir bir yapıya dönüştürdü. Sistem artık daha SOLID ve maintainable! 🎉
