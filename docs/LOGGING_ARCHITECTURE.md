# BlogApp Logging Architecture

Güncel loglama altyapısı; Serilog tabanlı çoklu sink, request izleme ve otomatik temizlik katmanlarıyla üretim kullanımını hedefler. Bu doküman mimariyi, varsayılan ayarları ve operasyon rehberini özetler.

## 1. Genel Bakış
- **Çekirdek kurulum:** `src/BlogApp.API/Configuration/SerilogConfiguration.cs`
- **Aktif middleware:** 
  - `app.UseSerilogRequestLogging()` (request/response süresi ve meta veri)
  - `RequestResponseLoggingFilter` (action filter - detaylı request/response logging)
  - `ExceptionHandlingMiddleware` (global exception handling ve logging)
- **Arka plan servisleri:** 
  - `LogCleanupService` (Serilog Logs tablosu temizliği)
  - `OutboxProcessorService` (Domain event işleme ve activity log)
- **Consumers:**
  - `ActivityLogConsumer` (RabbitMQ - activity log kayıtları)
  - `SendTelegramMessageConsumer` (RabbitMQ - bildirim logging)
- **MediatR Behaviors:**
  - `LoggingBehavior` (command/query logging)
- **Konfigürasyon kaynakları:** `appsettings*.json` + environment değişkenleri
- **Minimum seviyeler:**
  - Default: `Debug`
  - Microsoft: `Information`
  - Microsoft.EntityFrameworkCore & System: `Warning`

## 2. Loglama Katmanları

BlogApp'te iki ana loglama katmanı bulunmaktadır:

### 2.1. Serilog Logs (Sistem & Teknik Loglar)
**Tablo:** `Logs`  
**Amaç:** Sistem olayları, hatalar, performans metrikleri  
**Retention:** 90 gün (otomatik temizlik)  
**Sink'ler:** Console, File, PostgreSQL, Seq

**Kullanım alanları:**
- HTTP request/response logging
- Exception handling
- Background service logging
- MediatR command/query logging
- Performance monitoring

### 2.2. Activity Logs (İş Aktiviteleri & Audit Trail)
**Tablo:** `ActivityLogs`  
**Amaç:** Kullanıcı aktiviteleri, iş olayları, compliance  
**Retention:** Süresiz (0 gün = temizlenmiyor)  
**Kayıt yöntemi:** Domain Events → Outbox Pattern → RabbitMQ → ActivityLogConsumer

**Kullanım alanları:**
- Post oluşturma/güncelleme/silme
- Comment işlemleri
- Kullanıcı yetkilendirme değişiklikleri
- Audit trail gereksinimleri

> **Önemli:** Activity loglar ve Serilog loglar birbirinden bağımsızdır. Activity loglar compliance gereksinimleri nedeniyle süresiz saklanır ve asla silinmez.

## 3. Sink Ayrımı (Serilog Logs)

| Sink | Dosya/Tablo | Minimum Level | Amaç | Saklama |
|------|--------------|---------------|------|---------|
| Console | stdout | Debug | Lokal geliştirme & Docker logs | Anlık |
| File | `logs/blogapp-<date>.txt` | Debug | Geliştirme & hızlı inceleme | 31 gün (rolling) |
| PostgreSQL | `Logs` tablosu | Information | Üretim analizi, uyarılar | 90 gün (otomatik silme) |
| Seq | `Serilog:SeqUrl` | Debug | İzleme & dashboard | Harici depolama |

**Notlar:**
- File yolu proje köküne göredir; Docker konteynerinde `/app/logs`. Dosya boyutu limiti: 10 MB.
- PostgreSQL sütunları: `message`, `message_template`, `level`, `raise_date`, `exception`, `properties`, `props_test`, `machine_name`
- Seq URL: Development ortamında `http://localhost:5341`, Production'da `http://seq:80`
- Console output template: `[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}`

## 4. Request Logging

BlogApp'te 3 katmanlı request logging stratejisi uygulanmaktadır:

### 4.1. Serilog Request Logging Middleware
`Program.cs` içindeki `UseSerilogRequestLogging` her HTTP isteğini otomatik olarak loglar.

