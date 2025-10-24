import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { fetchPublishedPosts } from '../../features/posts/api';
import { getAllCategories } from '../../features/categories/api';
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

  return (
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
            {postItems.map((post) => (
              <PostCard key={post.id} post={post} variant="horizontal" />
            ))}
          </div>
        )}
      </section>
    </div>
  );
}
