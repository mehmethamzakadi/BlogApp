namespace BlogApp.Application.Abstractions;

/// <summary>
/// Çalışma zamanında kullanılan aktör bilgisini sağlar ve gerektiğinde geçici olarak üzerine yazmaya
/// imkan tanır. HTTP istekleri dışındaki (ör. arka plan işler) senaryolarda audit alanlarının doğru
/// doldurulabilmesi için kullanılır.
/// </summary>
public interface IExecutionContextAccessor
{
    /// <summary>
    /// Geçerli yürütme bağlamındaki kullanıcı/aktör kimliğini döndürür.
    /// </summary>
    /// <returns>Bağlam tanımlıysa kullanıcı ID'si, aksi halde null.</returns>
    Guid? GetCurrentUserId();

    /// <summary>
    /// Belirtilen kullanıcı kimliğiyle geçici bir yürütme bağlamı başlatır.
    /// </summary>
    /// <param name="userId">Bağlam boyunca geçerli olacak kullanıcı kimliği.</param>
    /// <returns>Bağlam sona erdiğinde önceki değeri geri yükleyen disposable kapsam.</returns>
    IDisposable BeginScope(Guid userId);
}
