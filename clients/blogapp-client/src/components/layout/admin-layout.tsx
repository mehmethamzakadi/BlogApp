import { useEffect, useState } from 'react';
import { Outlet, useLocation } from 'react-router-dom';
import { motion } from 'framer-motion';
import { AdminSidebar } from '../admin/sidebar';
import { AdminHeader } from '../admin/admin-header';
import { ScrollToTopButton } from '../ui/scroll-to-top-button';

export function AdminLayout() {
  const [collapsed, setCollapsed] = useState(false);
  const location = useLocation();

  useEffect(() => {
    window.scrollTo({ top: 0, behavior: 'auto' });
  }, [location.pathname, location.search]);

  return (
    <div className="flex min-h-screen bg-muted/30">
      <AdminSidebar collapsed={collapsed} />
      <div className="flex flex-1 flex-col">
        <AdminHeader
          isCollapsed={collapsed}
          onToggleSidebar={() => setCollapsed((prev) => !prev)}
        />
        <motion.main
          className="flex-1 p-6"
          initial={{ opacity: 0, y: 16 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.25 }}
        >
          <Outlet />
        </motion.main>
      </div>
      <ScrollToTopButton />
    </div>
  );
}
