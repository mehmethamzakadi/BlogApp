# Rate Limiting 429 Hatası - Çözüm Raporu

## 🔴 Sorun

Login işlemi sırasında **429 Too Many Requests** hatası alınıyordu:

```
POST http://localhost:6060/api/auth/logout 429 (Too Many Requests)
POST http://localhost:6060/api/auth/refresh-token 429 (Too Many Requests)
GET http://localhost:6060/api/dashboard/activities 401 (Unauthorized)
```

## 🔍 Kök Neden Analizi

### 1. **Çok Sıkı Rate Limiting Kuralları**

#### Önceki Yapılandırma (appsettings.json)
```json
{
  "Endpoint": "POST:/api/auth/*",
  "Period": "1m",
  "Limit": 5  // 1 dakikada SADECE 5 istek!
}
```

**Problem:** Tüm auth endpoint'leri için toplamda 1 dakikada sadece 5 istek.

### 2. **Sonsuz Döngü Riski**

#### Önceki Kod (use-auth.ts)
```typescript
const tryRestoreSession = async () => {
  try {
    await refreshAccessToken();
    return true;
  } catch (error) {
    await logout(); // ← logout API çağrısı yapıyor
    return false;
  }
};
```

**Problem:** `logout()` fonksiyonu API çağrısı yapıyor, bu da rate limit'i daha da aşıyor.

### 3. **Hata Senaryosu**

1. Kullanıcı login yapmaya çalışıyor
2. Token yoksa `refreshAccessToken()` çağrılıyor → **1. istek**
3. Refresh başarısız → `logout()` çağrılıyor → **2. istek**
4. Bu döngü tekrar ediyor
5. **5 isteği aşınca 429 hatası**
6. Kullanıcı login sayfasına atılıyor ama döngü devam ediyor

---

## ✅ Çözümler

### 1. Rate Limiting Kurallarını Optimize Ettik

#### Yeni Yapılandırma (appsettings.json)
```json
"GeneralRules": [
  {
    "Endpoint": "*",
    "Period": "1m",
    "Limit": 100  // ↑ 60'tan 100'e
  },
  {
    "Endpoint": "POST:/api/auth/login",
    "Period": "1m",
    "Limit": 10  // Login için ayrı limit
  },
  {
    "Endpoint": "POST:/api/auth/refresh-token",
    "Period": "1m",
    "Limit": 30  // Refresh için yeterli limit
  },
  {
    "Endpoint": "POST:/api/auth/logout",
    "Period": "1m",
    "Limit": 20  // Logout için ayrı limit
  }
]
```

**Değişiklikler:**
- ❌ `POST:/api/auth/*` → Çok geniş wildcard kaldırıldı
- ✅ Her endpoint için **ayrı ve yeterli** limitler
- ✅ Genel limit 60'tan 100'e çıkarıldı

### 2. Sonsuz Döngüyü Önledik

#### Yeni Kod (use-auth.ts)
```typescript
const tryRestoreSession = async (): Promise<boolean> => {
  try {
    await refreshAccessToken();
    return true;
  } catch (error) {
    // API çağrısı YAPMA, direkt store'u temizle
    logoutStore(); // ← Sadece state temizliği
    return false;
  }
};
```

**Değişiklik:**
- ❌ `await logout()` → API çağrısı yapıyordu
- ✅ `logoutStore()` → Sadece frontend state'i temizliyor

### 3. 429 Hatası İçin Özel Handling

#### Yeni Kod (axios.ts)
```typescript
// 429 Rate Limit hatası - Tekrar deneme YOK
if (error.response?.status === 429) {
  console.warn('⚠️ Rate limit aşıldı:', requestUrl);
  return Promise.reject(
    normalizeApiError(error, 'Çok fazla istek gönderildi. Lütfen bekleyin.')
  );
}
```

