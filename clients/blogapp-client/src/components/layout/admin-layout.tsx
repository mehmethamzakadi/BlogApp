import { useState } from 'react';
import { Outlet } from 'react-router-dom';
import { motion } from 'framer-motion';
import { AdminSidebar } from '../admin/sidebar';
import { AdminHeader } from '../admin/admin-header';

export function AdminLayout() {
  const [collapsed, setCollapsed] = useState(false);

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
    </div>
  );
}
