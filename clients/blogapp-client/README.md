# BlogApp React Client

Modern, production-ready React istemcisi. BlogApp REST API ile tam entegre edilmiş, TypeScript ve modern React teknolojileri kullanılarak geliştirilmiştir.

## 🚀 Hızlı Başlangıç

### Gereksinimler
- Node.js 18+ 
- npm veya yarn

### Kurulum

1. **Bağımlılıkları yükleyin:**
   ```bash
   npm install
   ```

2. **Ortam değişkenlerini yapılandırın:**
   
   `.env.example` dosyasını kopyalayarak `.env` oluşturun:
   ```bash
   cp .env.example .env
   ```
   
   Gerekli API adresini güncelleyin:
   ```env
   VITE_API_BASE_URL=http://localhost:5000/api
   ```

3. **Geliştirme sunucusunu başlatın:**
   ```bash
   npm run dev
   ```
   
   Uygulama varsayılan olarak `http://localhost:5173` adresinde çalışacaktır.

### Production Build

```bash
npm run build
```

Build çıktıları `dist/` klasöründe oluşturulur.

## 🛠️ Teknoloji Stack

### Core
- **React 18** - Modern React hooks ve features
- **TypeScript** - Type-safe development
- **Vite** - Lightning-fast build tool

### UI/UX
- **TailwindCSS** - Utility-first CSS framework
- **shadcn/ui** - High-quality React components
- **Lucide React** - Beautiful icon library
- **Framer Motion** - Smooth animations
- **React Hot Toast** - Elegant notifications

### State Management & Data Fetching
- **Zustand** - Lightweight state management (auth store)
- **TanStack Query (React Query)** - Server state management
- **Axios** - HTTP client with interceptors

### Routing & Forms
- **React Router v7** - Client-side routing
- **React Hook Form** - Performant form handling
- **Zod** - Runtime type validation

### Data Visualization
- **TanStack Table** - Powerful table component
- **Recharts** - Responsive charts
- **date-fns** - Date manipulation

## 📁 Proje Yapısı

```
src/
├── components/          # Reusable UI components
│   ├── ui/             # shadcn/ui components
│   ├── layout/         # Layout components (Header, Sidebar, etc.)
│   └── ...
├── features/           # Feature-based modules
│   ├── auth/           # Authentication (Login, Register, etc.)
│   ├── posts/          # Blog post management
│   ├── categories/     # Category management
│   └── dashboard/      # Dashboard & analytics
├── lib/               # Utility libraries
│   ├── api/           # API client & endpoints
│   ├── hooks/         # Custom React hooks
│   └── utils/         # Helper functions
├── store/             # Zustand stores
│   └── authStore.ts   # Authentication state
├── types/             # TypeScript type definitions
├── App.tsx            # Main app component
└── main.tsx           # Entry point
```

## 🎨 Özellikler

### ✅ Kimlik Doğrulama
- JWT-based authentication
- Automatic token refresh
- Protected routes
- Persistent login state (localStorage)
- Axios interceptors for auth headers

### ✅ Blog Yönetimi
- Post CRUD operations (Create, Read, Update, Delete)
- Rich text editing support
- Image upload
- Category assignment
- Tag management
- Draft/publish states

### ✅ Kategori Yönetimi
- Server-side sorting, filtering, pagination
- TanStack Table integration
- Real-time search
- Bulk operations

### ✅ Dashboard & Analytics
- Activity logs monitoring
- User statistics
- Charts and visualizations (Recharts)
- Recent activities feed

### ✅ UI/UX
- Responsive design (mobile-first)
- Dark mode support (optional)
- Loading states & skeletons
- Error handling with toast notifications
- Accessible components (ARIA compliant)

## 🔧 Yapılandırma

### Environment Variables

```env
# API Configuration
VITE_API_BASE_URL=http://localhost:5000/api

# Optional
VITE_ENABLE_DEV_TOOLS=true
```

### API Client

API client (`src/lib/api/client.ts`) otomatik olarak:
- Base URL configuration
- JWT token injection
- Token refresh on 401 errors
- Error handling and logging

## 🧪 Geliştirme

### Linting
```bash
npm run lint
```

### Type Checking
```bash
npm run build  # TypeScript type check dahil
```

### Preview Production Build
```bash
npm run preview
```

## 📚 Kullanım Örnekleri

### API Çağrısı (TanStack Query)

```typescript
import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api/client';

function Posts() {
  const { data, isLoading, error } = useQuery({
    queryKey: ['posts'],
    queryFn: () => api.get('/posts'),
  });

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return <div>{/* Render posts */}</div>;
}
```

### Form Validation (React Hook Form + Zod)

```typescript
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';

const schema = z.object({
  email: z.string().email(),
  password: z.string().min(8),
});

function LoginForm() {
  const { register, handleSubmit } = useForm({
    resolver: zodResolver(schema),
  });

  return <form onSubmit={handleSubmit(onSubmit)}>...</form>;
}
```

### State Management (Zustand)

```typescript
import { useAuthStore } from '@/store/authStore';

function Profile() {
  const { user, logout } = useAuthStore();

  return (
    <div>
      <p>Welcome, {user?.name}</p>
      <button onClick={logout}>Logout</button>
    </div>
  );
}
```

## 🔗 İlgili Bağlantılar

- [Ana README](../../README.md) - Genel proje bilgisi
- [API Documentation](http://localhost:5000/scalar/v1) - Scalar API docs
- [TailwindCSS Docs](https://tailwindcss.com/docs)
- [shadcn/ui Components](https://ui.shadcn.com/)
- [TanStack Query](https://tanstack.com/query/latest)

## 📝 Notlar

- API çağrıları için `withCredentials: true` kullanılıyor (cookie-based auth destekli)
- Token yenileme otomatik olarak axios interceptor tarafından yönetiliyor
- Protected route'lar için `ProtectedRoute` component'i kullanılıyor
- Form validation Zod schema'ları ile runtime type-safety sağlıyor

## 🚧 Gelecek Geliştirmeler

- [ ] i18n (Çoklu dil desteği)
- [ ] Dark mode toggle
- [ ] PWA support
- [ ] Offline mode
- [ ] Advanced search & filters
- [ ] Social sharing
- [ ] Comment system
- [ ] User profile management

## 📄 Lisans

Bu proje eğitim amaçlı geliştirilmiştir.