**Loglanan alanlar:**
- `RequestMethod`, `RequestPath`, `StatusCode`, `Elapsed`
- `RequestHost`, `RequestScheme`
- `RemoteIpAddress`, `UserAgent`
- `UserName` (authenticated kullanıcılar için)

**Template:**
```
HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms
```

### 4.2. RequestResponseLoggingFilter
Action filter olarak her controller metodunda detaylı logging yapar.

**Özellikler:**
- İstek başlangıcı ve bitişi
- POST/PUT istekleri için request body (Debug seviyesinde)
- Yanıt süresi (Stopwatch ile ölçüm)
- Status code'a göre dinamik log seviyesi:
  - 5xx → Error
  - 4xx → Warning
  - 2xx/3xx → Information

**Örnek log:**
```csharp
_logger.LogInformation(
    "HTTP {Method} {Path} başladı. Kullanıcı: {User}, RemoteIp: {RemoteIp}",
    request.Method,
    request.Path,
    context.HttpContext.User?.Identity?.Name ?? "Anonymous",
    context.HttpContext.Connection.RemoteIpAddress?.ToString()
);
```

### 4.3. ExceptionHandlingMiddleware
Global exception handling ve logging.

**Loglanan bilgiler:**
- Exception details
- Request path, method
- User identity
- Remote IP address

**Örnek:**
```csharp
_logger.LogError(
    ex,
    "İşlenmeyen bir hata oluştu. Yol: {Path}, Metod: {Method}, Kullanıcı: {User}, IP: {RemoteIp}",
    request.Path,
    request.Method,
    user,
    context.Connection.RemoteIpAddress?.ToString()
);
```

Bu kayıtlar tüm sink'lere (console/file/PostgreSQL/Seq) aynı anda akar ve troubleshooting'de kullanılabilir.

## 5. Otomatik Temizlik

### 5.1. Serilog Logs Temizliği
**Service:** `LogCleanupService` (BackgroundService)  
**Çalışma süresi:** Her gün UTC 03:00  
**Retention:** 90 gün (yapılandırılabilir: `Logging:Database:RetentionDays`)

**İşleyiş:**
```csharp
// Eski kayıtları sil
DELETE FROM "Logs" WHERE raise_date < (CURRENT_DATE - INTERVAL '90 days')

// Tabloyu optimize et
VACUUM ANALYZE "Logs"
```

**Loglama:**
```csharp
_logger.LogInformation(
    "Cleaned up {Count} old log entries older than {CutoffDate}",
    deletedCount,
    cutoffDate
);
```

### 5.2. Activity Logs
**Retention:** 0 gün = Süresiz (silinmiyor)  
**Konfigürasyon:** `Logging:ActivityLogs.RetentionDays = 0`  
**Sebep:** Compliance ve audit trail gereksinimleri

> **Uyarı:** Activity loglar asla otomatik olarak silinmez. Manuel temizlik gerekirse DBA ile koordinasyon yapılmalıdır.

### 5.3. File Logs
**Retention:** 31 gün  
**Yönetim:** Serilog sink tarafından otomatik (`retainedFileCountLimit = 31`)  
**Dosya boyutu limiti:** 10 MB (rolling)

**Konfigürasyon:**
```csharp
.WriteTo.File(
    path: "logs/blogapp-.txt",
    rollingInterval: RollingInterval.Day,
    retainedFileCountLimit: 31,
    fileSizeLimitBytes: 10 * 1024 * 1024
)
```

## 6. MediatR Logging (LoggingBehavior)

**Konum:** `src/BlogApp.Application/Behaviors/LoggingBehavior.cs`  
**Amaç:** Tüm CQRS command ve query'leri otomatik loglama

### Pipeline İşleyişi
Her MediatR request (command/query) için:

**Başlangıç:**
```csharp
Log.Information("{RequestType} isteği başlatılıyor", typeof(TRequest).Name);
```

**Tamamlanma:**
```csharp
Log.Information("{RequestType} isteği tamamlandı", typeof(TRequest).Name);
```

**Hata durumu:**
```csharp
Log.Error(ex, "{RequestType} isteği sırasında hata oluştu", typeof(TRequest).Name);
```

### Örnek Loglar
```
[12:34:56 INF] CreatePostCommand isteği başlatılıyor
[12:34:57 INF] CreatePostCommand isteği tamamlandı

[12:45:23 ERR] UpdateCommentCommand isteği sırasında hata oluştu
System.InvalidOperationException: Comment not found
   at BlogApp.Application.Features.Commands.UpdateComment...
```

