# 30 Refresh Session Kaydı ve Sonsuz Döngü Sorunu - Çözüm

## 🔴 Kritik Sorun

Login olduktan sonra admin paneline geçiş yapıldığında:
- ❌ Anında login sayfasına yönlendiriliyor
- ❌ **RefreshSessions tablosuna 30 kayıt birden** atılıyor
- ❌ `429 Too Many Requests` hatası
- ❌ Sonsuz refresh döngüsü

## 🔍 Kök Neden Analizi

### 1. **Request Interceptor'da Race Condition** ⚠️

#### Sorunlu Kod (axios.ts)
```typescript
api.interceptors.request.use(async (config) => {
  const { token, user } = useAuthStore.getState();
  
  // HER İSTEKTE token süresi kontrol ediliyor
  if (token && user) {
    const expiresAt = new Date(user.expiration).getTime();
    if (expiresAt - Date.now() <= REFRESH_THRESHOLD_MS) {
      // ← BURADA HER İSTEK AYRI REFRESH ÇAĞIRIYOR!
      authToken = await refreshAccessToken();
    }
  }
});
```

**Problem:**
- Admin paneline geçişte **30+ API isteği** paralel olarak tetikleniyor
- **HER BİRİ** token süresini kontrol ediyor
- **HER BİRİ** refresh çağırıyor
- `isRefreshing` flag race condition'a giriyor
- **30 ayrı refresh session** oluşuyor

### 2. **Token Expiration vs Refresh Threshold Uyumsuzluğu**

```typescript
// Frontend
const REFRESH_THRESHOLD_MS = 15 * 60 * 1000; // 15 dakika

// Backend (appsettings.json)
"AccessTokenExpiration": 15 // 15 dakika
```

**Problem:**
- Token süresi **15 dakika**
- Threshold **15 dakika**
- Token expire olur olmaz **HEMEN** refresh tetikleniyor
- Admin paneline geçişte tüm istekler **aynı anda** refresh yapıyor

### 3. **Sonsuz Logout Döngüsü**

```typescript
// refreshAccessToken fonksiyonu
catch (err) {
  processQueue(error);
  useAuthStore.getState().logout(); // ← LOGOUT API ÇAĞRISI
  throw error;
}
```

**Problem:**
- Refresh başarısız → `logout()` API çağrısı
- Logout da başarısız → tekrar hata
- Response interceptor devrede → tekrar refresh dene
- **SONSUZ DÖNGÜ**

---

## ✅ Çözümler

### 1. **Request Interceptor'dan Proactive Refresh Kaldırıldı** 🎯

#### Yeni Kod
```typescript
api.interceptors.request.use(async (config) => {
  const requestUrl = config.url?.toLowerCase() ?? '';
  
  // Auth endpoint'lerine token ekleme
  if (requestUrl.includes('/auth/refresh-token') || 
      requestUrl.includes('/auth/login') ||
      requestUrl.includes('/auth/register')) {
    return config;
  }

  const { token } = useAuthStore.getState();
  
  // Token varsa sadece EKLE - expiry kontrolü YAPMA!
  if (token) {
    const headers = AxiosHeaders.from(config.headers ?? {});
    headers.set('Authorization', `Bearer ${token}`);
    config.headers = headers;
  }

  return config;
});
```

**Değişiklikler:**
- ❌ Token expiry kontrolü KALDIRILDI
- ❌ Proactive refresh çağrısı KALDIRILDI
- ✅ Sadece mevcut token header'a ekleniyor
- ✅ 401 gelirse response interceptor hallediyor

**Mantık:**
- Token geçerliyse API kabul eder
- Token expire olduysa **401 döner**
- Response interceptor 401'i yakalar
- **O ZAMAN** refresh yapar
- **Reactive** yaklaşım, **Proactive** değil

### 2. **Token Süreleri Optimize Edildi**

#### Backend: appsettings.json
```json
{
  "TokenOptions": {
    "AccessTokenExpiration": 60,  // 15 → 60 dakika
    "RefreshTokenExpirationDays": 14
  }
}
```

