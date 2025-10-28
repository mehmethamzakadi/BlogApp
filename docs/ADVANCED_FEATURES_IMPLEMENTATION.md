# BlogApp – Advanced Features & Permission Stack

Bu belge, BlogApp’in yetkilendirme, toplu işlemler, audit logging ve veri dışa aktarma gibi gelişmiş özelliklerinin güncel durumunu özetler. Backend (.NET 9) ve frontend (React + TypeScript) tarafındaki implementasyonlar birlikte ele alınmıştır.

## 1. Backend Yetkilendirme Katmanı
- `HasPermissionAttribute` controller action’larında policy bazlı koruma sağlıyor (`PermissionRequirement`).
- `PermissionAuthorizationHandler` JWT içindeki `permission` claim’lerini doğruluyor; claim yoksa erişim reddediliyor.
- Aşağıdaki controller uçları permission ile korunuyor:
  - `UserController` (CRUD + bulk + export)
  - `RoleController` (CRUD + bulk)
  - `PostController` (CRUD, search; public get uçları `AllowAnonymous`)
  - `CategoryController` (CRUD + search; `GetAll` public)
  - `PermissionController` (listeleme + rol atama)
  - `ActivityLogsController` (listeleme)
  - `BookshelfController` (CRUD işlemleri)
  - `ImagesController` (media upload)
- `DashboardController` için herhangi bir permission attribute bulunmuyor; dashboard erişimi frontend'de kontrol ediliyor.

**Örnek kullanım:**
```csharp
[HttpPost]
[HasPermission(Permissions.UsersCreate)]
public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
{
    var response = await Mediator.Send(command);
    return Ok(response);
}
```

## 2. Frontend Permission Guard’ları
- `routes/protected-route.tsx` authenticated kullanıcıyı ve gerekli izin(ler)i kontrol ediyor, aksi halde `/login` veya 403 sayfasına yönlendiriyor.
- `components/guards/PermissionGuard.tsx` component bazında izin denetimi, toast mesajı ve opsiyonel fallback desteği sunuyor.
- `axios` interceptors 403 hatalarında kullanıcıyı “yetki yok” mesajıyla bilgilendiriyor; 401 için refresh token akışı yönetiliyor.

**Örnek:**
```tsx
<ProtectedRoute requiredPermission={Permissions.UsersViewAll}>
  <UsersPage />
</ProtectedRoute>

<PermissionGuard permissions={[Permissions.UsersCreate, Permissions.UsersUpdate]} requireAll>
  <UserManagementPanel />
</PermissionGuard>
```

## 3. Permission Seti (Güncel)
Backend `BlogApp.Domain.Constants.Permissions` sınıfındaki değerler seed ediliyor ve policies oluşturuluyor.

- Dashboard: `Dashboard.View`
- Users: `Users.Create`, `Users.Read`, `Users.Update`, `Users.Delete`, `Users.ViewAll`
- Roles: `Roles.Create`, `Roles.Read`, `Roles.Update`, `Roles.Delete`, `Roles.ViewAll`, `Roles.AssignPermissions`
- Posts: `Posts.Create`, `Posts.Read`, `Posts.Update`, `Posts.Delete`, `Posts.ViewAll`, `Posts.Publish`
- Categories: `Categories.Create`, `Categories.Read`, `Categories.Update`, `Categories.Delete`, `Categories.ViewAll`
- Comments: `Comments.Create`, `Comments.Read`, `Comments.Update`, `Comments.Delete`, `Comments.ViewAll`, `Comments.Moderate`
- Bookshelf: `Bookshelf.Create`, `Bookshelf.Read`, `Bookshelf.Update`, `Bookshelf.Delete`, `Bookshelf.ViewAll`
- Media: `Media.Upload`
- Activity Logs: `ActivityLogs.View`

Frontend'de `clients/blogapp-client/src/lib/permissions.ts` aynı string değerleri export ediyor; admin sidebar ve router bu sabitleri kullanıyor.

## 4. Audit Logging ve Activity Log UI
- Domain event → outbox → RabbitMQ → `ActivityLogConsumer` zinciriyle audit kayıtları oluşuyor.
- `ActivityLogsController` `POST /api/activitylogs/search` ile `DataGridRequest` kabul ediyor.
- Frontend `/admin/activity-logs` sayfası React Query ile pagination, filtre ve arama destekliyor.

**İzin:** `Permissions.ActivityLogsView` (backend’de attribute, frontend’de route guard).

