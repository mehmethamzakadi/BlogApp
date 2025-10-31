# Konfigürasyon Kılavuzu

Bu dokümanda BlogApp'in farklı ortamlardaki konfigürasyon yapısı açıklanmaktadır.

## 📁 Konfigürasyon Dosyaları

### ASP.NET Core Konfigürasyon Hiyerarşisi

```
appsettings.json                    (Base - Tüm ortamlar için)
    ↓ Override
appsettings.Development.json        (Development ortamı)
    ↓ Override
appsettings.Production.json         (Production ortamı)
    ↓ Override
Environment Variables               (Docker/Kubernetes)
```

### Dosya Açıklamaları

#### 1. appsettings.json
**Amaç:** Tüm ortamlar için ortak temel ayarlar

**İçerik:**
- Logging seviyeleri
- Rate limiting kuralları
- Image storage ayarları
- Temel connection string şablonları

**Önemli:** Bu dosya production'da da kullanılır, hassas bilgi içermemelidir.

#### 2. appsettings.Development.json
**Amaç:** Lokal geliştirme ortamı ayarları

**İçerik:**
- Lokal PostgreSQL bağlantısı (`postgresdb:5432`)
- Lokal Redis bağlantısı (`redis_server:6379`)
- Development CORS ayarları (`localhost:5173`)
- Seq log URL'i (`http://localhost:5341`)

**Kullanım:**
```bash
# Otomatik olarak Development modda çalışır
cd src/BlogApp.API
dotnet run
```

#### 3. appsettings.Production.json
**Amaç:** Production sunucu ayarları

**İçerik:**
- Production PostgreSQL bağlantısı (optimized)
- Production Redis bağlantısı (optimized)
- Production CORS ayarları (gerçek domain)
- Seq log URL'i (`http://seq:80`)

**Kullanım:**
```bash
# Production modda çalıştır
ASPNETCORE_ENVIRONMENT=Production dotnet run
```

---

## 🐳 Docker Konfigürasyonu

### docker-compose.yml (Base)
**Amaç:** Tüm ortamlar için ortak servis tanımları

**İçerik:**
- PostgreSQL (max_connections=300, shared_buffers=256MB)
- RabbitMQ
- Redis
- Seq

**Kullanım:** Tek başına kullanılmaz, diğer compose dosyaları ile birlikte kullanılır.

### docker-compose.local.yml
**Amaç:** Lokal geliştirme için ek ayarlar ve override'lar

**İçerik:**
- Optimized connection strings (environment variables)
- Development environment (`ASPNETCORE_ENVIRONMENT=Development`)
- Port mapping (`6060:8080` - React client ile uyumlu)
- Volume mounting (hot reload için)

**Kullanım:**
```bash
docker compose -f docker-compose.yml -f docker-compose.local.yml up -d
```

**Not:** `docker-compose.yml` ile birlikte kullanılır, PostgreSQL tuning ayarları base'den gelir.

### docker-compose.prod.yml
**Amaç:** Production için ek ayarlar ve override'lar

**İçerik:**
- Optimized connection strings (environment variables)
- Network konfigürasyonu
- Restart policies

**Kullanım:**
```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

---

## 🔧 Performans Optimizasyonları

### Connection String Parametreleri

#### PostgreSQL
```
Pooling=true                      # Connection pooling aktif
Minimum Pool Size=10              # Minimum 10 connection hazır bekler
Maximum Pool Size=200             # Maximum 200 connection (Development: 100)
Connection Idle Lifetime=300      # 5 dakika idle connection'ı kapat
Connection Pruning Interval=10    # Her 10 saniyede idle connection temizle
```

**Neden bu değerler?**
- **Min 10:** Uygulama başladığında hemen 10 connection hazır
- **Max 200 (Prod):** 2000 kullanıcı için yeterli (her kullanıcı ~0.1 connection)
- **Max 100 (Dev):** Lokal geliştirmede daha az kaynak kullanımı
- **Idle Lifetime 300s:** Kullanılmayan connection'ları temizle
- **Pruning Interval 10s:** Sık kontrol et, kaynak israfını önle

#### Redis
```
abortConnect=false                # Bağlantı hatası olursa devam et
connectTimeout=5000               # 5 saniye connection timeout
syncTimeout=5000                  # 5 saniye sync timeout
connectRetry=3                    # 3 kez tekrar dene
keepAlive=60                      # 60 saniyede bir keepalive gönder
```

**Neden bu değerler?**
- **abortConnect=false:** Redis çökerse uygulama çalışmaya devam eder
- **connectTimeout=5000:** Yavaş network'te bile bağlanabilir
- **connectRetry=3:** Geçici network sorunlarında otomatik düzelir
- **keepAlive=60:** Connection'ın canlı kalmasını sağlar

### PostgreSQL Server Tuning

```yaml
command:
  - "postgres"
  - "-c"
  - "max_connections=300"           # Max 300 connection
  - "-c"
  - "shared_buffers=256MB"          # RAM'in %25'i
  - "-c"
  - "effective_cache_size=1GB"      # Toplam RAM
  - "-c"
  - "work_mem=4MB"                  # Sorting/hashing için
```

**Neden bu değerler?**
- **max_connections=300:** Connection pool max 200, buffer için 100 ekstra
- **shared_buffers=256MB:** 1GB RAM için optimal (RAM'in %25'i)
- **effective_cache_size=1GB:** PostgreSQL'e sistemin tüm RAM'ini kullanabileceğini söyler
- **work_mem=4MB:** Her connection için sorting/hashing memory

---

## 🌍 Ortam Değişkenleri (Environment Variables)

### Öncelik Sırası
1. **Environment Variables** (En yüksek öncelik)
2. **appsettings.{Environment}.json**
3. **appsettings.json** (En düşük öncelik)

### Docker'da Override Etme

**Örnek: Connection string'i override et**
```yaml
# docker-compose.prod.yml
environment:
  ConnectionStrings__BlogAppPostgreConnectionString: "Host=prod-db;Port=5432;..."
