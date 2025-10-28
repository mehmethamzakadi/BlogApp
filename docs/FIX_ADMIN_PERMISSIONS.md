# Admin Panel Erişim ve Permission Sistemi Dokümantasyonu

## Genel Bakış
Bu doküman, BlogApp uygulamasındaki permission-based authorization sisteminin nasıl çalıştığını ve admin panel erişim mekanizmasını açıklar.

## Sistem Mimarisi

### 1. Permission Tabanlı Yetkilendirme
Uygulama, rol tabanlı (RBAC) değil, **permission tabanlı** bir yetkilendirme sistemi kullanır:

- **Roller (Roles)**: Kullanıcı grupları (Admin, User, Moderator, Editor)
- **Permissions**: Granular yetkiler (Dashboard.View, Users.Create, Posts.Update vb.)
- **RolePermissions**: Rollere atanan permission'lar

### 2. Mevcut Roller ve Yetkiler

#### Tanımlı Roller
- **Admin**: Tüm yetkiler (20000000-0000-0000-0000-000000000001)
- **User**: Temel kullanıcı yetkileri (20000000-0000-0000-0000-000000000002)
- **Moderator**: İçerik moderasyon yetkileri (20000000-0000-0000-0000-000000000003)
- **Editor**: İçerik yönetim yetkileri (20000000-0000-0000-0000-000000000004)

#### Permission Modülleri
```csharp
// Dashboard
Dashboard.View

// User Management
Users.Create, Users.Read, Users.Update, Users.Delete, Users.ViewAll

// Role Management
Roles.Create, Roles.Read, Roles.Update, Roles.Delete, Roles.ViewAll, Roles.AssignPermissions

// Post Management
Posts.Create, Posts.Read, Posts.Update, Posts.Delete, Posts.ViewAll, Posts.Publish

// Category Management
Categories.Create, Categories.Read, Categories.Update, Categories.Delete, Categories.ViewAll

// Comment Management
Comments.Create, Comments.Read, Comments.Update, Comments.Delete, Comments.ViewAll, Comments.Moderate

// Bookshelf Management
Bookshelf.Create, Bookshelf.Read, Bookshelf.Update, Bookshelf.Delete, Bookshelf.ViewAll

// Media & Activity Logs
Media.Upload, ActivityLogs.View
```

## JWT Token ve Permission Akışı

### Token Oluşturma Süreci

**Dosya**: `src/BlogApp.Infrastructure/Services/JwtTokenService.cs`

```csharp
public async Task<List<Claim>> GetAuthClaims(User user)
{
    var userRoles = await _userRepository.GetRolesAsync(user);
    var authClaims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Name, user.UserName),
    };

    // 1. Rol claim'lerini ekle
    foreach (var roleName in userRoles)
    {
        authClaims.Add(new Claim(ClaimTypes.Role, roleName));
    }

    // 2. Permission claim'lerini ekle
    if (userRoles.Any())
    {
        // Dinamik olarak rol ID'lerini al (❌ ASLA hardcode etme!)
        var roleIds = await _dbContext.Roles
            .Where(r => userRoles.Contains(r.Name))
            .Select(r => r.Id)
            .ToListAsync();

        if (roleIds.Any())
        {
            // Rollere ait tüm permission'ları getir
            var permissions = await _permissionRepository.GetPermissionsByRoleIdsAsync(roleIds);
            foreach (var permission in permissions)
            {
                authClaims.Add(new Claim("permission", permission.Name));
            }
        }
    }

    return authClaims;
}
```

### Önemli Notlar
✅ **Doğru Yaklaşım**: Rol ID'leri veritabanından dinamik olarak alınır
❌ **Yanlış Yaklaşım**: Rol ID'lerini hardcode etmek (örn: `roleIds.Add(1)`)

**Neden?**
- GUID tabanlı ID'ler her migration'da değişebilir
- Veritabanı sıralaması farklı olabilir
- Seed verisi değiştiğinde ID'ler güncellenebilir

## Permission Seed Sistemi

### Otomatik Seed Mekanizması

**Dosya**: `src/BlogApp.Persistence/DatabaseInitializer/PermissionSeeder.cs`

Uygulama başlangıcında otomatik olarak çalışır ve şu işlemleri yapar:

