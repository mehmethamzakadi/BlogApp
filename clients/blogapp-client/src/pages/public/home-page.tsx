import { useQuery } from '@tanstack/react-query';
import { motion } from 'framer-motion';
import { fetchPublishedPosts } from '../../features/posts/api';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/card';
import { Badge } from '../../components/ui/badge';
import { Button } from '../../components/ui/button';

export function HomePage() {
  const publishedPostsQueryKey = ['posts', 'published', { pageIndex: 0, pageSize: 9 }] as const;

  const { data, isLoading, isError } = useQuery({
    queryKey: publishedPostsQueryKey,
    queryFn: () => fetchPublishedPosts(0, 9)
  });

  return (
    <div className="space-y-10">
      <section className="grid gap-6 lg:grid-cols-[1.2fr_0.8fr]">
        <div className="space-y-4">
          <motion.h1
            className="text-3xl font-bold tracking-tight sm:text-4xl"
            initial={{ y: 10, opacity: 0 }}
            animate={{ y: 0, opacity: 1 }}
            transition={{ delay: 0.1 }}
          >
            En Yeni Hikayelerle İlham Alın
          </motion.h1>
          <motion.p
            className="text-lg text-muted-foreground"
            initial={{ y: 10, opacity: 0 }}
            animate={{ y: 0, opacity: 1 }}
            transition={{ delay: 0.2 }}
          >
            BlogApp, teknoloji, tasarım ve üretkenlik dünyasından en güncel içerikleri
            sunar. Topluluğumuza katılın ve uzmanlardan öğrenin.
          </motion.p>
          <motion.div
            initial={{ y: 10, opacity: 0 }}
            animate={{ y: 0, opacity: 1 }}
            transition={{ delay: 0.3 }}
          >
            <Button size="lg">Keşfetmeye Başla</Button>
          </motion.div>
        </div>
        <motion.div
          className="relative hidden rounded-3xl border bg-gradient-to-br from-primary/10 to-secondary/10 p-8 lg:flex lg:flex-col lg:justify-between"
          initial={{ opacity: 0, scale: 0.98 }}
          animate={{ opacity: 1, scale: 1 }}
        >
          <div>
            <Badge variant="secondary">Trend Konu</Badge>
            <h2 className="mt-4 text-2xl font-semibold">Yapay Zeka ile Üretkenlik</h2>
            <p className="mt-2 text-sm text-muted-foreground">
              Modern iş akışlarında yapay zekanın rolü ve geleceğin yetenekleri üzerine derinlemesine analiz.
            </p>
          </div>
          <div className="mt-6 text-sm text-muted-foreground">
            Her hafta onlarca yeni makale yayımlanıyor. Haber bültenimize abone olun.
          </div>
        </motion.div>
      </section>

      <section className="space-y-6">
        <div className="flex items-center justify-between">
          <h2 className="text-2xl font-semibold">Öne Çıkan Yazılar</h2>
          <Button variant="ghost" size="sm">
            Tüm Yazılar
          </Button>
        </div>
        {isError && (
          <p className="text-sm text-destructive">Gönderiler yüklenirken bir hata oluştu.</p>
        )}
        <div className="grid gap-6 sm:grid-cols-2 xl:grid-cols-3">
          {isLoading &&
            Array.from({ length: 6 }).map((_, index) => (
              <div key={index} className="h-56 animate-pulse rounded-xl bg-muted" />
            ))}
          {!isLoading && data?.items.length === 0 && (
            <p className="text-sm text-muted-foreground">Henüz yayınlanmış gönderi bulunmuyor.</p>
          )}
          {data?.items.map((post, index) => (
            <motion.article
              key={post.title}
              initial={{ opacity: 0, y: 12 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: index * 0.05 }}
            >
              <Card className="h-full overflow-hidden">
                {post.thumbnail && (
                  <div className="h-40 w-full overflow-hidden">
                    <img
                      src={post.thumbnail}
                      alt={post.title}
                      className="h-full w-full object-cover transition-transform duration-300 hover:scale-105"
                    />
                  </div>
                )}
                <CardHeader className="space-y-2">
                  <Badge variant="outline">{post.categoryName}</Badge>
                  <CardTitle className="line-clamp-2 text-xl">{post.title}</CardTitle>
                  <CardDescription className="line-clamp-3 text-sm">
                    {post.summary}
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <Button variant="link" className="px-0">
                    Devamını Oku
                  </Button>
                </CardContent>
              </Card>
            </motion.article>
          ))}
        </div>
      </section>
    </div>
  );
}