## 5. Toplu Silme (Bulk Operations)
- `BulkDeleteUsersCommand/BulkDeleteRolesCommand` domain event tetikliyor, UnitOfWork ile tek transaction’da tamamlanıyor.
- Kullanıcı tarafı, seçilen kayıtları POST `/api/user/bulk-delete` veya `/api/role/bulk-delete` ile iletiyor.
- Yanıt yapısı `DeletedCount`, `FailedCount`, `Errors` alanlarını içeriyor.

**Not:** Admin rolü `BulkDeleteRolesCommandHandler` içinde silinmeye karşı korunuyor.

## 6. CSV Export
- `ExportUsersQuery` yalnızca `csv` formatını destekliyor; Excel desteği henüz yok.
- Endpoint: `GET /api/user/export?format=csv`, izin: `Permissions.UsersViewAll`.
- Dönen içerik `text/csv`, dosya adı `users_{timestamp}.csv`. Alanlar: `Id`, `UserName`, `Email`, `FirstName`, `LastName`, `CreatedDate`.

Frontend’de `window.location.href` ile indiriliyor; hook veya React Query entegrasyonu bulunmuyor.

## 7. Middleware ve Guard Uyumları
- JWT token oluşturma `JwtTokenService` ile yapılıyor; roller `ClaimTypes.Role`, izinler custom `permission` claim’i olarak ekleniyor.
- Policy tanımları `InfrastructureServicesRegistration` içinde `Permissions.GetAllPermissions()` ile dinamik olarak ekleniyor.
- CORS, Rate Limit, Exception middleware’leri Program.cs’de aktif; `UseCors` çağrısının `UseRouting` sonrasına taşınması öneriliyor (ANALYSIS.md’de işaretlendi).

## 8. UI/UX ve Bildirimler
- `ForbiddenPage` hem global rota (`/forbidden`) hem de guard fallback olarak kullanılıyor.
- `react-hot-toast` ile yetki eksikliği ve işlem sonuçları kullanıcıya iletiliyor.
- Admin sidebar menüleri izin bazlı gösteriliyor (`components/admin/sidebar.tsx`).
- Sidebar'da yeni eklenen Kitaplık (Bookshelf) menüsü `Permissions.BookshelfViewAll` izni ile korunuyor.

## 9. Kitaplık Yönetimi (Bookshelf)
- `BookshelfController` ile kullanıcılar okudukları kitapları yönetebiliyorlar.
- CRUD işlemleri permission ile korunuyor:
  - `Bookshelf.ViewAll` - Tüm kitapları listeleme
  - `Bookshelf.Read` - Tek bir kitap detayını görüntüleme
  - `Bookshelf.Create` - Yeni kitap ekleme
  - `Bookshelf.Update` - Kitap güncelleme
  - `Bookshelf.Delete` - Kitap silme
- Frontend `/admin/bookshelf` sayfasında React Query ile pagination ve filtreleme desteği mevcut.
- Domain katmanında `BookshelfItem` entity'si ve ilgili domain event'leri tanımlanmış.

## 10. Media Yönetimi
- `ImagesController` ile görsel yükleme işlemleri yapılıyor.
- `Permissions.MediaUpload` izni ile korunan endpoint mevcut.
- Yüklenen görseller `wwwroot` dizininde saklanıyor.

## 11. Gelecek İyileştirme Fikirleri
1. CSV dışa aktarmada alan seçimi ve XLSX desteği.
2. Toplu işlemler için asenkron job/queue ve background sonuç bildirimi.
3. Permission yönetimi için UI: roller/izinler matris görünümü, role klonlama.
4. Activity log için export ve istatistik görünümü.
5. SignalR ile gerçek zamanlı izin güncelleme/aktivite bildirimi.
6. Kitaplık için kitap değerlendirme ve notlar sistemi.
7. Media yönetimi için toplu yükleme ve görsel düzenleme özellikleri.

## 12. Referans Dosyalar
- Backend
  - `src/BlogApp.API/Controllers/*.cs`
  - `src/BlogApp.Infrastructure/Authorization/*`
  - `src/BlogApp.Application/Features/*`
  - `src/BlogApp.Domain/Constants/Permissions.cs`
  - `src/BlogApp.Domain/Entities/BookshelfItem.cs`
- Frontend
  - `clients/blogapp-client/src/routes/protected-route.tsx`
  - `clients/blogapp-client/src/components/guards/PermissionGuard.tsx`
  - `clients/blogapp-client/src/components/admin/sidebar.tsx`
  - `clients/blogapp-client/src/lib/axios.ts`
  - `clients/blogapp-client/src/lib/permissions.ts`
  - `clients/blogapp-client/src/pages/admin/*`

Bu özet, izin tabanlı güvenlik ve gelişmiş yönetim fonksiyonlarının mevcut durumunu doğrular; yeni özellik eklerken yukarıdaki yapıların korunması tavsiye edilir.
