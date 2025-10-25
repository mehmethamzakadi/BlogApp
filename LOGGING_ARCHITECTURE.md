# BlogApp - Logging Architecture & Best Practices

## 📊 Mevcut Loglama Mimarisi

BlogApp'te **3 katmanlı loglama stratejisi** uygulanmıştır. Bu yaklaşım industry best practice'lerine uygundur ve her log türünün farklı amaçlara hizmet etmesini sağlar.

---

## 🏗️ Loglama Katmanları

### 1️⃣ **Application Logs (File-based)**

**Lokasyon:** `C:\Users\PC\Desktop\Calismalarim\BlogApp\src\BlogApp.API\logs\blogapp-YYYY-MM-DD.txt`

**Yapılandırma:**
```csharp
// SerilogConfiguration.cs
.WriteTo.File(
    path: "logs/blogapp-.txt",
    rollingInterval: RollingInterval.Day,
    retainedFileCountLimit: 31,
    fileSizeLimitBytes: 10 * 1024 * 1024 // 10 MB
)
```

**Özellikler:**
- ✅ Günlük dosyalar (rolling daily)
- ✅ 31 gün saklama süresi
- ✅ Tüm log seviyelerini içerir (Debug, Information, Warning, Error, Critical)
- ✅ Stack trace'ler ve exception detayları
- ✅ Request/Response detayları

**Kullanım Senaryoları:**
- 🔍 Development ortamında debugging
- 🐛 Production'da hata analizi (stack trace review)
- ⚡ Hızlı log tarama (grep/tail kullanımı)
- 📊 Geçici sorun giderme

**Avantajlar:**
- Hızlı yazma (disk I/O)
- Veritabanını şişirmiyor
- Offline erişilebilir
- Grep, tail, less gibi CLI araçlarıyla kolay analiz

**Dezavantajlar:**
- Structured query yapılamaz
- Aggregate/istatistik çıkarma zor
- Dosya boyutu sınırı var

---

### 2️⃣ **Structured Logs (Database - PostgreSQL)**

**Lokasyon:** PostgreSQL `Logs` tablosu (Serilog tarafından otomatik oluşturulur)

**Yapılandırma:**
```csharp
// SerilogConfiguration.cs
.WriteTo.PostgreSQL(
    connectionString: connectionString,
    tableName: "Logs",
    columnOptions: columnWriters,
    needAutoCreateTable: true,
    restrictedToMinimumLevel: LogEventLevel.Information  // ⚠️ Önemli!
)
```

**Tablo Yapısı:**
```sql
CREATE TABLE "Logs" (
    message TEXT,
    message_template TEXT,
    level VARCHAR,
    raise_date TIMESTAMP,
    exception TEXT,
    properties JSONB,
    machine_name VARCHAR
);
```

**Özellikler:**
- ✅ Sadece **Information ve üzeri** loglar (Warning, Error, Critical)
- ✅ Structured data (JSON properties)
- ✅ SQL ile sorgulanabilir
- ✅ 90 gün saklama (otomatik cleanup)
- ✅ Aggregate ve analytics

**Neden Sadece Information+?**
```
Debug logları DB'yi şişirir ve performans düşürür.
File logs debug için yeterlidir.
Production'da Information seviyesi yeterli bilgi sağlar.
```

**Kullanım Senaryoları:**
- 📈 Production monitoring
- 🔔 Alert/notification sistemleri
- 📊 Log aggregation (error patterns)
- 🎯 Metrics extraction (response times, error rates)
- 🔎 Complex query'ler ("Son 24 saatte kaç 500 hatası aldık?")

