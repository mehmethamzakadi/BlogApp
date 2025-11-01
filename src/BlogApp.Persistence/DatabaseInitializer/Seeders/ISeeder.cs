namespace BlogApp.Persistence.DatabaseInitializer.Seeders;

/// <summary>
/// Seed işlemleri için base interface
/// Tüm seeder sınıfları bu interface'i implement etmelidir
/// </summary>
public interface ISeeder
{
    /// <summary>
    /// Seed işleminin öncelik sırası (düşük değer = önce çalışır)
    /// Role: 1, Permission: 2, RolePermission: 3, User: 4, UserRole: 5, vb.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Seed işleminin adı (loglama için)
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Seed işlemini gerçekleştirir
    /// İdempotent olmalıdır (birden fazla çalıştırılabilir)
    /// </summary>
    Task SeedAsync(CancellationToken cancellationToken = default);
}
