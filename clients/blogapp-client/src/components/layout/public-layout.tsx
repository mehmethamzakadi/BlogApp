import { Outlet } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Navbar } from '../navigation/navbar';
import { Footer } from './footer';

export function PublicLayout() {
  return (
    <div className="flex min-h-screen flex-col">
      <Navbar />
      <main className="flex-1">
        <motion.div
          initial={{ opacity: 0, y: 16 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.3 }}
          className="container py-10"
        >
          <Outlet />
        </motion.div>
      </main>
      <Footer />
    </div>
  );
}