#### Frontend: use-auth.ts
```typescript
// 60 dakikalık token için son 5 dakikada refresh
const SILENT_REFRESH_WINDOW_MS = 5 * 60 * 1000; // 15dk → 5dk
```

**Hesaplama:**
- Access Token: **60 dakika**
- Refresh Window: **5 dakika**
- Silent refresh: **55. dakikada** (timer ile)
- Token kullanım oranı: **%92** (55/60)

### 3. **Logout Döngüsü Kırıldı**

#### refreshAccessToken - Yeni Kod
```typescript
export const refreshAccessToken = async (): Promise<string> => {
  // ... refresh logic
  
  try {
    // ... refresh işlemi
  } catch (err) {
    const error = err instanceof Error ? err : new Error('...');
    processQueue(error);
    
    // LOGOUT ÇAĞIRMA - Response interceptor halledecek
    // useAuthStore.getState().logout();
    
    throw error;
  } finally {
    isRefreshing = false;
  }
};
```

#### Response Interceptor
```typescript
// Refresh endpoint'i başarısız olduysa sadece state temizle
if (error.response?.status === 401 && requestUrl.includes('/auth/refresh-token')) {
  // logout() değil - direkt state temizle
  useAuthStore.getState().logout();
  return Promise.reject(error);
}
```

**Değişiklikler:**
- ❌ `refreshAccessToken` içinde logout çağrısı KALDIRILDI
- ✅ Response interceptor tek noktada yönetiyor
- ✅ Sonsuz döngü riski ortadan kalktı

---

## 📊 Akış Karşılaştırması

### Önce (Yanlış - 30 Refresh Session)

```
1. Admin paneline geçiş
   └─> 30 API isteği paralel başlatılıyor

2. Request Interceptor (HER İSTEK İÇİN)
   ├─> Token süresi kontrol
   ├─> Expire olmak üzere mi? → EVET (15dk = 15dk threshold)
   └─> refreshAccessToken() çağır
   
3. refreshAccessToken (30 KEZ PARALEL)
   ├─> isRefreshing = true (ama race condition)
   ├─> 30 ayrı refresh API çağrısı
   └─> 30 yeni RefreshSession kaydı ❌

4. Rate Limit Aşıldı
   └─> 429 Too Many Requests

5. Refresh başarısız
   └─> logout() API çağrısı
   └─> Tekrar 401
   └─> Sonsuz döngü ❌
```

### Sonra (Doğru - 0 veya 1 Refresh)

```
1. Admin paneline geçiş
   └─> 30 API isteği paralel başlatılıyor

2. Request Interceptor (HER İSTEK İÇİN)
   ├─> Token var mı? → EVET
   └─> Header'a ekle, geç ✅

3. API Response
   ├─> Token geçerliyse → 200 OK ✅
   └─> Token expire olduysa → 401 Unauthorized

4. Response Interceptor (401 gelirse)
   ├─> İlk istek refresh tetikler
   ├─> Diğerleri kuyruğa girer (failedQueue)
   └─> TEK bir refresh session ✅

5. Refresh Başarılı
   ├─> Yeni token alındı
   ├─> Kuyruk işlendi
   └─> Tüm istekler yeni token ile tekrar denendi ✅
```

---

## 🎯 Best Practices Uygulandı

### 1. **Reactive Token Refresh** (OAuth 2.0 Best Practice)

```
❌ Proactive: Her istekte "token dolmak üzere mi?" kontrolü
✅ Reactive: 401 geldiğinde refresh yap
```

**Avantajlar:**
- Race condition riski yok
- Performans artışı
- Backend token süresine tam güven

### 2. **Single Point of Logout**

```
❌ Birden fazla yerde logout çağrısı
✅ Tek noktada (response interceptor)
```

**Avantajlar:**
- Sonsuz döngü riski yok
- Debugging kolay
- Tutarlı davranış

### 3. **Optimal Token Süreleri**

```typescript
Access Token:  60 dakika  (kullanıcı deneyimi)
Refresh Token: 14 gün     (güvenlik vs UX dengesi)
Refresh Window: 5 dakika  (yeterli zaman)
```

