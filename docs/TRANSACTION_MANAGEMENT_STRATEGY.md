# Transaction Management Strategy

## ?? BlogApp Transaction Yönetimi

BlogApp'te **iki farklý transaction yönetim stratejisi** mevcuttur:

### 1?? Unit of Work (Primary Strategy)

**Kullaným Alaný:** Standart CRUD iþlemleri (tek DbContext)

**Nasýl Kullanýlýr:**
```csharp
public sealed class CreatePostCommandHandler(
    IPostRepository postRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreatePostCommand, IResult>
{
    public async Task<IResult> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
  var post = new Post { ... };
  await postRepository.AddAsync(post);
        await unitOfWork.SaveChangesAsync(cancellationToken); // ? DB transaction
        
    return new SuccessResult("...");
    }
}
```

**Avantajlarý:**
- ? Lightweight
- ? Performanslý
- ? Platform agnostic
- ? EF Core native transaction

---

### 2?? TransactionScope Behavior (Advanced Strategy)

**Kullaným Alaný:** Distributed transactions (DB + Message Queue + Cache vs.)

**Nasýl Kullanýlýr:**

**Adým 1:** Command'a `ITransactionalRequest` marker interface'ini ekle:
```csharp
using BlogApp.Application.Behaviors.Transaction;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Orders.Commands.Process;

public sealed record ProcessOrderCommand(
    int OrderId,
  decimal Amount,
    string PaymentMethod
) : IRequest<IResult>, ITransactionalRequest; // ? Marker interface
```

**Adým 2:** Handler'da birden fazla kaynak kullan:
```csharp
public sealed class ProcessOrderCommandHandler(
    IOrderRepository orderRepository,
    IPaymentService paymentService,
    IPublishEndpoint publishEndpoint, // RabbitMQ
  ICacheService cacheService,       // Redis
    IUnitOfWork unitOfWork
) : IRequestHandler<ProcessOrderCommand, IResult>
{
    public async Task<IResult> Handle(ProcessOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. DB'ye sipariþ kaydet
    var order = new Order { ... };
   await orderRepository.AddAsync(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 2. Ödeme iþlemini yap (External API)
  await paymentService.ProcessPayment(request.Amount, request.PaymentMethod);

        // 3. RabbitMQ'ya mesaj gönder
        await publishEndpoint.Publish(new OrderProcessedEvent(order.Id), cancellationToken);

   // 4. Redis cache'i güncelle
   await cacheService.Remove($"order:{order.Id}");

        return new SuccessResult("Sipariþ baþarýyla iþlendi.");
        
        // TransactionScopeBehavior sayesinde yukarýdaki TÜMÜ ayný transaction içinde!
        // Herhangi biri baþarýsýz olursa hepsi rollback olur!
    }
}
```

**TransactionScopeBehavior Pipeline:**
```csharp
// ApplicationServicesRegistration.cs
configuration.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));

// Sadece ITransactionalRequest implement eden Command'lar için çalýþýr!
```

**Avantajlarý:**
- ? Distributed transaction desteði
- ? Otomatik rollback (tüm kaynaklar)
- ? Merkezi transaction yönetimi (Pipeline Behavior)
- ? Cross-resource atomicity garantisi

**Dezavantajlarý:**
- ?? MSDTC gerektirebilir (Windows)
- ?? Performance overhead
- ?? Complexity

---

## ?? Karar Aðacý: Hangi Stratejiyi Kullanmalýyým?

```
        Transaction gerekiyor mu?
         ?
      ?????????????????????
       Evet      Hayýr
     ?       ?
       ?   ?
       Birden fazla kaynak var mý?    Transaction'a gerek yok
       (DB + RabbitMQ + Redis vs.)
   ?
      ?????????????????????
         Evet          Hayýr
   ?             ?
  ?        ?
   TransactionScope     UnitOfWork
   (ITransactionalRequest)   (Standart)
```

### Örnekler:

| Senaryo | Strateji | Açýklama |
|---------|----------|----------|
| Post oluþturma | UnitOfWork | Sadece DB iþlemi |
| Kategori güncelleme | UnitOfWork | Sadece DB iþlemi |
| Sipariþ iþleme (DB + Payment + RabbitMQ) | TransactionScope | Distributed |
| Kullanýcý kaydý (DB + Email + RabbitMQ) | TransactionScope | Distributed |
| Post listeleme (Query) | Yok | Read-only operation |