**Örnek Sorgular:**
```sql
-- Son 24 saatteki hatalar
SELECT message, level, raise_date, exception
FROM "Logs"
WHERE level IN ('Error', 'Fatal')
  AND raise_date > NOW() - INTERVAL '24 hours'
ORDER BY raise_date DESC;

-- En çok hata veren endpoint'ler
SELECT properties->>'RequestPath' as endpoint, COUNT(*) as error_count
FROM "Logs"
WHERE level = 'Error'
  AND properties ? 'RequestPath'
  AND raise_date > NOW() - INTERVAL '7 days'
GROUP BY endpoint
ORDER BY error_count DESC
LIMIT 10;

-- Response time ortalaması
SELECT 
    DATE(raise_date) as date,
    AVG((properties->>'ElapsedMilliseconds')::numeric) as avg_response_time
FROM "Logs"
WHERE properties ? 'ElapsedMilliseconds'
  AND raise_date > NOW() - INTERVAL '30 days'
GROUP BY DATE(raise_date)
ORDER BY date;
```

**Avantajlar:**
- Structured query desteği
- Aggregate/analytics yapılabilir
- Centralized logging
- Alert/monitoring entegrasyonu kolay

**Dezavantajlar:**
- Disk space kullanımı (cleanup gerekli)
- Write performance overhead
- Backup'a dahil

---

### 3️⃣ **Activity Logs (Business Audit Trail)**

**Lokasyon:** PostgreSQL `ActivityLogs` tablosu

**Yapılandırma:**
```csharp
// ActivityLoggingBehavior.cs - MediatR Pipeline
public class ActivityLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    // Otomatik loglama
}
```

**Tablo Yapısı:**
```sql
CREATE TABLE "ActivityLogs" (
    "Id" INTEGER PRIMARY KEY,
    "ActivityType" VARCHAR(50),
    "EntityType" VARCHAR(50),
    "EntityId" INTEGER,
    "Title" VARCHAR(500),
    "Details" VARCHAR(2000),
    "UserId" INTEGER,
    "Timestamp" TIMESTAMP,
    FOREIGN KEY ("UserId") REFERENCES "AppUsers"("Id")
);
```

**Özellikler:**
- ✅ Business-critical events only
- ✅ User action tracking
- ✅ **Süresiz saklama** (compliance için)
- ✅ Otomatik loglama (MediatR pipeline)
- ✅ Dashboard entegrasyonu

**Loglanan Aktiviteler:**
- ✏️ Post created/updated/deleted
- 🏷️ Category created/updated/deleted
- 💬 Comment created/updated/deleted
- 👤 User login/logout
- 🔐 Password change
- ⚙️ System configuration changes

**Kullanım Senaryoları:**
- 📋 Compliance/audit trail (GDPR, SOC2, ISO 27001)
- 🔒 Security investigations ("Kim bu veriyi sildi?")
- 📊 User behavior analytics
- ⚖️ Legal/dispute resolution
- 📈 Business intelligence

**Örnek Sorgular:**
```sql
-- Belirli bir kullanıcının tüm aktiviteleri
SELECT a.*, u."UserName"
FROM "ActivityLogs" a
JOIN "AppUsers" u ON a."UserId" = u."Id"
WHERE a."UserId" = 5
ORDER BY a."Timestamp" DESC;

-- Son 10 aktivite
SELECT 
    a."ActivityType",
    a."Title",
    a."Timestamp",
    u."UserName"
FROM "ActivityLogs" a
LEFT JOIN "AppUsers" u ON a."UserId" = u."Id"
ORDER BY a."Timestamp" DESC
LIMIT 10;

-- Silinmiş post'ları kim sildi?
SELECT 
    a."Title",
    a."Timestamp",
    u."UserName",
    a."Details"
FROM "ActivityLogs" a
LEFT JOIN "AppUsers" u ON a."UserId" = u."Id"
WHERE a."ActivityType" = 'post_deleted'
ORDER BY a."Timestamp" DESC;
```

**Avantajlar:**
- Compliance requirements karşılar
- Security audit trail
- Business insights
- User accountability
- Dispute resolution

**Dezavantajlar:**
- Süresiz saklama (disk space)
- Privacy considerations (GDPR - right to be forgotten)

---

## 🎯 3 Katmanlı Strateji Neden Best Practice?

### **Separation of Concerns**