### Avantajlar
- ✅ Merkezi loglama - her request otomatik
- ✅ Structured logging - performans analizi için uygun
- ✅ Exception tracking - hata takibi kolaylaşır
- ✅ Request tracing - hangi command/query ne kadar sürdü

## 7. Structured Logging Örnekleri

### 7.1. Doğru Kullanım ✅

```csharp
// Business olayı – bilgi seviyesi (DB + file + Seq)
Log.Information("User {UserId} created post {PostId}", userId, postId);

// Uyarı – rate limit
Log.Warning("Rate limit approaching for IP {IP}: {Count}/minute", ip, count);

// Hata – exception ile birlikte
Log.Error(exception, "Failed to send email to {Email}", email);

// Kritik – altyapı sorunu
Log.Fatal(exception, "Database connection lost");

// Performance-critical kısımlarda log level kontrolü
if (Log.IsEnabled(LogEventLevel.Debug))
{
    Log.Debug("Expensive operation result: {@Result}", ExpensiveOperation());
}
```

### 7.2. Yanlış Kullanım ❌

```csharp
// ❌ String concatenation
_logger.LogInformation("User " + userId + " logged in");

// ✅ Doğrusu
_logger.LogInformation("User {UserId} logged in", userId);

// ❌ Sensitive data
_logger.LogInformation("User {UserId} password: {Password}", userId, password);

// ✅ Doğrusu
_logger.LogInformation("User {UserId} authentication successful", userId);

// ❌ Exception swallowing
catch (Exception ex) {
    // Hiçbir şey yapma
}

// ✅ Doğrusu
catch (Exception ex) {
    _logger.LogError(ex, "Operation failed");
    throw;
}
```

### 7.3. Log Levels Kılavuzu

| Level | Kullanım | Örnek |
|-------|----------|-------|
| **Trace** | Çok detaylı debug | Method enter/exit |
| **Debug** | Geliştirme aşaması | Variable değerleri, akış kontrol |
| **Information** | Normal işlem akışı | User logged in, post created |
| **Warning** | Beklenmeyen durum (hata değil) | Rate limit warning, cache miss |
| **Error** | Hata durumu (iyileştirilebilir) | Validation failed, API call failed |
| **Fatal** | Kritik sistem hatası | Database down, unrecoverable error |

### 7.4. Önemli Notlar
- Sensitif verileri (şifre, token, kredi kartı vb.) **asla** loglamayın
- `@` operatörü ile destructuring yapın: `{@User}` yerine `{UserId}`
- Exception'ları swallow etmeyin, her zaman loglayın
- Production'da Debug logları kapalı tutun (performans)

## 8. Query Örnekleri (PostgreSQL)

### 8.1. Serilog Logs Tablosu

#### Son 24 saat hata logları
```sql
SELECT raise_date, message, exception
FROM "Logs"
WHERE level IN ('Error', 'Fatal')
  AND raise_date > NOW() - INTERVAL '24 hours'
ORDER BY raise_date DESC;
```

#### Endpoint bazlı hata sayısı
```sql
SELECT 
    properties->>'RequestPath' AS endpoint, 
    COUNT(*) as error_count
FROM "Logs"
WHERE level = 'Error'
  AND properties ? 'RequestPath'
  AND raise_date > NOW() - INTERVAL '7 days'
GROUP BY endpoint
ORDER BY error_count DESC
LIMIT 10;
```

#### Ortalama yanıt süresi (ms) – son 7 gün
```sql
SELECT 
    DATE(raise_date) AS log_day,
    AVG((properties->>'Elapsed')::numeric) AS avg_elapsed_ms,
    MAX((properties->>'Elapsed')::numeric) AS max_elapsed_ms,
    COUNT(*) as request_count
FROM "Logs"
WHERE properties ? 'Elapsed'
  AND raise_date > NOW() - INTERVAL '7 days'
GROUP BY log_day
ORDER BY log_day DESC;
```

