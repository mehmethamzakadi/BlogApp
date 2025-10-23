import { motion } from 'framer-motion';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/card';
import { Separator } from '../../components/ui/separator';
import { Button } from '../../components/ui/button';

const highlights = [
  {
    title: 'Son 7 Gün',
    description: 'Yeni yayınlanan gönderiler',
    value: '12'
  },
  {
    title: 'Aktif Kategoriler',
    description: 'İçerikleriniz bu kategorilerde sınıflandırıldı',
    value: '8'
  },
  {
    title: 'Bekleyen Taslaklar',
    description: 'Yayınlanmayı bekleyen içerikler',
    value: '4'
  }
];

export function DashboardPage() {
  return (
    <div className="space-y-8">
      <motion.div
        className="flex flex-col gap-4 rounded-xl border bg-card p-6 shadow-sm lg:flex-row lg:items-center lg:justify-between"
        initial={{ opacity: 0, y: 16 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.25 }}
      >
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Yönetim Paneline Hoş Geldiniz</h1>
          <p className="mt-2 max-w-xl text-sm text-muted-foreground">
            BlogApp içeriklerinizi kolayca yönetin, yeni kategoriler oluşturun ve gönderilerinizi düzenleyin. Sağ menüden ilgili alanlara hızlıca ulaşabilirsiniz.
          </p>
        </div>
        <Button size="lg">Yeni Gönderi Oluştur</Button>
      </motion.div>

      <div className="grid gap-6 md:grid-cols-2 xl:grid-cols-3">
        {highlights.map((item, index) => (
          <motion.div
            key={item.title}
            initial={{ opacity: 0, y: 12 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: index * 0.1 }}
          >
            <Card className="h-full">
              <CardHeader>
                <CardTitle className="text-lg">{item.title}</CardTitle>
                <CardDescription>{item.description}</CardDescription>
              </CardHeader>
              <CardContent>
                <p className="text-3xl font-semibold">{item.value}</p>
              </CardContent>
            </Card>
          </motion.div>
        ))}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Son Aktiviteler</CardTitle>
          <CardDescription>Ekibinizin son gerçekleştirdiği işlemleri görüntüleyin.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          {[1, 2, 3].map((activity) => (
            <div key={activity} className="space-y-2">
              <div className="flex items-center justify-between text-sm">
                <span className="font-medium">Yeni kategori oluşturuldu</span>
                <span className="text-muted-foreground">2 saat önce</span>
              </div>
              <Separator />
            </div>
          ))}
        </CardContent>
      </Card>
    </div>
  );
}
