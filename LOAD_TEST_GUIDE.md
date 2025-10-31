# 500-2000 Eşzamanlı Kullanıcı Load Test Kılavuzu

Bu dokümanda BlogApp'in 500-2000 eşzamanlı kullanıcı ile nasıl test edileceği adım adım açıklanmaktadır.

## 📋 Ön Hazırlık

### 1. Test Ortamı Gereksinimleri

**Minimum Sistem Gereksinimleri:**
- CPU: 4 core
- RAM: 8GB
- Disk: SSD (önerilen)
- Network: 100 Mbps+

**Önerilen Sistem (Production-like):**
- CPU: 8+ core
- RAM: 16GB+
- Disk: NVMe SSD
- Network: 1 Gbps

### 2. k6 Kurulumu

**Windows:**
```bash
choco install k6
```

**macOS:**
```bash
brew install k6
```

**Linux (Ubuntu/Debian):**
```bash
sudo gpg -k
sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update
sudo apt-get install k6
```

**Docker:**
```bash
docker pull grafana/k6
```

### 3. Test Verisi Hazırlama

Veritabanında yeterli test verisi olmalı:
```sql
-- PostgreSQL'e bağlan
psql -h localhost -p 5435 -U postgres -d BlogAppDb

-- Veri kontrolü
SELECT 
    (SELECT COUNT(*) FROM "Posts" WHERE "DeletedDate" IS NULL) as posts,
    (SELECT COUNT(*) FROM "Categories" WHERE "DeletedDate" IS NULL) as categories,
    (SELECT COUNT(*) FROM "Users" WHERE "DeletedDate" IS NULL) as users;

-- Minimum gereksinim: 50+ post, 5+ kategori
```

Eğer yeterli veri yoksa, admin panelinden veya seed script ile veri ekleyin.

---

## 🚀 Test Senaryoları

### Senaryo 1: Temel Load Test (50-500 Kullanıcı)

**Amaç:** Sistemin normal yük altında performansını ölçmek

**Komut:**
```bash
k6 run performance-test.js
```

**Beklenen Sonuçlar:**
- P95 < 500ms
- P99 < 1000ms
- Hata oranı < %1
- Throughput > 300 req/s

**Test Süresi:** ~33 dakika

---

### Senaryo 2: Stress Test (500-1000 Kullanıcı)

**Amaç:** Sistemin yüksek yük altında davranışını görmek

**Komut:**
```bash
k6 run --vus 1000 --duration 10m performance-test.js
```

**Beklenen Sonuçlar:**
- P95 < 800ms
- P99 < 1500ms
- Hata oranı < %5
- Throughput > 500 req/s

---

### Senaryo 3: Spike Test (Ani Yük Artışı)

**Amaç:** Ani trafik artışlarında sistemin tepkisini görmek

**spike-test.js oluşturun:**
```javascript
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '1m', target: 100 },   // Normal
    { duration: '30s', target: 2000 }, // Ani artış!
    { duration: '3m', target: 2000 },  // Yüksek yük
    { duration: '1m', target: 100 },   // Normale dön
    { duration: '1m', target: 0 },
  ],
};

const BASE_URL = 'http://localhost:6060/api';

export default function () {
  const res = http.get(`${BASE_URL}/post?page=1&pageSize=10`);
  check(res, { 'status 200': (r) => r.status === 200 });
  sleep(1);
}
```

**Komut:**
```bash
k6 run spike-test.js
```

---

### Senaryo 4: Soak Test (Uzun Süreli Dayanıklılık)

**Amaç:** Memory leak, connection leak gibi sorunları tespit etmek

**Komut:**
```bash
k6 run --vus 500 --duration 2h performance-test.js
```

**İzlenmesi Gerekenler:**
- Memory kullanımı artıyor mu?
- Connection pool dolup taşıyor mu?
- Response time zamanla artıyor mu?

---

## 📊 Test Çalıştırma Adımları

### Adım 1: Sistemi Başlat

**Docker ile:**
```bash
# Tüm servisleri başlat
docker compose -f docker-compose.yml -f docker-compose.local.yml up -d

# Logları izle
docker compose -f docker-compose.yml -f docker-compose.local.yml logs -f blogapp.api
```

**Lokal geliştirme:**
```bash
cd src/BlogApp.API
dotnet run --urls http://localhost:6060
```

### Adım 2: Sistem Sağlık Kontrolü

```bash
# API çalışıyor mu?
curl http://localhost:6060/api/category

# PostgreSQL bağlantısı var mı?
docker exec -it postgresdb pg_isready -U postgres

# Redis çalışıyor mu?
docker exec -it redis_server redis-cli ping
```

