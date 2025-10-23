# BlogApp React Client

Modern, production-grade React istemcisi BlogApp API ile entegre olacak şekilde hazırlanmıştır.

## Başlangıç

1. Bağımlılıkları yükleyin:
   ```bash
   npm install
   ```
2. Ortam değişkenlerini ayarlayın:
   - `.env.example` dosyasını kopyalayarak `.env` oluşturun ve gerekli API adresini güncelleyin.
3. Geliştirme sunucusunu başlatın:
   ```bash
   npm run dev
   ```

## Özellikler

- Vite + React + TypeScript
- TailwindCSS, shadcn/ui ve lucide-react ile modern arayüz
- React Router v7 ile yönlendirme
- TanStack Query ve Axios ile veri yönetimi
- Zustand tabanlı oturum yönetimi, JWT desteği ve interceptor
- React Hook Form + Zod ile form doğrulama
- Kategori yönetimi için sunucu taraflı sıralama/filtreleme/sayfalama