```
┌─────────────────────────────────────────────────────────────┐
│  Application Logs (File)                                    │
│  • Development & debugging                                  │
│  • Technical details                                        │
│  • Short retention (31 days)                                │
│  • High volume (Debug+)                                     │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  Structured Logs (Database)                                 │
│  • Production monitoring                                    │
│  • Operational insights                                     │
│  • Medium retention (90 days)                               │
│  • Medium volume (Info+)                                    │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  Activity Logs (Database)                                   │
│  • Business audit trail                                     │
│  • Compliance & legal                                       │
│  • Unlimited retention                                      │
│  • Low volume (Critical events only)                        │
└─────────────────────────────────────────────────────────────┘
```

### **Performans Optimizasyonu**

```csharp
// ❌ Kötü: Her log DB'ye yazılırsa
[RequestResponseLoggingFilter]
public async Task OnActionExecutionAsync()
{
    _logger.LogDebug("Request started..."); // DB'ye yazar ❌
    // Her request için DB write = Yavaş!
}

// ✅ İyi: Log level separation
restrictedToMinimumLevel: LogEventLevel.Information
// Debug logları sadece file'a yazar ✅
// DB sadece kritik logları alır ✅
```

### **Maliyet Optimizasyonu**

| Log Type | Storage | Retention | Cost/Month |
|----------|---------|-----------|------------|
| File | Disk | 31 days | ~ $0.02/GB |
| DB (Info+) | PostgreSQL | 90 days | ~ $0.10/GB |
| Activity | PostgreSQL | Unlimited | ~ $0.10/GB (but low volume) |

**Total:** ~$5-10/month (medium traffic blog app)

---

## 🔧 Otomatik Log Cleanup

**Service:** `LogCleanupService.cs`

```csharp
// Her gün saat 03:00'da çalışır
// 90 günden eski "Logs" kayıtlarını siler
// ActivityLogs'a dokunmaz (süresiz)

protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        await CleanupOldLogsAsync(stoppingToken);
        
        // Bir sonraki gün 03:00'a kadar bekle
        var next3AM = DateTime.UtcNow.Date.AddDays(1).AddHours(3);
        await Task.Delay(next3AM - DateTime.UtcNow, stoppingToken);
    }
}
```

**Konfigürasyon:**
```json
// appsettings.json
{
  "Logging": {
    "Database": {
      "RetentionDays": 90,
      "EnableAutoCleanup": true
    },
    "ActivityLogs": {
      "RetentionDays": 0  // 0 = süresiz
    }
  }
}
```

---

## 📊 Log Levels Stratejisi

### **Log Level Kullanımı**

```csharp
// 🔍 Debug - Development only, verbose details
_logger.LogDebug("User {UserId} attempting to login with email {Email}", userId, email);

// ℹ️ Information - Important business events
_logger.LogInformation("User {UserId} logged in successfully", userId);

// ⚠️ Warning - Potential issues, recoverable errors
_logger.LogWarning("Rate limit approaching for IP {IP}: {RequestCount}/60", ip, count);

// ❌ Error - Handled exceptions, business logic errors
_logger.LogError(ex, "Failed to send email to {Email}", email);

// 🔥 Critical - System failures, data loss
_logger.LogCritical(ex, "Database connection lost. Application shutting down.");
```

### **Environment-based Configuration**

**Development:**
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information",
        "System": "Information"
      }
    }
  }
}
```

**Production:**
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    }
  }
}
```

---

## 🎯 Best Practices Summary

### ✅ **DO (Yapılması Gerekenler)**

1. **Log levels'ı doğru kullan**
   ```csharp
   _logger.LogInformation("Order {OrderId} created", orderId);  // ✅
   _logger.LogDebug("Processing order details...");             // ✅
   ```

2. **Structured logging kullan**
   ```csharp
   _logger.LogError(ex, "Payment failed for order {OrderId}, amount {Amount}", orderId, amount);  // ✅
   ```

3. **Sensitive data loglama**
   ```csharp
   // ❌ Kötü
   _logger.LogInformation("User logged in with password: {Password}", password);
   
   // ✅ İyi
   _logger.LogInformation("User {UserId} logged in successfully", userId);
   ```

4. **Log retention policy uygula**
   - File logs: 7-31 gün
   - Structured logs: 30-90 gün
   - Activity logs: Süresiz (compliance)

