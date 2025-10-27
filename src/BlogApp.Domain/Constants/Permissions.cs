namespace BlogApp.Domain.Constants;

/// <summary>
/// Sistemdeki tüm permission'ları string constant olarak tanımlar.
/// Format: {ModuleName}.{PermissionType}
/// </summary>
public static class Permissions
{
    // Dashboard Permissions
    public const string DashboardView = "Dashboard.View";

    // User Management Permissions
    public const string UsersCreate = "Users.Create";
    public const string UsersRead = "Users.Read";
    public const string UsersUpdate = "Users.Update";
    public const string UsersDelete = "Users.Delete";
    public const string UsersViewAll = "Users.ViewAll";

    // Role Management Permissions
    public const string RolesCreate = "Roles.Create";
    public const string RolesRead = "Roles.Read";
    public const string RolesUpdate = "Roles.Update";
    public const string RolesDelete = "Roles.Delete";
    public const string RolesViewAll = "Roles.ViewAll";
    public const string RolesAssignPermissions = "Roles.AssignPermissions";

    // Post Management Permissions
    public const string PostsCreate = "Posts.Create";
    public const string PostsRead = "Posts.Read";
    public const string PostsUpdate = "Posts.Update";
    public const string PostsDelete = "Posts.Delete";
    public const string PostsViewAll = "Posts.ViewAll";
    public const string PostsPublish = "Posts.Publish";

    // Category Management Permissions
    public const string CategoriesCreate = "Categories.Create";
    public const string CategoriesRead = "Categories.Read";
    public const string CategoriesUpdate = "Categories.Update";
    public const string CategoriesDelete = "Categories.Delete";
    public const string CategoriesViewAll = "Categories.ViewAll";

    // Comment Management Permissions
    public const string CommentsCreate = "Comments.Create";
    public const string CommentsRead = "Comments.Read";
    public const string CommentsUpdate = "Comments.Update";
    public const string CommentsDelete = "Comments.Delete";
    public const string CommentsViewAll = "Comments.ViewAll";
    public const string CommentsModerate = "Comments.Moderate";

    // Bookshelf Management Permissions
    public const string BookshelfCreate = "Bookshelf.Create";
    public const string BookshelfRead = "Bookshelf.Read";
    public const string BookshelfUpdate = "Bookshelf.Update";
    public const string BookshelfDelete = "Bookshelf.Delete";
    public const string BookshelfViewAll = "Bookshelf.ViewAll";
    
    // Media Management Permissions
    public const string MediaUpload = "Media.Upload";

    // Activity Logs Permissions
    public const string ActivityLogsView = "ActivityLogs.View";

    /// <summary>
    /// Tüm permission'ları liste olarak döndürür. Seed işlemleri için kullanılır.
    /// </summary>
    public static List<string> GetAllPermissions()
    {
        return new List<string>
        {
            // Dashboard
            DashboardView,

            // Users
            UsersCreate, UsersRead, UsersUpdate, UsersDelete, UsersViewAll,

            // Roles
            RolesCreate, RolesRead, RolesUpdate, RolesDelete, RolesViewAll, RolesAssignPermissions,

            // Posts
            PostsCreate, PostsRead, PostsUpdate, PostsDelete, PostsViewAll, PostsPublish,

            // Categories
            CategoriesCreate, CategoriesRead, CategoriesUpdate, CategoriesDelete, CategoriesViewAll,

            // Comments
            CommentsCreate, CommentsRead, CommentsUpdate, CommentsDelete, CommentsViewAll, CommentsModerate,

            // Bookshelf
            BookshelfCreate, BookshelfRead, BookshelfUpdate, BookshelfDelete, BookshelfViewAll,

            // Media
            MediaUpload,

            // Activity Logs
            ActivityLogsView
        };
    }

    /// <summary>
    /// Admin rolü için tüm permission'ları döndürür
    /// </summary>
    public static List<string> GetAdminPermissions()
    {
        return GetAllPermissions();
    }

    /// <summary>
    /// User rolü için temel permission'ları döndürür
    /// </summary>
    public static List<string> GetUserPermissions()
    {
        return new List<string>
        {
            // User sadece kendi postlarını yönetebilir
            PostsCreate,
            PostsRead,
            PostsUpdate,
            
            // Kategorileri okuyabilir
            CategoriesRead,

            // Yorum yapabilir ve kendi yorumlarını yönetebilir
            CommentsCreate,
            CommentsRead,
            CommentsUpdate
        };
    }
}