### Adım 3: Baseline Ölçümü

Önce tek kullanıcı ile baseline ölçümü yapın:
```bash
k6 run --vus 1 --duration 30s performance-test.js
```

Bu size sistemin "ideal" performansını gösterir.

### Adım 4: Ana Test

```bash
# Detaylı rapor ile
k6 run --out json=test-results.json performance-test.js

# Gerçek zamanlı grafik için (opsiyonel)
k6 run --out influxdb=http://localhost:8086/k6 performance-test.js
```

### Adım 5: Sonuçları Analiz Et

Test bittiğinde 3 dosya oluşur:
1. **Console output** - Özet sonuçlar
2. **performance-summary.json** - Detaylı metrikler
3. **performance-report.html** - Görsel rapor

HTML raporunu tarayıcıda açın:
```bash
# Windows
start performance-report.html

# macOS
open performance-report.html

# Linux
xdg-open performance-report.html
```

---

## 📈 Sonuçları Yorumlama

### Başarı Kriterleri

| Metrik | Hedef | İyi | Orta | Kötü |
|--------|-------|-----|------|------|
| P95 Response Time | <300ms | <500ms | <1000ms | >1000ms |
| P99 Response Time | <800ms | <1000ms | <2000ms | >2000ms |
| Hata Oranı | <%0.5 | <%1 | <%5 | >%5 |
| Throughput | >500 req/s | >300 req/s | >100 req/s | <100 req/s |

### Örnek Sonuç Analizi

**İyi Performans:**
```
✅ P95: 287ms
✅ P99: 654ms
✅ Hata Oranı: %0.3
✅ Throughput: 687 req/s
🎉 Sistem 2000 kullanıcıyı sorunsuz destekliyor!
```

**Optimizasyon Gerekli:**
```
⚠️ P95: 1234ms
⚠️ P99: 3456ms
⚠️ Hata Oranı: %3.2
⚠️ Throughput: 156 req/s
🔧 Sistem optimizasyon gerektirebilir
```

---

## 🔍 Monitoring (Test Sırasında İzleme)

### 1. Seq Logs

Test sırasında Seq'i açık tutun:
```
http://localhost:5341
```

**İzlenecek Sorgular:**
```
// Yavaş istekler
Elapsed > 1000

// Hatalar
StatusCode >= 500

// Throughput (1 dakikalık)
select count(*) from stream group by time(1m)
```

### 2. PostgreSQL Monitoring

Başka bir terminal'de:
```bash
docker exec -it postgresdb psql -U postgres -d BlogAppDb

-- Aktif bağlantılar
SELECT count(*), state FROM pg_stat_activity GROUP BY state;

-- Yavaş sorgular (test sırasında)
SELECT pid, now() - query_start as duration, query 
FROM pg_stat_activity 
WHERE state = 'active' AND now() - query_start > interval '1 second'
ORDER BY duration DESC;

-- Connection pool kullanımı
SELECT 
    max_conn,
    used,
    res_for_super,
    max_conn - used - res_for_super as available
FROM (
    SELECT 
        setting::int AS max_conn,
        (SELECT count(*) FROM pg_stat_activity) AS used,
        (SELECT setting::int FROM pg_settings WHERE name = 'superuser_reserved_connections') AS res_for_super
    FROM pg_settings 
    WHERE name = 'max_connections'
) s;
```

### 3. Docker Stats

```bash
# Kaynak kullanımını izle
docker stats blogapp.api postgresdb redis_server

# Sürekli izleme
watch -n 1 'docker stats --no-stream'
```

### 4. Windows Performance Monitor (Windows)

```bash
# CPU ve Memory kullanımı
perfmon
```

**İzlenecek Counterlar:**
- Processor Time
- Available Memory
- Network Bytes/sec

---

## 🐛 Sorun Giderme

### Problem 1: Yüksek Response Time

**Belirtiler:**
- P95 > 1000ms
- P99 > 2000ms

**Çözümler:**
1. Seq'te yavaş sorguları kontrol edin
2. Database index'leri kontrol edin
3. Connection pool boyutunu artırın
4. Cache'lerin çalıştığını doğrulayın

### Problem 2: Connection Timeout

**Belirtiler:**
- "connection refused" hataları
- "timeout" hataları

**Çözümler:**
```bash
# Connection pool limitlerini artır
# appsettings.json
"Maximum Pool Size=300"

# Kestrel limitlerini artır
MaxConcurrentConnections = 2000
```

