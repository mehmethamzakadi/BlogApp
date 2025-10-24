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
  const curatedPosts = postItems.slice(1, 4);
  const postsToDisplay = postItems.slice(featuredPost ? 1 : 0);

  return (
    <div className="space-y-16">
      <section className="rounded-[2.75rem] border border-border/60 bg-background px-8 py-14 shadow-sm sm:px-12 lg:px-16">
        <div className="grid gap-12 lg:grid-cols-[minmax(0,1.2fr)_minmax(0,1fr)]">
          <motion.div
            className="space-y-8"
            initial={{ y: 16, opacity: 0 }}
            animate={{ y: 0, opacity: 1 }}
            transition={{ duration: 0.45 }}
          >
            <div className="inline-flex items-center gap-2 rounded-full border border-border/70 bg-muted/40 px-4 py-1 text-xs font-medium uppercase tracking-wide text-muted-foreground">
              <Sparkles className="h-4 w-4 text-primary" />
              Sade blog deneyimi
            </div>
            <h1 className="text-4xl font-semibold tracking-tight text-foreground sm:text-5xl">
              Merakı yüksek okurlar için minimalist içerik kütüphanesi
            </h1>
            <p className="max-w-2xl text-base text-muted-foreground sm:text-lg">
              Teknoloji, tasarım ve üretkenlik alanlarında seçtiğimiz yazıları tek bir temiz arayüzde buluşturuyoruz. Kategoriler arasında gezinin, ilham verici hikayelere hızlıca ulaşın.
            </p>
            <div className="flex flex-wrap items-center gap-4">
              <Button size="lg" variant="default" asChild>
                <a href="#posts" className="inline-flex items-center gap-2">
                  Yazıları keşfet
                  <ArrowRight className="h-4 w-4" />
                </a>
              </Button>
              <Badge variant="outline" className="rounded-full px-4 py-2 text-sm font-medium">
                Seçili kategori: {activeCategoryName}
              </Badge>
            </div>
            <div className="flex flex-wrap gap-10 text-sm text-muted-foreground">
              <div>
                <p className="text-3xl font-semibold text-foreground">{posts?.count ?? '—'}</p>
                <p className="mt-1">yayınlanmış yazı</p>
              </div>
              <div>
                <p className="text-3xl font-semibold text-foreground">{categories?.length ?? '—'}</p>
                <p className="mt-1">kategorilik seçki</p>
              </div>
            </div>
          </motion.div>

          <motion.div
            className="space-y-6 rounded-3xl border border-border/60 bg-muted/20 p-8"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.1, duration: 0.45 }}
          >
            <div className="space-y-2">
              <p className="text-xs font-medium uppercase tracking-[0.24em] text-muted-foreground">Günün seçkisi</p>
              <h2 className="text-2xl font-semibold text-foreground">
                {featuredPost ? featuredPost.title : 'Trend olan içerikler burada'}
              </h2>
              <p className="text-sm text-muted-foreground">
                {featuredPost
                  ? featuredPost.summary
                  : 'En çok ilgi gören yazıları keşfedin, yeni fikirler edinin ve üretkenliğinizi artırın.'}
              </p>
              {featuredPost && (
                <Button variant="ghost" className="group h-auto px-0 text-primary" asChild>
                  <Link to={`/posts/${featuredPost.id}`} className="inline-flex items-center gap-2 text-sm font-medium">
                    Yazıyı aç
                    <ArrowRight className="h-4 w-4 transition-transform duration-300 group-hover:translate-x-1" />
                  </Link>
                </Button>
              )}
            </div>

            {curatedPosts.length > 0 && (
              <div className="space-y-4">
                <p className="text-xs font-semibold uppercase tracking-[0.3em] text-muted-foreground">Editörün notları</p>
                <ul className="space-y-3">
                  {curatedPosts.map((post) => (
                    <li key={post.id} className="group rounded-2xl border border-transparent bg-background/60 p-4 transition-colors hover:border-border/80">
                      <Link to={`/posts/${post.id}`} className="flex flex-col gap-2">
                        <span className="text-sm font-medium text-muted-foreground">{post.categoryName}</span>
                        <span className="text-base font-semibold text-foreground group-hover:text-primary">{post.title}</span>
                      </Link>
                    </li>
                  ))}
                </ul>
              </div>
            )}
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
                    variant={isActive ? 'default' : 'ghost'}
                    className="rounded-full border border-border/60 bg-background/80 text-sm font-medium text-muted-foreground transition-colors hover:border-border/80 hover:bg-muted/40 hover:text-foreground"
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
          <div className="space-y-6">
            {Array.from({ length: 4 }).map((_, index) => (
              <PostCardSkeleton key={index} variant="horizontal" />
            ))}
          </div>
        ) : postItems.length === 0 ? (
          <div className="rounded-3xl border border-border/50 bg-muted/20 p-10 text-center text-muted-foreground">
            Bu kategoride henüz yayınlanmış gönderi bulunmuyor. Çok yakında yeni içerikler eklenecek!
          </div>
        ) : (
          <div className="space-y-6">
            {postsToDisplay.map((post) => (
              <PostCard key={post.id} post={post} variant="horizontal" />
            ))}
          </div>
        )}
      </section>
    </div>
  );
}
