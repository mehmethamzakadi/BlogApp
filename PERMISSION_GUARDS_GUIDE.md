# Permission-Based Route Guards & UI Controls

## 📋 Genel Bakış

BlogApp'e **permission bazlı route koruması ve UI kontrolü** sistemi başarıyla entegre edildi. Kullanıcılar artık sadece yetkili oldukları sayfalara erişebilir ve yetkisi olmayan butonları göremez.

---

## ✨ İmplementasyon Detayları

### 🎯 Backend Değişiklikleri

#### 1. **LoginResponse'a Permissions Eklendi**
**Dosya:** `src/BlogApp.Application/Features/Auths/Login/LoginResponse.cs`

```csharp
public sealed record LoginResponse(
    int UserId,
    string UserName,
    DateTime Expiration,
    string Token,
    string RefreshToken,
    List<string> Permissions  // ✅ YENİ
);
```

#### 2. **JWT Token'dan Permission'lar Çıkarıldı**
**Dosya:** `src/BlogApp.Infrastructure/Services/Identity/JwtTokenService.cs`

```csharp
// JWT claim'lerinden permission'ları çıkar ve response'a ekle
var permissions = claims
    .Where(c => c.Type == "permission")
    .Select(c => c.Value)
    .ToList();
```

---

### 🎨 Frontend Değişiklikleri

#### 1. **AuthUser Interface Güncellendi**
**Dosya:** `clients/blogapp-client/src/stores/auth-store.ts`

```typescript
export interface AuthUser {
  userId: number;
  userName: string;
  expiration: string;
  refreshToken: string;
  permissions: string[];  // ✅ YENİ
}
```

#### 2. **Permission Hook Oluşturuldu**
**Dosya:** `clients/blogapp-client/src/hooks/use-permission.ts`

```typescript
export function usePermission() {
  const { user } = useAuth();
  const permissions = user?.permissions || [];

  const hasPermission = (permission: string): boolean => {
    return permissions.includes(permission);
  };

  const hasAnyPermission = (...requiredPermissions: string[]): boolean => {
    return requiredPermissions.some((permission) => permissions.includes(permission));
  };

  const hasAllPermissions = (...requiredPermissions: string[]): boolean => {
    return requiredPermissions.every((permission) => permissions.includes(permission));
  };

  return { permissions, hasPermission, hasAnyPermission, hasAllPermissions };
}
```

#### 3. **Permission Constants**
**Dosya:** `clients/blogapp-client/src/lib/permissions.ts`

Backend'deki `Permissions.cs` ile senkronize edilmiş permission constant'ları:

```typescript
export const Permissions = {
  DashboardView: 'Dashboard.View',
  UsersCreate: 'Users.Create',
  UsersRead: 'Users.Read',
  UsersUpdate: 'Users.Update',
  UsersDelete: 'Users.Delete',
  UsersViewAll: 'Users.ViewAll',
  RolesCreate: 'Roles.Create',
  // ... daha fazlası
} as const;
```

#### 4. **ProtectedRoute Component'i Geliştirildi**
**Dosya:** `clients/blogapp-client/src/routes/protected-route.tsx`

```typescript
interface ProtectedRouteProps {
  children: React.ReactNode;
  requiredPermission?: string;
  requiredAnyPermissions?: string[];
  requiredAllPermissions?: string[];
}
```

**Özellikler:**
- ✅ Login kontrolü (authentication)
- ✅ Permission kontrolü (authorization)
- ✅ 403 Forbidden sayfası gösterimi
- ✅ Esnek permission matching (any/all)

#### 5. **403 Forbidden Sayfası**
**Dosya:** `clients/blogapp-client/src/pages/error/forbidden-page.tsx`

Kullanıcı dostu, profesyonel tasarımda erişim engelleme sayfası.

#### 6. **PermissionGuard Component'i**
**Dosya:** `clients/blogapp-client/src/components/auth/permission-guard.tsx`

UI elementlerini permission'a göre göster/gizle:

```typescript
<PermissionGuard requiredPermission={Permissions.UsersCreate}>
  <Button>Yeni Kullanıcı</Button>
</PermissionGuard>
```

#### 7. **Router Konfigürasyonu**
**Dosya:** `clients/blogapp-client/src/routes/router.tsx`

Her route için gerekli permission tanımlandı:

```typescript
{
  path: 'users',
  element: (
    <ProtectedRoute requiredPermission={Permissions.UsersViewAll}>
      <UsersPage />
    </ProtectedRoute>
  )
}
```

#### 8. **Admin Sidebar Filtreleme**
**Dosya:** `clients/blogapp-client/src/components/admin/sidebar.tsx`

Menü itemları kullanıcının permission'larına göre dinamik filtrelenir:

```typescript
const visibleLinks = links.filter((link) => hasPermission(link.requiredPermission));
```

#### 9. **Action Butonları Koruması**
**Dosya:** `clients/blogapp-client/src/pages/admin/users-page.tsx`

Tablo içindeki edit/delete butonları permission'a göre gösterilir:

```typescript
{hasPermission(Permissions.UsersUpdate) && (
  <Button onClick={() => setEditingUser(row.original)}>
    <Pencil />
  </Button>
)}
```

---

## 🚀 Kullanım Örnekleri

### 1. **Route Koruması**

```typescript
// Tek permission gerektiren route
<ProtectedRoute requiredPermission={Permissions.PostsCreate}>
  <CreatePostPage />
</ProtectedRoute>

// Birden fazla permission'dan biri gereken route
<ProtectedRoute requiredAnyPermissions={[Permissions.PostsUpdate, Permissions.PostsCreate]}>
  <PostEditorPage />
</ProtectedRoute>

// Tüm permission'ların gerekli olduğu route
<ProtectedRoute requiredAllPermissions={[Permissions.RolesViewAll, Permissions.RolesAssignPermissions]}>
  <RoleManagementPage />
</ProtectedRoute>
```

### 2. **UI Element Koruması**

```typescript
import { PermissionGuard } from '@/components/auth/permission-guard';
import { Permissions } from '@/lib/permissions';

function MyComponent() {
  return (
    <div>
      <PermissionGuard requiredPermission={Permissions.UsersCreate}>
        <Button onClick={handleCreate}>Yeni Kullanıcı</Button>
      </PermissionGuard>
      
      <PermissionGuard 
        requiredAnyPermissions={[Permissions.PostsUpdate, Permissions.PostsDelete]}
        fallback={<p>Düzenleme yetkiniz yok</p>}
      >
        <PostEditor />
      </PermissionGuard>
    </div>
  );
}
```

### 3. **Hook Kullanımı**

