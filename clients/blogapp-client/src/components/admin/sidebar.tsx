import { NavLink } from 'react-router-dom';
import { FolderKanban, LayoutDashboard } from 'lucide-react';
import { cn } from '../../lib/utils';

const links = [
  {
    to: '/admin/dashboard',
    label: 'Dashboard',
    icon: LayoutDashboard
  },
  {
    to: '/admin/categories',
    label: 'Kategoriler',
    icon: FolderKanban
  }
];

export function AdminSidebar({ collapsed }: { collapsed: boolean }) {
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
        {links.map((link) => (
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
