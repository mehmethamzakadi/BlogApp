import { useMemo } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { motion } from 'framer-motion';
import { ArrowLeft, ArrowRight, Calendar, Clock } from 'lucide-react';
import { fetchPublishedPosts, getPostById } from '../../features/posts/api';
import { Badge } from '../../components/ui/badge';
import { Button } from '../../components/ui/button';
import { sanitizeHtml } from '../../lib/sanitize-html';

const HTML_TAG_REGEX = /<\/?[a-z][^>]*>/i;

function convertPlainTextToHtml(text: string): string {
  const paragraphs = text
    .replace(/\r\n/g, '\n')
    .split(/\n\s*\n/g)
    .map((paragraph) => paragraph.trim())
    .filter(Boolean);

  if (paragraphs.length === 0) {
    return '';
  }

  const escapeHtml = (value: string) =>
    value
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#39;');

  return paragraphs
    .map((paragraph) => `<p>${escapeHtml(paragraph).replace(/\n/g, '<br />')}</p>`)
    .join('');
}

export function PostDetailPage() {
  const { postId } = useParams();
  const numericId = Number(postId);
  const isValidId = Number.isInteger(numericId) && numericId > 0;

  const {
    data: post,
    isLoading,
    isError
  } = useQuery({
    queryKey: ['posts', 'detail', numericId],
    queryFn: () => getPostById(numericId),
    enabled: isValidId
  });

  const sanitizedContent = useMemo(() => {
    if (!post?.body) {
      return '';
    }

    const trimmedBody = post.body.trim();

    if (!trimmedBody) {
      return '';
    }

    const content = HTML_TAG_REGEX.test(trimmedBody) ? post.body : convertPlainTextToHtml(post.body);

    return sanitizeHtml(content);
  }, [post?.body]);

  const { data: publishedPosts } = useQuery({
    queryKey: ['posts', 'published', 'all'],
    queryFn: () =>
      fetchPublishedPosts({
        pageIndex: 0,
        pageSize: 200
      }),
    enabled: !!post
  });

  const { previousPost, nextPost } = useMemo(() => {
    if (!post || !publishedPosts?.items?.length) {
      return { previousPost: undefined, nextPost: undefined };
    }

    const currentIndex = publishedPosts.items.findIndex((item) => item.id === post.id);

    if (currentIndex === -1) {
      return { previousPost: undefined, nextPost: undefined };
    }

    const previous = currentIndex < publishedPosts.items.length - 1 ? publishedPosts.items[currentIndex + 1] : undefined;
    const next = currentIndex > 0 ? publishedPosts.items[currentIndex - 1] : undefined;

    return { previousPost: previous, nextPost: next };
  }, [post, publishedPosts?.items]);

  const readingInsights = useMemo(() => {
    if (!post?.body) {
      return { wordCount: 0, readingMinutes: 0 };
    }

    const text = post.body
      .replace(/<[^>]+>/g, ' ')
      .replace(/&nbsp;/g, ' ')
      .replace(/\s+/g, ' ')
      .trim();

    if (!text) {
      return { wordCount: 0, readingMinutes: 0 };
    }

    const words = text.split(' ').filter(Boolean);
    const readingMinutes = Math.max(1, Math.round(words.length / 200));

    return { wordCount: words.length, readingMinutes };
  }, [post?.body]);

  if (!isValidId) {
    return (
      <div className="mx-auto max-w-2xl space-y-6 text-center">
        <h1 className="text-3xl font-semibold">Gönderi bulunamadı</h1>
        <p className="text-muted-foreground">
          Üzgünüz, aradığınız gönderiye erişilemedi. Lütfen tekrar deneyin ya da ana sayfaya dönün.
        </p>
        <Button asChild>
          <Link to="/">Ana sayfaya dön</Link>
        </Button>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="space-y-10">
        <div className="h-72 animate-pulse rounded-[2.75rem] bg-muted/40" />
        <div className="mx-auto max-w-4xl space-y-4">
          {Array.from({ length: 6 }).map((_, index) => (
            <div key={index} className="h-5 animate-pulse rounded-full bg-muted/50" />
          ))}
        </div>
      </div>
    );
  }

  if (isError || !post) {
    return (
      <div className="mx-auto max-w-2xl space-y-6 text-center">
        <h1 className="text-3xl font-semibold">Gönderi yüklenemedi</h1>
        <p className="text-muted-foreground">
          Şu anda gönderiyi görüntüleyemiyoruz. Lütfen daha sonra tekrar deneyin.
        </p>
        <Button asChild>
          <Link to="/">Ana sayfaya dön</Link>
        </Button>
      </div>
    );
  }

  const categoryLabel = post.categoryName?.trim() || 'Kategori belirtilmemiş';
  const previousPostUrl = previousPost ? `/posts/${previousPost.id}` : '';
  const nextPostUrl = nextPost ? `/posts/${nextPost.id}` : '';

  return (
    <div className="relative min-h-screen bg-background overflow-x-hidden">
      <div
        className="pointer-events-none absolute inset-0 -z-10 bg-[radial-gradient(120%_120%_at_50%_0%,hsl(var(--primary)_/_0.08)_0%,transparent_55%)]"
        aria-hidden
      />
      <div
        className="pointer-events-none absolute -left-32 top-32 -z-10 h-[28rem] w-[28rem] rounded-full bg-primary/10 blur-[140px]"
        aria-hidden
      />
      <div
        className="pointer-events-none absolute -right-24 top-1/3 -z-10 h-[30rem] w-[30rem] rounded-full bg-secondary/15 blur-[160px]"
        aria-hidden
      />

      <div className="w-full px-6 py-8 sm:px-10 md:px-16 lg:px-20 xl:px-28 2xl:px-32">
        <div className="flex w-full flex-col gap-16">
        <Button variant="ghost" className="group mt-4 h-auto w-fit px-0 text-sm" asChild>
          <Link to="/" className="inline-flex items-center gap-2 text-muted-foreground transition-colors group-hover:text-primary">
            <ArrowLeft className="h-4 w-4 transition-transform duration-300 group-hover:-translate-x-1" />
            Ana sayfaya dön
          </Link>
        </Button>

        {/* Hero Section - Full Width */}
        <motion.section
          className="relative overflow-hidden rounded-[3rem] border border-border/40 shadow-xl shadow-primary/10"
          initial={{ opacity: 0, y: 16 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.4 }}
        >
          {/* Background Image with Overlay */}
          {post.thumbnail ? (
            <>
              <div className="absolute inset-0">
                <img 
                  src={post.thumbnail} 
                  alt={post.title} 
                  className="h-full w-full object-cover"
                />
              </div>
              <div className="absolute inset-0 bg-gradient-to-br from-background/95 via-background/90 to-background/85 backdrop-blur-sm" />
            </>
          ) : (
            <div className="absolute inset-0 bg-gradient-to-br from-primary/10 via-background to-secondary/15" />
          )}

          {/* Content */}
          <div className="relative px-6 py-16 sm:px-12 sm:py-20 lg:px-20 lg:py-24 xl:px-24 xl:py-28 2xl:px-28 2xl:py-32">
            <div className="mx-auto max-w-4xl space-y-8 text-center">
              <Badge className="mx-auto w-fit rounded-full bg-primary/90 px-5 py-1.5 text-xs uppercase tracking-wider text-primary-foreground shadow-lg backdrop-blur-sm">
                {categoryLabel}
              </Badge>
              
              <h1 className="text-balance text-3xl font-bold tracking-tight text-foreground drop-shadow-sm sm:text-4xl lg:text-5xl xl:text-6xl">
                {post.title}
              </h1>
              
              <p className="mx-auto max-w-3xl text-lg text-foreground/90 drop-shadow-sm sm:text-xl lg:text-2xl">
                {post.summary}
              </p>
              
              <div className="flex flex-wrap items-center justify-center gap-3 pt-4 text-sm">
                <span className="inline-flex items-center gap-2 rounded-full bg-background/70 px-5 py-2.5 font-medium text-foreground shadow-md backdrop-blur-md ring-1 ring-border/50">
                  <Calendar className="h-4 w-4" />
                  Yayınlanan makale
                </span>
                {readingInsights.wordCount > 0 && (
                  <span className="inline-flex items-center gap-2 rounded-full bg-background/70 px-5 py-2.5 font-medium text-foreground shadow-md backdrop-blur-md ring-1 ring-border/50">
                    <Clock className="h-4 w-4" />
                    {readingInsights.readingMinutes} dakikalık okuma · {readingInsights.wordCount} kelime
                  </span>
                )}
              </div>
            </div>
          </div>
        </motion.section>

        {/* Main Content Section - Full Width */}
        <motion.section
          className="grid gap-10 lg:grid-cols-[minmax(0,2fr)_minmax(320px,0.85fr)] xl:grid-cols-[minmax(0,2.2fr)_minmax(360px,0.7fr)] 2xl:grid-cols-[minmax(0,2.35fr)_minmax(380px,0.65fr)]"
          initial={{ opacity: 0, y: 24 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1, duration: 0.4 }}
        >
          <div className="overflow-hidden rounded-[2.75rem] border border-border/70 bg-card/95 shadow-xl backdrop-blur">
            <div className="bg-background/95 px-4 py-8 sm:px-6 sm:py-10 lg:px-8 lg:py-12 xl:px-10 xl:py-14">
              <div className="mb-12 flex flex-wrap items-center justify-center gap-3 text-sm text-muted-foreground/80 sm:justify-between">
                <Badge variant="secondary" className="rounded-full bg-secondary/80 px-4 py-1 text-xs font-semibold uppercase tracking-wide text-secondary-foreground">
                  {categoryLabel}
                </Badge>
                {readingInsights.wordCount > 0 && (
                  <span className="inline-flex items-center gap-2 rounded-full bg-background/70 px-3 py-1.5 ring-1 ring-border/60">
                    <Clock className="h-4 w-4" />
                    {readingInsights.readingMinutes} dakika · {readingInsights.wordCount} kelime
                  </span>
                )}
              </div>

              {sanitizedContent ? (
                <article className="blog-content" dangerouslySetInnerHTML={{ __html: sanitizedContent }} />
              ) : (
                <article className="blog-content">
                  <p>{post.summary ?? 'Bu gönderi için içerik bulunamadı.'}</p>
                </article>
              )}
            </div>
          </div>

          <aside className="flex flex-col gap-6 lg:pt-4">
            <div className="sticky top-28 space-y-6">
              <div className="overflow-hidden rounded-[2rem] border border-border/70 bg-background/95 p-8 shadow-lg">
                <h2 className="text-xl font-semibold text-foreground">Makale bilgileri</h2>
                <p className="mt-2 text-sm text-muted-foreground">
                  Göz yormayan renkler ve ferah bir düzen ile bu yazıyı keyifle okuyabilirsiniz.
                </p>
                <div className="mt-6 space-y-4 text-sm text-muted-foreground">
                  <div className="flex items-center justify-between rounded-xl bg-background/80 px-4 py-3 ring-1 ring-border/70">
                    <span className="font-medium text-foreground">Kategori</span>
                    <span className="truncate text-right text-foreground/90" title={categoryLabel}>
                      {categoryLabel}
                    </span>
                  </div>
                  {readingInsights.wordCount > 0 && (
                    <div className="flex items-center justify-between rounded-xl bg-background/80 px-4 py-3 ring-1 ring-border/70">
                      <span className="font-medium text-foreground">Okuma süresi</span>
                      <span>{readingInsights.readingMinutes} dakika</span>
                    </div>
                  )}
                  {readingInsights.wordCount > 0 && (
                    <div className="flex items-center justify-between rounded-xl bg-background/80 px-4 py-3 ring-1 ring-border/70">
                      <span className="font-medium text-foreground">Kelime sayısı</span>
                      <span>{readingInsights.wordCount}</span>
                    </div>
                  )}
                </div>
              </div>

              <div className="rounded-[2rem] border border-border/70 bg-background/95 p-8 shadow-lg">
                <h3 className="text-lg font-semibold text-foreground">Yeni hikayeleri keşfet</h3>
                <p className="mt-2 text-sm text-muted-foreground">
                  Okumaya ara verdiğinizde bile sonraki veya önceki yazıya kolayca geçebilirsiniz.
                </p>
                <div className="mt-6 space-y-3 text-sm">
                  {previousPost ? (
                    <Link
                      to={previousPostUrl}
                      className="group flex items-center justify-between rounded-xl border border-border/60 bg-background/85 px-4 py-3 font-medium text-foreground transition hover:border-primary/60 hover:bg-primary/10"
                    >
                      <span>Önceki yazıya git</span>
                      <ArrowLeft className="h-4 w-4 text-primary transition-transform duration-300 group-hover:-translate-x-1" />
                    </Link>
                  ) : (
                    <div className="flex items-center justify-between rounded-xl border border-dashed border-border/60 bg-muted/30 px-4 py-3 text-muted-foreground">
                      <span>Önceki yazı bulunamadı</span>
                      <ArrowLeft className="h-4 w-4" />
                    </div>
                  )}

                  {nextPost ? (
                    <Link
                      to={nextPostUrl}
                      className="group flex items-center justify-between rounded-xl border border-border/60 bg-background/85 px-4 py-3 font-medium text-foreground transition hover:border-primary/60 hover:bg-primary/10"
                    >
                      <span>Sonraki yazıya git</span>
                      <ArrowRight className="h-4 w-4 text-primary transition-transform duration-300 group-hover:translate-x-1" />
                    </Link>
                  ) : (
                    <div className="flex items-center justify-between rounded-xl border border-dashed border-border/60 bg-muted/30 px-4 py-3 text-muted-foreground">
                      <span>Sonraki yazı bulunamadı</span>
                      <ArrowRight className="h-4 w-4" />
                    </div>
                  )}
                </div>
              </div>
            </div>
          </aside>
        </motion.section>

        {/* Post Navigation - Full Width */}
        <motion.nav
          className="rounded-[2.75rem] border border-border/50 bg-background/95 p-6 shadow-2xl shadow-primary/10 backdrop-blur"
          initial={{ opacity: 0, y: 24 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.2, duration: 0.4 }}
        >
          <div className="grid gap-4 lg:grid-cols-2">
            {previousPost ? (
              <Link
                to={previousPostUrl}
                className="group flex h-full flex-col justify-between rounded-3xl border border-border/60 bg-background/95 p-6 text-left shadow-lg transition hover:-translate-y-1 hover:border-primary/60 hover:bg-primary/10"
              >
                <span className="text-xs font-semibold uppercase tracking-wider text-muted-foreground group-hover:text-primary">
                  Bir önceki yazı
                </span>
                <span className="text-lg font-semibold text-balance text-foreground group-hover:text-primary/90">
                  {previousPost.title}
                </span>
                <span className="mt-3 inline-flex items-center gap-2 text-sm font-medium text-primary/80 group-hover:text-primary">
                  <ArrowLeft className="h-4 w-4 transition-transform duration-300 group-hover:-translate-x-1" />
                  Önceki gönderiye git
                </span>
              </Link>
            ) : (
              <div className="flex h-full flex-col justify-center rounded-3xl border border-dashed border-border/60 bg-muted/30 p-6 text-sm text-muted-foreground">
                Daha eski bir yazı bulunamadı.
              </div>
            )}

            {nextPost ? (
              <Link
                to={nextPostUrl}
                className="group flex h-full flex-col justify-between rounded-3xl border border-border/60 bg-background/95 p-6 text-left shadow-lg transition hover:-translate-y-1 hover:border-primary/60 hover:bg-primary/10"
              >
                <span className="text-xs font-semibold uppercase tracking-wider text-muted-foreground group-hover:text-primary">
                  Bir sonraki yazı
                </span>
                <span className="text-lg font-semibold text-balance text-foreground group-hover:text-primary/90">
                  {nextPost.title}
                </span>
                <span className="mt-3 inline-flex items-center gap-2 text-sm font-medium text-primary/80 group-hover:text-primary">
                  Sonraki gönderiye git
                  <ArrowRight className="h-4 w-4 transition-transform duration-300 group-hover:translate-x-1" />
                </span>
              </Link>
            ) : (
              <div className="flex h-full flex-col justify-center rounded-3xl border border-dashed border-border/60 bg-muted/30 p-6 text-sm text-muted-foreground">
                Daha yeni bir yazı bulunamadı.
              </div>
            )}
          </div>
        </motion.nav>
        </div>
      </div>
    </div>
  );
}