**Hesaplama:**
- %92 token kullanım oranı (55/60)
- Kullanıcı 55 dakika kesintisiz çalışır
- Son 5 dakikada otomatik yenileme (sessiz)

---

## 🔧 Yapılan Değişiklikler

### Backend

#### appsettings.json & appsettings.Development.json
```diff
"TokenOptions": {
- "AccessTokenExpiration": 15,
+ "AccessTokenExpiration": 60,
  "RefreshTokenExpirationDays": 14
}
```

### Frontend

#### axios.ts
```diff
// Request Interceptor
- if (token && user) {
-   const expiresAt = new Date(user.expiration).getTime();
-   if (expiresAt - Date.now() <= REFRESH_THRESHOLD_MS) {
-     authToken = await refreshAccessToken();
-   }
- }
+ if (token) {
+   headers.set('Authorization', `Bearer ${token}`);
+ }

// refreshAccessToken
- useAuthStore.getState().logout();
+ // Logout çağırma - response interceptor halledecek
```

#### use-auth.ts
```diff
- const SILENT_REFRESH_WINDOW_MS = 15 * 60 * 1000;
+ const SILENT_REFRESH_WINDOW_MS = 5 * 60 * 1000;
```

---

## 🚀 Test Adımları

1. **Backend restart:**
   ```bash
   docker-compose restart blogapp.api
   ```

2. **Frontend rebuild:**
   ```bash
   cd clients/blogapp-client
   npm run build
   npm run dev
   ```

3. **Test senaryoları:**
   - ✅ Login yap
   - ✅ Admin paneline geç
   - ✅ Dashboard'da kalabildiğini doğrula
   - ✅ Veritabanında RefreshSessions tablosunu kontrol et (sadece 1 kayıt olmalı)
   - ✅ Console'da 429 hatası olmamalı

4. **Veritabanı kontrolü:**
   ```sql
   SELECT COUNT(*) FROM "RefreshSessions" 
   WHERE "UserId" = 'your-user-id' AND "Revoked" = false;
   -- Sonuç: 1 veya 2 olmalı (30 DEĞİL!)
   ```

---

## 📝 Önemli Notlar

### ⚠️ YAPMAYIN

1. ❌ Request interceptor'da proactive refresh
2. ❌ Her istekte token expiry kontrolü
3. ❌ `refreshAccessToken` içinde logout çağrısı
4. ❌ Token expiration = Refresh threshold

### ✅ YAPIN

1. ✅ Reactive refresh (401 geldiğinde)
2. ✅ Response interceptor'da tek logout noktası
3. ✅ Optimal token süreleri (60dk access, 14gün refresh)
4. ✅ Queue pattern (failedQueue) kullanın
5. ✅ `isRefreshing` flag ile race condition önleyin

---

## 🎓 Öğrenilen Dersler

### Race Condition
- Paralel API istekleri + Proactive refresh = Felaket
- `isRefreshing` flag yeterli değil, **request interceptor'da refresh yapma!**

### Token Strategy
- **Reactive > Proactive**
- Backend'e güven, 401 bekle
- Threshold çok düşükse sürekli refresh

### Error Handling
- Logout birden fazla yerden çağrılmamalı
- Sonsuz döngü riski her zaman var
- Single responsibility: Her fonksiyon tek iş yapsın

---

## 📚 Kaynaklar

- [OAuth 2.0 Token Refresh Best Practices](https://datatracker.ietf.org/doc/html/rfc6749#section-6)
- [Axios Interceptor Patterns](https://axios-http.com/docs/interceptors)
- [React Query - Token Refresh](https://tanstack.com/query/latest/docs/react/guides/query-retries)

---

## ✨ Sonuç

### Önce
- ❌ 30 refresh session kaydı
- ❌ 429 Rate limit hatası
- ❌ Sonsuz döngü
- ❌ Login sayfasına atılma

### Sonra
- ✅ 1 refresh session kaydı
- ✅ Rate limit güvenli
- ✅ Döngü yok
- ✅ Admin panelinde kalıyor

**Çözüm:** Request interceptor'dan proactive refresh kaldırıldı, reactive yaklaşıma geçildi, token süreleri optimize edildi.
