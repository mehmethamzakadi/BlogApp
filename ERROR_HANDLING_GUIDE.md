# 🎯 Hata Yönetimi Kullanım Kılavuzu

Bu dokümantasyon, projede **tutarlı ve kullanıcı dostu hata yönetimi** için oluşturulmuş yapıyı açıklamaktadır.

## 📋 İçindekiler

- [Genel Bakış](#genel-bakış)
- [Backend Yapısı](#backend-yapısı)
- [Frontend Yapısı](#frontend-yapısı)
- [Kullanım Örnekleri](#kullanım-örnekleri)
- [Yeni Modül Ekleme](#yeni-modül-ekleme)

---

## 🎨 Genel Bakış

### Özellikler

✅ **Tek toast ile birden fazla hata** - Liste formatında gösterilir  
✅ **Tutarlı API yapısı** - Backend'den frontend'e standart format  
✅ **Kullanıcı dostu** - Bullet liste formatı ile okunabilirlik  
✅ **Genişletilebilir** - Tüm modüllerde kullanılabilir  
✅ **Type-safe** - TypeScript ile tip güvenliği  

### Hata Mesajı Formatı

**Önceki format:**
```
Kullanıcı oluşturulurken hatalar oluştu: PasswordTooShort-Şifre en az 8 karakter olmalıdır.; PasswordRequiresNonAlphanumeric-...
```

**Yeni format:**
```
Kullanıcı oluşturulurken hatalar oluştu:
• Şifre en az 8 karakter olmalıdır.
• Şifre en az bir alfanumerik olmayan karakter içermelidir.
• Şifre en az bir rakam içermelidir.
• Şifre en az bir büyük harf içermelidir.
```

---

## 🔧 Backend Yapısı

### 1. Result Sınıfları

#### `IResult` Interface
```csharp
public interface IResult
{
    bool Success { get; }
    string Message { get; }
    List<string> Errors { get; }  // ✨ Yeni eklendi
}
```

#### `Result` Base Sınıfı
```csharp
public class Result : IResult
{
    public Result(bool success, string message)
    public Result(bool success, string message, List<string> errors)  // ✨ Yeni
    
    public bool Success { get; }
    public string Message { get; } = string.Empty;
    public List<string> Errors { get; } = new();  // ✨ Yeni
}
```

#### `ErrorResult` Sınıfı
```csharp
public class ErrorResult : Result
{
    public ErrorResult(string message)
    public ErrorResult(string message, List<string> errors)  // ✨ Yeni
}
```

### 2. Handler'da Kullanım

```csharp
public async Task<IResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
{
    // ... validation işlemleri ...
    
    IdentityResult creationResult = await userService.CreateAsync(user, request.Password);
    if (!creationResult.Succeeded)
    {
        // ❌ ESKİ YOL (kullanma):
        // string message = "Hatalar: " + string.Join("; ", errors);
        // return new ErrorResult(message);
        
        // ✅ YENİ YOL (önerilen):
        List<string> errors = creationResult.Errors
            .Select(error => error.Description)
            .ToList();
        
        return new ErrorResult("Kullanıcı oluşturulurken hatalar oluştu", errors);
    }
    
    return new SuccessResult("İşlem başarılı");
}
```

---

## 💻 Frontend Yapısı

### 1. API Tipler

`src/types/api.ts` dosyasında tanımlı:

```typescript
export interface ApiResult<T = undefined> {
  success: boolean;
  message: string;
  data: T;
  internalMessage?: string;
  errors?: string[];  // ✨ Backend'den gelen hatalar
}
```

### 2. Utility Fonksiyonlar

`src/lib/api-error.ts` dosyasında 3 fonksiyon:

#### a) `getApiErrorMessage()` - Sadece mesaj al
```typescript
const message = getApiErrorMessage(error, 'Varsayılan mesaj');
// Kullanım: Mesajı almak ama toast göstermemek için
```

#### b) `handleApiError()` - onError callback'de kullan
```typescript
const deleteMutation = useMutation({
  mutationFn: deleteCategory,
  onError: (error) => handleApiError(error, 'Kategori silinemedi')
});
```

#### c) `showApiResponseError()` - onSuccess callback'de kullan
```typescript
const createMutation = useMutation({
  mutationFn: createCategory,
  onSuccess: (result) => {
    if (!result.success) {
      showApiResponseError(result, 'Kategori eklenemedi');  // ✨
      return;
    }
    toast.success('Başarılı!');
  }
});
```

---

## 📚 Kullanım Örnekleri

### Örnek 1: Create/Update/Delete İşlemleri

```typescript
import { useMutation } from '@tanstack/react-query';
import { handleApiError, showApiResponseError } from '../../lib/api-error';
import { createCategory } from '../../features/categories/api';
import toast from 'react-hot-toast';

// ✅ CREATE
const createMutation = useMutation({
  mutationFn: createCategory,
  onSuccess: (result) => {
    if (!result.success) {
      showApiResponseError(result, 'Kategori eklenemedi');
      return;
    }
    toast.success(result.message || 'Kategori eklendi');
    // ... invalidate queries ...
  },
  onError: (error) => handleApiError(error, 'Kategori eklenemedi')
});

// ✅ UPDATE
const updateMutation = useMutation({
  mutationFn: (values) => updateCategory(categoryId, values),
  onSuccess: (result) => {
    if (!result.success) {
      showApiResponseError(result, 'Kategori güncellenemedi');
      return;
    }
    toast.success(result.message || 'Kategori güncellendi');
  },
  onError: (error) => handleApiError(error, 'Kategori güncellenemedi')
});

// ✅ DELETE
const deleteMutation = useMutation({
  mutationFn: deleteCategory,
  onSuccess: (result) => {
    if (!result.success) {
      showApiResponseError(result, 'Kategori silinemedi');
      return;
    }
    toast.success(result.message || 'Kategori silindi');
  },
  onError: (error) => handleApiError(error, 'Kategori silinemedi')
});
```

### Örnek 2: Authentication (Login/Register)

```typescript
const registerMutation = useMutation({
  mutationFn: register,
  onSuccess: (response) => {
    if (!response.success) {
      showApiResponseError(response, 'Kayıt başarısız oldu');
      return;
    }
    toast.success('Kayıt başarılı!');
    navigate('/login');
  },
  onError: (error) => handleApiError(error, 'Kayıt yapılamadı')
});
```

### Örnek 3: Form Validation Hataları

Backend'de FluentValidation hataları:

```csharp
// Backend: ValidationException otomatik olarak middleware'de yakalanır
// ApiResult.Errors array'i otomatik doldurulur

// Frontend: Aynı yapı çalışır!
onSuccess: (result) => {
  if (!result.success) {
    showApiResponseError(result, 'Form hataları var');
    // Otomatik olarak liste formatında gösterir:
    // • E-posta adresi geçerli değil
    // • Şifre en az 8 karakter olmalıdır
    return;
  }
}
```

---

## 🆕 Yeni Modül Ekleme

Yeni bir modül (örn: Comments, Users, etc.) eklerken aynı pattern'i kullanın:

### 1. Backend - Handler Oluştur

```csharp
public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, IResult>
{
    public async Task<IResult> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (hasErrors)
        {
            List<string> errors = new List<string>
            {
                "Yorum metni boş olamaz",
                "Yorum en az 10 karakter olmalıdır"
            };
            return new ErrorResult("Yorum oluşturulamadı", errors);
        }
        
        // ... işlem ...
        
        return new SuccessResult("Yorum başarıyla eklendi");
    }
}
```

### 2. Frontend - API Fonksiyonu

```typescript
// src/features/comments/api.ts
export async function createComment(values: CommentFormValues) {
  const response = await api.post<ApiResult>('/comment', values);
  return normalizeApiResult(response.data);
}
```

### 3. Frontend - Mutation Oluştur

```typescript
// src/pages/admin/comments-page.tsx
import { handleApiError, showApiResponseError } from '../../lib/api-error';

const createMutation = useMutation({
  mutationFn: createComment,
  onSuccess: (result) => {
    if (!result.success) {
      showApiResponseError(result, 'Yorum eklenemedi');
      return;
    }
    toast.success(result.message || 'Yorum eklendi');
    queryClient.invalidateQueries({ queryKey: ['comments'] });
  },
  onError: (error) => handleApiError(error, 'Yorum eklenemedi')
});
```

---

## ✨ Best Practices

### ✅ Yapılması Gerekenler

1. **Her zaman `showApiResponseError()` kullan** - `onSuccess` callback'lerde
2. **Her zaman `handleApiError()` kullan** - `onError` callback'lerde
3. **Anlamlı fallback mesajlar** - Kullanıcıya ne olduğunu açıkla
4. **Backend'de errors array'i doldur** - Detaylı hata bilgisi ver
5. **Tutarlı mesaj formatı** - Tüm modüllerde aynı yapı

### ❌ Yapılmaması Gerekenler

1. **Doğrudan `toast.error()` kullanma** - Utility fonksiyonları kullan
2. **Hataları birleştirme** - Backend'de ayrı ayrı gönder, frontend listeleyecek
3. **Error code'ları gösterme** - Sadece açıklama göster
4. **Çok uzun mesajlar** - Kısa ve öz ol

---

## 🎨 Toast Özellikleri

Errors array varsa otomatik olarak:
- ✅ **Duration:** 5000ms (5 saniye)
- ✅ **Max Width:** 500px
- ✅ **Format:** Bullet liste
- ✅ **Whitespace:** Pre-line (her hata yeni satırda)

---

## 🔍 Debugging

Hata mesajlarını debug etmek için:

```typescript
// Console'da görmek için
onSuccess: (result) => {
  if (!result.success) {
    console.log('API Errors:', result.errors);
    console.log('API Message:', result.message);
    showApiResponseError(result, 'İşlem başarısız');
    return;
  }
}
```

---

## 📝 Notlar

- Bu yapı **tüm projeye** uygulanmıştır
- Mevcut modüller: **Auth, Categories, Posts**
- Yeni modüller için **aynı pattern** kullanılmalıdır
- Backend'de `Result`, `ErrorResult`, `SuccessResult` sınıfları hazır
- Frontend'de `handleApiError`, `showApiResponseError` fonksiyonları hazır

---

## 🚀 Özet

**Backend:**
```csharp
return new ErrorResult("Mesaj", new List<string> { "Hata 1", "Hata 2" });
```

**Frontend:**
```typescript
if (!result.success) {
  showApiResponseError(result, 'Varsayılan mesaj');
  return;
}
```

**Sonuç:**
```
Toast mesajı:
Mesaj:
• Hata 1
• Hata 2
```

---

**Son Güncelleme:** 25 Ekim 2025  
**Versiyon:** 1.0.0
