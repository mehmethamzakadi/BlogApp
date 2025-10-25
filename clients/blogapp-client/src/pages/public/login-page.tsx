import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import { login } from '../../features/auth/api';
import { useAuthStore } from '../../stores/auth-store';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/card';
import toast from 'react-hot-toast';
import { handleApiError, showApiResponseError } from '../../lib/api-error';

const loginSchema = z.object({
  email: z.string().email('Geçerli bir e-posta adresi girin'),
  password: z.string().min(1, 'Şifre boş olamaz')
});

type LoginFormValues = z.infer<typeof loginSchema>;

export function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const loginStore = useAuthStore((state) => state.login);

  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: '',
      password: ''
    }
  });

  const { mutateAsync, isPending } = useMutation({
    mutationFn: login,
    onSuccess: (response) => {
      if (!response.success) {
        showApiResponseError(response, 'Giriş başarısız oldu');
        return;
      }

      loginStore({
        user: {
          userId: response.data.userId,
          userName: response.data.userName,
          expiration: response.data.expiration,
          refreshToken: response.data.refreshToken,
          permissions: response.data.permissions || []
        },
        token: response.data.token
      });

      toast.success('Yönetim paneline hoş geldiniz!');
      const redirectTo =
        (location.state as { from?: { pathname?: string } })?.from?.pathname ?? '/admin/dashboard';
      navigate(redirectTo, { replace: true });
    },
    onError: (error: unknown) => {
      handleApiError(error, 'Giriş yapılamadı');
    }
  });

  const onSubmit = async (values: LoginFormValues) => {
    await mutateAsync({
      email: values.email,
      password: values.password
    });
  };

  return (
    <div className="flex justify-center py-10">
      <motion.div
        className="w-full max-w-md"
        initial={{ opacity: 0, y: 24 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.3 }}
      >
        <Card>
          <CardHeader>
            <CardTitle>Admin Girişi</CardTitle>
            <CardDescription>Blog yönetim paneline erişmek için giriş yapın.</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
              <div className="space-y-2">
                <Label htmlFor="email">E-posta</Label>
                <Input id="email" type="email" placeholder="ornek@mail.com" {...register('email')} />
                {errors.email && <p className="text-sm text-destructive">{errors.email.message}</p>}
              </div>
              <div className="space-y-2">
                <Label htmlFor="password">Şifre</Label>
                <Input id="password" type="password" placeholder="********" {...register('password')} />
                {errors.password && <p className="text-sm text-destructive">{errors.password.message}</p>}
              </div>
              <Button type="submit" className="w-full" disabled={isPending}>
                {isPending ? 'Giriş yapılıyor...' : 'Giriş Yap'}
              </Button>
              <div className="text-center text-sm">
                Hesabınız yok mu?{' '}
                <Link to="/register" className="text-primary hover:underline">
                  Kayıt Ol
                </Link>
              </div>
            </form>
          </CardContent>
        </Card>
      </motion.div>
    </div>
  );
}
