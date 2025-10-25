# Permission System - Quick Reference

## 🎯 Hook Kullanımı

```typescript
import { usePermission } from '@/hooks/use-permission';
import { Permissions } from '@/lib/permissions';

function MyComponent() {
  const { hasPermission, hasAnyPermission, hasAllPermissions } = usePermission();
  
  // Tek permission kontrolü
  const canCreate = hasPermission(Permissions.UsersCreate);
  
  // En az birinin kontrolü
  const canManage = hasAnyPermission(Permissions.UsersUpdate, Permissions.UsersDelete);
  
  // Hepsinin kontrolü
  const isAdmin = hasAllPermissions(
    Permissions.UsersViewAll, 
    Permissions.RolesViewAll
  );
}
```

## 🛡️ Route Koruması

```typescript
import { ProtectedRoute } from '@/routes/protected-route';
import { Permissions } from '@/lib/permissions';

// Tek permission
<ProtectedRoute requiredPermission={Permissions.UsersViewAll}>
  <UsersPage />
</ProtectedRoute>

// Birden fazla seçenek (OR)
<ProtectedRoute requiredAnyPermissions={[
  Permissions.PostsUpdate, 
  Permissions.PostsCreate
]}>
  <PostEditorPage />
</ProtectedRoute>

// Hepsi gerekli (AND)
<ProtectedRoute requiredAllPermissions={[
  Permissions.RolesViewAll,
  Permissions.RolesAssignPermissions
]}>
  <RoleManagementPage />
</ProtectedRoute>
```

## 🎨 UI Element Koruması

```typescript
import { PermissionGuard } from '@/components/auth/permission-guard';
import { Permissions } from '@/lib/permissions';

// Basit kullanım
<PermissionGuard requiredPermission={Permissions.UsersCreate}>
  <Button>Yeni Kullanıcı</Button>
</PermissionGuard>

// Fallback ile
<PermissionGuard 
  requiredPermission={Permissions.PostsDelete}
  fallback={<span className="text-muted">Yetkiniz yok</span>}
>
  <DeleteButton />
</PermissionGuard>

// Koşullu render
{hasPermission(Permissions.UsersUpdate) && (
  <EditButton />
)}
```

## 📋 Mevcut Permission'lar

```typescript
// Dashboard
Permissions.DashboardView

// Users
Permissions.UsersCreate
Permissions.UsersRead
Permissions.UsersUpdate
Permissions.UsersDelete
Permissions.UsersViewAll

// Roles
Permissions.RolesCreate
Permissions.RolesRead
Permissions.RolesUpdate
Permissions.RolesDelete
Permissions.RolesViewAll
Permissions.RolesAssignPermissions

// Posts
Permissions.PostsCreate
Permissions.PostsRead
Permissions.PostsUpdate
Permissions.PostsDelete
Permissions.PostsViewAll
Permissions.PostsPublish

// Categories
Permissions.CategoriesCreate
Permissions.CategoriesRead
Permissions.CategoriesUpdate
Permissions.CategoriesDelete
Permissions.CategoriesViewAll

// Comments
Permissions.CommentsCreate
Permissions.CommentsRead
Permissions.CommentsUpdate
Permissions.CommentsDelete
Permissions.CommentsViewAll
Permissions.CommentsModerate
```

## 🔧 Yeni Permission Ekleme

### 1. Backend
```csharp
// src/BlogApp.Domain/Constants/Permissions.cs
public const string MyNewPermission = "Module.Action";
```

### 2. Frontend
```typescript
// clients/blogapp-client/src/lib/permissions.ts
export const Permissions = {
  // ... mevcut permission'lar
  MyNewPermission: 'Module.Action',
} as const;
```

### 3. Kullanım
```typescript
<ProtectedRoute requiredPermission={Permissions.MyNewPermission}>
  <MyNewPage />
</ProtectedRoute>
```

## 🧪 Test

### Login sonrası permission kontrol
```typescript
// Browser console
localStorage.getItem('blogapp-auth')
// user.permissions array'ini kontrol et
```

### Permission debug
```typescript
import { usePermission } from '@/hooks/use-permission';

function DebugPermissions() {
  const { permissions } = usePermission();
  
  console.log('User permissions:', permissions);
  
  return (
    <pre>{JSON.stringify(permissions, null, 2)}</pre>
  );
}
```

## ⚠️ Önemli Notlar

1. **Backend senkronizasyonu:** Frontend permission'ları backend ile senkron tutun
2. **Naming convention:** `{Module}.{Action}` formatını kullanın
3. **Type safety:** `Permissions` object'inden import edin, string kullanmayın
4. **Security:** Frontend kontrolü sadece UX içindir, backend kontrolü zorunludur
5. **Caching:** Permission'lar JWT token'da gelir, refresh gerekebilir
