import React from 'react';
import ReactDOM from 'react-dom/client';
import { RouterProvider } from 'react-router-dom';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClient } from './providers/query-client';
import { router } from './routes/router';
import { Toaster } from 'react-hot-toast';
import { ThemeProvider } from './providers/theme-provider';
import './index.css';

const rootElement = document.getElementById('root');

if (!rootElement) {
  throw new Error('Root element not found');
}

ReactDOM.createRoot(rootElement).render(
  <React.StrictMode>
    <ThemeProvider>
      <QueryClientProvider client={queryClient}>
        <RouterProvider router={router} />
        <Toaster 
        position="bottom-left"
        toastOptions={{
          duration: 4000,
          style: {
            background: 'hsl(var(--background))',
            color: 'hsl(var(--foreground))',
            border: '1px solid hsl(var(--border))',
            borderRadius: '0.75rem',
            padding: '1rem 1.25rem',
            fontSize: '0.875rem',
            fontWeight: '500',
            boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -4px rgba(0, 0, 0, 0.1)',
            maxWidth: '400px',
          },
          success: {
            duration: 3500,
            iconTheme: {
              primary: 'hsl(var(--primary))',
              secondary: 'hsl(var(--primary-foreground))',
            },
            style: {
              border: '1px solid hsl(var(--primary) / 0.3)',
              background: 'hsl(var(--background))',
            },
          },
          error: {
            duration: 4500,
            iconTheme: {
              primary: 'hsl(var(--destructive))',
              secondary: 'hsl(var(--destructive-foreground))',
            },
            style: {
              border: '1px solid hsl(var(--destructive) / 0.3)',
              background: 'hsl(var(--background))',
            },
          },
          loading: {
            iconTheme: {
              primary: 'hsl(var(--primary))',
              secondary: 'hsl(var(--primary-foreground))',
            },
          },
        }}
      />
    </QueryClientProvider>
    </ThemeProvider>
  </React.StrictMode>
);
