import { Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import { useQuery } from '@tanstack/react-query';
import { FileText, CheckCircle2, Clock, FolderKanban, TrendingUp } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/card';
import { Button } from '../../components/ui/button';
import { StatCard } from '../../components/dashboard/stat-card';
import { ChartCard } from '../../components/dashboard/chart-card';
import { ActivityFeed, Activity } from '../../components/dashboard/activity-feed';
import { fetchStatistics } from '../../features/posts/api';

export function DashboardPage() {
  const { data: stats, isLoading } = useQuery({
    queryKey: ['dashboard-statistics'],
    queryFn: fetchStatistics,
    refetchInterval: 30000 // Her 30 saniyede bir güncelle
  });

  // Mock aktiviteler - Gerçek implementasyonda backend'den gelecek
  const recentActivities: Activity[] = [
    {
      id: 1,
      type: 'post_created',
      title: 'Yeni gönderi oluşturuldu: "React Best Practices"',
      timestamp: new Date(Date.now() - 1000 * 60 * 30) // 30 dk önce
    },
    {
      id: 2,
      type: 'category_created',
      title: 'Yeni kategori eklendi: "Frontend Development"',
      timestamp: new Date(Date.now() - 1000 * 60 * 60 * 2) // 2 saat önce
    },
    {
      id: 3,
      type: 'post_updated',
      title: '"TypeScript Fundamentals" başlıklı gönderi güncellendi',
      timestamp: new Date(Date.now() - 1000 * 60 * 60 * 5) // 5 saat önce
    },
    {
      id: 4,
      type: 'post_deleted',
      title: '"Old Post" başlıklı gönderi silindi',
      timestamp: new Date(Date.now() - 1000 * 60 * 60 * 24) // 1 gün önce
    }
  ];

  // Grafik verileri
  const weeklyPostsData = stats
    ? [
        { name: 'Son 7 Gün', value: stats.postsLast7Days },
        { name: 'Son 30 Gün', value: stats.postsLast30Days },
        { name: 'Toplam', value: stats.totalPosts }
      ]
    : [];

  const postStatusData = stats
    ? [
        { name: 'Yayında', value: stats.publishedPosts },
        { name: 'Taslak', value: stats.draftPosts }
      ]
    : [];

  if (isLoading) {
    return (
      <div className="space-y-8">
        <Card className="p-6">
          <div className="flex items-center justify-center">
            <div className="h-8 w-8 animate-spin rounded-full border-4 border-primary border-t-transparent" />
            <span className="ml-3 text-muted-foreground">Yükleniyor...</span>
          </div>
        </Card>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      {/* Header */}
      <motion.div
        className="flex flex-col gap-4 rounded-xl border bg-gradient-to-br from-blue-50 to-indigo-50 dark:from-blue-950/20 dark:to-indigo-950/20 p-6 shadow-sm lg:flex-row lg:items-center lg:justify-between"
        initial={{ opacity: 0, y: 16 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.25 }}
      >
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Yönetim Paneline Hoş Geldiniz</h1>
          <p className="mt-2 max-w-xl text-sm text-muted-foreground">
            BlogApp içeriklerinizi kolayca yönetin, istatistiklerinizi takip edin ve yeni içerikler oluşturun.
          </p>
        </div>
        <div className="flex flex-col gap-2 sm:flex-row">
          <Button size="lg" asChild>
            <Link to="/admin/posts/new">
              <FileText className="mr-2 h-4 w-4" />
              Yeni Gönderi Oluştur
            </Link>
          </Button>
        </div>
      </motion.div>

      {/* İstatistik Kartları */}
      <div className="grid gap-6 md:grid-cols-2 xl:grid-cols-4">
        <StatCard
          title="Toplam Gönderiler"
          value={stats?.totalPosts ?? 0}
          description="Sistemdeki tüm gönderiler"
          icon={FileText}
          color="blue"
          delay={0}
        />
        <StatCard
          title="Yayındaki Gönderiler"
          value={stats?.publishedPosts ?? 0}
          description="Aktif olarak görüntülenen"
          icon={CheckCircle2}
          color="green"
          delay={0.1}
        />
        <StatCard
          title="Taslak Gönderiler"
          value={stats?.draftPosts ?? 0}
          description="Yayınlanmayı bekleyen"
          icon={Clock}
          color="yellow"
          delay={0.2}
        />
        <StatCard
          title="Toplam Kategoriler"
          value={stats?.totalCategories ?? 0}
          description="Aktif kategori sayısı"
          icon={FolderKanban}
          color="purple"
          delay={0.3}
        />
      </div>

      {/* Grafikler ve Aktiviteler */}
      <div className="grid gap-6 lg:grid-cols-2">
        <ChartCard
          title="Gönderi İstatistikleri"
          description="Zaman içindeki gönderi sayıları"
          data={weeklyPostsData}
          type="bar"
          dataKey="value"
          delay={0.4}
        />
        <ChartCard
          title="Gönderi Durumu Dağılımı"
          description="Yayında ve taslak gönderiler"
          data={postStatusData}
          type="pie"
          dataKey="value"
          colors={['#10b981', '#f59e0b']}
          delay={0.5}
        />
      </div>

      {/* Hızlı Aksiyonlar ve Aktiviteler */}
      <div className="grid gap-6 lg:grid-cols-3">
        <Card className="lg:col-span-1">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <TrendingUp className="h-5 w-5 text-primary" />
              Hızlı Aksiyonlar
            </CardTitle>
            <CardDescription>Sık kullanılan işlemler</CardDescription>
          </CardHeader>
          <CardContent className="space-y-2">
            <Button variant="outline" className="w-full justify-start" asChild>
              <Link to="/admin/posts">
                <FileText className="mr-2 h-4 w-4" />
                Tüm Gönderileri Görüntüle
              </Link>
            </Button>
            <Button variant="outline" className="w-full justify-start" asChild>
              <Link to="/admin/categories">
                <FolderKanban className="mr-2 h-4 w-4" />
                Kategorileri Yönet
              </Link>
            </Button>
            <Button variant="outline" className="w-full justify-start" asChild>
              <Link to="/admin/posts/new">
                <FileText className="mr-2 h-4 w-4" />
                Yeni Gönderi Ekle
              </Link>
            </Button>
          </CardContent>
        </Card>

        <div className="lg:col-span-2">
          <ActivityFeed activities={recentActivities} delay={0.6} />
        </div>
      </div>

      {/* Ekstra Bilgi Kartları */}
      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Son 7 Günde</CardTitle>
            <CardDescription>Yeni eklenen içerikler</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex items-baseline gap-2">
              <span className="text-3xl font-bold text-primary">{stats?.postsLast7Days ?? 0}</span>
              <span className="text-sm text-muted-foreground">yeni gönderi</span>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Son 30 Günde</CardTitle>
            <CardDescription>Aylık içerik üretimi</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex items-baseline gap-2">
              <span className="text-3xl font-bold text-primary">{stats?.postsLast30Days ?? 0}</span>
              <span className="text-sm text-muted-foreground">yeni gönderi</span>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