```

**Örnek: Redis URL'ini override et**
```yaml
environment:
  ConnectionStrings__RedisCache: "prod-redis:6379,password=secret"
```

**Not:** `__` (çift alt çizgi) JSON'daki `:` (iki nokta) yerine kullanılır.

---

## 📊 Ortam Karşılaştırması

| Ayar | Development | Production |
|------|-------------|------------|
| **PostgreSQL Pool Max** | 100 | 200 |
| **PostgreSQL max_connections** | 300 | 300 |
| **Redis Timeout** | 5000ms | 5000ms |
| **CORS Origins** | localhost:5173 | Gerçek domain |
| **Seq URL** | localhost:5341 | seq:80 |
| **Error Detail** | true | false (önerilen) |
| **Logging Level** | Information | Warning |

---

## 🔐 Güvenlik Notları

### appsettings.Production.json
**❌ YAPMAYIN:**
```json
{
  "ConnectionStrings": {
    "BlogAppPostgreConnectionString": "Host=prod;Password=RealPassword123"
  }
}
```

**✅ YAPIN:**
```json
{
  "ConnectionStrings": {
    "BlogAppPostgreConnectionString": "Pooling=true;Maximum Pool Size=200"
  }
}
```

Gerçek şifreleri environment variable olarak verin:
```bash
export ConnectionStrings__BlogAppPostgreConnectionString="Host=prod;Password=RealPassword123;Pooling=true;Maximum Pool Size=200"
```

### .gitignore
Hassas bilgi içeren dosyaları commit etmeyin:
```
appsettings.Production.json  # Eğer gerçek şifre içeriyorsa
.env
.env.local
.env.production
```

---

## 🧪 Test Ortamları

### Lokal Test (Development)
```bash
# appsettings.Development.json kullanır
cd src/BlogApp.API
dotnet run
```

### Docker Test (Development)
```bash
# appsettings.Development.json + docker-compose.local.yml
docker compose -f docker-compose.yml -f docker-compose.local.yml up -d
```

### Production Test (Staging)
```bash
# appsettings.Production.json + docker-compose.prod.yml
ASPNETCORE_ENVIRONMENT=Production docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

---

## 🔄 Konfigürasyon Değiştirme

### Senaryo 1: Connection Pool Boyutunu Artırma

**Development için:**
```json
// appsettings.Development.json
"ConnectionStrings": {
  "BlogAppPostgreConnectionString": "...;Maximum Pool Size=150;..."
}
```

**Production için:**
```json
// appsettings.Production.json
"ConnectionStrings": {
  "BlogAppPostgreConnectionString": "...;Maximum Pool Size=300;..."
}
```

**PostgreSQL'i de güncelle:**
```yaml
# docker-compose.yml
command:
  - "postgres"
  - "-c"
  - "max_connections=400"  # Pool max'tan 100 fazla
```

### Senaryo 2: Redis Şifresi Ekleme

**appsettings.Production.json:**
```json
"ConnectionStrings": {
  "RedisCache": "redis_server:6379,password=YourPassword,abortConnect=false,..."
}
```

**docker-compose.prod.yml:**
```yaml
redis.cache:
  command: redis-server --requirepass YourPassword
```

### Senaryo 3: Farklı Database Sunucusu

**Environment variable ile:**
```bash
export ConnectionStrings__BlogAppPostgreConnectionString="Host=external-db.com;Port=5432;Database=BlogAppDb;Username=user;Password=pass;Pooling=true;Maximum Pool Size=200"
```

---

## 📝 Checklist: Production'a Geçiş

- [ ] `appsettings.Production.json` içinde gerçek domain/IP var
- [ ] Hassas bilgiler environment variable olarak verilmiş
- [ ] PostgreSQL `max_connections` >= Connection Pool Max + 100
- [ ] Redis şifresi ayarlanmış (önerilen)
- [ ] CORS sadece gerçek domain'leri içeriyor
- [ ] Seq production URL'i doğru (`http://seq:80`)
- [ ] `Include Error Detail=false` (güvenlik için)
- [ ] Logging seviyesi `Warning` veya `Error`
- [ ] SSL sertifikaları yüklenmiş (HTTPS için)
- [ ] Firewall kuralları ayarlanmış

---

## 🆘 Sorun Giderme

### Problem: "Connection pool exhausted"
**Çözüm:**
1. `Maximum Pool Size` artır (200 → 300)
2. PostgreSQL `max_connections` artır (300 → 400)
3. Connection leak var mı kontrol et

### Problem: "Redis timeout"
**Çözüm:**
1. `connectTimeout` artır (5000 → 10000)
2. Redis memory kullanımını kontrol et
3. Network latency'yi ölç

### Problem: "Configuration değişikliği uygulanmıyor"
**Çözüm:**
1. Doğru environment'ta mısınız? (`ASPNETCORE_ENVIRONMENT`)
2. Docker container'ı yeniden başlatın
3. Environment variable override ediyor mu kontrol edin

---

## 📞 Destek

Konfigürasyon sorunları için:
1. `OPTIMIZATION_CHANGES.md` - Yapılan değişiklikler
2. `LOAD_TEST_GUIDE.md` - Performans testi
3. Seq logs - `http://localhost:5341`
