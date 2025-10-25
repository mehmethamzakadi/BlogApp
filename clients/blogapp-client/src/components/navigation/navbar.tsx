import { useState } from 'react';
import { Link, NavLink } from 'react-router-dom';
import { LogOut, Menu, Moon, Sun, UserCircle, X } from 'lucide-react';
import { Button } from '../ui/button';
import { useAuth } from '../../hooks/use-auth';
import { cn } from '../../lib/utils';
import { useThemeContext } from '../../providers/theme-provider';

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  cn(
    'px-3 py-2 text-sm font-medium transition-colors hover:text-primary',
    isActive ? 'text-primary' : 'text-muted-foreground'
  );

export function Navbar() {
  const [open, setOpen] = useState(false);
  const { isAuthenticated, logout, user } = useAuth();
  const { theme, toggleTheme } = useThemeContext();

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
            <div className="flex items-center gap-3">
              <NavLink to="/admin/dashboard" className={navLinkClass}>
                Admin Paneli
              </NavLink>
              <Button
                variant="ghost"
                size="icon"
                onClick={toggleTheme}
                className="h-9 w-9 rounded-full border"
              >
                {theme === 'dark' ? (
                  <Sun className="h-4 w-4" />
                ) : (
                  <Moon className="h-4 w-4" />
                )}
                <span className="sr-only">Tema Değiştir</span>
              </Button>
              <div className="flex items-center gap-2 rounded-full border bg-muted/40 px-3 py-1 text-sm shadow-sm">
                <UserCircle className="h-4 w-4 text-primary" />
                <span className="font-medium text-foreground">{user?.userName}</span>
              </div>
              <Button
                variant="ghost"
                size="icon"
                onClick={handleLogout}
                className="h-9 w-9 rounded-full border"
              >
                <LogOut className="h-4 w-4" />
                <span className="sr-only">Çıkış Yap</span>
              </Button>
            </div>
          ) : (
            <div className="flex items-center gap-3">
              <Button
                variant="ghost"
                size="icon"
                onClick={toggleTheme}
                className="h-9 w-9 rounded-full border"
              >
                {theme === 'dark' ? (
                  <Sun className="h-4 w-4" />
                ) : (
                  <Moon className="h-4 w-4" />
                )}
                <span className="sr-only">Tema Değiştir</span>
              </Button>
              <NavLink to="/login" className={navLinkClass}>
                Giriş
              </NavLink>
              <NavLink to="/register" className={navLinkClass}>
                Kayıt Ol
              </NavLink>
            </div>
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
              <div className="flex flex-col gap-3">
                <NavLink
                  to="/admin/dashboard"
                  className={navLinkClass}
                  onClick={() => setOpen(false)}
                >
                  Admin Paneli
                </NavLink>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={toggleTheme}
                  className="justify-start gap-2"
                >
                  {theme === 'dark' ? (
                    <>
                      <Sun className="h-4 w-4" />
                      Açık Tema
                    </>
                  ) : (
                    <>
                      <Moon className="h-4 w-4" />
                      Koyu Tema
                    </>
                  )}
                </Button>
                <div className="flex items-center justify-between rounded-xl border bg-muted/40 px-3 py-2">
                  <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <UserCircle className="h-4 w-4 text-primary" />
                    <span className="font-medium text-foreground">{user?.userName}</span>
                  </div>
                  <Button
                    variant="ghost"
                    size="icon"
                    className="h-9 w-9 rounded-full border"
                    onClick={() => {
                      handleLogout();
                      setOpen(false);
                    }}
                  >
                    <LogOut className="h-4 w-4" />
                    <span className="sr-only">Çıkış Yap</span>
                  </Button>
                </div>
              </div>
            ) : (
              <div className="flex flex-col gap-3">
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={toggleTheme}
                  className="justify-start gap-2"
                >
                  {theme === 'dark' ? (
                    <>
                      <Sun className="h-4 w-4" />
                      Açık Tema
                    </>
                  ) : (
                    <>
                      <Moon className="h-4 w-4" />
                      Koyu Tema
                    </>
                  )}
                </Button>
                <NavLink to="/login" className={navLinkClass} onClick={() => setOpen(false)}>
                  Giriş
                </NavLink>
                <NavLink to="/register" className={navLinkClass} onClick={() => setOpen(false)}>
                  Kayıt Ol
                </NavLink>
              </div>
            )}
          </nav>
        </div>
      )}
    </header>
  );
}
