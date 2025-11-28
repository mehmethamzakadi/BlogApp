# Güvenlik Yapılandırması Rehberi

Bu rehber, BlogApp projesinde hassas bilgilerin (secret keys, şifreler, API anahtarları) nasıl güvenli bir şekilde yönetileceğini açıklar.

## 🔐 User Secrets Kurulumu

### 1. User Secrets'ı Başlat

```bash
cd src/BlogApp.API
dotnet user-secrets init
```

### 2. Gerekli Secret'ları Ayarla

```bash
# Veritabanı bağlantı bilgileri
dotnet user-secrets set "ConnectionStrings:BlogAppPostgreConnectionString" "Host=localhost;Port=5435;Database=BlogAppDb;Username=postgres;Password=YOUR_PASSWORD;Include Error Detail=true;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=100"

# Redis bağlantı bilgileri
dotnet user-secrets set "ConnectionStrings:RedisCache" "localhost:6379,abortConnect=false,connectTimeout=5000,syncTimeout=5000"

# JWT Token ayarları
dotnet user-secrets set "TokenOptions:SecurityKey" "YOUR_SUPER_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG!"
dotnet user-secrets set "TokenOptions:Audience" "https://localhost:5000"
dotnet user-secrets set "TokenOptions:Issuer" "https://localhost:5000"

# RabbitMQ
dotnet user-secrets set "RabbitMQOptions:UserName" "blogapp"
dotnet user-secrets set "RabbitMQOptions:Password" "YOUR_RABBITMQ_PASSWORD"

# Email (opsiyonel)
dotnet user-secrets set "EmailOptions:Username" "your-email@gmail.com"
dotnet user-secrets set "EmailOptions:Password" "YOUR_APP_PASSWORD"
```

### 3. Secret'ları Görüntüle

```bash
dotnet user-secrets list
```

### 4. Secret Sil

```bash
dotnet user-secrets remove "ConnectionStrings:BlogAppPostgreConnectionString"
```

---

## 🐳 Docker Environment Variables

Docker Compose ile çalışırken, `.env` dosyası kullanın:

### 1. `.env` Dosyası Oluştur

Proje kök dizininde `.env` dosyası oluşturun:

```bash
# .env dosyası (Git'e eklemeyin!)

# PostgreSQL
POSTGRES_DB=BlogAppDb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_secure_password

# RabbitMQ
RABBITMQ_DEFAULT_USER=blogapp
RABBITMQ_DEFAULT_PASS=your_rabbitmq_password

# Seq
SEQ_ADMIN_PASSWORD=your_seq_password

# JWT
TOKEN_SECURITY_KEY=your_super_secret_key_at_least_32_characters_long
```

### 2. `.env.example` Şablonu

```bash
# .env.example (Bu dosyayı Git'e ekleyin)

# PostgreSQL
POSTGRES_DB=BlogAppDb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=

# RabbitMQ
RABBITMQ_DEFAULT_USER=blogapp
RABBITMQ_DEFAULT_PASS=

# Seq
SEQ_ADMIN_PASSWORD=

# JWT
TOKEN_SECURITY_KEY=
```

---

## ☁️ Production Ortamı

### Azure Key Vault

Production ortamında Azure Key Vault kullanmanızı öneririz:

```csharp
// Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

### Environment Variables (Kubernetes/Docker)

```yaml
# kubernetes secret
apiVersion: v1
kind: Secret
metadata:
  name: blogapp-secrets
type: Opaque
stringData:
  ConnectionStrings__BlogAppPostgreConnectionString: "Host=..."
  TokenOptions__SecurityKey: "your-key"
```

---

## ⚠️ Güvenlik En İyi Pratikleri

1. **Asla** hassas bilgileri kaynak koduna eklemeyin
2. `.env` dosyasını `.gitignore`'a ekleyin
3. Minimum 32 karakter uzunluğunda security key kullanın
4. Production'da farklı, güçlü şifreler kullanın
5. Düzenli olarak secret'ları rotate edin
6. Erişim loglarını izleyin

---

## 📋 Kontrol Listesi

- [ ] User Secrets kuruldu
- [ ] `.env` dosyası oluşturuldu
- [ ] `.env` `.gitignore`'da mevcut
- [ ] Production secret'ları ayrı yönetiliyor
- [ ] Security key 32+ karakter

---

## 🔗 Faydalı Kaynaklar

- [ASP.NET Core User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Azure Key Vault](https://docs.microsoft.com/en-us/azure/key-vault/)
- [Docker Secrets](https://docs.docker.com/engine/swarm/secrets/)
