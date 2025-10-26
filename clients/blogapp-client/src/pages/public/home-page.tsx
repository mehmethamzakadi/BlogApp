import { useMemo, useState, useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { fetchPublishedPosts } from '../../features/posts/api';
import { getAllCategories } from '../../features/categories/api';
import { Button } from '../../components/ui/button';
import { PostCard, PostCardSkeleton } from '../../components/posts/post-card';
import { Loader2 } from 'lucide-react';
import type { PostSummary } from '../../features/posts/types';

export function HomePage() {
  const [activeCategory, setActiveCategory] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState(0);
  const [allPosts, setAllPosts] = useState<PostSummary[]>([]);
  const pageSize = 10; // İlk yüklemede 10 post (1 featured + 9 grid)

  const { data: categories, isLoading: isCategoriesLoading } = useQuery({
    queryKey: ['categories', 'all'],
    queryFn: getAllCategories
  });

  const {
    data: posts,
    isLoading: isPostsLoading,
    isError: isPostsError,
    isFetching
  } = useQuery({
    queryKey: ['posts', 'published', { pageIndex: currentPage, pageSize, categoryId: activeCategory ?? undefined }],
    queryFn: () => fetchPublishedPosts({ pageIndex: currentPage, pageSize, categoryId: activeCategory ?? undefined })
  });

  const categoryOptions = useMemo(
    () => [
      { id: null as string | null, name: 'Tümü' },
      ...(categories?.map((category) => ({ id: category.id, name: category.name })) ?? [])
    ],
    [categories]
  );

  const activeCategoryName = useMemo(
    () => categoryOptions.find((option) => option.id === activeCategory)?.name ?? 'Tümü',
    [activeCategory, categoryOptions]
  );

  // Posts yüklendiğinde state'e ekle
  useEffect(() => {
    if (posts?.items) {
      if (currentPage === 0) {
        setAllPosts(posts.items);
      } else {
        setAllPosts((prev) => [...prev, ...posts.items]);
      }
    }
  }, [posts?.items, currentPage]);

  // Kategori değiştiğinde sayfayı sıfırla
  const handleCategoryChange = (categoryId: string | null) => {
    setActiveCategory(categoryId);
    setCurrentPage(0);
    setAllPosts([]);
  };

  // Daha fazla yükle
  const handleLoadMore = () => {
    setCurrentPage((prev) => prev + 1);
  };

  const hasMore = posts ? (currentPage + 1) * pageSize < posts.count : false;
  const postItems = allPosts;

  return (
    <div className="relative min-h-screen bg-background">
      {/* Background Effects */}
      <div
        className="pointer-events-none absolute inset-0 -z-10 bg-[radial-gradient(120%_120%_at_50%_0%,hsl(var(--primary)_/_0.05)_0%,transparent_55%)]"
        aria-hidden
      />
      
      {/* Content Container */}
      <div className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
        <div className="space-y-16">
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
                    onClick={() => handleCategoryChange(option.id)}
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
            {/* Featured Post Skeleton */}
            <PostCardSkeleton variant="featured" />
            
            {/* Grid Posts Skeleton */}
            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {Array.from({ length: 6 }).map((_, index) => (
                <PostCardSkeleton key={index} variant="compact" />
              ))}
            </div>
          </div>
        ) : postItems.length === 0 ? (
          <div className="rounded-3xl border border-border/50 bg-muted/20 p-10 text-center text-muted-foreground">
            Bu kategoride henüz yayınlanmış gönderi bulunmuyor. Çok yakında yeni içerikler eklenecek!
          </div>
        ) : (
          <div className="space-y-8">
            {/* Featured Post - İlk Post */}
            {postItems.length > 0 && (
              <PostCard post={postItems[0]} variant="featured" />
            )}
            
            {/* Grid Posts - Kalan Postlar */}
            {postItems.length > 1 && (
              <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
                {postItems.slice(1).map((post) => (
                  <PostCard key={post.id} post={post} variant="compact" />
                ))}
              </div>
            )}

            {/* Load More Button */}
            {hasMore && (
              <div className="flex justify-center pt-4">
                <Button
                  onClick={handleLoadMore}
                  disabled={isFetching}
                  size="lg"
                  variant="outline"
                  className="group rounded-full border-2 border-border/60 bg-background/80 px-8 py-6 text-base font-semibold transition-all hover:border-primary/60 hover:bg-primary/10 hover:shadow-lg disabled:opacity-50"
                >
                  {isFetching ? (
                    <>
                      <Loader2 className="mr-2 h-5 w-5 animate-spin" />
                      Yükleniyor...
                    </>
                  ) : (
                    <>
                      Daha Fazla Göster
                      <span className="ml-2 text-sm text-muted-foreground">
                        ({posts?.count ? posts.count - postItems.length : 0} yazı daha)
                      </span>
                    </>
                  )}
                </Button>
              </div>
            )}
          </div>
        )}
      </section>
        </div>
      </div>
    </div>
  );
}