```csharp
public async Task SeedPermissionsAsync()
{
    // 1. Permission'ları kontrol et ve oluştur
    await EnsurePermissionsExistAsync();
    await _context.SaveChangesAsync();
    
    // 2. Admin rolüne TÜM permission'ları ata
    await AssignAllPermissionsToAdminAsync();
    await _context.SaveChangesAsync();
    
    // 3. User rolüne temel permission'ları ata
    await AssignBasicPermissionsToUserAsync();
    await _context.SaveChangesAsync();
}
```

### Admin Role Permission Atama

```csharp
private async Task AssignAllPermissionsToAdminAsync()
{
    var adminRole = await _roleRepository.Query()
        .FirstOrDefaultAsync(r => r.NormalizedName == UserRoles.Admin.ToUpper());

    var allPermissions = await _context.Permissions
        .Where(p => !p.IsDeleted)
        .ToListAsync();

    var existingRolePermissions = await _context.RolePermissions
        .Where(rp => rp.RoleId == adminRole.Id)
        .Select(rp => rp.PermissionId)
        .ToListAsync();

    // ✅ Sadece eksik permission'ları ekle
    var permissionsToAdd = allPermissions
        .Where(p => !existingRolePermissions.Contains(p.Id))
        .Select(p => new RolePermission
        {
            RoleId = adminRole.Id,
            PermissionId = p.Id,
            GrantedAt = DateTime.UtcNow
        })
        .ToList();

    if (permissionsToAdd.Any())
    {
        _context.RolePermissions.AddRange(permissionsToAdd);
    }
}
```

### User Role Permission Atama

```csharp
private async Task AssignBasicPermissionsToUserAsync()
{
    var userRole = await _roleRepository.Query()
        .FirstOrDefaultAsync(r => r.NormalizedName == UserRoles.User.ToUpper());

    // ⚠️ User rolünün mevcut permission'ları varsa koruma altına alır
    var hasExistingPermissions = await _context.RolePermissions
        .AnyAsync(rp => rp.RoleId == userRole.Id);

    if (hasExistingPermissions)
    {
        return; // Manuel değişiklikleri koru
    }

    // İlk kurulumda temel permission'ları ata
    var userPermissionNames = Permissions.GetUserPermissions();
    var userPermissions = await _context.Permissions
        .Where(p => userPermissionNames.Contains(p.Name) && !p.IsDeleted)
        .ToListAsync();

    var permissionsToAdd = userPermissions
        .Select(p => new RolePermission
        {
            RoleId = userRole.Id,
            PermissionId = p.Id,
            GrantedAt = DateTime.UtcNow
        })
        .ToList();

    _context.RolePermissions.AddRange(permissionsToAdd);
}
```

## Migration ve Seed Verileri

### Initial Migration
**Dosya**: `src/BlogApp.Persistence/Migrations/PostgreSql/20251027200711_Init.cs`

- Tüm roller GUID ID ile oluşturulur
- Tüm permission'lar GUID ID ile oluşturulur
- RolePermission ilişkileri seed edilir

### Seed Dosyaları

1. **RoleSeed.cs**: Rolleri tanımlar
   - Admin, User, Moderator, Editor rolleri

2. **PermissionSeed.cs**: Permission'ları tanımlar
   - `Permissions.GetAllPermissions()` metodundan alır
   - Her permission için açıklama oluşturur

3. **RolePermissionSeed.cs**: Rol-Permission eşleştirmelerini yapar
   ```csharp
   // Admin tüm permission'lara sahip
   AddPermissions(adminRoleId, Permissions.GetAllPermissions());
   
   // Editor içerik yönetimi permission'larına sahip
   AddPermissions(editorRoleId, new[]
   {
       Permissions.PostsCreate,
       Permissions.PostsRead,
       Permissions.PostsUpdate,
       // ...
   });
   ```

## Frontend ve Permission Kontrolü

### Permission Guards

Frontend'de kullanıcı permission'larına göre UI elementleri gösterilir/gizlenir:

```typescript
// Token'dan permission'ları al
const permissions = user?.permissions || [];

// Dashboard erişim kontrolü
if (!permissions.includes('Dashboard.View')) {
    return <AccessDenied />;
}

// Buton görünürlük kontrolü
{permissions.includes('Users.Create') && (
    <Button>Yeni Kullanıcı</Button>
)}
```

### LocalStorage Token Yapısı

```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "base64string...",
  "expiresAt": "2025-10-28T10:00:00Z",
  "permissions": [
    "Dashboard.View",
    "Users.ViewAll",
    "Users.Create",
    "Posts.ViewAll",
    // ... diğer permission'lar
  ]
}
```

## Sorun Giderme ve Test

