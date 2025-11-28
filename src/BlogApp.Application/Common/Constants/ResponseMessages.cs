namespace BlogApp.Application.Common.Constants;

/// <summary>
/// Centralized response messages for consistent user-facing messages.
/// Supports future localization by keeping all messages in one place.
/// </summary>
public static class ResponseMessages
{
    #region Generic Messages
    
    public static class Generic
    {
        public const string OperationSuccessful = "İşlem başarıyla tamamlandı.";
        public const string OperationFailed = "İşlem sırasında bir hata oluştu.";
        public const string NotFound = "Kayıt bulunamadı.";
        public const string AlreadyExists = "Bu kayıt zaten mevcut.";
        public const string InvalidOperation = "Geçersiz işlem.";
    }
    
    #endregion

    #region Post Messages
    
    public static class Post
    {
        public const string Created = "Post bilgisi başarıyla eklendi.";
        public const string Updated = "Post bilgisi başarıyla güncellendi.";
        public const string Deleted = "Post bilgisi başarıyla silindi.";
        public const string NotFound = "Post bilgisi bulunamadı!";
        public const string InvalidCategory = "Geçersiz kategori seçildi!";
    }
    
    #endregion

    #region Category Messages
    
    public static class Category
    {
        public const string Created = "Kategori bilgisi başarıyla eklendi.";
        public const string Updated = "Kategori bilgisi başarıyla güncellendi.";
        public const string Deleted = "Kategori bilgisi başarıyla silindi.";
        public const string NotFound = "Kategori bilgisi bulunamadı!";
        public const string AlreadyExists = "Bu kategori adı zaten mevcut!";
        public const string HasActivePosts = "Bu kategoriye ait aktif postlar bulunmaktadır. Önce postları silmeli veya başka kategoriye taşımalısınız.";
    }
    
    #endregion

    #region User Messages
    
    public static class User
    {
        public const string Created = "Kullanıcı bilgisi başarıyla eklendi.";
        public const string Updated = "Kullanıcı bilgisi başarıyla güncellendi.";
        public const string Deleted = "Kullanıcı bilgisi başarıyla silindi.";
        public const string NotFound = "Kullanıcı bulunamadı!";
        public const string EmailAlreadyExists = "Bu e-posta adresi zaten kullanılıyor!";
        public const string UsernameAlreadyExists = "Bu kullanıcı adı zaten kullanılıyor!";
    }
    
    #endregion

    #region Role Messages
    
    public static class Role
    {
        public const string Created = "Rol başarıyla eklendi.";
        public const string Updated = "Rol güncellendi.";
        public const string Deleted = "Rol başarıyla silindi.";
        public const string NotFound = "Rol bulunamadı!";
        public const string AlreadyExists = "Bu rol adı zaten mevcut!";
        
        public static string AlreadyExistsWithName(string roleName) 
            => $"Güncellemek istediğiniz {roleName} rolü sistemde mevcut!";
    }
    
    #endregion

    #region Permission Messages
    
    public static class Permission
    {
        public const string Assigned = "İzinler başarıyla atandı.";
        public const string RoleNotFound = "Rol bulunamadı";
    }
    
    #endregion

    #region Auth Messages
    
    public static class Auth
    {
        public const string RegisterSuccess = "Kayıt işlemi başarılı. Giriş yapabilirsiniz.";
        public const string LoginSuccess = "Giriş başarılı.";
        public const string LogoutSuccess = "Çıkış başarılı.";
        public const string InvalidCredentials = "Geçersiz kullanıcı adı veya şifre.";
        public const string EmailAlreadyRegistered = "Bu e-posta adresi zaten kullanılıyor!";
        public const string TokenRefreshed = "Token yenilendi.";
        public const string InvalidRefreshToken = "Geçersiz refresh token.";
    }
    
    #endregion

    #region BookshelfItem Messages
    
    public static class BookshelfItem
    {
        public const string Created = "Kitap kaydı başarıyla eklendi.";
        public const string Updated = "Kitap kaydı başarıyla güncellendi.";
        public const string Deleted = "Kitap kaydı başarıyla silindi.";
        public const string NotFound = "Kitap kaydı bulunamadı.";
    }
    
    #endregion
}
