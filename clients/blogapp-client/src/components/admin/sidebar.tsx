import { NavLink } from 'react-router-dom';
import { FolderKanban, LayoutDashboard, FileText, Users, Shield, Activity, BookOpenCheck } from 'lucide-react';
import { cn } from '../../lib/utils';
import { usePermission } from '../../hooks/use-permission';
import { Permissions } from '../../lib/permissions';

const links = [
  {
    to: '/admin/dashboard',
    label: 'Dashboard',
    icon: LayoutDashboard,
    requiredPermission: Permissions.DashboardView
  },
  {
    to: '/admin/categories',
    label: 'Kategoriler',
    icon: FolderKanban,
    requiredPermission: Permissions.CategoriesViewAll
  },
  {
    to: '/admin/posts',
    label: 'Gönderiler',
    icon: FileText,
    requiredPermission: Permissions.PostsViewAll
  },
  {
    to: '/admin/users',
    label: 'Kullanıcılar',
    icon: Users,
    requiredPermission: Permissions.UsersViewAll
  },
  {
    to: '/admin/roles',
    label: 'Roller & Yetkiler',
    icon: Shield,
    requiredPermission: Permissions.RolesViewAll
  },
  {
    to: '/admin/bookshelf',
    label: 'Kitaplık',
    icon: BookOpenCheck,
    requiredPermission: Permissions.BookshelfViewAll
  },
  {
    to: '/admin/activity-logs',
    label: 'Aktivite Logları',
    icon: Activity,
    requiredPermission: Permissions.ActivityLogsView
  }  
];

export function AdminSidebar({ collapsed }: { collapsed: boolean }) {
  const { hasPermission } = usePermission();

  // Permission'a göre menü itemlarını filtrele
  const visibleLinks = links.filter((link) => hasPermission(link.requiredPermission));

  return (
    <aside
      className={cn(
        'border-r bg-card transition-all duration-300',
        collapsed ? 'w-16' : 'w-64'
      )}
    >
      <div className="flex h-16 items-center justify-center border-b text-lg font-semibold">
        {collapsed ? 'BA' : 'BlogApp'}
      </div>
      <nav className="space-y-1 p-4">
        {visibleLinks.map((link) => (
          <NavLink
            key={link.to}
            to={link.to}
            className={({ isActive }) =>
              cn(
                'flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors hover:bg-muted',
                isActive ? 'bg-muted text-primary' : 'text-muted-foreground'
              )
            }
          >
            <link.icon className="h-5 w-5" />
            {!collapsed && <span>{link.label}</span>}
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}
