import { Menu, SunMoon } from 'lucide-react';
import { motion } from 'framer-motion';
import { Button } from '../ui/button';
import { useAuth } from '../../hooks/use-auth';

interface AdminHeaderProps {
  onToggleSidebar: () => void;
  isCollapsed: boolean;
}

export function AdminHeader({ onToggleSidebar, isCollapsed }: AdminHeaderProps) {
  const { user, logout } = useAuth();

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
        <Button variant="ghost" size="icon">
          <SunMoon className="h-5 w-5" />
          <span className="sr-only">Tema Değiştir</span>
        </Button>
        <div className="flex flex-col items-end text-sm">
          <span className="font-medium">{user?.userName}</span>
          <button className="text-xs text-muted-foreground hover:text-primary" onClick={logout}>
            Çıkış Yap
          </button>
        </div>
      </div>
    </motion.header>
  );
}
