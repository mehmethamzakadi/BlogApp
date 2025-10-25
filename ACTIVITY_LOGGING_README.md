# BlogApp - Activity Logging & Advanced Logging Güncellemeleri

## 📊 Yapılan Geliştirmeler

### 1. **Activity Log Sistemi (Gerçek Verilerle Dashboard)**

#### Oluşturulan Dosyalar:
- ✅ `ActivityLog` Entity
- ✅ `IActivityLogRepository` Interface
- ✅ `ActivityLogRepository` Implementation
- ✅ `ActivityLogConfiguration` (EF Core)
- ✅ `ActivityLoggingBehavior` (MediatR Pipeline)
- ✅ `GetRecentActivitiesQuery/Handler`
- ✅ Dashboard Controller endpoint
- ✅ Frontend API integration

#### Özellikler:
- **Otomatik Loglama**: MediatR pipeline behavior ile tüm Create/Update/Delete komutları otomatik loglanır
- **Kullanıcı İzleme**: Hangi kullanıcının ne zaman ne yaptığını gösterir
- **Dashboard Entegrasyonu**: Frontend dashboard sayfasında gerçek aktiviteler gösterilir
- **Genişletilebilir**: Yeni entity tipleri kolayca eklenebilir

### 2. **Gelişmiş Structured Logging (Serilog)**

#### Oluşturulan/Güncellenen Dosyalar:
- ✅ `SerilogConfiguration.cs` - Merkezi loglama konfigürasyonu
- ✅ `RequestResponseLoggingFilter.cs` - HTTP request/response logging
- ✅ `ExceptionHandlingMiddleware.cs` - Gelişmiş exception logging
- ✅ `Program.cs` - Serilog entegrasyonu
- ✅ `docker-compose.yml` - Seq servisi eklendi

#### Log Sinks (Hedefler):
1. **Console** - Development için renkli konsol çıktısı
2. **File** - Günlük dosyalara kayıt (31 gün saklama)
3. **PostgreSQL** - Structured logging için veritabanı
4. **Seq** - Profesyonel log analiz platformu

#### Enrichers (Zenginleştirme):
- Machine Name
- Environment (Dev/Prod)
- Process ID
- Thread ID
- User Information
- Request Context (IP, User-Agent, etc.)

## 🚀 Kullanım Talimatları

### 1. Migration Oluşturma

ActivityLog tablosunu veritabanına eklemek için:

```bash
cd src/BlogApp.Persistence
dotnet ef migrations add AddActivityLog --startup-project ../BlogApp.API
dotnet ef database update --startup-project ../BlogApp.API
```

### 2. Seq Başlatma (Docker)

```bash
docker-compose up -d seq
```

Seq UI: http://localhost:5341

### 3. Uygulamayı Çalıştırma

```bash
cd src/BlogApp.API
dotnet run
```

### 4. Frontend Build

```bash
cd clients/blogapp-client
npm run build
```

## 📈 Activity Logging Nasıl Çalışır?

### Otomatik Loglama:
```csharp
// Örnek: Post oluştururken
var command = new CreatePostCommand { Title = "Yeni Post", ... };
await _mediator.Send(command);

// ActivityLoggingBehavior otomatik olarak şunu loglar:
// - ActivityType: "post_created"
// - EntityType: "Post"
// - Title: "Yeni Post oluşturuldu"
// - UserId: Giriş yapmış kullanıcı
// - Timestamp: UTC zaman
```

### Manuel Loglama:
```csharp
// İhtiyaç duyulursa repository'yi direkt kullanabilirsiniz
var activityLog = new ActivityLog
{
    ActivityType = "custom_action",
    EntityType = "CustomEntity",
    Title = "Özel işlem gerçekleştirildi",
    UserId = currentUserId
};
await _activityLogRepository.AddAsync(activityLog);
```

## 📊 Logging Best Practices

### Log Levels:
```csharp
// Debug - Geliştirme detayları
_logger.LogDebug("User {UserId} attempting to login", userId);

// Information - Önemli olaylar
_logger.LogInformation("Post {PostId} created successfully", postId);

// Warning - Uyarılar
_logger.LogWarning("Rate limit approaching for IP {IP}", ipAddress);

// Error - Hatalar
_logger.LogError(ex, "Failed to process payment for order {OrderId}", orderId);

// Critical - Kritik hatalar
_logger.LogCritical(ex, "Database connection lost");
```

### Structured Logging Örneği:
```csharp
_logger.LogInformation(
    "User {UserId} created post {PostId} in category {CategoryId}",
    userId, postId, categoryId
);
```

## 🔍 Seq Kullanımı

### Örnek Sorgular:

1. **Son hatalar:**
   ```
   @Level = 'Error' or @Level = 'Fatal'
   ```

2. **Belirli kullanıcının aktiviteleri:**
   ```
   UserId = 5
   ```

3. **Yavaş istekler (>1000ms):**
   ```
   ElapsedMilliseconds > 1000
   ```

4. **Belirli endpoint'e gelen istekler:**
   ```
   RequestPath like '%/api/post%'
   ```

## 🔧 Konfigürasyon

### appsettings.json:
```json
{
  "Serilog": {
    "SeqUrl": "http://localhost:5341",
    "SeqApiKey": null  // Production'da API key kullanın
  }
}
```

### Production Önerileri:
1. Seq için API key kullanın
2. File sink için retention policy ayarlayın
3. Log level'ları ayarlayın (Production'da Debug kapalı)
4. Sensitive data'yı loglama

## 📝 Notlar

- **ActivityLog** tablosu otomatik indekslenmiştir (Timestamp, Entity bazlı)
- Log dosyaları `logs/` klasöründe saklanır
- PostgreSQL'de `Logs` tablosu otomatik oluşturulur
- Seq container volume'u `seq_data` olarak persist edilir

## 🎯 Sonraki Adımlar

İsterseniz şunları da ekleyebiliriz:
- [ ] Email/SMS bildirimleri (kritik hatalar için)
- [ ] Elasticsearch entegrasyonu (büyük ölçekli log analizi)
- [ ] Application Insights (Azure monitoring)
- [ ] Custom dashboard grafikleri (log metrikleri)
- [ ] Audit log export (CSV/Excel)