### 1. Permission Sorunlarının Çözümü

#### Senaryo: Admin Panel'e Erişilemiyor

**Belirtiler**:
- Admin kullanıcısı login olabiliyor
- Dashboard'a girdiğinde "Erişim Reddedildi" hatası alıyor

**Çözüm Adımları**:

1. **Veritabanını Kontrol Edin**
   ```sql
   -- Admin rolünün permission'larını kontrol et
   SELECT p.Name 
   FROM Permissions p
   INNER JOIN RolePermissions rp ON p.Id = rp.PermissionId
   INNER JOIN Roles r ON rp.RoleId = r.Id
   WHERE r.Name = 'Admin' AND p.IsDeleted = false;
   
   -- Dashboard.View permission'ı var mı?
   SELECT * FROM Permissions WHERE Name = 'Dashboard.View';
   ```

2. **Permission Seeder'ı Yeniden Çalıştırın**
   ```bash
   # Docker kullanıyorsanız
   docker-compose down
   docker-compose up -d
   
   # Veya doğrudan çalıştırıyorsanız
   cd src/BlogApp.API
   dotnet run
   ```
   
   PermissionSeeder uygulama başlangıcında otomatik çalışır ve eksik permission'ları ekler.

3. **Token'ı Yenileyin**
   - Browser'da F12 > Application > Local Storage
   - `blogapp-auth` key'ini silin
   - Yeniden login yapın

4. **Token İçeriğini Kontrol Edin**
   ```javascript
   // Browser Console'da
   const auth = JSON.parse(localStorage.getItem('blogapp-auth'));
   console.log(auth.permissions);
   // ["Dashboard.View", "Users.ViewAll", ...] gibi bir array dönmeli
   ```

### 2. Yeni Permission Ekleme

1. **Permission Constant'ı Tanımlayın**
   ```csharp
   // src/BlogApp.Domain/Constants/Permissions.cs
   public const string MyNewPermission = "MyModule.MyAction";
   ```

2. **GetAllPermissions'a Ekleyin**
   ```csharp
   public static List<string> GetAllPermissions()
   {
       return new List<string>
       {
           // ... mevcut permission'lar
           MyNewPermission
       };
   }
   ```

3. **Uygulamayı Yeniden Başlatın**
   - PermissionSeeder otomatik olarak yeni permission'ı ekleyecek
   - Admin rolüne otomatik atanacak

4. **Migration Oluşturun** (Opsiyonel - Production için)
   ```bash
   cd src/BlogApp.Persistence
   dotnet ef migrations add AddMyNewPermission --context BlogAppDbContext
   ```

### 3. Test Senaryoları

#### Test 1: Admin Login ve Permission Kontrolü
```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "Admin123!"
}

# Response'da permissions array'ini kontrol edin:
{
  "success": true,
  "data": {
    "accessToken": "...",
    "permissions": [
      "Dashboard.View",
      "Users.Create",
      "Users.Read",
      // ... tüm permission'lar
    ]
  }
}
```

#### Test 2: User Login ve Sınırlı Permission
```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "User123!"
}

# Response:
{
  "permissions": [
    "Posts.Create",
    "Posts.Read",
    "Posts.Update",
    "Categories.Read",
    "Comments.Create",
    "Comments.Read",
    "Comments.Update"
  ]
}
```

#### Test 3: Dashboard Erişim
```bash
GET /api/dashboard/stats
Authorization: Bearer {accessToken}

# Admin için: 200 OK
# User için: 403 Forbidden
```

### 4. Monitoring ve Logging

**LogLevel**: Permission işlemleri varsayılan olarak `Information` seviyesinde loglanır.

```csharp
// PermissionSeeder logları
[Information] Starting permission seeding process
[Information] Adding {count} missing permissions
[Information] Assigned {count} new permissions to Admin role
[Information] Permission seeding completed successfully
```

**Log Dosyası**: `src/BlogApp.API/logs/blogapp-{Date}.txt`

### 5. Yaygın Hatalar ve Çözümleri

| Hata | Neden | Çözüm |
|------|-------|-------|
| "Erişim Reddedildi" | Token'da permission yok | Logout yapıp yeniden login olun |
| "Permission not found" | Yeni permission seed edilmemiş | Uygulamayı yeniden başlatın |
| "Role not found" | Rol silinmiş veya seed edilmemiş | Migration'ları kontrol edin |
| Token'da eski permission'lar var | Cached token kullanılıyor | LocalStorage'ı temizleyin |

