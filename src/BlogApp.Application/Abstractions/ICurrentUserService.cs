namespace BlogApp.Application.Abstractions;

/// <summary>
/// Mevcut kimliği doğrulanmış kullanıcının bilgilerine erişim sağlar
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Mevcut kimliği doğrulanmış kullanıcının ID'sini getirir
    /// </summary>
    /// <returns>Kimlik doğrulanmışsa kullanıcı ID'si, aksi takdirde null</returns>
    Guid? GetCurrentUserId();
}
