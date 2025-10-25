namespace BlogApp.Domain.Constants;

/// <summary>
/// CRUD operasyonları için permission tipleri
/// </summary>
public static class PermissionType
{
    public const string Create = "Create";
    public const string Read = "Read";
    public const string Update = "Update";
    public const string Delete = "Delete";
    public const string ViewAll = "ViewAll"; // Tüm kayıtları görme yetkisi
}