#### En yavaş endpoint'ler (>1 saniye)
```sql
SELECT 
    properties->>'RequestPath' AS endpoint,
    properties->>'RequestMethod' AS method,
    (properties->>'Elapsed')::numeric AS elapsed_ms,
    raise_date,
    message
FROM "Logs"
WHERE properties ? 'Elapsed'
  AND (properties->>'Elapsed')::numeric > 1000
  AND raise_date > NOW() - INTERVAL '24 hours'
ORDER BY elapsed_ms DESC
LIMIT 20;
```

#### Kullanıcı bazlı hata analizi
```sql
SELECT 
    properties->>'UserName' AS username,
    COUNT(*) as error_count,
    array_agg(DISTINCT properties->>'RequestPath') as affected_endpoints
FROM "Logs"
WHERE level IN ('Error', 'Fatal')
  AND properties ? 'UserName'
  AND raise_date > NOW() - INTERVAL '24 hours'
GROUP BY username
ORDER BY error_count DESC;
```

### 8.2. Activity Logs Tablosu

#### Kullanıcı aktiviteleri (son 30 gün)
```sql
SELECT 
    al."ActivityType",
    al."EntityType",
    al."Title",
    al."Timestamp",
    u."UserName"
FROM "ActivityLogs" al
LEFT JOIN "Users" u ON al."UserId" = u."Id"
WHERE al."Timestamp" > NOW() - INTERVAL '30 days'
ORDER BY al."Timestamp" DESC
LIMIT 100;
```

#### Entity bazlı aktivite geçmişi
```sql
SELECT 
    al."ActivityType",
    al."Title",
    al."Details",
    al."Timestamp",
    u."UserName"
FROM "ActivityLogs" al
LEFT JOIN "Users" u ON al."UserId" = u."Id"
WHERE al."EntityType" = 'Post'
  AND al."EntityId" = '123e4567-e89b-12d3-a456-426614174000'
ORDER BY al."Timestamp" DESC;
```

#### Günlük aktivite özeti
```sql
SELECT 
    DATE(al."Timestamp") AS activity_date,
    al."ActivityType",
    al."EntityType",
    COUNT(*) as activity_count
FROM "ActivityLogs" al
WHERE al."Timestamp" > NOW() - INTERVAL '7 days'
GROUP BY activity_date, al."ActivityType", al."EntityType"
ORDER BY activity_date DESC, activity_count DESC;
```

## 9. Monitoring & Alerting

### 9.1. Seq Integration

Seq, development ve production ortamlarında structured log analizi için kullanılmaktadır.

#### Development Konfigürasyonu
**appsettings.Development.json:**
```json
{
  "Serilog": {
    "SeqUrl": "http://localhost:5341",
    "SeqApiKey": null
  }
}
```

#### Production Konfigürasyonu (Docker)
**docker-compose.prod.yml:**
```yaml
services:
  seq:
    image: datalust/seq:latest
    container_name: blogapp-seq
    ports:
      - "5341:80"
    environment:
      - ACCEPT_EULA=Y
    volumes:
      - seq-data:/data
    networks:
      - blogapp-network
    restart: unless-stopped

volumes:
  seq-data:

networks:
  blogapp-network:
    driver: bridge
```

### 9.2. Seq Alert Örnekleri

#### Error Spike Detection
```
@Level = 'Error' 
| group by time(5m) 
| where count() > 10
```
**Açıklama:** 5 dakika içinde 10'dan fazla error oluşursa alarm ver.

#### Slow Requests
```
Elapsed > 1000
```
**Açıklama:** 1 saniyeden uzun süren istekleri tespit et.

#### 500 Errors
```
StatusCode >= 500
```
**Açıklama:** Server error'ları anında bildir.

#### Rate Limit Warnings
```
@Message like '%Rate limit%' and @Level = 'Warning'
```
**Açıklama:** Rate limit uyarılarını takip et.

#### Database Errors
```
@Exception like '%Npgsql%' or @Exception like '%PostgreSQL%'
```
**Açıklama:** Veritabanı bağlantı sorunlarını tespit et.

### 9.3. PostgreSQL Monitoring

#### Index Performansı
```sql
-- raise_date index kullanım istatistikleri
SELECT 
    schemaname,
    tablename,
    indexname,
    idx_scan as index_scans,
    idx_tup_read as tuples_read,
    idx_tup_fetch as tuples_fetched
FROM pg_stat_user_indexes
WHERE tablename = 'Logs'
ORDER BY idx_scan DESC;
```

