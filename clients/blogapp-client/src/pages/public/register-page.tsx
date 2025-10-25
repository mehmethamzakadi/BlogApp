import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { useNavigate, Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import { register } from '../../features/auth/api';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/card';
import toast from 'react-hot-toast';
import { handleApiError, showApiResponseError } from '../../lib/api-error';

const registerSchema = z.object({
  userName: z
    .string()
    .min(3, 'Kullanıcı adı en az 3 karakter olmalıdır')
    .max(50, 'Kullanıcı adı en fazla 50 karakter olabilir')
    .regex(/^[a-zA-Z0-9\-._@]+$/, 'Kullanıcı adı sadece harf, rakam ve -._@ karakterlerini içerebilir'),
  email: z.string().email('Geçerli bir e-posta adresi girin'),
  password: z
    .string()
    .min(8, 'Şifre en az 8 karakter olmalıdır')
    .max(100, 'Şifre en fazla 100 karakter olabilir')
    .regex(/[a-z]/, 'Şifre en az bir küçük harf içermelidir')
    .regex(/[A-Z]/, 'Şifre en az bir büyük harf içermelidir')
    .regex(/[0-9]/, 'Şifre en az bir rakam içermelidir')
    .regex(/[^a-zA-Z0-9]/, 'Şifre en az bir özel karakter içermelidir'),
  confirmPassword: z.string()
}).refine((data) => data.password === data.confirmPassword, {
  message: 'Şifreler eşleşmiyor',
  path: ['confirmPassword']
});

type RegisterFormValues = z.infer<typeof registerSchema>;

export function RegisterPage() {
  const navigate = useNavigate();

  const {
    register: registerField,
    handleSubmit,
    formState: { errors }
  } = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      userName: '',
      email: '',
      password: '',
      confirmPassword: ''
    }
  });

  const { mutateAsync, isPending } = useMutation({
    mutationFn: register,
    onSuccess: (response) => {
      if (!response.success) {
        showApiResponseError(response, 'Kayıt başarısız oldu');
        return;
      }

      toast.success('Kayıt başarılı! Giriş yapabilirsiniz.');
      navigate('/login', { replace: true });
    },
    onError: (error: unknown) => {
      handleApiError(error, 'Kayıt yapılamadı');
    }
  });

  const onSubmit = async (values: RegisterFormValues) => {
    await mutateAsync({
      userName: values.userName,
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
            <CardTitle>Kayıt Ol</CardTitle>
            <CardDescription>Blog platformuna katılın ve içerik okumaya başlayın.</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
              <div className="space-y-2">
                <Label htmlFor="userName">Kullanıcı Adı</Label>
                <Input
                  id="userName"
                  type="text"
                  placeholder="kullaniciadi"
                  {...registerField('userName')}
                />
                {errors.userName && <p className="text-sm text-destructive">{errors.userName.message}</p>}
              </div>
              <div className="space-y-2">
                <Label htmlFor="email">E-posta</Label>
                <Input
                  id="email"
                  type="email"
                  placeholder="ornek@mail.com"
                  {...registerField('email')}
                />
                {errors.email && <p className="text-sm text-destructive">{errors.email.message}</p>}
              </div>
              <div className="space-y-2">
                <Label htmlFor="password">Şifre</Label>
                <Input
                  id="password"
                  type="password"
                  placeholder="********"
                  {...registerField('password')}
                />
                {errors.password && <p className="text-sm text-destructive">{errors.password.message}</p>}
              </div>
              <div className="space-y-2">
                <Label htmlFor="confirmPassword">Şifre Tekrar</Label>
                <Input
                  id="confirmPassword"
                  type="password"
                  placeholder="********"
                  {...registerField('confirmPassword')}
                />
                {errors.confirmPassword && (
                  <p className="text-sm text-destructive">{errors.confirmPassword.message}</p>
                )}
              </div>
              <Button type="submit" className="w-full" disabled={isPending}>
                {isPending ? 'Kayıt yapılıyor...' : 'Kayıt Ol'}
              </Button>
              <div className="text-center text-sm">
                Zaten hesabınız var mı?{' '}
                <Link to="/login" className="text-primary hover:underline">
                  Giriş Yap
                </Link>
              </div>
            </form>
          </CardContent>
        </Card>
      </motion.div>
    </div>
  );
}
