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

Kalıcı veriler Docker volume'ları (`postgres_data`, `rabbitmq_data`, `redis_data`) üzerinde saklanır.