#### Tablo Boyutu İzleme
```sql
SELECT 
    pg_size_pretty(pg_total_relation_size('"Logs"')) as total_size,
    pg_size_pretty(pg_relation_size('"Logs"')) as table_size,
    pg_size_pretty(pg_total_relation_size('"Logs"') - pg_relation_size('"Logs"')) as indexes_size;
```

### 9.4. Alert Senaryoları

| Senaryo | Metrik | Threshold | Aksiyon |
|---------|--------|-----------|---------|
| Error Rate Artışı | Error/Fatal count | >10/5 dakika | DevOps bildirimi |
| Slow Requests | Elapsed time | >1000ms | Performance analizi |
| Rate Limit | Warning count | >50/dakika | DDoS kontrolü |
| Database Connection | Fatal + PostgreSQL | >1 | Acil müdahale |
| Disk Space | Logs table size | >10GB | Cleanup review |

## 10. Konfigürasyon Detayları

### 10.1. appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },
    "File": {
      "RetentionDays": 31
    },
    "Database": {
      "RetentionDays": 90,
      "EnableAutoCleanup": true
    },
    "ActivityLogs": {
      "RetentionDays": 0
    }
  }
}
```

### 10.2. SerilogConfiguration.cs Detayları

#### PostgreSQL Column Writers
```csharp
IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
{
    { "message", new RenderedMessageColumnWriter() },
    { "message_template", new MessageTemplateColumnWriter() },
    { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
    { "raise_date", new TimestampColumnWriter() },
    { "exception", new ExceptionColumnWriter() },
    { "properties", new LogEventSerializedColumnWriter() },
    { "props_test", new PropertiesColumnWriter() },
    { "machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.Raw) }
};
```

#### Enrichers
```csharp
.Enrich.FromLogContext()
.Enrich.WithProcessId()
.Enrich.WithThreadId()
.Enrich.WithProperty("Application", "BlogApp")
.Enrich.WithProperty("Environment", environment)
```

#### Output Templates

**Console:**
```
[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}
```

**File:**
```
{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}
```

### 10.3. Docker Volume Mapping

**docker-compose.yml:**
```yaml
services:
  blogapp-api:
    volumes:
      - ./logs:/app/logs  # File logs için
      
  seq:
    volumes:
      - seq-data:/data    # Seq data için
```

### 10.4. Environment-Specific Settings

#### Development
- Console logging: Aktif
- File logging: Aktif (`logs/blogapp-*.txt`)
- Seq URL: `http://localhost:5341`
- Min Level: Debug

#### Production
- Console logging: Aktif (Docker logs için)
- File logging: Opsiyonel
- Seq URL: `http://seq:80`
- Min Level: Information
- PostgreSQL: Zorunlu

## 11. Background Services & Consumers

### 11.1. LogCleanupService
**Konum:** `src/BlogApp.Infrastructure/Services/LogCleanupService.cs`  
**Tip:** BackgroundService (HostedService)  
**Çalışma Sıklığı:** Her gün UTC 03:00

**İşlevler:**
1. 90 günden eski Serilog loglarını sil
2. VACUUM ANALYZE ile tablo optimizasyonu
3. Cleanup istatistiklerini logla

**Kayıt:**
```csharp
// InfrastructureServicesRegistration.cs
services.AddHostedService<LogCleanupService>();
```

### 11.2. OutboxProcessorService
**Konum:** `src/BlogApp.Infrastructure/Services/BackgroundServices/OutboxProcessorService.cs`  
**Tip:** BackgroundService  
**Amaç:** Domain events'leri işle ve RabbitMQ'ya gönder

**İşleyiş:**
1. Outbox tablosundan bekleyen event'leri oku
2. ActivityLog event'lerini `ActivityLogIntegrationEvent`'e dönüştür
3. RabbitMQ'ya publish et
4. Başarılı event'leri Outbox'tan sil

**Logging:**
```csharp
_logger.LogInformation(
    "Processing {Count} outbox messages",
    pendingMessages.Count
);
```

### 11.3. ActivityLogConsumer
**Konum:** `src/BlogApp.Infrastructure/Consumers/ActivityLogConsumer.cs`  
**Tip:** MassTransit Consumer  
**Queue:** `activity-log-queue`

**İşlevler:**
1. RabbitMQ'dan ActivityLogIntegrationEvent al
2. ActivityLog entity'sine dönüştür
3. Veritabanına kaydet

**Retry Konfigürasyonu:**
```csharp
endpointConfigurator.UseMessageRetry(retryConfigurator =>
    retryConfigurator.Exponential(5,
        TimeSpan.FromSeconds(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromSeconds(2)));
```

**Logging:**
```csharp
_logger.LogInformation(
    "Processing ActivityLog: {ActivityType} for {EntityType} (ID: {EntityId})",
    message.ActivityType,
    message.EntityType,
    message.EntityId
);
```

### 11.4. SendTelegramMessageConsumer
**Konum:** `src/BlogApp.Infrastructure/Consumers/SendTelegramMessageConsumer.cs`  
**Queue:** `send-telegram-text-message-queue`

**Logging:**
- Telegram mesaj gönderimi başarı/hata durumları
- Rate limit ve API error'ları
## 12. En İyi Uygulamalar

### 12.1. Log Seviyeleri
- ✅ **Production'da Information+** kullanın (Debug kapalı)
- ✅ **Development'ta Debug** aktif olabilir
- ❌ Trace seviyesi sadece özel debug durumlarında
- ❌ Her yerde Fatal kullanmayın (sadece kritik sistem hataları)

### 12.2. Structured Logging
- ✅ Placeholder kullanın: `{UserId}`, `{PostId}`
- ❌ String concatenation: `"User " + userId`
- ✅ Exception'ları ilk parametre olarak geçin
- ✅ Destructuring için `@` kullanın: `{@Order}` (dikkatli kullanın)

### 12.3. Sensitive Data
**Asla loglamayın:**
- Şifreler (plain veya hashed)
- API keys, tokens
- Kredi kartı bilgileri
- Kişisel veriler (GDPR/KVKK)

**Loglayabilirsiniz:**
- User ID'ler
- Entity ID'ler
- Timestamp'ler
- Request path'ler

### 12.4. Performance
```csharp
// ❌ Kötü - her zaman serialize eder
_logger.LogDebug("Complex object: {@ComplexObject}", expensiveObject);

// ✅ İyi - sadece debug aktifse serialize eder
if (_logger.IsEnabled(LogLevel.Debug))
{
    _logger.LogDebug("Complex object: {@ComplexObject}", expensiveObject);
}
```

### 12.5. Exception Handling
```csharp
// ❌ Kötü - exception swallow
try {
    await operation();
} catch { }

// ✅ İyi - logla ve tekrar fırlat
try {
    await operation();
} 
catch (Exception ex) {
    _logger.LogError(ex, "Operation failed for {UserId}", userId);
    throw;
}

// ✅ İyi - logla ve custom exception fırlat
catch (Exception ex) {
    _logger.LogError(ex, "Operation failed for {UserId}", userId);
    throw new BusinessException("Operation failed", ex);
}
```

### 12.6. Request/Response Logging
- ✅ Request body'yi Debug seviyesinde logla
- ❌ Response body'yi loglama (performans)
- ✅ Status code ve elapsed time her zaman logla
- ❌ Binary data (dosyalar) loglama

### 12.7. Database Logging
- ✅ Sadece Information+ seviyeyi DB'ye yaz
- ❌ Debug logları DB'ye yazma
- ✅ Retention policy uygula (90 gün)
- ✅ Regular VACUUM/ANALYZE yap

### 12.8. Activity Logging
- ✅ Business event'leri Activity log'a
- ❌ Technical log'ları Activity log'a
- ✅ Outbox Pattern kullan (reliability)
- ✅ Süresiz saklama (audit trail)

### 12.9. Monitoring
- ✅ Seq dashboard'ları düzenli kontrol et
- ✅ Alert'leri konfigüre et
- ✅ Error spike'ları analiz et
- ✅ Performance trend'lerini takip et

### 12.10. Cleanup & Maintenance
- ✅ LogCleanupService çalıştığını doğrula
- ✅ Disk space'i düzenli kontrol et
- ✅ Log retention policy'yi revizyona al
- ✅ Archive stratejisi belirle (compliance)

## 13. Troubleshooting

### 13.1. Loglar Görünmüyor

**Seq'te log göremiyorum:**
```bash
# Seq servisini kontrol et
docker ps | grep seq

# Seq URL'i kontrol et
# appsettings.Development.json
"Serilog": { "SeqUrl": "http://localhost:5341" }

# Container log'larını kontrol et
docker logs blogapp-seq
```

**PostgreSQL'de log yok:**
```sql
-- Logs tablosu var mı?
SELECT * FROM information_schema.tables WHERE table_name = 'Logs';

-- Son kayıtlar
SELECT * FROM "Logs" ORDER BY raise_date DESC LIMIT 10;
```

**File log oluşmuyor:**
```bash
# logs klasörü var mı?
ls -la logs/

# Yazma izni var mı?
chmod 755 logs/

# Docker volume mapping kontrolü
docker-compose config | grep volumes -A 5
```

### 13.2. Performance Sorunları

**Logs tablosu çok büyüdü:**
```sql
-- Tablo boyutunu kontrol et
SELECT pg_size_pretty(pg_total_relation_size('"Logs"'));

-- Manuel cleanup
DELETE FROM "Logs" WHERE raise_date < NOW() - INTERVAL '30 days';
VACUUM ANALYZE "Logs";
```

**Log yazımı yavaşladı:**
```csharp
// PostgreSQL minimum level'ı yükselt
.WriteTo.PostgreSQL(
    restrictedToMinimumLevel: LogEventLevel.Warning  // Information yerine
)
```

### 13.3. LogCleanupService Çalışmıyor

**Kontrol:**
```bash
# Service log'larını kontrol et
docker logs blogapp-api | grep LogCleanupService

# Beklenen output:
# LogCleanupService started. Retention: 90 days
# Next log cleanup scheduled for: 2025-10-29T03:00:00Z
```

**Konfigürasyon:**
```json
{
  "Logging": {
    "Database": {
      "RetentionDays": 90,
      "EnableAutoCleanup": true  // Bu mutlaka true olmalı
    }
  }
}
```

### 13.4. ActivityLog Kaydedilmiyor

**Outbox Pattern kontrolü:**
```sql
-- Outbox'ta bekleyen mesajlar var mı?
SELECT * FROM "OutboxMessages" WHERE "ProcessedOnUtc" IS NULL;

-- ActivityLogs tablosu
SELECT * FROM "ActivityLogs" ORDER BY "Timestamp" DESC LIMIT 10;
```

**RabbitMQ kontrolü:**
```bash
# RabbitMQ management UI
http://localhost:15672

# Queue'da mesaj var mı?
# activity-log-queue → Messages
```

**Consumer log'ları:**
```bash
docker logs blogapp-api | grep ActivityLogConsumer
```

### 13.5. Seq Dashboard Boş

**Filtre kontrolü:**
```
# Tüm loglar
@Level is not null

# Son 1 saat
@Timestamp > Now() - 1h

# Belirli request path
RequestPath like '/api%'
```

---

## 14. İlgili Dosyalar

### 14.1. Kod Dosyaları

| Dosya | Açıklama |
|-------|----------|
| `src/BlogApp.API/Configuration/SerilogConfiguration.cs` | Serilog yapılandırması (sinks, enrichers) |
| `src/BlogApp.API/Program.cs` | Middleware registration ve request logging |
| `src/BlogApp.API/Filters/RequestResponseLoggingFilter.cs` | HTTP request/response logging filter |
| `src/BlogApp.API/Middlewares/ExceptionHandlingMiddleware.cs` | Global exception handling ve logging |
| `src/BlogApp.Application/Behaviors/LoggingBehavior.cs` | MediatR pipeline - CQRS logging |
| `src/BlogApp.Infrastructure/Services/LogCleanupService.cs` | Otomatik log cleanup service |
| `src/BlogApp.Infrastructure/Services/BackgroundServices/OutboxProcessorService.cs` | Outbox pattern processor |
| `src/BlogApp.Infrastructure/Consumers/ActivityLogConsumer.cs` | RabbitMQ activity log consumer |
| `src/BlogApp.Persistence/DatabaseInitializer/DbInitializer.cs` | Serilog Logs tablosu oluşturma |

### 14.2. Konfigürasyon Dosyaları

| Dosya | Açıklama |
|-------|----------|
| `src/BlogApp.API/appsettings.json` | Base configuration (retention policy) |
| `src/BlogApp.API/appsettings.Development.json` | Development settings (Seq URL, log levels) |
| `src/BlogApp.API/appsettings.Production.json` | Production settings |
| `docker-compose.yml` | Docker volume mapping |
| `docker-compose.prod.yml` | Production Seq container |

### 14.3. Dokümantasyon

| Dosya | Açıklama |
|-------|----------|
| `docs/LOGGING_ARCHITECTURE.md` | Bu dosya - mimari ve konfigürasyon |
| `docs/LOGGING_QUICK_REFERENCE.md` | Hızlı referans ve kod şablonları |
| `docs/ACTIVITY_LOGGING_README.md` | Activity logging detayları |
| `docs/ERROR_HANDLING_GUIDE.md` | Exception handling stratejileri |
| `docs/LOGGING_COMPARISON.md` | Single vs multi-tier logging |

---

## 15. Kaynaklar ve Referanslar

### 15.1. Serilog
- [Serilog GitHub](https://github.com/serilog/serilog)
- [Serilog Best Practices](https://github.com/serilog/serilog/wiki/Best-Practices)
- [Serilog Enrichers](https://github.com/serilog/serilog/wiki/Enrichment)
- [PostgreSQL Sink](https://github.com/serilog-contrib/serilog-sinks-postgresql)
- [Seq Sink](https://github.com/serilog/serilog-sinks-seq)

### 15.2. Structured Logging
- [Structured Logging Nedir?](https://stackify.com/what-is-structured-logging-and-why-developers-need-it/)
- [Microsoft Logging Guide](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging)
- [Semantic Logging](https://docs.microsoft.com/en-us/previous-versions/msp-n-p/dn440729(v=pandp.60))

### 15.3. Seq
- [Seq Documentation](https://docs.datalust.co/docs)
- [Seq Query Language](https://docs.datalust.co/docs/the-seq-query-language)
- [Seq Alerts](https://docs.datalust.co/docs/alerts-and-notifications)

### 15.4. Compliance & Security
- [GDPR Logging Guidelines](https://ico.org.uk/for-organisations/guide-to-data-protection/)
- [OWASP Logging Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Logging_Cheat_Sheet.html)
- [PCI DSS Logging Requirements](https://www.pcisecuritystandards.org/)

---

## 16. Özet ve Sonuç

### ✅ Mevcut Yapının Güçlü Yönleri

1. **Çoklu Sink Stratejisi**
   - Console → Development debugging
   - File → Quick inspection
   - PostgreSQL → Production analysis
   - Seq → Advanced monitoring

2. **İki Katmanlı Loglama**
   - Serilog Logs → Teknik loglar (90 gün)
   - Activity Logs → İş logları (süresiz)
   - Her biri kendi amacına uygun optimize edilmiş

3. **Kapsamlı Request Tracking**
   - Serilog middleware → Otomatik HTTP logging
   - RequestResponseLoggingFilter → Detaylı action logging
   - ExceptionHandlingMiddleware → Global error handling

4. **Otomatik Bakım**
   - LogCleanupService → Daily cleanup
   - VACUUM ANALYZE → Performance optimization
   - Configurable retention policies

5. **Asenkron İş Loglama**
   - Outbox Pattern → Reliable event processing
   - RabbitMQ → Decoupled consumers
   - Retry mechanisms → Fault tolerance

### 🎯 Best Practice Uyumluluğu

- ✅ Structured logging (placeholder kullanımı)
- ✅ Log seviyesi ayrımı (Debug/Info/Warning/Error/Fatal)
- ✅ Exception tracking ve stack trace
- ✅ Performance metrics (elapsed time)
- ✅ Security (sensitive data masking)
- ✅ Compliance (audit trail - activity logs)
- ✅ Monitoring (Seq integration)
- ✅ Maintenance (automatic cleanup)

### 💡 Sonuç

**BlogApp loglama mimarisi production-ready ve best practice'lere uygundur!**

Sistem, debugging, monitoring, performance analysis ve compliance gereksinimlerini başarıyla karşılamaktadır. İki katmanlı yapı (Serilog + Activity Logs), her log türünün kendi gereksinimlerine göre optimize edilmesini sağlamaktadır.
