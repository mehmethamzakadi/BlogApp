import { useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { motion } from 'framer-motion';
import { ArrowRight, Sparkles } from 'lucide-react';
import { fetchPublishedPosts } from '../../features/posts/api';
import { getAllCategories } from '../../features/categories/api';
import { Badge } from '../../components/ui/badge';
import { Button } from '../../components/ui/button';
import { PostCard, PostCardSkeleton } from '../../components/posts/post-card';

export function HomePage() {
  const [activeCategory, setActiveCategory] = useState<number | null>(null);

  const { data: categories, isLoading: isCategoriesLoading } = useQuery({
    queryKey: ['categories', 'all'],
    queryFn: getAllCategories
  });

  const {
    data: posts,
    isLoading: isPostsLoading,
    isError: isPostsError
  } = useQuery({
    queryKey: ['posts', 'published', { pageIndex: 0, pageSize: 12, categoryId: activeCategory ?? undefined }],
    queryFn: () => fetchPublishedPosts({ pageIndex: 0, pageSize: 12, categoryId: activeCategory ?? undefined })
  });

  const categoryOptions = useMemo(
    () => [
      { id: null as number | null, name: 'Tümü' },
      ...(categories?.map((category) => ({ id: category.id, name: category.name })) ?? [])
    ],
    [categories]
  );

  const activeCategoryName = useMemo(
    () => categoryOptions.find((option) => option.id === activeCategory)?.name ?? 'Tümü',
    [activeCategory, categoryOptions]
  );

  const postItems = posts?.items ?? [];
  const featuredPost = postItems[0];
  const spotlightPosts = postItems.slice(1, 3);
  const remainingPosts = postItems.slice(3);

  return (
    <div className="space-y-16">
      <section className="relative overflow-hidden rounded-[2.75rem] border border-border/50 bg-gradient-to-br from-primary/10 via-background to-secondary/10 shadow-2xl">
        <div className="absolute -top-24 left-1/2 h-64 w-64 -translate-x-1/2 rounded-full bg-primary/20 blur-3xl" aria-hidden />
        <div className="absolute -bottom-16 -right-12 h-72 w-72 rounded-full bg-secondary/20 blur-3xl" aria-hidden />
        <div className="relative grid gap-12 px-8 py-14 sm:px-12 lg:grid-cols-[minmax(0,1fr)_minmax(0,360px)] lg:px-16">
          <motion.div
            className="space-y-8"
            initial={{ y: 12, opacity: 0 }}
            animate={{ y: 0, opacity: 1 }}
            transition={{ duration: 0.4 }}
          >
            <div className="inline-flex items-center gap-2 rounded-full border border-primary/40 bg-primary/10 px-4 py-1 text-sm font-medium text-primary">
              <Sparkles className="h-4 w-4" />
              Yeni nesil blog deneyimi
            </div>
            <h1 className="text-4xl font-semibold tracking-tight text-foreground sm:text-5xl">
              İlham verici hikayelerle modern ve sade bir deneyim
            </h1>
            <p className="max-w-2xl text-lg text-muted-foreground">
              BlogApp topluluğu; teknoloji, tasarım ve üretkenlik alanlarındaki en güncel içerikleri buluşturur.
              Kategoriler arasında dolaşarak merak ettiğiniz konuları keşfedin.
            </p>
            <div className="flex flex-wrap items-center gap-4">
              <Button size="lg" asChild>
                <a href="#posts" className="inline-flex items-center gap-2">
                  Yazıları Keşfet
                  <ArrowRight className="h-4 w-4" />
                </a>
              </Button>
              <Badge variant="outline" className="rounded-full px-4 py-2 text-sm">
                Seçili kategori: {activeCategoryName}
              </Badge>
            </div>
          </motion.div>

          <motion.div
            className="relative overflow-hidden rounded-3xl border border-border/40 bg-background/80 p-8 shadow-lg backdrop-blur"
            initial={{ opacity: 0, scale: 0.96 }}
            animate={{ opacity: 1, scale: 1 }}
            transition={{ delay: 0.1, duration: 0.4 }}
          >
            <div className="absolute inset-0 bg-gradient-to-br from-primary/20 via-transparent to-secondary/30" aria-hidden />
            <div className="relative z-10 flex h-full flex-col justify-between gap-6">
              <div className="space-y-4">
                <Badge className="w-fit rounded-full bg-primary/80 px-4 py-1 text-xs uppercase tracking-wider text-primary-foreground">
                  Günün önerisi
                </Badge>
                <h2 className="text-2xl font-semibold text-foreground">
                  {featuredPost ? featuredPost.title : 'Trend olan içerikler burada'}
                </h2>
                <p className="text-sm text-muted-foreground">
                  {featuredPost
                    ? featuredPost.summary
                    : 'En çok ilgi gören yazıları keşfedin, yeni fikirler edinin ve üretkenliğinizi artırın.'}
                </p>
              </div>
              <div>
                {featuredPost ? (
                  <Button variant="ghost" className="group h-auto px-0 text-primary" asChild>
                    <Link to={`/posts/${featuredPost.id}`} className="inline-flex items-center gap-2">
                      En popüler yazıyı incele
                      <ArrowRight className="h-4 w-4 transition-transform duration-300 group-hover:translate-x-1" />
                    </Link>
                  </Button>
                ) : (
                  <p className="text-sm text-muted-foreground">
                    Yeni içerikler eklendikçe bu alan sizin için öneriler sunar.
                  </p>
                )}
              </div>
            </div>
          </motion.div>
        </div>
      </section>

      <section className="space-y-10" id="posts">
        <div className="flex flex-wrap items-end justify-between gap-6">
          <div className="space-y-2">
            <h2 className="text-3xl font-semibold tracking-tight text-foreground">Öne çıkan yazılar</h2>
            <p className="text-muted-foreground">
              {activeCategory === null
                ? 'Tüm kategorilerden en yeni içerikleri keşfedin.'
                : `${activeCategoryName} kategorisindeki güncel gönderileri keşfedin.`}
            </p>
          </div>
          <div className="flex items-center gap-3 text-sm text-muted-foreground">
            <span className="inline-flex h-10 w-10 items-center justify-center rounded-full border border-border/60 bg-background font-semibold text-foreground">
              {posts?.count ?? 0}
            </span>
            toplam yayınlanmış yazı
          </div>
        </div>

        <div className="flex flex-wrap gap-3">
          {isCategoriesLoading
            ? Array.from({ length: 6 }).map((_, index) => (
                <div key={index} className="h-10 w-24 animate-pulse rounded-full bg-muted/60" />
              ))
            : categoryOptions.map((option) => {
                const isActive = option.id === activeCategory;
                return (
                  <Button
                    key={option.id ?? 'all'}
                    variant={isActive ? 'default' : 'outline'}
                    className="rounded-full border border-border/60 bg-background/80 text-sm font-medium transition-colors hover:border-primary"
                    type="button"
                    aria-pressed={isActive}
                    onClick={() => setActiveCategory(option.id)}
                  >
                    {option.name}
                  </Button>
                );
              })}
        </div>

        {isPostsError && (
          <div className="rounded-2xl border border-destructive/40 bg-destructive/10 p-4 text-sm text-destructive">
            Gönderiler yüklenirken bir sorun oluştu. Lütfen daha sonra tekrar deneyin.
          </div>
        )}

        {isPostsLoading ? (
          <div className="space-y-8">
            <PostCardSkeleton variant="horizontal" />
            <div className="grid gap-6 md:grid-cols-2 xl:grid-cols-3">
              {Array.from({ length: 6 }).map((_, index) => (
                <PostCardSkeleton key={index} />
              ))}
            </div>
          </div>
        ) : postItems.length === 0 ? (
          <div className="rounded-3xl border border-border/50 bg-muted/20 p-10 text-center text-muted-foreground">
            Bu kategoride henüz yayınlanmış gönderi bulunmuyor. Çok yakında yeni içerikler eklenecek!
          </div>
        ) : (
          <div className="space-y-10">
            {featuredPost && (
              <PostCard key={featuredPost.id} post={featuredPost} variant="horizontal" />
            )}

            {spotlightPosts.length > 0 && (
              <div className="grid gap-6 md:grid-cols-2">
                {spotlightPosts.map((post) => (
                  <PostCard key={post.id} post={post} variant="horizontal" />
                ))}
              </div>
            )}

            {remainingPosts.length > 0 && (
              <div className="grid gap-6 sm:grid-cols-2 xl:grid-cols-3">
                {remainingPosts.map((post) => (
                  <PostCard key={post.id} post={post} />
                ))}
              </div>
            )}
          </div>
        )}
      </section>
    </div>
  );
}
