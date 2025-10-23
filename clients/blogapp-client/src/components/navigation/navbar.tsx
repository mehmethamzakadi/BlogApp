import { useState } from 'react';
import { Link, NavLink } from 'react-router-dom';
import { Menu, X } from 'lucide-react';
import { Button } from '../ui/button';
import { useAuth } from '../../hooks/use-auth';
import { cn } from '../../lib/utils';

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  cn(
    'px-3 py-2 text-sm font-medium transition-colors hover:text-primary',
    isActive ? 'text-primary' : 'text-muted-foreground'
  );

export function Navbar() {
  const [open, setOpen] = useState(false);
  const { isAuthenticated, logout, user } = useAuth();

  const handleLogout = () => {
    logout();
  };

  return (
    <header className="border-b bg-background/80 backdrop-blur">
      <div className="container flex items-center justify-between py-4">
        <Link to="/" className="text-xl font-semibold">
          BlogApp
        </Link>
        <nav className="hidden items-center gap-4 md:flex">
          <NavLink to="/" className={navLinkClass} end>
            Anasayfa
          </NavLink>
          {isAuthenticated ? (
            <>
              <NavLink to="/admin/dashboard" className={navLinkClass}>
                Admin
              </NavLink>
              <span className="text-sm text-muted-foreground">{user?.userName}</span>
              <Button variant="ghost" size="sm" onClick={handleLogout}>
                Çıkış
              </Button>
            </>
          ) : (
            <NavLink to="/login" className={navLinkClass}>
              Giriş
            </NavLink>
          )}
        </nav>
        <button
          className="md:hidden"
          onClick={() => setOpen((prev) => !prev)}
          aria-label="Menüyü Aç"
        >
          {open ? <X className="h-6 w-6" /> : <Menu className="h-6 w-6" />}
        </button>
      </div>
      {open && (
        <div className="border-t bg-background md:hidden">
          <nav className="container flex flex-col gap-2 py-4">
            <NavLink to="/" className={navLinkClass} end onClick={() => setOpen(false)}>
              Anasayfa
            </NavLink>
            {isAuthenticated ? (
              <>
                <NavLink
                  to="/admin/dashboard"
                  className={navLinkClass}
                  onClick={() => setOpen(false)}
                >
                  Admin
                </NavLink>
                <div className="flex items-center justify-between px-3 text-sm text-muted-foreground">
                  <span>{user?.userName}</span>
                  <Button variant="ghost" size="sm" onClick={handleLogout}>
                    Çıkış
                  </Button>
                </div>
              </>
            ) : (
              <NavLink to="/login" className={navLinkClass} onClick={() => setOpen(false)}>
                Giriş
              </NavLink>
            )}
          </nav>
        </div>
      )}
    </header>
  );
}
