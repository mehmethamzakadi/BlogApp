# BlogApp

BlogApp; ASP.NET Core tabanlı modern bir blog uygulaması olup, Clean Architecture prensiplerine göre geliştirilmiş REST API ve React tabanlı web client'ı içerir.

## 🏗️ Proje Yapısı

### Backend (ASP.NET Core 9.0)
- **BlogApp.API** - REST API katmanı
- **BlogApp.Application** - İş mantığı ve CQRS implementasyonu (MediatR)
- **BlogApp.Domain** - Domain entities ve business rules
- **BlogApp.Infrastructure** - External servisler (JWT, Email, RabbitMQ, Redis)
- **BlogApp.Persistence** - Veritabanı operasyonları (EF Core + PostgreSQL)

### Frontend (React + TypeScript)
- **blogapp-client** - Modern React SPA (Vite + TailwindCSS + shadcn/ui)

## 🚀 Projeyi Çalıştırma

### Sunucu (Backend API)
`src/BlogApp.API` klasöründen REST API'yi başlatın:
```bash
cd src/BlogApp.API
dotnet run
```
Varsayılan launch profili `https://localhost:7285` ve `http://localhost:5285` adreslerini dinler.

> **Not:** React istemcisi `.env.local` dosyasında `VITE_API_URL=http://localhost:6060/api` kullandığı için API'yi 6060 portunda çalıştırmanız gerekiyorsa:
> - `dotnet run --urls http://localhost:6060` komutunu verin **veya**
> - Docker Compose ile `blogapp.api` servisini başlatın (otomatik olarak `http://localhost:6060` portunu map eder).

### React İstemci
`clients/blogapp-client` klasöründen React uygulamasını başlatın:
```bash
cd clients/blogapp-client
npm install
npm run dev
```

İstemci varsayılan olarak `http://localhost:5173` üzerinde çalışır.

## 🐳 Docker ile Çalıştırma

Projeyi Docker Compose ile hem lokal geliştirme ortamında hem de üretim sunucusunda çalıştırabilirsiniz.

### Lokal Geliştirme
İmajları oluşturup konteynerleri başlatmak için:
```bash
docker compose -f docker-compose.yml -f docker-compose.local.yml up --build
```

**Servisler:**
- **API:** `http://localhost:6060`
- **React Client:** `http://localhost:5173`
- **PostgreSQL:** `localhost:5435` (User: postgres, Password: postgres, DB: BlogAppDb)
- **RabbitMQ Management:** `http://localhost:15672` (User: blogapp, Password: supersecret)
- **Redis:** `localhost:6379`
- **Seq (Log Viewer):** `http://localhost:5341`

Konteynerleri durdurmak için:
```bash
docker compose -f docker-compose.yml -f docker-compose.local.yml down
```

### Üretim Ortamı (Docker + Nginx)
Servisleri arka planda başlatmak için:
```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
```

**Yapılandırma:**
- API konteyneri dahili Docker ağında 8080 portunda çalışır
- Nginx reverse proxy dış dünyaya 8080 portunu açar ve trafiği API'ye yönlendirir
- `ASPNETCORE_ENVIRONMENT` otomatik olarak `Production` ayarlanır
- Seq servisi `http://seq:80` adresinden erişilebilir

Servisleri durdurmak için:
```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml down
```

### Ortam Değişkenleri ve Docker Volumes

**Temel Yapılandırmalar:**
- `ASPNETCORE_ENVIRONMENT`: `Development` (lokal) / `Production` (üretim)
- `POSTGRES_DB`: Veritabanı adı (varsayılan: blogappdb)
- `POSTGRES_USER`: PostgreSQL kullanıcı adı (varsayılan: postgres)
- `POSTGRES_PASSWORD`: PostgreSQL şifresi (varsayılan: postgres)

**Docker Volumes:**
Kalıcı veriler aşağıdaki volume'larda saklanır:
- `postgres_data`: PostgreSQL veritabanı
- `rabbitmq_data`: RabbitMQ mesaj kuyruğu
- `redis_data`: Redis cache
- `seq_data`: Seq log verileri

**⚠️ RabbitMQ Credentials Değiştirme:**
RabbitMQ kullanıcı adı/şifresini değiştirirseniz, mevcut volume'daki eski kimlik bilgileri ile çakışma yaşanabilir. Bu durumda volume'u silip yeniden oluşturun:
```bash
docker compose down
docker volume rm blogapp_rabbitmq_data
docker compose up --build
```

---

## 📊 Logging & Monitoring Yapısı

BlogApp **3-katmanlı loglama mimarisi** kullanır:

### 1. **File Logs** (`logs/blogapp-*.txt`)
- **Amaç:** Development & debugging
- **Seviye:** Debug, Info, Warning, Error, Critical
- **Saklama:** 31 gün
- **Kullanım:** Hızlı hata ayıklama, stack trace inceleme

### 2. **Structured Logs** (PostgreSQL `Logs` tablosu)
- **Amaç:** Production monitoring & analytics
- **Seviye:** Information ve üzeri (Warning, Error, Critical)
- **Saklama:** 90 gün (otomatik temizleme)
- **Kullanım:** SQL sorguları ile log analizi, performans metrikleri

### 3. **Activity Logs** (PostgreSQL `ActivityLogs` tablosu)
- **Amaç:** Compliance & audit trail
- **Kapsam:** Kullanıcı aksiyonları (create/update/delete)
- **Saklama:** Süresiz
- **Kullanım:** GDPR/SOC2 uyumluluk, güvenlik soruşturmaları

### Monitoring Araçları
- **Seq** (`http://localhost:5341`) - Gelişmiş log görselleştirme ve analiz
- **PostgreSQL** - SQL sorguları ile detaylı log analizi
- **File Logs** - CLI araçları (tail, grep) ile hızlı debug

**📖 Detaylı Dokümantasyon:**
- [LOGGING_ARCHITECTURE.md](LOGGING_ARCHITECTURE.md) - Kapsamlı mimari dokümantasyonu
- [LOGGING_QUICK_REFERENCE.md](LOGGING_QUICK_REFERENCE.md) - Hızlı referans ve kod örnekleri
- [LOGGING_COMPARISON.md](LOGGING_COMPARISON.md) - Tek-tier vs Multi-tier karşılaştırması
- [ACTIVITY_LOGGING_README.md](ACTIVITY_LOGGING_README.md) - Activity logging detayları

---

## 🛠️ Teknoloji Stack

### Backend
- **Framework:** ASP.NET Core 9.0
- **ORM:** Entity Framework Core 9.0
- **Database:** PostgreSQL 16
- **Cache:** Redis
- **Message Queue:** RabbitMQ
- **Authentication:** JWT Bearer
- **Validation:** FluentValidation
- **CQRS:** MediatR
- **Logging:** Serilog (File + PostgreSQL + Seq)
- **Rate Limiting:** AspNetCoreRateLimit
- **API Documentation:** Scalar (OpenAPI 3.0)

### Frontend
- **Framework:** React 18 + TypeScript
- **Build Tool:** Vite
- **UI Library:** TailwindCSS + shadcn/ui
- **Icons:** Lucide React
- **Routing:** React Router v7
- **State Management:** Zustand
- **HTTP Client:** Axios
- **Data Fetching:** TanStack Query
- **Form Validation:** React Hook Form + Zod

### DevOps
- **Containerization:** Docker + Docker Compose
- **Reverse Proxy:** Nginx
- **Log Monitoring:** Seq

---

## 🔐 Yetkilendirme & Güvenlik

BlogApp **permission-based authorization** sistemi kullanır:

### Özellikler
- ✅ **JWT-based Authentication** - Güvenli token tabanlı kimlik doğrulama
- ✅ **Permission-based Authorization** - Granüler yetki kontrolü
- ✅ **Route Guards** - Sayfa seviyesinde erişim kontrolü
- ✅ **UI Guards** - Component/buton seviyesinde görünürlük kontrolü
- ✅ **403 Forbidden Page** - Kullanıcı dostu erişim engelleme sayfası
- ✅ **Dynamic Sidebar** - Kullanıcı yetkilerine göre menü filtreleme

### Kullanım
```typescript
// Route koruması
<ProtectedRoute requiredPermission={Permissions.UsersViewAll}>
  <UsersPage />
</ProtectedRoute>

// UI element koruması
<PermissionGuard requiredPermission={Permissions.UsersCreate}>
  <Button>Yeni Kullanıcı</Button>
</PermissionGuard>

// Hook kullanımı
const { hasPermission } = usePermission();
if (hasPermission(Permissions.PostsDelete)) {
  // İşlem yap
}
```

**📖 Detaylı Dokümantasyon:**
- [PERMISSION_GUARDS_GUIDE.md](PERMISSION_GUARDS_GUIDE.md) - Kapsamlı implementasyon kılavuzu
- [PERMISSION_CHEATSHEET.md](PERMISSION_CHEATSHEET.md) - Hızlı referans

---

## 📚 Ek Kaynaklar

- [ANALYSIS.md](ANALYSIS.md) - Kod tabanı analizi ve iyileştirme önerileri
- [Solution Items/Migrations.txt](Solution%20Items/Migrations.txt) - Veritabanı migration notları

---

## 📝 Lisans

Bu proje eğitim amaçlı geliştirilmiştir.

````