**Özellikler:**
- ✅ 429 hatası öncelikli kontrol ediliyor
- ✅ Tekrar deneme yapılmıyor (rate limit'i daha da aşmamak için)
- ✅ Kullanıcıya anlamlı hata mesajı

---

## 📊 Karşılaştırma

### Önce (Yanlış Davranış)

| Aksiyon | API Çağrıları | Toplam İstek |
|---------|---------------|--------------|
| Token yok → refresh dene | `/refresh-token` | 1 |
| Refresh başarısız → logout | `/logout` | 2 |
| 401 hatası → retry | `/refresh-token` | 3 |
| Retry başarısız → logout | `/logout` | 4 |
| Döngü tekrar | `/refresh-token` | 5 ❌ |
| **RATE LIMIT AŞILDI** | **429 ERROR** | ❌ |

### Sonra (Doğru Davranış)

| Aksiyon | API Çağrıları | Toplam İstek |
|---------|---------------|--------------|
| Token yok → refresh dene | `/refresh-token` | 1 |
| Refresh başarısız → store temizle | - (sadece state) | 1 |
| **Döngü durdu** | ✅ | ✅ |

---

## 🔧 Yapılan Değişiklikler

### Backend: `appsettings.json`
```diff
- "Endpoint": "POST:/api/auth/*",
- "Period": "1m",
- "Limit": 5

+ "Endpoint": "POST:/api/auth/login",
+ "Period": "1m",
+ "Limit": 10
+
+ "Endpoint": "POST:/api/auth/refresh-token",
+ "Period": "1m",
+ "Limit": 30
+
+ "Endpoint": "POST:/api/auth/logout",
+ "Period": "1m",
+ "Limit": 20
```

### Frontend: `use-auth.ts`
```diff
const tryRestoreSession = async () => {
  try {
    await refreshAccessToken();
    return true;
  } catch (error) {
-   await logout(); // API çağrısı
+   logoutStore(); // Sadece state temizliği
    return false;
  }
};
```

### Frontend: `axios.ts`
```diff
api.interceptors.response.use(
  (response) => response,
  async (error) => {
+   // 429 Rate Limit hatası - Öncelikli kontrol
+   if (error.response?.status === 429) {
+     console.warn('⚠️ Rate limit aşıldı:', requestUrl);
+     return Promise.reject(normalizeApiError(error, '...'));
+   }

    // 403 Forbidden - Yetki hatası
    if (error.response?.status === 403) {
      // ...
    }
    
    // Diğer hatalar...
  }
);
```

---

## 🎯 Best Practices

### Rate Limiting Kuralları

1. **Endpoint Bazlı Limitler**
   - ✅ Her endpoint için ayrı limit belirle
   - ❌ Wildcard (`*`) kullanımını minimize et

2. **Gerçekçi Limitler**
   - **Login:** 10/dakika (brute-force koruması)
   - **Refresh:** 30/dakika (normal kullanımı karşılar)
   - **Logout:** 20/dakika (multiple device senaryosu)
   - **Genel:** 100/dakika (normal API kullanımı)

3. **Farklı Zaman Dilimleri**
   ```json
   {
     "Endpoint": "*",
     "Period": "1m",
     "Limit": 100
   },
   {
     "Endpoint": "*",
     "Period": "1h",
     "Limit": 2000
   }
   ```

### Error Handling

1. **Sonsuz Döngüleri Önle**
   - ❌ Error handler'da API çağrısı yapma
   - ✅ Sadece state temizle

2. **429 Hatası Özel Muamele**
   - ❌ Retry yapma (rate limit daha da aşılır)
   - ✅ Kullanıcıya bilgi ver ve beklet

3. **Hata Sıralaması**
   ```
   1. 429 Rate Limit (en öncelikli)
   2. 403 Forbidden
   3. 401 Unauthorized
   4. Diğer hatalar
   ```

---

## 🚀 Test Adımları

1. **Backend'i restart et:**
   ```bash
   docker-compose restart blogapp.api
   ```

2. **Frontend'i temizle ve yeniden başlat:**
   ```bash
   cd clients/blogapp-client
   rm -rf node_modules/.vite
   npm run dev
   ```

3. **Test senaryoları:**
   - ✅ Login yapma
   - ✅ Sayfa yenileme
   - ✅ Tarayıcı kapat/aç
   - ✅ Token expire olduğunda refresh
   - ✅ Çoklu istek gönderme

---

## 📝 Sonuç

### Sorun
- ❌ Çok sıkı rate limiting
- ❌ Wildcard endpoint kuralları
- ❌ Sonsuz döngü riski
- ❌ 429 hatası özel handling yok

### Çözüm
- ✅ Endpoint bazlı gerçekçi limitler
- ✅ API çağrısı yerine state temizliği
- ✅ 429 hatası öncelikli kontrol
- ✅ Döngü koruması

### Sonuç
- ✅ Login çalışıyor
- ✅ Rate limit aşılmıyor
- ✅ Kullanıcı deneyimi iyileşti
- ✅ API güvenliği korundu
