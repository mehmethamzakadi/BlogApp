import { Home, LogOut, Menu, Moon, Sun, UserCircle } from 'lucide-react';
import { Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Button } from '../ui/button';
import { useAuth } from '../../hooks/use-auth';
import { useThemeContext } from '../../providers/theme-provider';

interface AdminHeaderProps {
  onToggleSidebar: () => void;
  isCollapsed: boolean;
}

export function AdminHeader({ onToggleSidebar, isCollapsed }: AdminHeaderProps) {
  const { user, logout } = useAuth();
  const { theme, toggleTheme } = useThemeContext();

  return (
    <motion.header
      className="flex h-16 items-center justify-between border-b bg-background px-4"
      initial={{ y: -20, opacity: 0 }}
      animate={{ y: 0, opacity: 1 }}
    >
      <div className="flex items-center gap-3">
        <Button variant="ghost" size="icon" onClick={onToggleSidebar}>
          <Menu className="h-5 w-5" />
          <span className="sr-only">Menüyü Aç/Kapat</span>
        </Button>
        <span className="text-sm text-muted-foreground">
          {isCollapsed ? 'Panel' : 'Yönetim Paneli'}
        </span>
      </div>
      <div className="flex items-center gap-3">
        <Button variant="ghost" size="icon" asChild className="sm:hidden">
          <Link to="/">
            <Home className="h-5 w-5" />
            <span className="sr-only">Anasayfaya Dön</span>
          </Link>
        </Button>
        <Button variant="outline" size="sm" asChild className="hidden sm:flex">
          <Link to="/">Anasayfaya Dön</Link>
        </Button>
        <Button variant="ghost" size="icon" onClick={toggleTheme}>
          {theme === 'dark' ? (
            <Sun className="h-5 w-5" />
          ) : (
            <Moon className="h-5 w-5" />
          )}
          <span className="sr-only">Tema Değiştir</span>
        </Button>
        <div className="flex items-center gap-2 rounded-full border bg-muted/40 px-3 py-1.5 text-sm shadow-sm">
          <UserCircle className="h-5 w-5 text-primary" />
          <span className="font-medium text-foreground">{user?.userName}</span>
          <Button
            variant="ghost"
            size="sm"
            onClick={logout}
            className="-mr-2 h-7 rounded-full border px-2 text-xs"
          >
            <LogOut className="mr-1 h-3.5 w-3.5" /> Çıkış Yap
          </Button>
        </div>
      </div>
    </motion.header>
  );
}
