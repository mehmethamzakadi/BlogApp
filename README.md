# BlogApp

BlogApp; ASP.NET Core tabanlı arka uç ve Blazor Server tabanlı yönetim/son kullanıcı arayüzü içeren katmanlı bir örnek projedir.

## Projeyi Çalıştırma

### Sunucu
`src/BlogApp.API` klasöründen `dotnet run` komutunu çalıştırarak REST API'yi ayağa kaldırabilirsiniz.

### Blazor İstemci
`src/BlogApp.Client` klasöründe yeni oluşturulan Blazor Server projesi, Radzen bileşenleriyle zenginleştirilmiş iki temel sayfa içerir:

- `/` — Blog ana sayfası, öne çıkan yazıları kart yapısı ile listeler, arama ve kategori filtreleri barındırır.
- `/admin/dashboard` — Yönetim paneli, metrik kartları, trafik grafikleri ve gönderi listesi sunar.

İstemciyi çalıştırmak için ilgili klasörde `dotnet run` komutunu kullanabilirsiniz.

## Docker ile Çalıştırma

Projeyi Docker Compose ile hem lokal geliştirme ortamında hem de Ubuntu tabanlı üretim sunucusunda çalıştırmak için ortak `docker-compose.yml` dosyasını, ortamınıza uygun ek dosya ile birlikte kullanabilirsiniz.

### Lokal Geliştirme
1. İmajları oluşturup konteynerleri ayağa kaldırmak için:
   ```bash
   docker compose -f docker-compose.yml -f docker-compose.local.yml up --build
   ```
2. API `http://localhost:5000` üzerinden ulaşılabilir. PostgreSQL, RabbitMQ ve Redis portları da host makinenize yönlendirilir.
3. Konteynerleri durdurmak için `Ctrl+C` kombinasyonunu kullanabilir veya aşağıdaki komutu çalıştırabilirsiniz:
   ```bash
   docker compose -f docker-compose.yml -f docker-compose.local.yml down
   ```

### Ubuntu Üretim Sunucusu (Docker + Nginx)
1. Gerekli imajları oluşturup konteynerleri arka planda başlatmak için:
   ```bash
   docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
   ```
2. API konteyneri yalnızca dahili Docker ağı üzerinden 8080 portunu dinler. Nginx reverse proxy konteyneri host üzerindeki 8080 numaralı portu dış dünyaya açarak gelen trafiği container içindeki 80 numaralı porta ve oradan API servisine yönlendirir.
3. Servisleri durdurmak için aşağıdaki komutu kullanabilirsiniz:
   ```bash
   docker compose -f docker-compose.yml -f docker-compose.prod.yml down
   ```

### Ortam Değişkenleri
- `ASPNETCORE_ENVIRONMENT`: Varsayılan olarak üretimde `Production`, lokal ortamda `Development` olarak ayarlanır.
- `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`: PostgreSQL veritabanı ayarlarını özelleştirmek için kullanılabilir.

Kalıcı veriler Docker volume'ları (`postgres_data`, `rabbitmq_data`, `redis_data`) üzerinde saklanır. RabbitMQ için kullanıcı
adını/şifresini değiştirirseniz mevcut `rabbitmq_data` volume'unda eski kimlik bilgileri tutulmaya devam ettiği için konteyner yeni
değerlerle açıldığında oturum açma sorunları yaşayabilirsiniz. Bu durumda servisleri durdurduktan sonra volume'u silerek yeniden
oluşturmalısınız:

```bash
docker compose down
docker volume rm blogapp_rabbitmq_data
```

Ardından konteynerleri tekrar ayağa kaldırdığınızda (`docker compose up --build`) volume otomatik olarak yeniden oluşturulur.

---

## 📊 Logging & Monitoring

BlogApp **3-tier logging architecture** kullanır:

1. **File Logs** (`logs/blogapp-*.txt`) - Development & debugging
2. **Structured Logs** (PostgreSQL `Logs` table) - Production monitoring
3. **Activity Logs** (PostgreSQL `ActivityLogs` table) - Compliance & audit trail

**Detaylı bilgi için:**
- 📖 [LOGGING_ARCHITECTURE.md](LOGGING_ARCHITECTURE.md) - Kapsamlı mimari dokümantasyonu
- 🎯 [LOGGING_QUICK_REFERENCE.md](LOGGING_QUICK_REFERENCE.md) - Hızlı referans ve örnekler
- 📋 [ACTIVITY_LOGGING_README.md](ACTIVITY_LOGGING_README.md) - Activity logging detayları

**Monitoring Tools:**
- **Seq** (http://localhost:5341) - Log analiz ve monitoring
- **PostgreSQL** - Structured query ve analytics
- **File Logs** - Quick debugging ve tail/grep

````
