# 🎯 Hata Yönetimi Kullanım Kılavuzu

Bu dokümantasyon, projede **tutarlı ve kullanıcı dostu hata yönetimi** için oluşturulmuş yapıyı açıklamaktadır.

## 📋 İçindekiler

- [Genel Bakış](#genel-bakış)
- [Backend Yapısı](#backend-yapısı)
  - [Result Sınıfları](#1-result-sınıfları)
  - [ApiResult Sınıfı](#2-apiresult-sınıfı)
  - [Handler'da Kullanım](#3-handlerda-kullanım)
  - [Controller'da Kullanım](#4-controllerda-kullanım)
- [Exception Handling Middleware](#exception-handling-middleware)
- [BaseApiController Helper Metodları](#baseapicontroller-helper-metodları)
- [Custom Exception Kullanımı](#custom-exception-kullanımı)
- [Frontend Yapısı](#frontend-yapısı)
  - [API Tipler](#1-api-tipler)
  - [Utility Fonksiyonlar](#2-utility-fonksiyonlar)
- [Kullanım Örnekleri](#kullanım-örnekleri)
- [Yeni Modül Ekleme](#yeni-modül-ekleme)
- [Best Practices](#best-practices)
- [Toast Özellikleri](#toast-özellikleri)
- [Debugging](#debugging)
- [SSS (Sık Sorulan Sorular)](#sss-sık-sorulan-sorular)

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

**Konum:** `src/BlogApp.Domain/Common/Results/`

#### `IResult` Interface
```csharp
public interface IResult
{
    bool Success { get; }
    string Message { get; }
    List<string> Errors { get; }  // ✨ Hata listesi
}
```

#### `IDataResult<T>` Interface
```csharp
public interface IDataResult<out T> : IResult
{
    T Data { get; }
}
```

#### `Result` Base Sınıfı
```csharp
public class Result : IResult
{
    // Constructor'lar
    public Result(bool success)
    public Result(bool success, string message)
    public Result(bool success, string message, List<string> errors)
    
    // Properties
    public bool Success { get; }
    public string Message { get; } = string.Empty;
    public List<string> Errors { get; } = new();
}
```

#### `ErrorResult` Sınıfı
```csharp
public class ErrorResult : Result
{
    public ErrorResult()  // Sadece başarısız işaret eder
    public ErrorResult(string message)  // Mesaj ile
    public ErrorResult(string message, List<string> errors)  // ✨ Mesaj + hata listesi
}
```

#### `SuccessResult` Sınıfı
```csharp
public class SuccessResult : Result
{
    public SuccessResult()  // Sadece başarılı işaret eder
    public SuccessResult(string message)  // Mesaj ile
}
```

#### `DataResult<T>` ve Alt Sınıflar

```csharp
// Data ile birlikte sonuç döndürme
public class DataResult<T> : Result, IDataResult<T>
{
    public T Data { get; }
}

// Başarılı data result
public class SuccessDataResult<T> : DataResult<T>
{
    public SuccessDataResult(T data)
    public SuccessDataResult(T data, string message)
}

// Hatalı data result
public class ErrorDataResult<T> : DataResult<T>
{
    public ErrorDataResult(T data)
    public ErrorDataResult(T data, string message)
    public ErrorDataResult(T data, string message, List<string> errors)
}
```

### 2. ApiResult Sınıfı

HTTP response'lar için kullanılan wrapper sınıf:

**Konum:** `src/BlogApp.Domain/Common/Results/ApiResult.cs`

```csharp
public class ApiResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string InternalMessage { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();  // ✨ Frontend'e gönderilen hatalar
}
```

### 3. Handler'da Kullanım

```csharp
public async Task<IResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
{
    // Validation kontrolü
    var validator = new RegisterCommandValidator();
    var validationResult = await validator.ValidateAsync(request, cancellationToken);
    
    if (!validationResult.IsValid)
    {
        // FluentValidation hataları
        List<string> errors = validationResult.Errors
            .Select(error => error.ErrorMessage)
            .ToList();
        
        return new ErrorResult("Doğrulama hatası", errors);
    }
    
    // İş mantığı
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
    
    // Başarılı
    return new SuccessResult("İşlem başarılı");
}
```

### 4. Controller'da Kullanım

```csharp
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterCommand command)
{
    IResult result = await Mediator.Send(command);
    
    // Otomatik olarak ApiResult'a çevrilir ve errors array korunur
    return GetResponseOnlyResult(result);
    
    // Manuel ApiResult oluşturma da mümkün:
    // if (!result.Success)
    // {
    //     var apiResult = new ApiResult<object>
    //     {
    //         Success = false,
    //         Message = result.Message,
    //         Errors = result.Errors.ToList()
    //     };
    //     return BadRequest(apiResult);
    // }
}
```

---

## 💻 Frontend Yapısı

### 1. API Tipler

**Konum:** `clients/blogapp-client/src/types/api.ts`

#### `ApiResult<T>` Interface
```typescript
export interface ApiResult<T = undefined> {
  success: boolean;
  message: string;
  data: T;
  internalMessage?: string;
  errors?: string[];  // ✨ Backend'den gelen hatalar
}
```

#### `ApiError` Interface
```typescript
export interface ApiError extends ApiResult<undefined> {
  statusCode?: number;
}
```

#### `normalizeApiResult<T>()` - Response Normalize
```typescript
// Backend'den gelen response'u normalize eder
export function normalizeApiResult<T>(data: any): ApiResult<T>

// Kullanım:
const response = await api.post<ApiResult>('/auth/register', values);
return normalizeApiResult(response.data);
```

#### `normalizeApiError()` - Error Normalize
```typescript
export function normalizeApiError(error: unknown, fallbackMessage = 'Beklenmeyen bir hata oluştu'): ApiError
```

### 2. Utility Fonksiyonlar

**Konum:** `clients/blogapp-client/src/lib/api-error.ts`

#### a) `getApiErrorMessage()` - Sadece mesaj al
```typescript
/**
 * API hatalarından mesajı çıkarır (toast göstermez)
 */
function getApiErrorMessage(error: unknown, fallbackMessage = 'Beklenmeyen bir hata oluştu'): string

// Kullanım: Mesajı almak ama toast göstermemek için
const message = getApiErrorMessage(error, 'Kategori silinemedi');
console.log(message);
```

#### b) `handleApiError()` - onError callback'de kullan
```typescript
/**
 * API hatalarını toast ile gösterir. Eğer errors array varsa liste formatında gösterir.
 * onError callback'lerde kullanılır.
 */
function handleApiError(error: unknown, fallbackMessage = 'Beklenmeyen bir hata oluştu'): string

// Kullanım:
const deleteMutation = useMutation({
  mutationFn: deleteCategory,
  onError: (error) => handleApiError(error, 'Kategori silinemedi')
});
```

**Özellikler:**
- Errors array varsa → Başlık + bullet liste formatı
- Errors array yoksa → Sadece mesaj
- Duration: 5000ms
- Max width: 500px
- Whiteapace: pre-line

#### c) `showApiResponseError()` - onSuccess callback'de kullan
```typescript
/**
 * API response'daki hataları toast ile gösterir (onSuccess callback'lerde kullanılır).
 * Eğer errors array varsa liste formatında gösterir.
 */
function showApiResponseError(response: ApiResult<any>, fallbackMessage = 'İşlem başarısız oldu'): void

// Kullanım:
const createMutation = useMutation({
  mutationFn: createCategory,
  onSuccess: (result) => {
    if (!result.success) {
      showApiResponseError(result, 'Kategori eklenemedi');
      return;
    }
    toast.success('Başarılı!');
  }
});
```

### 3. Type Guard

```typescript
// Error objesinin ApiError olup olmadığını kontrol eder
function isApiError(error: unknown): error is ApiError
```

---

## 📚 Kullanım Örnekleri

### Örnek 1: CRUD İşlemleri (Categories)

#### Backend - Handler

```csharp
// CREATE
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, IResult>
{
    public async Task<IResult> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Validation (FluentValidation otomatik çalışır)
        
        // Duplicate kontrolü
        bool exists = await _repository.AnyAsync(c => c.Name == request.Name);
        if (exists)
        {
            return new ErrorResult("Bu isimde bir kategori zaten mevcut");
        }
        
        // İşlem
        var category = _mapper.Map<Category>(request);
        await _repository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new SuccessResult("Kategori başarıyla eklendi");
    }
}

// UPDATE
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, IResult>
{
    public async Task<IResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(request.Id);
        if (category == null)
        {
            throw new NotFoundException("Kategori bulunamadı");
        }
        
        _mapper.Map(request, category);
        await _repository.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new SuccessResult("Kategori başarıyla güncellendi");
    }
}

// DELETE
public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, IResult>
{
    public async Task<IResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(request.Id);
        if (category == null)
        {
            throw new NotFoundException("Kategori bulunamadı");
        }
        
        // İlişkili kayıt kontrolü
        bool hasPosts = await _postRepository.AnyAsync(p => p.CategoryId == request.Id);
        if (hasPosts)
        {
            return new ErrorResult("Bu kategoriye ait gönderiler var, silemezsiniz");
        }
        
        await _repository.DeleteAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new SuccessResult("Kategori başarıyla silindi");
    }
}

// GET BY ID (with data)
public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, IDataResult<CategoryDto>>
{
    public async Task<IDataResult<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(request.Id);
        if (category == null)
        {
            return new ErrorDataResult<CategoryDto>(null, "Kategori bulunamadı");
        }
        
        var dto = _mapper.Map<CategoryDto>(category);
        return new SuccessDataResult<CategoryDto>(dto, "Kategori getirildi");
    }
}
```

#### Backend - Controller

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
{
    IResult result = await Mediator.Send(command);
    return GetResponseOnlyResult(result);  // 200 veya 400
}

[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryCommand command)
{
    command.Id = id;
    IResult result = await Mediator.Send(command);
    return GetResponseOnlyResult(result);
}

[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    IResult result = await Mediator.Send(new DeleteCategoryCommand(id));
    return GetResponseOnlyResult(result);
}

[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    IDataResult<CategoryDto> result = await Mediator.Send(new GetCategoryByIdQuery(id));
    return GetResponse(result);  // Data ile birlikte
}
```

#### Frontend - API Functions

```typescript
// src/features/categories/api.ts
import { ApiResult } from '@/types/api';
import { normalizeApiResult } from '@/types/api';
import api from '@/lib/api';

export async function createCategory(values: CategoryFormValues): Promise<ApiResult> {
  const response = await api.post<ApiResult>('/category', values);
  return normalizeApiResult(response.data);
}

export async function updateCategory(id: number, values: CategoryFormValues): Promise<ApiResult> {
  const response = await api.put<ApiResult>(`/category/${id}`, values);
  return normalizeApiResult(response.data);
}

export async function deleteCategory(id: number): Promise<ApiResult> {
  const response = await api.delete<ApiResult>(`/category/${id}`);
  return normalizeApiResult(response.data);
}

export async function getCategoryById(id: number): Promise<ApiResult<CategoryDto>> {
  const response = await api.get<ApiResult<CategoryDto>>(`/category/${id}`);
  return normalizeApiResult(response.data);
}
```

#### Frontend - Component (Mutations)

```typescript
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { handleApiError, showApiResponseError } from '@/lib/api-error';
import toast from 'react-hot-toast';

// CREATE
const createMutation = useMutation({
  mutationFn: createCategory,
  onSuccess: (result) => {
    if (!result.success) {
      showApiResponseError(result, 'Kategori eklenemedi');
      return;
    }
    toast.success(result.message || 'Kategori eklendi');
    queryClient.invalidateQueries({ queryKey: ['categories'] });
    navigate('/categories');
  },
  onError: (error) => handleApiError(error, 'Kategori eklenemedi')
});

// UPDATE
const updateMutation = useMutation({
  mutationFn: (values) => updateCategory(categoryId, values),
  onSuccess: (result) => {
    if (!result.success) {
      showApiResponseError(result, 'Kategori güncellenemedi');
      return;
    }
    toast.success(result.message || 'Kategori güncellendi');
    queryClient.invalidateQueries({ queryKey: ['categories'] });
    navigate('/categories');
  },
  onError: (error) => handleApiError(error, 'Kategori güncellenemedi')
});

// DELETE
const deleteMutation = useMutation({
  mutationFn: deleteCategory,
  onSuccess: (result) => {
    if (!result.success) {
      showApiResponseError(result, 'Kategori silinemedi');
      return;
    }
    toast.success(result.message || 'Kategori silindi');
    queryClient.invalidateQueries({ queryKey: ['categories'] });
  },
  onError: (error) => handleApiError(error, 'Kategori silinemedi')
});
```

---

### Örnek 2: Authentication (Multiple Errors)

#### Backend - Register Handler

```csharp
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, IResult>
{
    public async Task<IResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // User oluştur
        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        
        IdentityResult result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            // Identity hataları - birden fazla olabilir
            List<string> errors = result.Errors
                .Select(e => e.Description)
                .ToList();
            
            return new ErrorResult("Kullanıcı oluşturulurken hatalar oluştu", errors);
            // Örnek errors:
            // • Şifre en az 8 karakter olmalıdır
            // • Şifre en az bir büyük harf içermelidir
            // • Şifre en az bir rakam içermelidir
        }
        
        // Rol ata
        await _userManager.AddToRoleAsync(user, "User");
        
        return new SuccessResult("Kayıt başarılı");
    }
}
```

#### Frontend - Register Page

```typescript
const registerMutation = useMutation({
  mutationFn: register,
  onSuccess: (response) => {
    if (!response.success) {
      showApiResponseError(response, 'Kayıt başarısız oldu');
      // Toast çıktısı:
      // Kullanıcı oluşturulurken hatalar oluştu:
      // • Şifre en az 8 karakter olmalıdır
      // • Şifre en az bir büyük harf içermelidir
      // • Şifre en az bir rakam içermelidir
      return;
    }
    toast.success('Kayıt başarılı! Giriş yapabilirsiniz.');
    navigate('/login');
  },
  onError: (error) => handleApiError(error, 'Kayıt yapılamadı')
});
```

---

### Örnek 3: FluentValidation Hataları

#### Backend - Validator

```csharp
public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Başlık boş olamaz")
            .MaximumLength(200).WithMessage("Başlık en fazla 200 karakter olabilir");
        
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("İçerik boş olamaz")
            .MinimumLength(50).WithMessage("İçerik en az 50 karakter olmalıdır");
        
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Geçerli bir kategori seçiniz");
    }
}
```

#### Backend - Validation Behavior (Otomatik)

```csharp
// ApplicationServicesRegistration.cs
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// ValidationBehavior otomatik çalışır
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();
        
        if (failures.Count != 0)
        {
            var errors = failures.Select(f => f.ErrorMessage).ToList();
            throw new ValidationException(errors);  // Middleware yakalar
        }
        
        return await next();
    }
}
```

#### Frontend - Otomatik Handle

```typescript
// Validation hataları otomatik olarak errors array'de gelir
const createPostMutation = useMutation({
  mutationFn: createPost,
  onSuccess: (result) => {
    if (!result.success) {
      showApiResponseError(result, 'Gönderi eklenemedi');
      // Toast çıktısı:
      // Doğrulama hatası:
      // • Başlık boş olamaz
      // • İçerik en az 50 karakter olmalıdır
      // • Geçerli bir kategori seçiniz
      return;
    }
    toast.success('Gönderi eklendi');
  },
  onError: (error) => handleApiError(error, 'Gönderi eklenemedi')
});
```

---

### Örnek 4: Bulk Operations (Toplu İşlemler)

#### Backend - Bulk Delete

```csharp
public class BulkDeleteCategoriesCommandHandler : IRequestHandler<BulkDeleteCategoriesCommand, IResult>
{
    public async Task<IResult> Handle(BulkDeleteCategoriesCommand request, CancellationToken cancellationToken)
    {
        List<string> errors = new();
        int successCount = 0;
        
        foreach (var id in request.Ids)
        {
            try
            {
                var category = await _repository.GetByIdAsync(id);
                if (category == null)
                {
                    errors.Add($"ID {id}: Kategori bulunamadı");
                    continue;
                }
                
                bool hasPosts = await _postRepository.AnyAsync(p => p.CategoryId == id);
                if (hasPosts)
                {
                    errors.Add($"{category.Name}: Bu kategoriye ait gönderiler var");
                    continue;
                }
                
                await _repository.DeleteAsync(category);
                successCount++;
            }
            catch (Exception ex)
            {
                errors.Add($"ID {id}: {ex.Message}");
            }
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        if (errors.Any())
        {
            return new ErrorResult($"{successCount} kategori silindi, {errors.Count} hata oluştu", errors);
        }
        
        return new SuccessResult($"{successCount} kategori başarıyla silindi");
    }
}
```

#### Frontend - Bulk Delete

```typescript
const bulkDeleteMutation = useMutation({
  mutationFn: bulkDeleteCategories,
  onSuccess: (result) => {
    if (!result.success) {
      showApiResponseError(result, 'Bazı kategoriler silinemedi');
      // Toast çıktısı:
      // 2 kategori silindi, 3 hata oluştu:
      // • ID 5: Kategori bulunamadı
      // • Teknoloji: Bu kategoriye ait gönderiler var
      // • Spor: Bu kategoriye ait gönderiler var
      return;
    }
    toast.success(result.message);
    queryClient.invalidateQueries({ queryKey: ['categories'] });
  },
  onError: (error) => handleApiError(error, 'Kategoriler silinemedi')
});
```

---

### Örnek 5: Custom Exception Kullanımı

#### Backend - Custom Exception

```csharp
// Domain/Exceptions/DuplicateResourceException.cs
public class DuplicateResourceException : ApplicationException
{
    public DuplicateResourceException(string resourceName)
        : base($"{resourceName} zaten mevcut")
    {
    }
    
    public DuplicateResourceException(string resourceName, string fieldName, string fieldValue)
        : base($"{fieldName} = '{fieldValue}' olan {resourceName} zaten mevcut")
    {
    }
}

// Middleware'e ekle
DuplicateResourceException => SetApiResult(apiResult, StatusCodes.Status409Conflict, exception.Message),

// Handler'da kullan
if (await _repository.AnyAsync(c => c.Name == request.Name))
{
    throw new DuplicateResourceException("Kategori", "İsim", request.Name);
    // Response: { success: false, message: "İsim = 'Teknoloji' olan Kategori zaten mevcut" }
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

## 🤔 SSS (Sık Sorulan Sorular)

### 1. Result vs ApiResult ne zaman kullanılır?

- **`Result` / `IResult`**: Handler'larda ve domain/application layer'da kullanılır
- **`ApiResult`**: Controller'dan HTTP response olarak dönerken kullanılır
- `BaseApiController` otomatik olarak `IResult` → `ApiResult` dönüşümü yapar

### 2. Errors array ne zaman doldurulmalı?

Birden fazla hata olduğunda:
- ✅ FluentValidation hataları
- ✅ Identity hataları (şifre kuralları, kullanıcı oluşturma vb.)
- ✅ Toplu işlem hataları (bulk delete, bulk update vb.)
- ❌ Tek bir hata mesajı için kullanma → Sadece `message` property'sini kullan

### 3. Exception mı yoksa ErrorResult mı?

**Exception fırlat:**
- Beklenmeyen durumlar (null reference, sistem hatası vb.)
- Kritik hatalar (database bağlantı hatası vb.)
- Flow'u durdurmak istediğin durumlar

**ErrorResult döndür:**
- Beklenen hatalar (validation, business rule vb.)
- Kullanıcıya gösterilmesi gereken hatalar
- Normal flow içinde devam edebilecek durumlar

### 4. Frontend'de errors array nasıl gösterilir?

Otomatik olarak:
```typescript
// Backend
return new ErrorResult("Hatalar var", new List<string> { "Hata 1", "Hata 2" });

// Frontend - handleApiError veya showApiResponseError kullan
onError: (error) => handleApiError(error, 'İşlem başarısız')

// Toast çıktısı:
// Hatalar var:
// • Hata 1
// • Hata 2
```

### 5. Middleware hangi sırada çalışmalı?

```csharp
// Program.cs
app.UseMiddleware<ExceptionHandlingMiddleware>();  // İlk sırada olmalı
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

### 6. Custom exception nasıl oluşturulur?

```csharp
// 1. Domain/Exceptions altında oluştur
public class DuplicateResourceException : ApplicationException
{
    public DuplicateResourceException(string resourceName)
        : base($"{resourceName} zaten mevcut")
    {
    }
}

// 2. ExceptionHandlingMiddleware'e ekle
DuplicateResourceException => SetApiResult(apiResult, StatusCodes.Status409Conflict, exception.Message),

// 3. Kullan
throw new DuplicateResourceException("Kategori");
```

### 7. InternalMessage ne için kullanılır?

- **`Message`**: Kullanıcıya gösterilir (Turkish)
- **`InternalMessage`**: Debugging için (teknik detay, English)
- Production'da `InternalMessage` loglara yazılır ama kullanıcıya gösterilmez

```csharp
return BadRequest("İşlem başarısız", "Database connection timeout", null);
```

### 8. GetResponse vs GetResponseOnlyResult farkı nedir?

```csharp
// GetResponse - Data ile birlikte
IDataResult<CategoryDto> result = await Mediator.Send(query);
return GetResponse(result);  // { success, message, data, errors }

// GetResponseOnlyResult - Sadece durum
IResult result = await Mediator.Send(command);
return GetResponseOnlyResult(result);  // { success, message, errors }
```

### 9. Validation hataları nasıl handle edilir?

FluentValidation otomatik:
```csharp
// Handler'da validator çalışır
var validator = new CreateCategoryCommandValidator();
var validationResult = await validator.ValidateAsync(request);

if (!validationResult.IsValid)
{
    // Otomatik olarak ValidationException fırlatılır (FluentValidation.AspNetCore)
    // VEYA manuel:
    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
    return new ErrorResult("Doğrulama hatası", errors);
}
```

### 10. Rate limiting hatası nasıl gösterilir?

```csharp
// Middleware otomatik 429 döner
// Frontend:
if (error.statusCode === 429) {
  toast.error('Çok fazla istek gönderdiniz. Lütfen bekleyip tekrar deneyin.');
}
```

---

## 📚 İlgili Dokümantasyon

- [LOGGING_ARCHITECTURE.md](./LOGGING_ARCHITECTURE.md) - Loglama mimarisi
- [ADVANCED_FEATURES_IMPLEMENTATION.md](./ADVANCED_FEATURES_IMPLEMENTATION.md) - Gelişmiş özellikler
- [PERMISSION_GUARDS_GUIDE.md](./PERMISSION_GUARDS_GUIDE.md) - Yetkilendirme
- [TRANSACTION_MANAGEMENT_STRATEGY.md](./TRANSACTION_MANAGEMENT_STRATEGY.md) - Transaction yönetimi

---

## 🛡️ Exception Handling Middleware

### ExceptionHandlingMiddleware

Projede merkezi hata yönetimi için `ExceptionHandlingMiddleware` kullanılmaktadır. Bu middleware, tüm exception'ları yakalar ve standart `ApiResult` formatında döndürür.

**Konum:** `src/BlogApp.API/Middlewares/ExceptionHandlingMiddleware.cs`

#### Desteklenen Exception Türleri

| Exception Türü | HTTP Status | Açıklama |
|----------------|-------------|----------|
| `ValidationException` | 400 | FluentValidation hataları - errors array otomatik doldurulur |
| `BadRequestException` | 400 | Geçersiz istek parametreleri |
| `NotFoundException` | 404 | İstenen kaynak bulunamadı |
| `AuthenticationErrorException` | 401 | Kimlik doğrulama başarısız |
| `PasswordChangeFailedException` | 400 | Şifre değiştirme hatası |
| `InvalidOperationException` | 400 | Geçersiz operasyon |
| `ArgumentException` | 400 | Geçersiz argüman |
| Diğer Exception'lar | 500 | Beklenmeyen hatalar |

#### Middleware Davranışı

```csharp
// ValidationException için özel işlem
private static int BuildValidationError(ValidationException validationException, ApiResult<object> apiResult)
{
    apiResult.Message = validationException.Errors.FirstOrDefault() ?? "Geçersiz veya eksik bilgiler mevcut.";
    apiResult.Errors = validationException.Errors;  // ✨ Tüm hatalar errors array'ine eklenir
    return StatusCodes.Status400BadRequest;
}

// Diğer exception'lar için
private static int SetApiResult(ApiResult<object> apiResult, int statusCode, string message)
{
    apiResult.Message = string.IsNullOrWhiteSpace(message)
        ? "İsteğiniz işlenirken bir hata oluştu."
        : message;
    return statusCode;
}
```

#### Loglama

Her exception otomatik olarak Serilog ile loglanır:

```csharp
_logger.LogError(
    ex,
    "İşlenmeyen bir hata oluştu. Yol: {Path}, Metod: {Method}, Kullanıcı: {User}, IP: {RemoteIp}",
    request.Path,
    request.Method,
    user,
    context.Connection.RemoteIpAddress?.ToString()
);
```

---

## 🎯 BaseApiController Helper Metodları

`BaseApiController` sınıfı, standart HTTP response'lar için yardımcı metodlar sağlar.

### Success Responses (2xx)

```csharp
// 200 OK
protected IActionResult Success<T>(string message, string internalMessage, T data)
protected IActionResult Success<T>(ApiResult<T> data)

// 201 Created
protected IActionResult Created<T>(string message, string internalMessage, T data)
protected IActionResult Created<T>(ApiResult<T> data)

// 204 No Content (200 olarak döner)
protected IActionResult NoContent<T>(string message, string internalMessage, T data)
protected IActionResult NoContent<T>(ApiResult<T> data)
```

### Error Responses (4xx, 5xx)

```csharp
// 400 Bad Request
protected IActionResult BadRequest<T>(string message, string internalMessage, T data)
protected IActionResult BadRequest<T>(ApiResult<T> data)

// 401 Unauthorized
protected IActionResult Unauthorized<T>(string message, string internalMessage, T data)
protected IActionResult Unauthorized<T>(ApiResult<T> data)

// 403 Forbidden
protected IActionResult Forbidden<T>(string message, string internalMessage, T data)
protected IActionResult Forbidden<T>(ApiResult<T> data)

// 404 Not Found
protected IActionResult NotFound<T>(string message, string internalMessage, T data)
protected IActionResult NotFound<T>(ApiResult<T> data)

// 500 Internal Server Error
protected IActionResult Error<T>(string message, string internalMessage, T data)
protected IActionResult Error<T>(ApiResult<T> data)
```

### Result to Response Converters

```csharp
// IDataResult<T> → IActionResult
public IActionResult GetResponse<T>(IDataResult<T> result)

// IResult → IActionResult (sadece sonuç)
public IActionResult GetResponseOnlyResult(IResult result)

// IResult → IActionResult (sadece mesaj)
public IActionResult GetResponseOnlyResultMessage(IResult result)

// IDataResult<T> → IActionResult (sadece data)
public IActionResult GetResponseOnlyResultData<T>(IDataResult<T> result)
```

**Kullanım Örneği:**

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
{
    IResult result = await Mediator.Send(command);
    return GetResponseOnlyResult(result);  // Otomatik olarak 200 veya 400 döner
}

[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    IDataResult<CategoryDto> result = await Mediator.Send(new GetCategoryByIdQuery(id));
    return GetResponse(result);  // Data ile birlikte döner
}
```

---

## 📁 Custom Exception Kullanımı

Projede özel exception sınıfları bulunmaktadır. Handler'larda bu exception'ları kullanabilirsiniz.

**Konum:** `src/BlogApp.Domain/Exceptions/`

### Mevcut Custom Exception'lar

```csharp
// 404 Not Found
throw new NotFoundException("Kategori bulunamadı");

// 400 Bad Request
throw new BadRequestException("Geçersiz kategori ID'si");

// 401 Unauthorized
throw new AuthenticationErrorException("Oturum süresi doldu");

// 400 - Şifre değiştirme hatası
throw new PasswordChangeFailedException("Mevcut şifre yanlış");

// 400 - Validation hatası (errors array ile)
throw new ValidationException(new List<string> {
    "E-posta adresi geçerli değil",
    "Şifre en az 8 karakter olmalıdır"
});
```

### Yeni Exception Ekleme

Yeni bir exception tipi eklemek için:

1. `BlogApp.Domain/Exceptions/` altına yeni sınıf oluşturun:

```csharp
namespace BlogApp.Domain.Exceptions
{
    public class DuplicateException : ApplicationException
    {
        public DuplicateException(string message) 
            : base(message)
        {
        }
    }
}
```

2. `ExceptionHandlingMiddleware.cs` içinde exception mapping'e ekleyin:

```csharp
response.StatusCode = exception switch
{
    // ... mevcut mapping'ler ...
    DuplicateException => SetApiResult(apiResult, StatusCodes.Status409Conflict, exception.Message),
    // ...
};
```

---

**Son Güncelleme:** 28 Ekim 2025  
**Versiyon:** 2.0.0
