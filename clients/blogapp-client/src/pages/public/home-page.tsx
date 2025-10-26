import { useMemo, useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Loader2 } from 'lucide-react';
import { fetchPublishedPosts } from '../../features/posts/api';
import { getAllCategories } from '../../features/categories/api';
import { Button } from '../../components/ui/button';
import type { PostSummary } from '../../features/posts/types';

function SimplePostCard({ post }: { post: PostSummary }) {
  const hasThumbnail = Boolean(post.thumbnail);

  const formattedDate = useMemo(() => {
    if (!post?.createdDate) return null;
    try {
      const date = new Date(post.createdDate);
      if (isNaN(date.getTime())) return null;
      return new Intl.DateTimeFormat('tr-TR', {
        day: 'numeric',
        month: 'short',
        year: 'numeric'
      }).format(date);
    } catch {
      return null;
    }
  }, [post?.createdDate]);

  const readingInfo = useMemo(() => {
    const text = post.body || post.summary || '';
    const plainText = text.replace(/<[^>]*>/g, ' ').replace(/\s+/g, ' ').trim();
    const words = plainText.split(' ').filter(Boolean);
    const wordCount = words.length;
    const readingMinutes = Math.max(1, Math.ceil(wordCount / 200));
    return { readingMinutes };
  }, [post.body, post.summary]);

  const metaItems = useMemo(() => {
    const items: string[] = [];
    if (post.categoryName) {
      items.push(post.categoryName);
    }
    if (formattedDate) {
      items.push(formattedDate);
    }
    return items;
  }, [post.categoryName, formattedDate]);

  return (
    <Link to={`/posts/${post.id}`} className="block h-full">
      <article className="flex h-full flex-col overflow-hidden rounded-2xl border border-border/60 bg-card transition-shadow duration-200 hover:shadow-sm md:flex-row">
        <div className="relative h-44 w-full overflow-hidden border-b border-border/60 md:h-auto md:w-72 md:border-b-0 md:border-r">
          {hasThumbnail ? (
            <img src={post.thumbnail} alt={post.title} className="h-full w-full object-cover" />
          ) : (
            <div className="flex h-full w-full items-center justify-center bg-muted text-xs font-medium text-muted-foreground">
              Görsel bulunmuyor
            </div>
          )}
        </div>
        <div className="flex flex-1 flex-col gap-3 p-5 md:p-6">
          {metaItems.length > 0 && (
            <p className="text-xs text-muted-foreground">{metaItems.join(' • ')}</p>
          )}
          <h3 className="text-lg font-semibold leading-snug text-foreground line-clamp-2 md:text-xl">{post.title}</h3>
          <p className="text-sm text-muted-foreground line-clamp-3 md:flex-1">{post.summary}</p>
          <span className="text-xs text-muted-foreground">{readingInfo.readingMinutes} dk okuma</span>
        </div>
      </article>
    </Link>
  );
}

function SimplePostSkeleton() {
  return (
    <div className="flex h-full animate-pulse flex-col overflow-hidden rounded-2xl border border-border/60 bg-muted/30 md:flex-row">
      <div className="h-44 w-full bg-muted md:h-auto md:w-72" />
      <div className="flex flex-1 flex-col gap-4 p-5 md:p-6">
        <div className="h-3 w-24 rounded bg-muted/60" />
        <div className="h-5 w-3/4 rounded bg-muted/60" />
        <div className="h-3 w-full rounded bg-muted/60" />
        <div className="h-3 w-2/3 rounded bg-muted/60" />
      </div>
    </div>
  );
}

export function HomePage() {
  const [activeCategory, setActiveCategory] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState(0);
  const [allPosts, setAllPosts] = useState<PostSummary[]>([]);
  const pageSize = 10; // İlk yüklemede 10 gönderi getir

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
  const totalPosts = posts?.count ?? 0;
  const description = activeCategory === null
    ? 'BlogApp yazarlarının en güncel paylaşımlarını keşfet.'
    : `${activeCategoryName} kategorisindeki yazılara göz at.`;

  return (
    <div className="bg-background">
      <div className="mx-auto max-w-6xl px-4 py-16 sm:px-6 lg:px-8">
        <div className="space-y-12">
          <header className="space-y-3">
            <h1 className="text-3xl font-semibold text-foreground sm:text-4xl">BlogApp</h1>
            <p className="max-w-2xl text-sm text-muted-foreground sm:text-base">{description}</p>
          </header>

          <section className="space-y-3">
            <h2 className="text-xs font-semibold uppercase tracking-[0.25em] text-muted-foreground/80">Kategoriler</h2>
            <div className="flex flex-wrap gap-2">
              {isCategoriesLoading
                ? Array.from({ length: 6 }).map((_, index) => (
                    <div key={index} className="h-9 w-20 animate-pulse rounded-full bg-muted/60" />
                  ))
                : categoryOptions.map((option) => {
                    const isActive = option.id === activeCategory;
                    return (
                      <Button
                        key={option.id ?? 'all'}
                        type="button"
                        size="sm"
                        variant="ghost"
                        data-active={isActive}
                        className="rounded-full border border-transparent px-4 py-2 text-sm font-medium text-muted-foreground transition-colors hover:border-border hover:bg-muted/40 hover:text-foreground data-[active=true]:border-border data-[active=true]:bg-muted/70 data-[active=true]:text-foreground"
                        aria-pressed={isActive}
                        onClick={() => handleCategoryChange(option.id)}
                      >
                        {option.name}
                      </Button>
                    );
                  })}
            </div>
          </section>

          <section className="space-y-6" id="posts">
            <div className="flex flex-wrap items-center justify-between gap-3">
              <h2 className="text-2xl font-semibold text-foreground sm:text-3xl">Son yazılar</h2>
              <span className="text-sm text-muted-foreground">{totalPosts} yazı</span>
            </div>

            {isPostsError && (
              <div className="rounded-2xl border border-destructive/40 bg-destructive/10 p-4 text-sm text-destructive">
                Gönderiler yüklenirken bir sorun oluştu. Lütfen daha sonra tekrar deneyin.
              </div>
            )}

            {isPostsLoading ? (
              <div className="space-y-4">
                {Array.from({ length: 6 }).map((_, index) => (
                  <SimplePostSkeleton key={index} />
                ))}
              </div>
            ) : postItems.length === 0 ? (
              <div className="rounded-2xl border border-border/60 bg-muted/20 p-10 text-center text-sm text-muted-foreground">
                Bu kategoride henüz yayınlanmış gönderi bulunmuyor.
              </div>
            ) : (
              <div className="space-y-4">
                {postItems.map((post) => (
                  <SimplePostCard key={post.id} post={post} />
                ))}
              </div>
            )}

            {hasMore && (
              <div className="flex justify-center">
                <Button
                  onClick={handleLoadMore}
                  disabled={isFetching}
                  size="lg"
                  variant="outline"
                  className="rounded-full px-6"
                >
                  {isFetching ? (
                    <>
                      <Loader2 className="mr-2 h-5 w-5 animate-spin" />
                      Yükleniyor...
                    </>
                  ) : (
                    'Daha Fazla Göster'
                  )}
                </Button>
              </div>
            )}
          </section>
        </div>
      </div>
    </div>
  );
}
