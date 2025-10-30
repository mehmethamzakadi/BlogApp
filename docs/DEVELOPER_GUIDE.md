# BlogApp Developer Guide
**Kapsamlı Geliştirici Kılavuzu**  
**Son Güncelleme:** 2025-01-25

---

## 📚 İçindekiler

1. [Proje Genel Bakış](#1-proje-genel-bakış)
2. [Mimari ve Tasarım Desenleri](#2-mimari-ve-tasarım-desenleri)
3. [Logging Sistemi](#3-logging-sistemi)
4. [Permission Sistemi](#4-permission-sistemi)
5. [Authentication & Authorization](#5-authentication--authorization)
6. [Outbox Pattern](#6-outbox-pattern)
7. [Activity Logging](#7-activity-logging)
8. [Test Yazma](#8-test-yazma)
9. [Best Practices](#9-best-practices)
10. [Troubleshooting](#10-troubleshooting)

---

## 1. Proje Genel Bakış

### Teknoloji Stack

**Backend:**
- ASP.NET Core 9.0
- Entity Framework Core 9.0
- PostgreSQL 16
- Redis (Cache)
- RabbitMQ (Message Queue)
- MediatR (CQRS)
- FluentValidation
- Serilog (Logging)

**Frontend:**
- React 18 + TypeScript
- Vite
- TailwindCSS + shadcn/ui
- TanStack Query
- Zustand
- React Hook Form + Zod

### Proje Yapısı

```
BlogApp/
├── src/
│   ├── BlogApp.API/              # REST API
│   ├── BlogApp.Application/      # CQRS Handlers
│   ├── BlogApp.Domain/           # Entities, Events
│   ├── BlogApp.Infrastructure/   # External Services
│   └── BlogApp.Persistence/      # EF Core, Repositories
├── clients/
│   └── blogapp-client/           # React SPA
├── tests/
│   ├── Domain.UnitTests/         # Domain tests
│   └── Application.UnitTests/    # Application tests
└── docs/                         # Documentation
```

---

## 2. Mimari ve Tasarım Desenleri

### Clean Architecture

**Bağımlılık Yönü:**
```
API → Application → Domain
         ↓
Infrastructure ← Persistence
```

**Katman Sorumlulukları:**

| Katman | Sorumluluk | Bağımlılık |
|--------|------------|------------|
| **Domain** | Entities, Value Objects, Domain Events | Hiçbiri |
| **Application** | CQRS Handlers, Validators, DTOs | Domain |
| **Infrastructure** | JWT, Email, RabbitMQ, Redis | Application, Domain |
| **Persistence** | EF Core, Repositories, Migrations | Application, Domain |
| **API** | Controllers, Middlewares, Filters | Tüm katmanlar |

### CQRS Pattern

**Command (Yazma İşlemleri):**
```csharp
// Command
public sealed record CreateUserCommand(string UserName, string Email, string Password) 
    : IRequest<IResult>;

// Handler
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IResult>
{
    public async Task<IResult> Handle(CreateUserCommand request, CancellationToken ct)
    {
        // Business logic
        return new SuccessResult();
    }
}
```

**Query (Okuma İşlemleri):**
```csharp
// Query
public sealed record GetUserByIdQuery(Guid Id) : IRequest<IDataResult<UserDto>>;

// Handler
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, IDataResult<UserDto>>
{
    public async Task<IDataResult<UserDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        // Data retrieval
        return new SuccessDataResult<UserDto>(userDto);
    }
}
```

### Repository Pattern

```csharp
// Generic Repository
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<List<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

// Specific Repository
public interface IUserRepository : IRepository<User>
{
    Task<User?> FindByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
}
```

### Unit of Work Pattern

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

// Kullanım
await _userRepository.AddAsync(user);
await _unitOfWork.SaveChangesAsync(); // Tek transaction
```

---

## 3. Logging Sistemi

### 3 Katmanlı Loglama

**1. File Logs** (`logs/blogapp-*.txt`)
- **Amaç:** Development & debugging
- **Retention:** 31 gün
- **Level:** Debug+

**2. PostgreSQL Logs** (`Logs` tablosu)
- **Amaç:** Production monitoring
- **Retention:** 90 gün (otomatik temizlik)
- **Level:** Information+

**3. Activity Logs** (`ActivityLogs` tablosu)
- **Amaç:** Audit trail & compliance
- **Retention:** Süresiz
- **Kayıt:** Domain Events → Outbox → RabbitMQ

### Structured Logging

**✅ Doğru Kullanım:**
```csharp
_logger.LogInformation("User {UserId} created post {PostId}", userId, postId);
_logger.LogError(exception, "Failed to send email to {Email}", email);
```

**❌ Yanlış Kullanım:**
```csharp
_logger.LogInformation("User " + userId + " created post"); // String concatenation
_logger.LogInformation($"User {userId} created post");      // String interpolation
```

### Log Levels

| Level | Kullanım | Örnek |
|-------|----------|-------|
| **Debug** | Geliştirme | Variable değerleri |
| **Information** | Normal akış | User logged in |
| **Warning** | Beklenmeyen durum | Cache miss |
| **Error** | Hata | Validation failed |
| **Fatal** | Kritik hata | Database down |

### MediatR Logging

`LoggingBehavior` tüm command/query'leri otomatik loglar:

```csharp
[12:34:56 INF] CreatePostCommand isteği başlatılıyor
[12:34:57 INF] CreatePostCommand isteği tamamlandı
```

### Seq Monitoring

**Development:** `http://localhost:5341`  
**Production:** `http://seq:80`

**Örnek Query:**
```
@Level = 'Error' and @Timestamp > Now() - 1h
```

---

## 4. Permission Sistemi

### Permission Tanımlama

**Backend:**
```csharp
// src/BlogApp.Domain/Constants/Permissions.cs
public static class Permissions
{
    public const string UsersCreate = "Users.Create";
    public const string UsersUpdate = "Users.Update";
    public const string UsersDelete = "Users.Delete";
}
```

**Frontend:**
```typescript
// clients/blogapp-client/src/lib/permissions.ts
export const Permissions = {
  UsersCreate: 'Users.Create',
  UsersUpdate: 'Users.Update',
  UsersDelete: 'Users.Delete',
} as const;
```

### Route Koruması

```typescript
<ProtectedRoute requiredPermission={Permissions.UsersViewAll}>
  <UsersPage />
</ProtectedRoute>

// Birden fazla permission (OR)
<ProtectedRoute requiredAnyPermissions={[Permissions.PostsUpdate, Permissions.PostsCreate]}>
  <PostEditorPage />
</ProtectedRoute>

// Tüm permission'lar gerekli (AND)
<ProtectedRoute requiredAllPermissions={[Permissions.RolesViewAll, Permissions.RolesAssignPermissions]}>
  <RoleManagementPage />
</ProtectedRoute>
```

### UI Element Koruması

```typescript
import { PermissionGuard } from '@/components/auth/permission-guard';

<PermissionGuard requiredPermission={Permissions.UsersCreate}>
  <Button>Yeni Kullanıcı</Button>
</PermissionGuard>

// Fallback ile
<PermissionGuard 
  requiredPermission={Permissions.PostsUpdate}
  fallback={<p>Düzenleme yetkiniz yok</p>}
>
  <PostEditor />
</PermissionGuard>
```

### Hook Kullanımı

```typescript
import { usePermission } from '@/hooks/use-permission';

function MyComponent() {
  const { hasPermission, hasAnyPermission } = usePermission();
  
  const canEdit = hasPermission(Permissions.UsersUpdate);
  const canManage = hasAnyPermission(Permissions.UsersUpdate, Permissions.UsersDelete);
  
  return (
    <div>
      {canEdit && <EditButton />}
      {canManage && <ManagePanel />}
    </div>
  );
}
```

---

## 5. Authentication & Authorization

### JWT Token Structure

```json
{
  "sub": "user-id",
  "email": "user@example.com",
  "role": "Admin",
  "permission": ["Users.Create", "Users.Update"],
  "exp": 1234567890
}
```

### Refresh Token Rotation

**Flow:**
1. Login → Access Token (60 min) + Refresh Token (14 gün)
2. Access Token expire → Refresh Token ile yenile
3. Yeni Access Token + Yeni Refresh Token
4. Eski Refresh Token invalid olur

**Güvenlik:**
- Refresh Token cookie'de (HttpOnly, Secure, SameSite)
- Her refresh'te rotation (eski token geçersiz)
- RefreshSession tablosunda tracking

### Login Flow

```typescript
// 1. Login request
const response = await login({ email, password });

// 2. Token store'a kaydet
const { token, ...user } = response.data;
loginStore({ user, token });

// 3. Redirect
navigate('/admin/dashboard');
```

### Logout Flow

```typescript
// 1. Logout request (refresh token cookie'yi siler)
await logout();

// 2. Store'u temizle
logoutStore();

// 3. Redirect
navigate('/login');
```

---

## 6. Outbox Pattern

### Neden Outbox Pattern?

**Problem:** Domain event'leri RabbitMQ'ya gönderirken hata olursa?  
**Çözüm:** Önce DB'ye kaydet, sonra arka planda RabbitMQ'ya gönder.

### Flow

```
1. Domain Event oluştur
   ↓
2. OutboxMessage olarak DB'ye kaydet
   ↓
3. OutboxProcessorService (5 saniyede bir)
   ↓
4. Integration Event'e dönüştür
   ↓
5. RabbitMQ'ya publish et
   ↓
6. OutboxMessage'ı processed olarak işaretle
```

### Örnek

```csharp
// 1. Domain Event
public class PostCreatedEvent : IDomainEvent
{
    public Guid PostId { get; }
    public string Title { get; }
}

// 2. Entity'de event ekle
public void Create()
{
    AddDomainEvent(new PostCreatedEvent(Id, Title));
}

// 3. SaveChanges'te otomatik OutboxMessage oluşur
await _unitOfWork.SaveChangesAsync();

// 4. OutboxProcessorService işler
// 5. ActivityLogConsumer alır ve ActivityLog'a kaydeder
```

### Converter Strategy

```csharp
public interface IIntegrationEventConverterStrategy
{
    string EventType { get; }
    object? Convert(string payload);
}

// Örnek
public class PostCreatedEventConverter : IIntegrationEventConverterStrategy
{
    public string EventType => "PostCreatedEvent";
    
    public object? Convert(string payload)
    {
        var domainEvent = JsonSerializer.Deserialize<PostCreatedEvent>(payload);
        return new ActivityLogIntegrationEvent(/* ... */);
    }
}
```

---

## 7. Activity Logging

### Ne Loglanır?

- ✅ Post create/update/delete
- ✅ User create/update/delete
- ✅ Role create/update/delete
- ✅ Permission assignments
- ✅ Category create/update/delete

### ActivityLog Entity

```csharp
public class ActivityLog
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string ActivityType { get; set; }  // Created, Updated, Deleted
    public string EntityType { get; set; }    // Post, User, Role
    public Guid? EntityId { get; set; }
    public string Title { get; set; }
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; }
}
```

### Query Örnekleri

```sql
-- Kullanıcı aktiviteleri
SELECT 
    al."ActivityType",
    al."EntityType",
    al."Title",
    al."Timestamp",
    u."UserName"
FROM "ActivityLogs" al
LEFT JOIN "Users" u ON al."UserId" = u."Id"
WHERE al."Timestamp" > NOW() - INTERVAL '30 days'
ORDER BY al."Timestamp" DESC;

-- Entity geçmişi
SELECT *
FROM "ActivityLogs"
WHERE "EntityType" = 'Post' AND "EntityId" = '...'
ORDER BY "Timestamp" DESC;
```

---

## 8. Test Yazma

### Test Yapısı

```
tests/
├── Domain.UnitTests/
│   ├── ValueObjects/
│   │   ├── EmailTests.cs
│   │   └── UserNameTests.cs
│   └── Entities/
│       └── UserTests.cs
└── Application.UnitTests/
    └── (pending)
```

### Test Örneği

```csharp
[TestFixture]
public class EmailTests
{
    [Test]
    public void Create_WithValidEmail_ShouldSucceed()
    {
        // Arrange
        var validEmail = "test@example.com";

        // Act
        var email = Email.Create(validEmail);

        // Assert
        Assert.That(email.Value, Is.EqualTo(validEmail));
    }

    [Test]
    [TestCase("invalid")]
    [TestCase("@example.com")]
    public void Create_WithInvalidFormat_ShouldThrowException(string invalidEmail)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
    }
}
```

### Test Çalıştırma

```bash
# Tüm testler
dotnet test BlogApp.sln

# Sadece Domain testleri
dotnet test tests/Domain.UnitTests/Domain.UnitTests.csproj

# Coverage ile
dotnet test --collect:"XPlat Code Coverage"
```

---

## 9. Best Practices

### 9.1. Entity Oluşturma

**✅ Doğru:**
```csharp
public static User Create(string userName, string email, string passwordHash)
{
    var user = new User
    {
        UserName = userName,
        Email = email,
        PasswordHash = passwordHash
    };
    
    user.AddDomainEvent(new UserCreatedEvent(user.Id, userName));
    return user;
}
```

**❌ Yanlış:**
```csharp
var user = new User(); // Constructor kullanma
user.UserName = userName;
user.Email = email;
```

### 9.2. Repository Kullanımı

**✅ Doğru:**
```csharp
await _userRepository.AddAsync(user);
await _roleRepository.AddAsync(role);
await _unitOfWork.SaveChangesAsync(); // Tek transaction
```

**❌ Yanlış:**
```csharp
await _userRepository.AddAsync(user);
await _userRepository.SaveChangesAsync(); // Her işlemde ayrı transaction
await _roleRepository.AddAsync(role);
await _roleRepository.SaveChangesAsync();
```

### 9.3. Error Handling

**✅ Doğru:**
```csharp
try
{
    await operation();
}
catch (Exception ex)
{
    _logger.LogError(ex, "Operation failed for {UserId}", userId);
    throw; // veya custom exception
}
```

**❌ Yanlış:**
```csharp
try
{
    await operation();
}
catch
{
    // Exception swallow - asla yapma!
}
```

### 9.4. Validation

**FluentValidation:**
```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
            
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}
```

**ValidationBehavior** otomatik çalışır (MediatR pipeline).

### 9.5. API Response

**✅ Doğru:**
```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
{
    var result = await Mediator.Send(command);
    return result.ToResponse(); // BaseApiController extension
}
```

**❌ Yanlış:**
```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
{
    var result = await Mediator.Send(command);
    return Ok(result); // Her zaman 200 döner
}
```

### 9.6. Frontend Mutation

**✅ Doğru:**
```typescript
const mutation = useMutation({
  mutationFn: createUser,
  onSuccess: (result) => {
    if (!result.success) {
      showApiResponseError(result, 'Kullanıcı oluşturulamadı');
      return;
    }
    toast.success(result.message);
    invalidateUsers();
  },
  onError: (error) => handleApiError(error, 'Hata oluştu')
});
```

**❌ Yanlış:**
```typescript
const mutation = useMutation({
  mutationFn: createUser,
  onSuccess: () => {
    toast.success('Başarılı'); // Backend message'ı göstermiyor
  }
});
```

---

## 10. Troubleshooting

### 10.1. Migration Hataları

**Problem:** Migration uygulanamıyor  
**Çözüm:**
```bash
# Migration listesi
dotnet ef migrations list --project src/BlogApp.Persistence

# Son migration'ı geri al
dotnet ef migrations remove --project src/BlogApp.Persistence

# Yeni migration
dotnet ef migrations add MigrationName --project src/BlogApp.Persistence

# Database'e uygula
dotnet ef database update --project src/BlogApp.Persistence
```

### 10.2. Docker Sorunları

**Problem:** Container başlamıyor  
**Çözüm:**
```bash
# Log'ları kontrol et
docker logs blogapp-api

# Container'ı yeniden başlat
docker-compose down
docker-compose up --build

# Volume'ları temizle
docker-compose down -v
docker volume prune
```

### 10.3. RabbitMQ Bağlantı Hatası

**Problem:** RabbitMQ'ya bağlanamıyor  
**Çözüm:**
```bash
# RabbitMQ container'ı kontrol et
docker ps | grep rabbitmq

# Management UI'a eriş
http://localhost:15672
# User: blogapp, Password: supersecret

# Queue'ları kontrol et
# Queues → activity-log-queue
```

### 10.4. Permission Çalışmıyor

**Problem:** Kullanıcı yetkisi olmasına rağmen erişemiyor  
**Çözüm:**
1. JWT token'ı kontrol et (jwt.io)
2. Permission claim'leri var mı?
3. Frontend Permissions constant'ı backend ile senkron mu?
4. Browser console'da permission kontrolünü debug et

### 10.5. Test Hataları

**Problem:** Test başarısız  
**Çözüm:**
```bash
# Detaylı output
dotnet test --verbosity detailed

# Tek test çalıştır
dotnet test --filter "FullyQualifiedName~EmailTests"

# Clean + rebuild
dotnet clean
dotnet build
dotnet test
```

---

## 📚 İlgili Dosyalar

### Önemli Dokümantasyon
- `README.md` - Proje genel bakış
- `docs/ANALYSIS.md` - Kod analizi ve iyileştirme önerileri
- `docs/DEVELOPER_GUIDE.md` - Bu dosya
- `tests/README.md` - Test dokümantasyonu

### Konfigürasyon
- `src/BlogApp.API/appsettings.json` - Base configuration
- `src/BlogApp.API/appsettings.Development.json` - Development settings
- `docker-compose.yml` - Docker base configuration
- `docker-compose.local.yml` - Local development
- `docker-compose.prod.yml` - Production

### Önemli Kod Dosyaları
- `src/BlogApp.API/Program.cs` - Application startup
- `src/BlogApp.Application/ApplicationServicesRegistration.cs` - MediatR, FluentValidation
- `src/BlogApp.Infrastructure/InfrastructureServicesRegistration.cs` - JWT, RabbitMQ, Redis
- `src/BlogApp.Persistence/PersistenceServicesRegistration.cs` - EF Core, Repositories

---

## 🎯 Hızlı Başlangıç

### 1. Geliştirme Ortamı Kurulumu

```bash
# 1. Repo'yu clone et
git clone https://github.com/your-repo/BlogApp.git
cd BlogApp

# 2. Docker servisleri başlat
docker-compose -f docker-compose.yml -f docker-compose.local.yml up -d

# 3. Backend'i çalıştır
cd src/BlogApp.API
dotnet run

# 4. Frontend'i çalıştır
cd clients/blogapp-client
npm install
npm run dev
```

### 2. İlk Feature Ekleme

```bash
# 1. Domain entity oluştur
# src/BlogApp.Domain/Entities/MyEntity.cs

# 2. Repository interface
# src/BlogApp.Domain/Repositories/IMyEntityRepository.cs

# 3. Repository implementation
# src/BlogApp.Persistence/Repositories/MyEntityRepository.cs

# 4. CQRS commands/queries
# src/BlogApp.Application/Features/MyEntity/Commands/Create/

# 5. Controller
# src/BlogApp.API/Controllers/MyEntityController.cs

# 6. Migration
dotnet ef migrations add AddMyEntity --project src/BlogApp.Persistence
dotnet ef database update --project src/BlogApp.Persistence

# 7. Frontend
# clients/blogapp-client/src/features/my-entity/
```

### 3. Test Yazma

```bash
# 1. Test dosyası oluştur
# tests/Domain.UnitTests/Entities/MyEntityTests.cs

# 2. Test yaz
[Test]
public void Create_WithValidData_ShouldSucceed()
{
    var entity = MyEntity.Create("test");
    Assert.That(entity.Name, Is.EqualTo("test"));
}

# 3. Çalıştır
dotnet test
```

---

**Son Güncelleme:** 2025-01-25  
**Versiyon:** 1.0  
**Yazar:** BlogApp Development Team
