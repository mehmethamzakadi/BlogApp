# BlogApp - Activity Logging & Advanced Logging

## 📊 Genel Bakış

Bu doküman BlogApp'te implementa edilen gelişmiş loglama sisteminin detaylarını içerir.

### Loglama Katmanları

1. **File Logs** - Development ve debugging için dosya tabanlı loglar
2. **Structured Logs** - Production monitoring için PostgreSQL tabanlı structured loglar  
3. **Activity Logs** - Compliance ve audit trail için kullanıcı aktivite logları

## 🎯 Activity Log Sistemi

### Özellikler
- ✅ **Otomatik Loglama:** MediatR pipeline behavior ile tüm Create/Update/Delete komutları otomatik loglanır
- ✅ **Kullanıcı İzleme:** Hangi kullanıcının ne zaman ne yaptığını kaydeder
- ✅ **Dashboard Entegrasyonu:** API endpoint'leri üzerinden aktiviteler sorgulanabilir
- ✅ **Genişletilebilir Yapı:** Yeni entity tipleri kolayca eklenebilir
- ✅ **Süresiz Saklama:** Compliance gereksinimleri için süresiz saklanır

### Veritabanı Yapısı

**ActivityLogs Tablosu:**
```sql
CREATE TABLE "ActivityLogs" (
    "Id" SERIAL PRIMARY KEY,
    "ActivityType" VARCHAR(50) NOT NULL,
    "EntityType" VARCHAR(50) NOT NULL,
    "EntityId" INTEGER,
    "Title" VARCHAR(500) NOT NULL,
    "Details" VARCHAR(2000),
    "UserId" INTEGER,
    "Timestamp" TIMESTAMP NOT NULL DEFAULT NOW(),
    CONSTRAINT "FK_ActivityLogs_AppUsers" FOREIGN KEY ("UserId") 
        REFERENCES "AppUsers"("Id") ON DELETE SET NULL
);

CREATE INDEX "IX_ActivityLogs_Timestamp" ON "ActivityLogs"("Timestamp");
CREATE INDEX "IX_ActivityLogs_UserId" ON "ActivityLogs"("UserId");
CREATE INDEX "IX_ActivityLogs_EntityType" ON "ActivityLogs"("EntityType");
```

### Loglanan Aktivite Tipleri

| ActivityType | EntityType | Açıklama |
|-------------|------------|----------|
| `post_created` | Post | Yeni blog yazısı oluşturuldu |
| `post_updated` | Post | Blog yazısı güncellendi |
| `post_deleted` | Post | Blog yazısı silindi |
| `category_created` | Category | Yeni kategori oluşturuldu |
| `category_updated` | Category | Kategori güncellendi |
| `category_deleted` | Category | Kategori silindi |
| `comment_created` | Comment | Yeni yorum yapıldı |
| `comment_updated` | Comment | Yorum düzenlendi |
| `comment_deleted` | Comment | Yorum silindi |

## 🔧 Serilog Yapılandırması

### Log Sinks (Hedefler)

1. **Console Sink** - Development için renkli konsol çıktısı
2. **File Sink** - Günlük rolling dosyalar (31 gün saklama, 10MB limit)
3. **PostgreSQL Sink** - Structured logging (Information ve üzeri, 90 gün saklama)
4. **Seq Sink** - Profesyonel log analiz ve görselleştirme platformu

### Log Enrichers (Zenginleştirme)

- **MachineName** - Sunucu adı
- **Environment** - Development/Production
- **ProcessId** - İşlem kimliği
- **ThreadId** - Thread kimliği
- **Application** - Uygulama adı (BlogApp)
- **FromLogContext** - Request bazlı context bilgileri

## � Kullanım Örnekleri

### Otomatik Loglama (MediatR Pipeline)

Activity logging, `ActivityLoggingBehavior` MediatR pipeline behavior'u sayesinde otomatik çalışır:

```csharp
// Post oluştururken
var command = new CreatePostCommand 
{ 
    Title = "Yeni Blog Yazısı",
    Content = "İçerik...",
    CategoryId = 1
};
await _mediator.Send(command);

// ActivityLoggingBehavior otomatik olarak şunu loglar:
// ActivityType: "post_created"
// EntityType: "Post"
// Title: "Yeni Blog Yazısı oluşturuldu"
// UserId: Giriş yapmış kullanıcı ID
// Timestamp: UTC zaman
```

### Manuel Activity Loglama

Gerektiğinde repository'yi direkt kullanarak manuel log eklenebilir:

```csharp
public class CustomService
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public async Task CustomActionAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var activityLog = new ActivityLog
        {
            ActivityType = "custom_action",
            EntityType = "CustomEntity",
            Title = "Özel işlem gerçekleştirildi",
            Details = "Ek detaylar...",
            UserId = userId != null ? int.Parse(userId) : null
        };

        await _activityLogRepository.AddAsync(activityLog);
    }
}
```

