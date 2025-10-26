# Transaction Management Strategy

## 1. Amaç
BlogApp tek bir PostgreSQL veritabanı etrafında dönen, domain-event tabanlı bir mimari kullanır. Transaction yönetimi; Entity Framework Core’un birimsel transaction desteği, UnitOfWork soyutlaması ve Outbox Pattern ile sağlanan eventual consistency üzerinden kurgulanmıştır. Bu doküman güncel stratejiyi ve geliştirici rehberini özetler.

## 2. Birincil Strateji: UnitOfWork

- `IUnitOfWork.SaveChangesAsync` EF Core `DbContext` üzerinde tek bir transaction açar; tüm entity değişiklikleri ve eşzamanlı outbox mesajları aynı ACID transaction içinde persist edilir.
- Domain event’ler `BaseEntity.DomainEvents` koleksiyonunda tutulur. `SaveChangesAsync` çağrısı sırasında `[StoreInOutbox]` ile işaretli event’ler Outbox tablosuna yazılır; commit başarılıysa veriler kalıcı hale gelir.
- Bu yaklaşım, komut başına tek `SaveChangesAsync` çağrısı yapıldığı sürece otomatik transaction yönetimi sağlar.

```csharp
public sealed class CreatePostCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
  : IRequestHandler<CreatePostCommand, Guid>
{
  public async Task<Guid> Handle(CreatePostCommand request, CancellationToken ct)
  {
    var post = new Post(request.Title, request.Content, request.CategoryId, currentUserId: request.ActorId);
    post.AddDomainEvent(new PostCreatedEvent(post.Id, post.Title, post.CategoryId, request.ActorId));

    await postRepository.AddAsync(post, ct);
    await unitOfWork.SaveChangesAsync(ct); // Tek transaction

    return post.Id;
  }
}
```

### 2.1 Ne Sağlar?
- **Basitlik:** Transaction yönetimi uygulama katmanına sızmaz.
- **ACID Garantisi:** Domain verisi ve Outbox mesajları aynı commit altında.
- **Performans:** EF Core’un doğal transaction desteği kullanılır.

## 3. Destekleyici Araçlar

### 3.1 Outbox Pattern
- Cross-resource tutarlılık artık TransactionScope yerine Outbox ile sağlanır.
- `OutboxProcessorService` işlenmemiş mesajları RabbitMQ’ya publish eder; MassTransit tüketicileri (örn. `ActivityLogConsumer`) asenkron olarak yan etkileri gerçekleştirir.
- Böylece veritabanı transaction’ı kısa tutulur, message broker veya e-posta gibi sistemlerle eventual consistency sağlanır.

### 3.2 Manuel Transaction Yönetimi (Gerekirse)
`UnitOfWork` gerektiğinde elle transaction başlatmak için `BeginTransactionAsync`, `CommitTransactionAsync`, `RollbackTransactionAsync` metodlarını sunar. Bu yöntem sadece tek `DbContext` ile çalışır; farklı kaynaklara uzanmaz.

```csharp
await unitOfWork.BeginTransactionAsync(ct);
try
{
  await repositoryOne.AddAsync(entityA, ct);
  await repositoryTwo.UpdateAsync(entityB, ct);

  await unitOfWork.CommitTransactionAsync(ct);
}
catch
{
  await unitOfWork.RollbackTransactionAsync(ct);
  throw;
}
```

## 4. Artık Kullanılmayan Yaklaşımlar

- **TransactionScopeBehavior, ITransactionalRequest:** Kod tabanından kaldırıldı. Ambient `TransactionScope` kullanımı platform bağımlılığı ve performans kaygıları nedeniyle tercih edilmiyor.
- **Distributed Transaction:** DB + RabbitMQ + Redis gibi kaynakları tek transaction altında toplamak yerine Outbox + retry mekanizmaları benimseniyor.

## 5. Geliştirici Rehberi

- Komutlarda **tek `SaveChangesAsync` çağrısı** yapın; ara `SaveChanges` çağrıları transaction bütünlüğünü böler.
- Uzun süren dış servis çağrılarını transaction dışında gerçekleştirin; sonuçlarını kaydetmek gerekiyorsa operasyonu iki aşamaya bölün (ör. önce kaydet → outbox → consumer external call tetikler).
- Aynı handler içinde birden fazla repository işlemi gerekiyorsa, `UnitOfWork` zaten hepsini tek transaction’da toplar; ekstra `BeginTransaction` çağrısı gerekmez.
- `OutboxMessages` tablosundaki RetryCount/Error alanları üzerinden başarısız yan etkileri takip edin; tekrarlanan hatalar için consumer tarafında idempotent davranış sağlayın.

## 6. Senaryo Tabloları

| Senaryo | Önerilen Strateji | Not |
|---------|-------------------|-----|
| CRUD (Post, Category, Role, User) | `SaveChangesAsync` (otomatik transaction) | Varsayılan yaklaşım |
| Domain event + Activity log | `SaveChangesAsync` + Outbox | Activity log asenkron oluşur |
| External API çağrısı (ödeme, e-posta) | Önce local DB commit → outbox → consumer | Dış sistem hataları retry edilir |
| Aynı request içinde iki ayrı `DbContext` | Desteklenmez | Gerekirse orchestration veya saga düşünün |

## 7. İzleme ve Bakım

- `OutboxMessages` tablosunu düzenli izleyin; yüksek `RetryCount` değerleri transaction sonrası yan etkilerin başarısız olduğuna işaret eder.
- `ActivityLogs` tablosu ana transaction tamamlandıktan sonra doldurulur; kullanıcı geri bildirimi (response) gönderildikten sonra bile yeni kayıtlar görünebilir.
- Gerektiğinde `unitOfWork.BeginTransactionAsync` ile manual transaction açıldığında, `CommitTransactionAsync` çağrısının gerçekten sonunda yapıldığından emin olun; aksi halde EF Core pending değişiklikleri commit etmez.

## 8. İlgili Dosyalar
- `src/BlogApp.Domain/Common/BaseEntity.cs`
- `src/BlogApp.Domain/Common/IUnitOfWork.cs`
- `src/BlogApp.Persistence/Repositories/UnitOfWork.cs`
- `src/BlogApp.Infrastructure/Services/BackgroundServices/OutboxProcessorService.cs`
- `src/BlogApp.Infrastructure/Services/BackgroundServices/Outbox/Converters/ActivityLogIntegrationEventConverters.cs`
- `docs/OUTBOX_PATTERN_IMPLEMENTATION.md`
- `docs/ACTIVITY_LOGGING_README.md`

## 9. Özet
BlogApp’te transaction yönetimi EF Core’un yerleşik kabiliyetleri üzerine kurulu olup Outbox Pattern ile desteklenir. TransactionScope tabanlı dağıtık transaction yaklaşımı kaldırıldı. Geliştiricilerin odak noktası, tek `DbContext` değişikliklerini `SaveChangesAsync` ile commit etmek ve cross-resource tutarlılığı Outbox üzerinden sağlamaktır.
