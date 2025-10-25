# Transaction Management Strategy

## ?? BlogApp Transaction Y�netimi

BlogApp'te **iki farkl� transaction y�netim stratejisi** mevcuttur:

### 1?? Unit of Work (Primary Strategy)

**Kullan�m Alan�:** Standart CRUD i�lemleri (tek DbContext)

**Nas�l Kullan�l�r:**
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

**Avantajlar�:**
- ? Lightweight
- ? Performansl�
- ? Platform agnostic
- ? EF Core native transaction

---

### 2?? TransactionScope Behavior (Advanced Strategy)

**Kullan�m Alan�:** Distributed transactions (DB + Message Queue + Cache vs.)

**Nas�l Kullan�l�r:**

**Ad�m 1:** Command'a `ITransactionalRequest` marker interface'ini ekle:
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

**Ad�m 2:** Handler'da birden fazla kaynak kullan:
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
        // 1. DB'ye sipari� kaydet
    var order = new Order { ... };
   await orderRepository.AddAsync(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 2. �deme i�lemini yap (External API)
  await paymentService.ProcessPayment(request.Amount, request.PaymentMethod);

        // 3. RabbitMQ'ya mesaj g�nder
        await publishEndpoint.Publish(new OrderProcessedEvent(order.Id), cancellationToken);

   // 4. Redis cache'i g�ncelle
   await cacheService.Remove($"order:{order.Id}");

        return new SuccessResult("Sipari� ba�ar�yla i�lendi.");
        
        // TransactionScopeBehavior sayesinde yukar�daki T�M� ayn� transaction i�inde!
        // Herhangi biri ba�ar�s�z olursa hepsi rollback olur!
    }
}
```

**TransactionScopeBehavior Pipeline:**
```csharp
// ApplicationServicesRegistration.cs
configuration.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));

// Sadece ITransactionalRequest implement eden Command'lar i�in �al���r!
```

**Avantajlar�:**
- ? Distributed transaction deste�i
- ? Otomatik rollback (t�m kaynaklar)
- ? Merkezi transaction y�netimi (Pipeline Behavior)
- ? Cross-resource atomicity garantisi

**Dezavantajlar�:**
- ?? MSDTC gerektirebilir (Windows)
- ?? Performance overhead
- ?? Complexity

---

## ?? Karar A�ac�: Hangi Stratejiyi Kullanmal�y�m?

```
        Transaction gerekiyor mu?
         ?
      ?????????????????????
       Evet      Hay�r
     ?       ?
       ?   ?
       Birden fazla kaynak var m�?    Transaction'a gerek yok
       (DB + RabbitMQ + Redis vs.)
   ?
      ?????????????????????
         Evet          Hay�r
   ?             ?
  ?        ?
   TransactionScope     UnitOfWork
   (ITransactionalRequest)   (Standart)
```

### �rnekler:

| Senaryo | Strateji | A��klama |
|---------|----------|----------|
| Post olu�turma | UnitOfWork | Sadece DB i�lemi |
| Kategori g�ncelleme | UnitOfWork | Sadece DB i�lemi |
| Sipari� i�leme (DB + Payment + RabbitMQ) | TransactionScope | Distributed |
| Kullan�c� kayd� (DB + Email + RabbitMQ) | TransactionScope | Distributed |
| Post listeleme (Query) | Yok | Read-only operation |

---

## ?? Best Practices

### ? DO (Yap�lmas� Gerekenler)

1. **Basit CRUD i�lemleri i�in UnitOfWork kullan:**
   ```csharp
 await repository.AddAsync(entity);
   await unitOfWork.SaveChangesAsync(cancellationToken);
   ```

2. **Distributed i�lemler i�in ITransactionalRequest kullan:**
   ```csharp
   public record ComplexCommand(...) : IRequest<IResult>, ITransactionalRequest;
   ```

3. **Transaction scope'lar� minimize et:**
   ```csharp
   // ? K�t�
   using var transaction = new TransactionScope(...);
   await Task.Delay(5000); // Long-running operation
   transaction.Complete();
   
   // ? �yi
   var data = await PrepareData(); // Transaction d���nda
   using var transaction = new TransactionScope(...);
   await SaveData(data); // H�zl� DB i�lemi
   transaction.Complete();
   ```

4. **Timeout ayarlar�n� yap�land�r:**
   ```csharp
   var options = new TransactionOptions
   {
       IsolationLevel = IsolationLevel.ReadCommitted,
   Timeout = TimeSpan.FromSeconds(30)
   };
   using var scope = new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled);
   ```

### ? DON'T (Yap�lmamas� Gerekenler)

1. **Her Command'a ITransactionalRequest ekleme:**
   ```csharp
   // ? Gereksiz overhead
   public record GetPostByIdQuery(...) : IRequest<...>, ITransactionalRequest;
 ```

2. **TransactionScope i�inde uzun i�lemler:**
   ```csharp
   // ? Deadlock riski
   using var scope = new TransactionScope(...);
   await SendEmailAsync(); // External API call
   await Task.Delay(10000);
   scope.Complete();
   ```

3. **Nested TransactionScope'lar (dikkatli kullan�lmal�):**
   ```csharp
   // ?? Dikkat
   using var outer = new TransactionScope(...);
   using var inner = new TransactionScope(...); // Nested
   ```

---

## ?? Yap�land�rma

### TransactionScope i�in Windows MSDTC

E�er distributed transaction kullanacaksan�z (production ortam�nda), MSDTC'yi etkinle�tirin:

```powershell
# Windows'ta MSDTC'yi ba�lat
net start msdtc

# G�venlik ayarlar�
# Component Services ? Computers ? My Computer ? Distributed Transaction Coordinator ? Local DTC
# ? Properties ? Security
# ? Network DTC Access
# ? Allow Inbound / Allow Outbound
# ? No Authentication Required (Development)
```

### Linux/Docker i�in Alternatif

TransactionScope Linux'ta baz� sorunlara yol a�abilir. Bu durumda:
- Sadece UnitOfWork kullan�n
- Saga pattern implementasyonu d���n�n
- Outbox pattern kullan�n (eventual consistency)

---

## ?? Performance Kar��la�t�rmas�

| Metrik | UnitOfWork | TransactionScope |
|--------|------------|------------------|
| Latency | ~5-10ms | ~20-50ms |
| Resource Usage | D���k | Orta |
| Scalability | Y�ksek | Orta |
| Complexity | D���k | Orta |
| Cross-DB Support | Hay�r | Evet |

---

## ?? �lgili Dosyalar

- `src/BlogApp.Domain/Common/IUnitOfWork.cs` - UnitOfWork interface
- `src/BlogApp.Persistence/Repositories/UnitOfWork.cs` - UnitOfWork implementation
- `src/BlogApp.Application/Behaviors/Transaction/TransactionScopeBehavior.cs` - TransactionScope behavior
- `src/BlogApp.Application/Behaviors/Transaction/ITransactionalRequest.cs` - Marker interface

---

## ?? Sonu�

**BlogApp'te iki strateji de mevcut ve ikisi de kullan�lmal�d�r:**

1. **%95 durumlarda UnitOfWork kullan�n** (basit CRUD)
2. **Complex senaryolarda TransactionScope kullan�n** (distributed transactions)

Bu hybrid yakla��m size hem performans hem de esneklik sa�lar! ??