### Activity Log Sorgulama

API endpoint'i üzerinden son aktiviteleri sorgulama:

```http
GET /api/dashboard/activities?pageSize=10
Authorization: Bearer {token}
```

Response:
```json
{
  "success": true,
  "data": [
    {
      "id": 42,
      "activityType": "post_created",
      "entityType": "Post",
      "title": "Yeni Blog Yazısı oluşturuldu",
      "timestamp": "2025-10-25T14:30:00Z",
      "userName": "admin@blog.com"
    }
  ]
}
```

## 📊 Structured Logging Best Practices

### Doğru Log Seviyeleri

```csharp
// Debug - Geliştirme detayları (sadece file'a yazılır)
_logger.LogDebug("Processing request for user {UserId}", userId);

// Information - Önemli olaylar (file + DB)
_logger.LogInformation("Post {PostId} created successfully by user {UserId}", 
    postId, userId);

// Warning - Potansiyel sorunlar (file + DB + uyarı)
_logger.LogWarning("Rate limit approaching for IP {IpAddress}: {RequestCount} requests", 
    ipAddress, requestCount);

// Error - Hatalar (file + DB + alert)
_logger.LogError(ex, "Failed to process payment for order {OrderId}", orderId);

// Critical - Kritik sistem hataları (file + DB + alert + on-call)
_logger.LogCritical(ex, "Database connection lost. ConnectionString: {ConnectionString}", 
    maskedConnectionString);
```

### Structured Logging Örnekleri

✅ **İYİ:**
```csharp
// Structured properties
_logger.LogInformation(
    "User {UserId} created post {PostId} in category {CategoryId}",
    userId, postId, categoryId
);

// Complex objects
_logger.LogInformation(
    "Order processed: {@Order}",
    new { OrderId = orderId, Amount = amount, Status = status }
);

// Performance-sensitive logging
if (_logger.IsEnabled(LogLevel.Debug))
{
    var result = await ExpensiveQueryAsync();
    _logger.LogDebug("Query result: {@Result}", result);
}
```

❌ **KÖTÜ:**
```csharp
// String concatenation
_logger.LogInformation("User " + userId + " created post " + postId);

// Sensitive data logging
_logger.LogInformation("User password: {Password}", password);
_logger.LogInformation("Credit card: {CreditCard}", creditCardNumber);

// Exception swallowing
try {
    await DeletePost(postId);
} catch { 
    // ❌ Hata loglanmıyor!
}

// Wrong log level
_logger.LogCritical("User clicked button"); // ❌ Kritik değil!
```

## 🔍 Seq Kullanım Örnekleri

### Seq Web Interface
- **URL:** `http://localhost:5341` (Development)
- **URL:** `http://seq:80` (Production - Docker network içinde)

### Temel Sorgular

**1. Son Hatalar:**
```
@Level = 'Error' or @Level = 'Fatal'
```

**2. Belirli Kullanıcının Logları:**
```
UserId = 5
```

**3. Yavaş Request'ler (>1000ms):**
```
ElapsedMilliseconds > 1000
```

**4. Belirli Endpoint'e Gelen İstekler:**
```
RequestPath like '%/api/post%'
```

**5. Exception İçeren Loglar:**
```
@Exception is not null
```

**6. Belirli Zaman Aralığı:**
```
@Timestamp > DateTime('2025-10-25T00:00:00Z')
```

## � PostgreSQL Log Sorguları

### Structured Logs Tablosu

**Son 24 Saatteki Hatalar:**
```sql
SELECT message, level, raise_date, exception
FROM "Logs"
WHERE level IN ('Error', 'Fatal')
  AND raise_date > NOW() - INTERVAL '24 hours'
ORDER BY raise_date DESC;
```

**En Çok Hata Veren Endpoint'ler:**
```sql
SELECT 
    properties->>'RequestPath' as endpoint,
    COUNT(*) as error_count
FROM "Logs"
WHERE level = 'Error'
  AND properties ? 'RequestPath'
  AND raise_date > NOW() - INTERVAL '7 days'
GROUP BY endpoint
ORDER BY error_count DESC
LIMIT 10;
```

**Ortalama Response Time:**
```sql
SELECT 
    DATE(raise_date) as date,
    AVG((properties->>'ElapsedMilliseconds')::numeric) as avg_response_time
FROM "Logs"
WHERE properties ? 'ElapsedMilliseconds'
  AND raise_date > NOW() - INTERVAL '30 days'
GROUP BY DATE(raise_date)
ORDER BY date;
```

### Activity Logs Tablosu

**Kullanıcının Son Aktiviteleri:**
```sql
SELECT 
    a."ActivityType",
    a."Title",
    a."Timestamp",
    u."UserName"
FROM "ActivityLogs" a
LEFT JOIN "AppUsers" u ON a."UserId" = u."Id"
WHERE a."UserId" = 5
ORDER BY a."Timestamp" DESC
LIMIT 50;
```