### Problem 3: Memory Leak

**Belirtiler:**
- Memory kullanımı sürekli artıyor
- Test ilerledikçe yavaşlama

**Çözümler:**
1. Docker container'ı yeniden başlatın
2. Connection'ların düzgün dispose edildiğini kontrol edin
3. EF Core tracking'in kapalı olduğunu doğrulayın

### Problem 4: Database Connection Pool Exhausted

**Belirtiler:**
- "connection pool exhausted" hataları
- "timeout expired" hataları

**Çözümler:**
```bash
# PostgreSQL max_connections artır
docker exec -it postgresdb psql -U postgres -c "ALTER SYSTEM SET max_connections = 300;"
docker restart postgresdb

# Connection pool boyutunu ayarla
"Maximum Pool Size=200"
```

---

## 📊 Gerçek Dünya Test Senaryosu

### Production-like Test

```javascript
// realistic-test.js
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '5m', target: 100 },   // Sabah trafiği
    { duration: '10m', target: 500 },  // Öğle trafiği
    { duration: '5m', target: 1000 },  // Peak saat
    { duration: '10m', target: 500 },  // Akşam trafiği
    { duration: '5m', target: 100 },   // Gece trafiği
  ],
};

const BASE_URL = 'http://localhost:6060/api';

export default function () {
  // %70 okuma, %20 yazma, %10 silme (gerçekçi oran)
  const rand = Math.random();
  
  if (rand < 0.7) {
    // Okuma işlemi
    http.get(`${BASE_URL}/post?page=1&pageSize=10`);
  } else if (rand < 0.9) {
    // Yazma işlemi (admin kullanıcı gerekir)
    // Bu kısım authentication gerektirir
  } else {
    // Silme işlemi
    // Bu kısım authentication gerektirir
  }
  
  sleep(Math.random() * 5 + 2); // 2-7 saniye (gerçekçi kullanıcı davranışı)
}
```

---

## 📝 Test Raporu Şablonu

Test sonrası aşağıdaki raporu doldurun:

```markdown
# Load Test Raporu

**Test Tarihi:** [YYYY-MM-DD]
**Test Süresi:** [XX dakika]
**Maksimum Kullanıcı:** [XXXX]

## Sistem Konfigürasyonu
- CPU: [X core]
- RAM: [X GB]
- Database: PostgreSQL [version]
- Cache: Redis [version]

## Test Sonuçları

### Performans Metrikleri
- P50: [XXX]ms
- P95: [XXX]ms
- P99: [XXX]ms
- Avg: [XXX]ms
- Max: [XXX]ms

### Throughput
- Requests/sec: [XXX]
- Total Requests: [XXXXX]
- Failed Requests: [XX] (%[X.XX])

### Kaynak Kullanımı
- CPU Peak: [XX]%
- Memory Peak: [X.X]GB
- DB Connections Peak: [XXX]

## Sorunlar
1. [Sorun açıklaması]
2. [Sorun açıklaması]

## Öneriler
1. [Öneri]
2. [Öneri]

## Sonuç
✅ / ⚠️ / ❌ [Genel değerlendirme]
```

---

## 🎯 Hedef Performans Tablosu

| Kullanıcı Sayısı | P95 Target | P99 Target | Throughput Target | Durum |
|------------------|------------|------------|-------------------|-------|
| 50 | <200ms | <500ms | >100 req/s | ✅ |
| 100 | <250ms | <600ms | >200 req/s | ✅ |
| 200 | <300ms | <700ms | >300 req/s | ✅ |
| 500 | <400ms | <900ms | >500 req/s | 🎯 |
| 1000 | <500ms | <1000ms | >700 req/s | 🎯 |
| 2000 | <800ms | <1500ms | >1000 req/s | 🎯 |

---

## 💡 İpuçları

1. **Test öncesi sistemi yeniden başlatın** - Temiz bir başlangıç için
2. **Birden fazla test çalıştırın** - Tutarlılığı doğrulamak için
3. **Farklı saatlerde test edin** - Sistem kaynaklarının etkisini görmek için
4. **Sonuçları kaydedin** - Zaman içinde performans trendini görmek için
5. **Production'a yakın ortamda test edin** - Gerçekçi sonuçlar için

---

## 📞 Destek

Test sırasında sorun yaşarsanız:
1. `OPTIMIZATION_CHANGES.md` dosyasını inceleyin
2. Seq loglarını kontrol edin
3. Docker container loglarını inceleyin
4. PostgreSQL query stats'ı kontrol edin
