# Domain Events İyileştirme Raporu

## 🎯 Sorunun Kökeni
Önceki sürümlerde kimlik yönetimi ASP.NET Identity tabanlı `AppUser`/`AppRole` modellerine dayanıyordu. Bu sınıflar `IdentityUser<int>` ve `IdentityRole<int>`'ten türediği için `BaseEntity` altyapımızdaki domain event listesinden yararlanamıyordu. Sonuç olarak kullanıcı/rol komutlarında domain event raise edilemedi ve handler'lar `OutboxMessageHelper` gibi yardımcı sınıflarla manuel outbox mesajı üretmek zorunda kaldı.

**Yan etkiler:**
- Domain event pattern'i aggregate'ler arasında tutarsız hale geldi.
- Her handler'da tekrar eden outbox kodu oluştu.
- `IOutboxMessageRepository` gereksiz bağımlılıklar ekledi.

## ✅ Çözümün Özeti
1. **Custom Kimlik Modelleri:** `User` ve `Role` entity'leri yeniden yazılarak `BaseEntity`'den türetildi (`src/BlogApp.Domain/Entities/User.cs`, `Role.cs`). Bu sayede domain event koleksiyonu tüm aggregate'lerde ortak hale geldi.
2. **IHasDomainEvents Interface'i:** `BaseEntity` halihazırda domain event listesi tutuyor ancak Identity dışındaki senaryolar için `IHasDomainEvents` (`Domain/Common/IHasDomainEvents.cs`) tanımlanarak UnitOfWork tarafında tip güvenliği güçlendirildi.
3. **UnitOfWork Güncellemesi:** `Persistence/Repositories/UnitOfWork.cs` domain event toplayıcısı hem `BaseEntity` hem de `IHasDomainEvents` implementasyonlarını tarayacak şekilde genişletildi; outbox mesajı üretimi merkezi hale geldi.
4. **Handler Temizliği:** Kullanıcı ve rol komutları dahil olmak üzere 10'dan fazla handler domain event raise edecek şekilde güncellendi (`Features/Users/...`, `Features/Roles/...`). Manuel outbox üretimi ve `IOutboxMessageRepository` bağımlılıkları kaldırıldı.
5. **OutboxMessageHelper Kaldırıldı:** Artık hiçbir senaryoda kullanılmadığı için helper sınıfı tamamen silindi.

## 🔁 Eski vs Yeni Akış
### ❌ Eski (manuel outbox)
```csharp
var domainEvent = new RoleCreatedEvent(role.Id, role.Name!, currentUserId);
var outboxMessage = OutboxMessageHelper.CreateFromDomainEvent(domainEvent);
await _outboxRepository.AddAsync(outboxMessage);
await _unitOfWork.SaveChangesAsync(ct);
```

### ✅ Yeni (standart domain event)
```csharp
role.AddDomainEvent(new RoleCreatedEvent(role.Id, role.Name!, currentUserId));
await _unitOfWork.SaveChangesAsync(ct);
// UnitOfWork event'i otomatik olarak OutboxMessages tablosuna yazar
```

## 📦 Güncellenen Başlıca Dosyalar
- `src/BlogApp.Domain/Common/BaseEntity.cs`
- `src/BlogApp.Domain/Common/IHasDomainEvents.cs`
- `src/BlogApp.Domain/Entities/User.cs`
- `src/BlogApp.Domain/Entities/Role.cs`
- `src/BlogApp.Persistence/Repositories/UnitOfWork.cs`
- `src/BlogApp.Application/Features/Users/*CommandHandler.cs`
- `src/BlogApp.Application/Features/Roles/*CommandHandler.cs`

## 🧪 Sonuçlar ve Kazanımlar
- **Mimari Tutarlılık:** Tüm aggregate'ler aynı domain event pattern'ini kullanıyor.
- **Daha Az Kod:** Manuel outbox kodu ve helper sınıfı ortadan kalktı.
- **Test Edilebilirlik:** Handler testleri bir domain event raise edildiğini kolayca doğrulayabiliyor.
- **Bakım Kolaylığı:** Outbox mesaj üretimi tek noktada (UnitOfWork) yönetiliyor.

## 🚀 Sonraki Adımlar
- Yeni handler eklerken `entity.AddDomainEvent(...)` çağrısını unutmayın.
- Outbox pipeline'ı için entegrasyon testlerini artırın (özellikle kullanıcı/rol senaryoları).
- Domain event raise eden komutlar için unit test yazılmamışsa eklenmesi önerilir.

---
**Tarih:** 26 Ekim 2025 – Güncel mimari ile uyumlu hale getirildi.