```typescript
import { usePermission } from '@/hooks/use-permission';
import { Permissions } from '@/lib/permissions';

function UserTable() {
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

## 🧪 Test Senaryoları

### Senaryo 1: Admin Kullanıcı
**Beklenen Davranış:**
- ✅ Tüm menü itemlarını görebilir
- ✅ Tüm sayfalara erişebilir
- ✅ Tüm CRUD butonlarını görebilir

### Senaryo 2: Sınırlı Yetkili Kullanıcı
**Adımlar:**
1. Rol yönetiminden yeni bir rol oluştur (örn: "Editor")
2. Sadece `Posts.ViewAll`, `Posts.Create`, `Posts.Update` permission'larını ver
3. Bu rolü bir kullanıcıya ata
4. O kullanıcı ile login ol

**Beklenen Davranış:**
- ❌ "Users" ve "Roles" menülerini görmemeli
- ✅ "Posts" sayfasını görebilmeli
- ✅ Yeni post oluşturabilmeli
- ✅ Post düzenleyebilmeli
- ❌ Post silemez (Delete butonu gözükmez)
- ❌ `/admin/users` URL'ine gitmeye çalışırsa → 403 Forbidden sayfası

### Senaryo 3: Readonly Kullanıcı
**Adımlar:**
1. Sadece `*.Read` ve `*.ViewAll` permission'ları olan rol oluştur
2. Login ol

**Beklenen Davranış:**
- ✅ Tüm menüleri görebilir
- ✅ Listeleme sayfalarına erişebilir
- ❌ Hiçbir Create/Update/Delete butonu görmez
- ❌ `/admin/posts/new` URL'ine gitmeye çalışırsa → 403 Forbidden

---

## 📊 Güvenlik Katmanları

| Katman | Konum | Açıklama |
|--------|-------|----------|
| **1. JWT Token** | Backend | Permission claim'leri token'da saklanır |
| **2. Route Guard** | Frontend | Sayfa erişimi kontrol edilir |
| **3. UI Guard** | Frontend | Butonlar/componentler gizlenir |
| **4. API Authorization** | Backend | Controller seviyesinde `[HasPermission]` attribute (gelecek) |

---

## 🔄 Senkronizasyon Notları

### Permission Ekleme/Güncelleme Süreci

1. **Backend'de tanımla:**
   ```csharp
   // src/BlogApp.Domain/Constants/Permissions.cs
   public const string CustomPermission = "Module.Action";
   ```

2. **Frontend'de güncelle:**
   ```typescript
   // clients/blogapp-client/src/lib/permissions.ts
   export const Permissions = {
     CustomPermission: 'Module.Action',
   };
   ```

3. **Kullan:**
   ```typescript
   <ProtectedRoute requiredPermission={Permissions.CustomPermission}>
     <MyPage />
   </ProtectedRoute>
   ```

---

## 🎉 Başarılan Özellikler

✅ **Backend permission sistemi** - JWT claim'lerde permission bilgisi  
✅ **Frontend permission hook** - Kolay kullanım için helper fonksiyonlar  
✅ **Route guards** - Sayfa seviyesinde koruma  
✅ **UI guards** - Component seviyesinde gizleme  
✅ **403 sayfası** - Kullanıcı dostu hata sayfası  
✅ **Sidebar filtreleme** - Dinamik menü görünürlüğü  
✅ **Action button guards** - Tablo işlem butonları koruması  
✅ **Permission constants** - Type-safe permission referansları  

---

## 🚧 Sonraki Adımlar (Opsiyonel)

1. **Backend Controller Koruma**
   - Controller action'larda `[HasPermission("Users.Create")]` attribute kullanımı
   
2. **Real-time Permission Control**
   - API 403 response'larını yakalama ve kullanıcıyı uyarma
   
3. **Audit Logging UI**
   - Activity log'ları görüntüleme sayfası
   
4. **Bulk Operations**
   - Çoklu kullanıcı/rol işlemleri
   
5. **Export/Import**
   - Kullanıcı/rol verilerini CSV/Excel export

---

## 📚 İlgili Dosyalar

### Backend
- `src/BlogApp.Application/Features/Auths/Login/LoginResponse.cs`
- `src/BlogApp.Infrastructure/Services/Identity/JwtTokenService.cs`
- `src/BlogApp.Infrastructure/Authorization/PermissionAuthorizationHandler.cs`
- `src/BlogApp.Domain/Constants/Permissions.cs`

### Frontend
- `clients/blogapp-client/src/hooks/use-permission.ts`
- `clients/blogapp-client/src/lib/permissions.ts`
- `clients/blogapp-client/src/routes/protected-route.tsx`
- `clients/blogapp-client/src/components/auth/permission-guard.tsx`
- `clients/blogapp-client/src/pages/error/forbidden-page.tsx`

---

## 💡 Best Practices

1. **Permission naming:** `{Module}.{Action}` formatını kullan
2. **Fallback davranış:** UI guard'larda varsayılan olarak null göster
3. **Error handling:** 403 yerine kullanıcı dostu mesajlar göster
4. **Permission sync:** Backend ve frontend constant'larını senkron tut
5. **Type safety:** TypeScript ile permission string'lerini type-safe yap

---

**Implementasyon Tarihi:** 25 Ekim 2025  
**Süre:** ~1 saat  
**Etkilenen Dosyalar:** 15 dosya
