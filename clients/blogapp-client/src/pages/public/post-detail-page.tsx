import { useMemo } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { motion } from 'framer-motion';
import { ArrowLeft, ArrowRight, Clock, BookOpen } from 'lucide-react';
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

  // Kelime sayısı ve okuma süresi hesaplama (makale içeriğine göre)
  const readingInfo = useMemo(() => {
    if (!post?.body) {
      return { wordCount: 0, readingMinutes: 1 };
    }

    // HTML taglerini temizle
    const plainText = post.body.replace(/<[^>]*>/g, ' ').replace(/\s+/g, ' ').trim();
    
    const words = plainText.split(' ').filter(Boolean);
    const wordCount = words.length;
    // Dakika başına 200 kelime okuma hızı varsayımı
    const readingMinutes = Math.max(1, Math.ceil(wordCount / 200));
    return { wordCount, readingMinutes };
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
        <div className="flex w-full flex-col gap-10">
        <Button variant="ghost" className="group mt-4 h-auto w-fit px-0 text-sm" asChild>
          <Link to="/" className="inline-flex items-center gap-2 text-muted-foreground transition-colors group-hover:text-primary">
            <ArrowLeft className="h-4 w-4 transition-transform duration-300 group-hover:-translate-x-1" />
            Ana sayfaya dön
          </Link>
        </Button>

        {/* Hero Section - Tam Genişlik */}
        <motion.section
          initial={{ opacity: 0, y: 16 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.4 }}
        >
          <div className="relative overflow-hidden rounded-[2rem] border border-border/40 shadow-xl shadow-primary/10">
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

            {/* Content - Sadeleştirilmiş */}
            <div className="relative px-6 py-8 sm:px-8 sm:py-10 lg:px-10 lg:py-12">
              <div className="space-y-5">
                <Badge className="w-fit rounded-full bg-primary/90 px-4 py-1 text-xs uppercase tracking-wider text-primary-foreground shadow-lg backdrop-blur-sm">
                  {categoryLabel}
                </Badge>
                
                <h1 className="text-balance text-2xl font-bold tracking-tight text-foreground drop-shadow-sm sm:text-3xl lg:text-4xl">
                  {post.title}
                </h1>
                
                <p className="text-base text-foreground/90 drop-shadow-sm sm:text-lg">
                  {post.summary}
                </p>

                {/* Makale Bilgileri */}
                <div className="flex items-center gap-4 text-sm text-foreground/80">
                  <div className="flex items-center gap-1.5">
                    <Clock className="h-4 w-4" />
                    <span>{readingInfo.readingMinutes} dk okuma</span>
                  </div>
                  <div className="h-1 w-1 rounded-full bg-foreground/30" />
                  <div className="flex items-center gap-1.5">
                    <BookOpen className="h-4 w-4" />
                    <span>{readingInfo.wordCount} kelime</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </motion.section>

        {/* Main Content Section - Tam Genişlik */}
        <motion.section
          initial={{ opacity: 0, y: 24 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1, duration: 0.4 }}
        >
          <div className="overflow-hidden rounded-[2.75rem] border border-border/70 bg-card/95 shadow-xl backdrop-blur">
            <div className="bg-background/95 px-4 py-8 sm:px-6 sm:py-10 lg:px-10 lg:py-12 xl:px-14 xl:py-14">
              {/* Makale İçeriği - Temiz ve Odaklanmış */}
              {sanitizedContent ? (
                <article className="blog-content" dangerouslySetInnerHTML={{ __html: sanitizedContent }} />
              ) : (
                <article className="blog-content">
                  <p>{post.summary ?? 'Bu gönderi için içerik bulunamadı.'}</p>
                </article>
              )}
            </div>
          </div>
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