## Best Practices

### ✅ Yapılması Gerekenler

1. **Rol ID'lerini Hardcode Etmeyin**
   ```csharp
   // ❌ YANLIŞ
   var adminRoleId = Guid.Parse("20000000-0000-0000-0000-000000000001");
   
   // ✅ DOĞRU
   var adminRole = await _roleRepository.GetByNameAsync("Admin");
   ```

2. **Permission Kontrollerini Merkezileştirin**
   ```csharp
   // Controller'da [Authorize] attribute kullanın
   [Authorize(Policy = PermissionPolicies.DashboardView)]
   public async Task<IActionResult> GetDashboardStats()
   ```

3. **Frontend'de Permission Guards Kullanın**
   ```typescript
   // Component seviyesinde kontrol
   if (!hasPermission('Users.Create')) {
       return <AccessDenied />;
   }
   ```

4. **Permission'ları Modüller Halinde Organize Edin**
   - Dashboard, Users, Posts, Categories, vb.
   - Her modül için CRUD permission'ları

5. **Yeni Permission'ları Dokümante Edin**
   - Açıklayıcı isimler kullanın
   - Description alanını doldurun

### ❌ Yapılmaması Gerekenler

1. **Hardcoded ID Kullanımı**
   ```csharp
   // ❌ Asla böyle yapmayın!
   if (roleId == 1) // Admin rolü mü?
   ```

2. **Role-Based Check'ler Yapmak**
   ```csharp
   // ❌ Rol kontrolü yerine permission kontrolü yapın
   if (User.IsInRole("Admin"))
   
   // ✅ Permission kontrolü yapın
   if (User.HasPermission("Users.Create"))
   ```

3. **Permission'ları String Literal Olarak Yazmak**
   ```csharp
   // ❌ Magic string kullanmayın
   [Authorize(Policy = "Dashboard.View")]
   
   // ✅ Constant kullanın
   [Authorize(Policy = Permissions.DashboardView)]
   ```

4. **Migration'sız Permission Değişikliği**
   - Production'da manuel seed güvenli değil
   - Her zaman migration oluşturun

5. **Permission Cache'ini Unutmak**
   - Token içindeki permission'lar statiktir
   - Değişiklikler için yeniden login gerekir

## Gelecek Geliştirmeler

### 1. Dynamic Permission Assignment UI
Admin panel'de rollere UI üzerinden permission atama

### 2. Permission Audit Logging
Hangi kullanıcının hangi permission'la ne yaptığını loglama

### 3. Permission Groups
İlişkili permission'ları gruplandırma (örn: "Content Management" grubu)

### 4. Temporal Permissions
Belirli bir süre için geçici permission atama

### 5. Resource-Based Permissions
Belirli kaynaklara özel permission'lar (örn: sadece kendi postlarını düzenleme)

## İlgili Dosyalar

### Core Files
- `src/BlogApp.Infrastructure/Services/JwtTokenService.cs` - Token ve claim oluşturma
- `src/BlogApp.Domain/Constants/Permissions.cs` - Permission tanımları
- `src/BlogApp.Domain/Constants/UserRoles.cs` - Rol tanımları

### Persistence Layer
- `src/BlogApp.Persistence/DatabaseInitializer/PermissionSeeder.cs` - Otomatik seed
- `src/BlogApp.Persistence/Seeds/PermissionSeed.cs` - Migration seed data
- `src/BlogApp.Persistence/Seeds/RoleSeed.cs` - Rol seed data
- `src/BlogApp.Persistence/Seeds/RolePermissionSeed.cs` - Rol-Permission eşleştirme

### Repositories
- `src/BlogApp.Persistence/Repositories/PermissionRepository.cs` - Permission CRUD
- `src/BlogApp.Persistence/Repositories/RoleRepository.cs` - Rol CRUD

### API Layer
- `src/BlogApp.API/Filters/PermissionAuthorizationHandler.cs` - Permission kontrolü
- `src/BlogApp.API/Controllers/AuthController.cs` - Login/Token endpoint

## Özet

Bu permission sistemi:
- ✅ Dinamik ve esnek
- ✅ Veritabanı bağımlılıklarını minimize eder
- ✅ Otomatik seed mekanizması ile çalışır
- ✅ Frontend ile entegre
- ✅ JWT token tabanlı
- ✅ Granular permission kontrolü sağlar

**Önemli**: JWT token'lar stateless'tır. Permission değişiklikleri için kullanıcının yeniden login yapması gerekir.