**Silinen Post'ların Audit Trail'i:**
```sql
SELECT 
    a."Title",
    a."Details",
    a."Timestamp",
    u."UserName",
    u."Email"
FROM "ActivityLogs" a
LEFT JOIN "AppUsers" u ON a."UserId" = u."Id"
WHERE a."ActivityType" = 'post_deleted'
  AND a."Timestamp" > NOW() - INTERVAL '30 days'
ORDER BY a."Timestamp" DESC;
```

**Günlük Aktivite İstatistikleri:**
```sql
SELECT 
    DATE(a."Timestamp") as date,
    a."ActivityType",
    COUNT(*) as activity_count
FROM "ActivityLogs" a
WHERE a."Timestamp" > NOW() - INTERVAL '7 days'
GROUP BY DATE(a."Timestamp"), a."ActivityType"
ORDER BY date DESC, activity_count DESC;
```

## ⚙️ Yapılandırma

### appsettings.json

**Development:**
```json
{
  "Serilog": {
    "SeqUrl": "http://localhost:5341",
    "SeqApiKey": null
  }
}
```

**Production:**
```json
{
  "Serilog": {
    "SeqUrl": "http://seq:80",
    "SeqApiKey": "YOUR_API_KEY_HERE"
  }
}
```

### Retention Policies

| Log Tipi | Saklama Süresi | Cleanup Mekanizması |
|----------|----------------|---------------------|
| File Logs | 31 gün | Serilog otomatik cleanup |
| Structured Logs | 90 gün | PostgreSQL scheduled job (03:00) |
| Activity Logs | Süresiz | Manuel cleanup gerektiğinde |

### Production Önerileri

1. **Seq API Key Kullanın:**
   - Seq admin panel'den API key oluşturun
   - `appsettings.Production.json` içinde tanımlayın

2. **Log Level Ayarları:**
   ```json
   {
     "Serilog": {
       "MinimumLevel": {
         "Default": "Information",
         "Override": {
           "Microsoft": "Warning",
           "System": "Warning"
         }
       }
     }
   }
   ```

3. **Sensitive Data Masking:**
   - Şifre, kredi kartı, kişisel bilgileri loglama
   - Connection string'leri mask'le
   - Email adreslerini kısalt (us***@example.com)

4. **Performance Considerations:**
   - Async logging kullan
   - Buffering ile batch write yap
   - Disk space monitoring kur

## 🚀 Kurulum ve Başlangıç

### Docker ile Çalıştırma

Tüm servisleri başlatmak için:

```bash
# Development
docker compose -f docker-compose.yml -f docker-compose.local.yml up -d

# Production
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### Migration Çalıştırma

ActivityLogs ve Logs tabloları migration'lar ile otomatik oluşturulur:

```bash
cd src/BlogApp.Persistence
dotnet ef database update --startup-project ../BlogApp.API
```

### Seq Erişimi

- **Development:** http://localhost:5341
- **Production:** Docker network içinden `http://seq:80`

## 📝 İlgili Dosyalar

### Backend Files
- `src/BlogApp.Domain/Entities/ActivityLog.cs` - Entity tanımı
- `src/BlogApp.Persistence/Configurations/ActivityLogConfiguration.cs` - EF Core konfigürasyonu
- `src/BlogApp.Persistence/Repositories/ActivityLogRepository.cs` - Repository implementasyonu
- `src/BlogApp.Application/Behaviors/ActivityLoggingBehavior.cs` - MediatR pipeline behavior
- `src/BlogApp.API/Configuration/SerilogConfiguration.cs` - Serilog yapılandırması
- `src/BlogApp.API/Middlewares/ExceptionHandlingMiddleware.cs` - Exception logging
- `src/BlogApp.API/Filters/RequestResponseLoggingFilter.cs` - Request/response logging

### Configuration Files
- `src/BlogApp.API/appsettings.Development.json` - Development ayarları
- `src/BlogApp.API/appsettings.Production.json` - Production ayarları
- `docker-compose.yml` - Docker servisleri
- `docker-compose.local.yml` - Local geliştirme ayarları
- `docker-compose.prod.yml` - Production ayarları

## 📚 Ek Kaynaklar

- [LOGGING_ARCHITECTURE.md](LOGGING_ARCHITECTURE.md) - Detaylı mimari dokümantasyonu
- [LOGGING_QUICK_REFERENCE.md](LOGGING_QUICK_REFERENCE.md) - Hızlı başvuru kılavuzu
- [LOGGING_COMPARISON.md](LOGGING_COMPARISON.md) - Loglama stratejileri karşılaştırması
- [Serilog Documentation](https://serilog.net/) - Resmi Serilog dokümantasyonu
- [Seq Documentation](https://docs.datalust.co/docs) - Resmi Seq dokümantasyonu
