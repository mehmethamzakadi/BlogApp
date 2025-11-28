# BlogApp

<div align="center">

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-18.3-61DAFB?style=for-the-badge&logo=react&logoColor=black)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-Latest-DC382D?style=for-the-badge&logo=redis&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge&logo=docker&logoColor=white)

**Modern, ölçeklenebilir ve güvenli blog platformu**

[Özellikler](#-özellikler) •
[Mimari](#-mimari) •
[Kurulum](#-kurulum) •
[API Dokümantasyonu](#-api-dokümantasyonu) •
[Geliştirme](#-geliştirme)

</div>

---

## 📋 Genel Bakış

BlogApp, **Clean Architecture** ve **Domain-Driven Design (DDD)** prensiplerine dayalı, kurumsal düzeyde bir blog yönetim sistemidir. Modern teknolojiler ve en iyi pratikler kullanılarak geliştirilmiştir.

## ✨ Özellikler

### Backend
- 🏗️ **Clean Architecture** - Katmanlı mimari ile sürdürülebilir kod
- 📦 **DDD (Domain-Driven Design)** - Aggregate Root, Value Objects, Domain Events
- 🔄 **CQRS Pattern** - MediatR ile Command/Query ayrımı
- 🔐 **JWT Authentication** - Access Token & Refresh Token rotation
- 🛡️ **Permission-Based Authorization** - Granüler yetkilendirme sistemi
- 📬 **Outbox Pattern** - Güvenilir mesaj iletimi (RabbitMQ)
- ⚡ **Redis Caching** - Dağıtık önbellek desteği
- 📊 **Activity Logging** - Detaylı aktivite takibi
- 🔒 **Rate Limiting** - DDoS koruması
- 📝 **Serilog** - Yapılandırılmış loglama (Console, File, PostgreSQL, Seq)

### Frontend
- ⚛️ **React 18** - Modern UI framework
- 📘 **TypeScript** - Tip güvenli geliştirme
- 🎨 **Tailwind CSS** - Utility-first CSS framework
- 🔄 **TanStack Query** - Server state management
- 🐻 **Zustand** - Client state management
- 📝 **React Hook Form + Zod** - Form validation
- 🚀 **Vite** - Hızlı build tool

### DevOps
- 🐳 **Docker & Docker Compose** - Container orchestration
- 🔄 **CI/CD Ready** - Pipeline hazır yapı
- 📈 **Seq Integration** - Merkezi log yönetimi

---

## 🏛️ Mimari

```
┌─────────────────────────────────────────────────────────────────┐
│                        Presentation Layer                        │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │   BlogApp.API   │  │  React Client   │  │    Swagger UI   │  │
│  └────────┬────────┘  └────────┬────────┘  └─────────────────┘  │
└───────────┼────────────────────┼────────────────────────────────┘
            │                    │
┌───────────▼────────────────────▼────────────────────────────────┐
│                       Application Layer                          │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │              BlogApp.Application                         │    │
│  │  • Commands & Queries (CQRS)                            │    │
│  │  • Validators (FluentValidation)                        │    │
│  │  • Behaviors (Logging, Validation, Caching)             │    │
│  │  • AutoMapper Profiles                                  │    │
│  └─────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
            │
┌───────────▼─────────────────────────────────────────────────────┐
│                         Domain Layer                             │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │                BlogApp.Domain                            │    │
│  │  • Entities (User, Post, Category, Role, etc.)          │    │
│  │  • Value Objects (Email, UserName)                      │    │
│  │  • Domain Events                                        │    │
│  │  • Repository Interfaces                                │    │
│  │  • Domain Services                                      │    │
│  └─────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
            │
┌───────────▼─────────────────────────────────────────────────────┐
│                     Infrastructure Layer                         │
│  ┌──────────────────────┐  ┌──────────────────────┐             │
│  │ BlogApp.Infrastructure│  │ BlogApp.Persistence  │             │
│  │ • JWT Token Service   │  │ • EF Core DbContext  │             │
│  │ • Email Service       │  │ • Repositories       │             │
│  │ • Redis Cache         │  │ • Unit of Work       │             │
│  │ • RabbitMQ/MassTransit│  │ • Migrations         │             │
│  │ • Background Services │  │ • Seeders            │             │
│  └──────────────────────┘  └──────────────────────┘             │
└─────────────────────────────────────────────────────────────────┘
```

### Klasör Yapısı

```
BlogApp/
├── src/
│   ├── BlogApp.API/                 # REST API & Controllers
│   │   ├── Controllers/
│   │   ├── Middlewares/
│   │   ├── Filters/
│   │   └── Configuration/
│   ├── BlogApp.Application/         # Business Logic
│   │   ├── Features/
│   │   │   ├── Posts/
│   │   │   ├── Categories/
│   │   │   ├── Users/
│   │   │   ├── Roles/
│   │   │   └── Auths/
│   │   ├── Behaviors/
│   │   └── Abstractions/
│   ├── BlogApp.Domain/              # Core Domain
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Events/
│   │   ├── Repositories/
│   │   └── Services/
│   ├── BlogApp.Infrastructure/      # External Services
│   │   ├── Services/
│   │   ├── Consumers/
│   │   └── Authorization/
│   └── BlogApp.Persistence/         # Data Access
│       ├── Contexts/
│       ├── Repositories/
│       ├── Configurations/
│       └── Migrations/
├── clients/
│   └── blogapp-client/              # React Frontend
│       ├── src/
│       │   ├── components/
│       │   ├── features/
│       │   ├── hooks/
│       │   ├── pages/
│       │   └── stores/
│       └── ...
├── tests/
│   ├── Domain.UnitTests/
│   └── Application.UnitTests/
├── docs/                            # Documentation
└── deploy/                          # Docker & Nginx configs
```

---

## 🚀 Kurulum

### Gereksinimler

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker & Docker Compose](https://www.docker.com/)
- [PostgreSQL 16](https://www.postgresql.org/) (Docker ile otomatik)
- [Redis](https://redis.io/) (Docker ile otomatik)
- [RabbitMQ](https://www.rabbitmq.com/) (Docker ile otomatik)

### Docker ile Hızlı Başlangıç

```bash
# Repository'yi klonla
git clone https://github.com/mehmethamzakadi/BlogApp.git
cd BlogApp

# Tüm servisleri başlat
docker-compose up -d

# Logları izle
docker-compose logs -f blogapp.api
```

### Manuel Kurulum

#### 1. Veritabanı ve Servisleri Başlat

```bash
# Sadece bağımlılık servislerini başlat
docker-compose up -d postgresdb redis.cache rabbitmq seq
```

#### 2. Backend'i Çalıştır

```bash
cd src/BlogApp.API

# User secrets ayarla (ilk kez)
dotnet user-secrets set "ConnectionStrings:BlogAppPostgreConnectionString" "Host=localhost;Port=5435;Database=BlogAppDb;Username=postgres;Password=postgres"
dotnet user-secrets set "ConnectionStrings:RedisCache" "localhost:6379"
dotnet user-secrets set "TokenOptions:SecurityKey" "your-super-secret-key-here-at-least-32-chars!"

# Uygulamayı çalıştır
dotnet run
```

#### 3. Frontend'i Çalıştır

```bash
cd clients/blogapp-client

# Bağımlılıkları yükle
npm install

# .env.local dosyası oluştur
echo "VITE_API_URL=http://localhost:5000/api" > .env.local

# Development server başlat
npm run dev
```

### Environment Variables

| Değişken | Açıklama | Varsayılan |
|----------|----------|------------|
| `ASPNETCORE_ENVIRONMENT` | Ortam | `Development` |
| `ConnectionStrings__BlogAppPostgreConnectionString` | PostgreSQL bağlantısı | - |
| `ConnectionStrings__RedisCache` | Redis bağlantısı | - |
| `TokenOptions__SecurityKey` | JWT secret key | - |
| `RabbitMQOptions__HostName` | RabbitMQ host | `localhost` |
| `RabbitMQOptions__UserName` | RabbitMQ kullanıcı | `blogapp` |
| `RabbitMQOptions__Password` | RabbitMQ şifre | - |

---

## 📚 API Dokümantasyonu

### Endpoints

API başladığında Scalar UI üzerinden dokümantasyona erişebilirsiniz:

```
http://localhost:5000/scalar/v1
```

### Ana Endpoint'ler

| Endpoint | Method | Açıklama | Auth |
|----------|--------|----------|------|
| `/api/auth/login` | POST | Kullanıcı girişi | ❌ |
| `/api/auth/register` | POST | Kullanıcı kaydı | ❌ |
| `/api/auth/refresh-token` | POST | Token yenileme | ❌ |
| `/api/post` | GET | Post listesi | ❌ |
| `/api/post/{id}` | GET | Post detayı | ❌ |
| `/api/post` | POST | Post oluştur | ✅ |
| `/api/post/{id}` | PUT | Post güncelle | ✅ |
| `/api/post/{id}` | DELETE | Post sil | ✅ |
| `/api/category` | GET | Kategori listesi | ❌ |
| `/api/user` | GET | Kullanıcı listesi | ✅ |
| `/api/role` | GET | Rol listesi | ✅ |

### Örnek İstekler

#### Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@blogapp.com", "password": "Admin123!"}'
```

#### Post Oluşturma
```bash
curl -X POST http://localhost:5000/api/post \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "title": "Yeni Post",
    "body": "<p>İçerik</p>",
    "summary": "Özet",
    "thumbnail": "/uploads/image.jpg",
    "categoryId": "guid",
    "isPublished": true
  }'
```

---

## 🛠️ Geliştirme

### Geliştirme Ortamı Kurulumu

```bash
# Repository'yi klonla
git clone https://github.com/mehmethamzakadi/BlogApp.git
cd BlogApp

# Solution'ı restore et
dotnet restore

# Servisleri başlat
docker-compose -f docker-compose.local.yml up -d

# API'yi çalıştır
cd src/BlogApp.API
dotnet watch run
```

### Migration Oluşturma

```bash
cd src/BlogApp.API

# Yeni migration oluştur
dotnet ef migrations add MigrationName -p ../BlogApp.Persistence -o Migrations/PostgreSql

# Migration uygula
dotnet ef database update -p ../BlogApp.Persistence
```

### Testleri Çalıştırma

```bash
# Tüm testleri çalıştır
dotnet test

# Coverage raporu ile
dotnet test --collect:"XPlat Code Coverage"
```

### Kod Kalitesi

```bash
# Format kontrolü
dotnet format --verify-no-changes

# Analyzer çalıştır
dotnet build /p:TreatWarningsAsErrors=true
```

---

## 📊 Monitoring

### Seq Log Viewer

```
http://localhost:5341
```

Varsayılan şifre: `Admin123!`

### RabbitMQ Management

```
http://localhost:15672
```

Kullanıcı/Şifre: `blogapp/supersecret`

### Redis Commander (Opsiyonel)

```bash
docker run -d -p 8081:8081 --name redis-commander \
  -e REDIS_HOSTS=local:redis.cache:6379 \
  rediscommander/redis-commander
```

---

## 🔐 Güvenlik

- **JWT Token Rotation:** Access ve Refresh token mekanizması
- **Password Hashing:** PBKDF2 ile güvenli şifre saklama
- **Rate Limiting:** IP bazlı istek sınırlama
- **CORS Policy:** Yapılandırılabilir origin kontrolü
- **SQL Injection:** Parametreli sorgular (EF Core)
- **XSS Protection:** Input validation ve sanitization

---

## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit yapın (`git commit -m 'feat: Add amazing feature'`)
4. Push yapın (`git push origin feature/amazing-feature`)
5. Pull Request açın

### Commit Mesajları

[Conventional Commits](https://www.conventionalcommits.org/) standardını kullanın:

- `feat:` Yeni özellik
- `fix:` Bug düzeltmesi
- `docs:` Dokümantasyon
- `refactor:` Kod iyileştirmesi
- `test:` Test ekleme
- `chore:` Bakım işleri

---

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

---

## 📞 İletişim

- **Proje Sahibi:** Mehmet Hamza Kadi
- **GitHub:** [@mehmethamzakadi](https://github.com/mehmethamzakadi)

---

<div align="center">

**BlogApp** ile ❤️ yapıldı

[⬆ Başa Dön](#blogapp)

</div>
