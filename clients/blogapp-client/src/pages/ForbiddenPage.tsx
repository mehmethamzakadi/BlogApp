import { Link } from 'react-router-dom';
import { ShieldAlert, Home, ArrowLeft } from 'lucide-react';
import { Button } from '../components/ui/button';

export default function ForbiddenPage() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-red-50 to-orange-50 dark:from-gray-900 dark:to-gray-800 px-4">
      <div className="max-w-md w-full text-center">
        <div className="mb-8 flex justify-center">
          <div className="relative">
            <div className="absolute inset-0 bg-red-500/20 blur-3xl rounded-full" />
            <ShieldAlert className="w-24 h-24 text-red-500 relative" strokeWidth={1.5} />
          </div>
        </div>

        <h1 className="text-6xl font-bold text-gray-900 dark:text-white mb-4">403</h1>
        
        <h2 className="text-2xl font-semibold text-gray-800 dark:text-gray-200 mb-4">
          Erişim Engellendi
        </h2>
        
        <p className="text-gray-600 dark:text-gray-400 mb-8">
          Bu sayfaya veya kaynağa erişim için yetkiniz bulunmamaktadır. 
          Eğer bu sayfaya erişmeniz gerektiğini düşünüyorsanız, 
          lütfen sistem yöneticinizle iletişime geçin.
        </p>

        <div className="flex flex-col sm:flex-row gap-4 justify-center">
          <Button
            variant="outline"
            onClick={() => window.history.back()}
            className="inline-flex items-center gap-2"
          >
            <ArrowLeft className="w-4 h-4" />
            Geri Dön
          </Button>
          
          <Button asChild>
            <Link to="/" className="inline-flex items-center gap-2">
              <Home className="w-4 h-4" />
              Ana Sayfaya Git
            </Link>
          </Button>
        </div>

        <div className="mt-12 p-4 bg-red-100 dark:bg-red-900/20 rounded-lg border border-red-200 dark:border-red-800">
          <p className="text-sm text-red-700 dark:text-red-300">
            <strong>Yardım:</strong> Eğer bu hatanın yanlışlıkla oluştuğunu düşünüyorsanız, 
            tarayıcınızı yenilemeyi veya çıkış yapıp tekrar giriş yapmayı deneyin.
          </p>
        </div>
      </div>
    </div>
  );
}