---

## ?? Best Practices

### ? DO (Yapýlmasý Gerekenler)

1. **Basit CRUD iþlemleri için UnitOfWork kullan:**
   ```csharp
 await repository.AddAsync(entity);
   await unitOfWork.SaveChangesAsync(cancellationToken);
   ```

2. **Distributed iþlemler için ITransactionalRequest kullan:**
   ```csharp
   public record ComplexCommand(...) : IRequest<IResult>, ITransactionalRequest;
   ```

3. **Transaction scope'larý minimize et:**
   ```csharp
   // ? Kötü
   using var transaction = new TransactionScope(...);
   await Task.Delay(5000); // Long-running operation
   transaction.Complete();
   
   // ? Ýyi
   var data = await PrepareData(); // Transaction dýþýnda
   using var transaction = new TransactionScope(...);
   await SaveData(data); // Hýzlý DB iþlemi
   transaction.Complete();
   ```

4. **Timeout ayarlarýný yapýlandýr:**
   ```csharp
   var options = new TransactionOptions
   {
       IsolationLevel = IsolationLevel.ReadCommitted,
   Timeout = TimeSpan.FromSeconds(30)
   };
   using var scope = new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled);
   ```

### ? DON'T (Yapýlmamasý Gerekenler)

1. **Her Command'a ITransactionalRequest ekleme:**
   ```csharp
   // ? Gereksiz overhead
   public record GetPostByIdQuery(...) : IRequest<...>, ITransactionalRequest;
 ```

2. **TransactionScope içinde uzun iþlemler:**
   ```csharp
   // ? Deadlock riski
   using var scope = new TransactionScope(...);
   await SendEmailAsync(); // External API call
   await Task.Delay(10000);
   scope.Complete();
   ```

3. **Nested TransactionScope'lar (dikkatli kullanýlmalý):**
   ```csharp
   // ?? Dikkat
   using var outer = new TransactionScope(...);
   using var inner = new TransactionScope(...); // Nested
   ```

---

## ?? Yapýlandýrma

### TransactionScope için Windows MSDTC

Eðer distributed transaction kullanacaksanýz (production ortamýnda), MSDTC'yi etkinleþtirin:

```powershell
# Windows'ta MSDTC'yi baþlat
net start msdtc

# Güvenlik ayarlarý
# Component Services ? Computers ? My Computer ? Distributed Transaction Coordinator ? Local DTC
# ? Properties ? Security
# ? Network DTC Access
# ? Allow Inbound / Allow Outbound
# ? No Authentication Required (Development)
```

### Linux/Docker için Alternatif

TransactionScope Linux'ta bazý sorunlara yol açabilir. Bu durumda:
- Sadece UnitOfWork kullanýn
- Saga pattern implementasyonu düþünün
- Outbox pattern kullanýn (eventual consistency)

---

## ?? Performance Karþýlaþtýrmasý

| Metrik | UnitOfWork | TransactionScope |
|--------|------------|------------------|
| Latency | ~5-10ms | ~20-50ms |
| Resource Usage | Düþük | Orta |
| Scalability | Yüksek | Orta |
| Complexity | Düþük | Orta |
| Cross-DB Support | Hayýr | Evet |

---

## ?? Ýlgili Dosyalar

- `src/BlogApp.Domain/Common/IUnitOfWork.cs` - UnitOfWork interface
- `src/BlogApp.Persistence/Repositories/UnitOfWork.cs` - UnitOfWork implementation
- `src/BlogApp.Application/Behaviors/Transaction/TransactionScopeBehavior.cs` - TransactionScope behavior
- `src/BlogApp.Application/Behaviors/Transaction/ITransactionalRequest.cs` - Marker interface

---

## ?? Sonuç

**BlogApp'te iki strateji de mevcut ve ikisi de kullanýlmalýdýr:**

1. **%95 durumlarda UnitOfWork kullanýn** (basit CRUD)
2. **Complex senaryolarda TransactionScope kullanýn** (distributed transactions)

Bu hybrid yaklaþým size hem performans hem de esneklik saðlar! ??
