# BlogApp Logging Architecture

Güncel loglama altyapısı; Serilog tabanlı çoklu sink, request izleme ve otomatik temizlik katmanlarıyla üretim kullanımını hedefler. Bu doküman mimariyi, varsayılan ayarları ve operasyon rehberini özetler.

## 1. Genel Bakış
- **Çekirdek kurulum:** `src/BlogApp.API/Configuration/SerilogConfiguration.cs`
- **Aktif middleware:** `app.UseSerilogRequestLogging()` (request/response süresi ve meta veri)
- **Arka plan servisleri:** `LogCleanupService` (DB log temizliği)
- **Konfigürasyon kaynakları:** `appsettings*.json` + environment değişkenleri
- **Minimum seviyeler:**
  - Default: `Debug`
  - Microsoft: `Information`
  - EF Core & System: `Warning`

## 2. Sink Ayrımı

| Sink | Dosya/Tablo | Minimum Level | Amaç | Saklama |
|------|--------------|---------------|------|---------|
| Console | stdout | Debug | Lokal geliştirme | Anlık |
| File | `logs/blogapp-<date>.txt` | Debug | Geliştirme & hızlı inceleme | 31 gün (rolling) |
| PostgreSQL | `Logs` tablosu | Information | Üretim analizi, uyarılar | 90 gün (otomatik silme) |
| Seq | `Serilog:SeqUrl` | Debug | İzleme & dashboard | Harici depolama |

**Notlar**
- File yolu proje köküne göredir; Docker konteynerinde `/app/logs`. Dosya boyutu limiti: 10 MB.
- PostgreSQL sütunları `message`, `level`, `raise_date`, `exception`, `properties`, `machine_name` vb. olarak oluşturulur (Serilog auto-create).
- Seq URL ve API anahtarı `appsettings.{Environment}.json` üzerinden konfigüre edilir (development: `http://localhost:5341`).

## 3. Request Logging
`Program.cs` içindeki `UseSerilogRequestLogging` aşağıdaki alanları log property’lerine ekler:
- `RequestMethod`, `RequestPath`, `StatusCode`, `Elapsed`
- `RequestHost`, `RequestScheme`, `RemoteIpAddress`, `UserAgent`
- Authenticated kullanıcı varsa `UserName`

Bu kayıtlar console/file/PostgreSQL/Seq sink’lerine aynı anda akar ve troubleshooting’de kullanılabilir.

## 4. Otomatik Temizlik
- `LogCleanupService` günlük olarak UTC 03:00 civarında `Logging:Database:RetentionDays` süresini aşmış `Logs` kayıtlarını siler (varsayılan 90 gün).
- Temizlikten sonra `VACUUM ANALYZE "Logs"` çalıştırılarak tablo optimize edilir.
- File loglarının saklama süresi Serilog sink tarafından (`retainedFileCountLimit = 31`) yönetilir.
- Activity logları (audit) **silinmez**; süre `Logging:ActivityLogs.RetentionDays = 0` (süresiz) olarak tutulur. Ayrıntılar için `docs/ACTIVITY_LOGGING_README.md`.

## 5. Structured Logging Örnekleri

```csharp
// Business olayı – bilgi seviyesi (DB + file + Seq)
Log.Information("User {UserId} created post {PostId}", userId, postId);

// Uyarı – rate limit
Log.Warning("Rate limit approaching for IP {IP}: {Count}/minute", ip, count);

// Hata – exception ile birlikte
Log.Error(exception, "Failed to send email to {Email}", email);

// Kritik – altyapı sorunu
Log.Fatal(exception, "Database connection lost");
```

- Sensitif değerleri (şifre, token vb.) loglamaktan kaçının.
- `IsEnabled` kontrolüyle pahalı nesnelerin debug’da serialize edilmesini yönetin.

## 6. Query Örnekleri (PostgreSQL `Logs`)

```sql
-- Son 24 saat hata logları
SELECT raise_date, message, exception
FROM "Logs"
WHERE level IN ('Error', 'Fatal')
  AND raise_date > NOW() - INTERVAL '24 hours'
ORDER BY raise_date DESC;

-- Endpoint bazlı hata sayısı
SELECT properties->>'RequestPath' AS endpoint, COUNT(*)
FROM "Logs"
WHERE level = 'Error'
  AND properties ? 'RequestPath'
GROUP BY endpoint
ORDER BY COUNT(*) DESC
LIMIT 10;

-- Ortalama yanıt süresi (ms) – son 7 gün
SELECT DATE(raise_date) AS log_day,
       AVG((properties->>'ElapsedMilliseconds')::numeric) AS avg_elapsed
FROM "Logs"
WHERE properties ? 'ElapsedMilliseconds'
  AND raise_date > NOW() - INTERVAL '7 days'
GROUP BY log_day
ORDER BY log_day;
```

## 7. Monitoring & Alerting
- Seq üzerinde saved query ve dashboard’lar oluşturun (`@Level in ['Error','Fatal']`).
- PostgreSQL üzerinde `raise_date` indeksleri (Serilog otomatik oluşturur) query performansını destekler.
- Üretimde alarm senaryoları:
  - Error/Fatal oranı artışı
  - Uzun süren request’ler (`Elapsed > 1000` ms)
  - Rate limit uyarıları

## 8. İlgili Kod & Konfigürasyon
- `src/BlogApp.API/Configuration/SerilogConfiguration.cs`
- `src/BlogApp.API/Program.cs` (middleware & request logging)
- `src/BlogApp.Infrastructure/Services/LogCleanupService.cs`
- `src/BlogApp.API/appsettings*.json`
- Docker: log klasörü volume olarak tanımlanmalıdır (örn. `- ./logs:/app/logs`).

## 9. En İyi Uygulamalar
- Log seviyelerini doğru seçin (Debug sadece geliştirme; Information+ prod).
- Exception’ları swallow etmeyin; `Log.Error` sonrasında tekrar fırlatın veya uygun yanıt üretin.
- Structured property isimlerini tutarlı kullanın (`RequestPath`, `ElapsedMilliseconds`).
- Kullanıcıya ait PII verilerini maskelayın veya loglamaktan kaçının.
- Uzun vadede `Logging.Database.RetentionDays` değerini trafik/depoya göre ayarlayın.

---

Ek kaynaklar: `LOGGING_QUICK_REFERENCE.md`, `ACTIVITY_LOGGING_README.md`, `ERROR_HANDLING_GUIDE.md`.
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