5. **Performance-critical kısımlarda log level kontrol et**
   ```csharp
   if (_logger.IsEnabled(LogLevel.Debug))
   {
       _logger.LogDebug("Expensive operation result: {@Result}", ExpensiveOperation());
   }
   ```

### ❌ **DON'T (Yapılmaması Gerekenler)**

1. **String concatenation kullanma**
   ```csharp
   _logger.LogInformation("User " + userId + " logged in");  // ❌
   _logger.LogInformation("User {UserId} logged in", userId);  // ✅
   ```

2. **Exception'ları yutma**
   ```csharp
   try {
       // ...
   } catch (Exception ex) {
       // ❌ Hiçbir şey yapma
   }
   
   // ✅
   catch (Exception ex) {
       _logger.LogError(ex, "Operation failed");
       throw;
   }
   ```

3. **Her log'u DB'ye yazma**
   ```csharp
   // ❌ Performans sorunu
   .WriteTo.PostgreSQL(..., restrictedToMinimumLevel: LogEventLevel.Debug)
   
   // ✅ Sadece önemli loglar
   .WriteTo.PostgreSQL(..., restrictedToMinimumLevel: LogEventLevel.Information)
   ```

4. **Business logic'i log'larla karıştırma**
   ```csharp
   // ❌ Activity log için Serilog kullanma
   _logger.LogInformation("User created post");
   
   // ✅ Activity log için ayrı tablo
   await _activityLogRepository.AddAsync(new ActivityLog { ... });
   ```

---

## 🔍 Monitoring & Alerting

### **Seq Integration**

Seq zaten yapılandırılmış. Production'da da kullanın:

**docker-compose.prod.yml:**
```yaml
services:
  seq:
    image: datalust/seq:latest
    ports:
      - "5341:80"
    environment:
      - ACCEPT_EULA=Y
    volumes:
      - seq-data:/data
```

**Alert Örnekleri:**
```
# Seq Alert: Error spike detection
@Level = 'Error' 
| group by time(5m) 
| where count() > 10

# Seq Alert: Slow requests
ElapsedMilliseconds > 5000

# Seq Alert: 500 errors
StatusCode >= 500
```

---

## 📚 İlgili Dosyalar

| Dosya | Açıklama |
|-------|----------|
| `SerilogConfiguration.cs` | Serilog yapılandırması |
| `RequestResponseLoggingFilter.cs` | HTTP request/response logging |
| `ExceptionHandlingMiddleware.cs` | Global exception logging |
| `ActivityLoggingBehavior.cs` | MediatR pipeline - activity logging |
| `LogCleanupService.cs` | Otomatik log cleanup |
| `appsettings.json` | Log retention policy |

---

## 🎓 Referanslar

- [Serilog Best Practices](https://github.com/serilog/serilog/wiki/Best-Practices)
- [Structured Logging](https://stackify.com/what-is-structured-logging-and-why-developers-need-it/)
- [Logging Levels Guide](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging)
- [GDPR Logging Compliance](https://ico.org.uk/for-organisations/guide-to-data-protection/guide-to-the-general-data-protection-regulation-gdpr/principles/storage-limitation/)

---

## 💡 Sonuç

**Mevcut loglama yapınız DOĞRU ve best practice'lere uygundur!**

- ✅ File logs → Development & debugging
- ✅ Structured logs (DB) → Production monitoring
- ✅ Activity logs → Compliance & audit

**Bu ayrım şunları sağlar:**
- 🚀 Performans (DB'ye her şey yazılmaz)
- 💰 Maliyet optimizasyonu (disk space)
- 📊 Doğru tool, doğru iş (file vs DB vs audit)
- ⚖️ Compliance (audit trail korunur)
- 🔍 Debugging kolaylığı (file logs)

**Tek merkezi yapı yerine bu ayrımı kullanmak daha iyi çünkü:**
- Her log türünün farklı retention policy'si var
- Performans gereksinimleri farklı
- Query pattern'leri farklı
- Compliance gereksinimleri farklı
