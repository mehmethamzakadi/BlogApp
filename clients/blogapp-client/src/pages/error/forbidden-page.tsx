import { useNavigate } from 'react-router-dom';
import { ShieldAlert, Home, ArrowLeft } from 'lucide-react';
import { Button } from '../../components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/card';

export function ForbiddenPage() {
  const navigate = useNavigate();

  return (
    <div className="flex items-center justify-center min-h-screen bg-background p-4">
      <Card className="w-full max-w-md">
        <CardHeader className="text-center space-y-4">
          <div className="flex justify-center">
            <div className="rounded-full bg-destructive/10 p-3">
              <ShieldAlert className="h-12 w-12 text-destructive" />
            </div>
          </div>
          <div>
            <CardTitle className="text-2xl">Erişim Reddedildi</CardTitle>
            <CardDescription className="text-base mt-2">
              Bu sayfayı görüntülemek için yetkiniz bulunmamaktadır.
            </CardDescription>
          </div>
        </CardHeader>
        <CardContent className="space-y-3">
          <div className="bg-muted p-4 rounded-lg text-sm text-muted-foreground">
            <p>
              Eğer bu sayfaya erişim iznine sahip olmanız gerektiğini düşünüyorsanız, lütfen sistem
              yöneticinizle iletişime geçin.
            </p>
          </div>
          <div className="flex gap-3 pt-2">
            <Button variant="outline" onClick={() => navigate(-1)} className="flex-1">
              <ArrowLeft className="mr-2 h-4 w-4" />
              Geri Dön
            </Button>
            <Button onClick={() => navigate('/admin/dashboard')} className="flex-1">
              <Home className="mr-2 h-4 w-4" />
              Ana Sayfa
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
