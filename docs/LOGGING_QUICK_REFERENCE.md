tail -f logs/blogapp-2025-10-25.txt
grep "ERROR" logs/blogapp-*.txt
grep -c "ERROR" logs/blogapp-*.txt
# 🎯 BlogApp Logging Quick Reference

## 📊 Katmanlar

```
┌──────────────────────────────────────────────────────────────────┐
│ Tier 0: Console (Development)                                   │
│ • stdout                                                        │
│ • Level: Debug                                                  │
│ • Amaç: Lokal debug                                             │
└──────────────────────────────────────────────────────────────────┘
                              ↓
┌──────────────────────────────────────────────────────────────────┐
│ Tier 1: File Logs (logs/blogapp-YYYY-MM-DD.txt)                 │
│ • Level: Debug ve üzeri                                         │
│ • Retention: 31 gün (rolling)                                   │
│ • Amaç: Debug, hızlı analiz                                     │
└──────────────────────────────────────────────────────────────────┘
                              ↓
┌──────────────────────────────────────────────────────────────────┐
│ Tier 2: Structured Logs (PostgreSQL "Logs")                     │
│ • Level: Information ve üzeri                                   │
│ • Retention: 90 gün (LogCleanupService)                         │
│ • Amaç: Üretim izleme, uyarı, rapor                             │
└──────────────────────────────────────────────────────────────────┘
                              ↓
┌──────────────────────────────────────────────────────────────────┐
│ Tier 3: Activity Logs (PostgreSQL "ActivityLogs")               │
│ • Kaynak: Domain event → Outbox → RabbitMQ → Consumer           │
│ • Retention: Sınırsız                                           │
│ • Amaç: Audit trail, güvenlik                                   │
└──────────────────────────────────────────────────────────────────┘
```

## 🎨 Log Level Seçimi

```
Debug   → Geliştirme detayı, diagnostik
Info    → İş olayı, başarılı işlem
Warning → Potansiyel sorun, recoverable hata
Error   → Yönetilen exception, başarısız işlem
Fatal   → Sistemsel felaket (örn. DB erişilemez)
```

## 🚦 Hangi Tier Kullanılır?

| Senaryo | File | Structured | Activity |
|---------|------|------------|----------|
| Request/Response ayrıntıları | ✅ | ✅ (Info) | ❌ |
| Kullanıcı login oldu | ✅ | ✅ | ❌ |
| Post oluşturuldu | ✅ | ✅ | ✅ |
| Validation hatası | ✅ (Warn) | ✅ (Warn) | ❌ |
| Sistem hatası | ✅ (Error/Fatal) | ✅ | ❌ |
| Audit gerektiren işlem (delete vb.) | ✅ | ✅ | ✅ |

## 💻 Kod Şablonları

```csharp
// Structured bilgi logu
Log.Information("User {UserId} created post {PostId}", userId, postId);

// Exception logu
try
{
    await handler();
}
catch (Exception ex)
{
    Log.Error(ex, "Unhandled exception for request {RequestPath}", httpContext.Request.Path);
    throw;
}

// Request logging (Serilog middleware)
// Program.cs → app.UseSerilogRequestLogging(...)
```

### Kaçınılması Gerekenler
- String birleştirerek log yazma (`"User " + userId ...`)
- Şifre/token gibi hassas bilgileri loglama
- Exception’ı swallow etmek
- Yanlış seviye kullanmak (ör. buton tıklandığında Critical)

## 📈 İzleme Komutları

### File (CLI)
```bash
tail -f logs/blogapp-$(date +%Y-%m-%d).txt
grep "ERROR" logs/blogapp-*.txt
```

### PostgreSQL (`Logs`)
```sql
SELECT message, raise_date
FROM "Logs"
WHERE level IN ('Error','Fatal')
  AND raise_date > NOW() - INTERVAL '24 hours';

SELECT properties->>'RequestPath' AS path,
       AVG((properties->>'ElapsedMilliseconds')::numeric) AS avg_ms
FROM "Logs"
WHERE properties ? 'ElapsedMilliseconds'
GROUP BY path
HAVING AVG((properties->>'ElapsedMilliseconds')::numeric) > 1000;
```

### Activity Logs
```sql
SELECT "ActivityType", "EntityType", "Title", "Timestamp"
FROM "ActivityLogs"
WHERE "UserId" = :userId
ORDER BY "Timestamp" DESC;
```

## ⚙️ Konfigürasyon Özeti

- `SerilogConfiguration.ConfigureSerilog()`
  - `MinimumLevel.Debug()` + override (Microsoft/System = Warning)
  - Console, File, PostgreSQL, Seq sink’leri
- `appsettings.json`
  - `Logging.Database.RetentionDays = 90`
  - `Logging.ActivityLogs.RetentionDays = 0` (sınırsız)
- `LogCleanupService`
  - Her gece UTC 03:00 civarında `Logs` tablosunu temizler

## 🔔 Seq Örnek Sorgular

```
@Level in ['Error','Fatal']

ElapsedMilliseconds > 2000

properties['RequestPath'] like '%/api/auth%'
```

## 📚 İlgili Dosyalar
- `docs/LOGGING_ARCHITECTURE.md`
- `docs/ACTIVITY_LOGGING_README.md`
- `src/BlogApp.API/Configuration/SerilogConfiguration.cs`
- `src/BlogApp.Infrastructure/Services/LogCleanupService.cs`
- `src/BlogApp.Infrastructure/Consumers/ActivityLogConsumer.cs`
