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
        <div className="mx-auto max-w-3xl space-y-4">
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

  return (
    <div className="relative isolate space-y-16 pb-24">
      <div
        className="pointer-events-none absolute inset-0 -z-10 bg-gradient-to-br from-primary/5 via-background to-secondary/20"
        aria-hidden
      />
      <div
        className="pointer-events-none absolute -left-20 top-24 -z-10 h-72 w-72 rounded-full bg-primary/15 blur-3xl sm:h-96 sm:w-96"
        aria-hidden
      />
      <div
        className="pointer-events-none absolute -right-24 top-1/2 -z-10 h-80 w-80 rounded-full bg-secondary/30 blur-3xl sm:h-[28rem] sm:w-[28rem]"
        aria-hidden
      />
      <Button variant="ghost" className="group h-auto px-0 text-sm" asChild>
        <Link to="/" className="inline-flex items-center gap-2 text-muted-foreground transition-colors group-hover:text-primary">
          <ArrowLeft className="h-4 w-4 transition-transform duration-300 group-hover:-translate-x-1" />
          Ana sayfaya dön
        </Link>
      </Button>

      <motion.section
        className="relative overflow-hidden rounded-[3.5rem] border border-border/40 bg-gradient-to-br from-primary/15 via-background to-secondary/20 shadow-xl shadow-primary/5"
        initial={{ opacity: 0, y: 16 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.4 }}
      >
        <div className="grid gap-10 px-6 py-14 sm:px-12 lg:grid-cols-[1.25fr_minmax(0,0.75fr)] lg:items-center lg:px-20">
          <div className="relative z-10 space-y-6 text-center lg:text-left">
            <Badge className="mx-auto w-fit rounded-full bg-primary/80 px-4 py-1 text-xs uppercase tracking-wider text-primary-foreground shadow-sm lg:mx-0">
              {post.categoryName}
            </Badge>
            <h1 className="mx-auto max-w-3xl text-4xl font-semibold tracking-tight text-foreground sm:text-5xl lg:mx-0 lg:text-6xl">
              {post.title}
            </h1>
            <p className="mx-auto max-w-2xl text-lg text-muted-foreground/90 sm:text-xl lg:mx-0">{post.summary}</p>
            <div className="flex flex-wrap items-center justify-center gap-4 text-sm text-muted-foreground/80 lg:justify-start">
              <span className="inline-flex items-center gap-2 rounded-full bg-background/60 px-4 py-2 shadow-sm ring-1 ring-border/60">
                <Calendar className="h-4 w-4" />
                Yayınlanan makale
              </span>
              {readingInsights.wordCount > 0 && (
                <span className="inline-flex items-center gap-2 rounded-full bg-background/60 px-4 py-2 shadow-sm ring-1 ring-border/60">
                  <Clock className="h-4 w-4" />
                  {readingInsights.readingMinutes} dakikalık okuma · {readingInsights.wordCount} kelime
                </span>
              )}
            </div>
          </div>

          <div className="relative">
            <div className="absolute inset-0 rounded-[3rem] bg-gradient-to-br from-primary/30 via-primary/10 to-secondary/30 opacity-70 blur-3xl" />
            <div className="relative overflow-hidden rounded-[2.75rem] border border-border/50 bg-background/80 shadow-2xl shadow-primary/10">
              {post.thumbnail ? (
                <img src={post.thumbnail} alt={post.title} className="h-full w-full object-cover" />
              ) : (
                <div className="flex h-full min-h-[240px] items-center justify-center bg-gradient-to-br from-primary/10 via-primary/5 to-secondary/20 p-10 text-center text-lg text-muted-foreground">
                  Bu makale için görsel henüz eklenmedi.
                </div>
              )}
            </div>
          </div>
        </div>
      </motion.section>

      <motion.section
        className="mx-auto w-full max-w-6xl"
        initial={{ opacity: 0, y: 24 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 0.1, duration: 0.4 }}
      >
        <div className="grid gap-10 lg:grid-cols-[minmax(0,1fr)_320px]">
          <div className="overflow-hidden rounded-[2.75rem] border border-border/70 bg-card/95 shadow-xl backdrop-blur">
            <div className="bg-gradient-to-b from-background/90 via-background to-secondary/10 px-6 py-10 sm:px-12 sm:py-14 lg:px-16 lg:py-20">
              {sanitizedContent ? (
                <article className="blog-content" dangerouslySetInnerHTML={{ __html: sanitizedContent }} />
              ) : (
                <article className="blog-content">
                  <p>{post.summary ?? 'Bu gönderi için içerik bulunamadı.'}</p>
                </article>
              )}
            </div>
          </div>

          <aside className="flex flex-col gap-6 lg:pt-6">
            <div className="sticky top-28 space-y-6">
              <div className="overflow-hidden rounded-[2rem] border border-border/70 bg-gradient-to-br from-background to-secondary/20 p-8 shadow-lg">
                <h2 className="text-xl font-semibold text-foreground">Makale bilgileri</h2>
                <p className="mt-2 text-sm text-muted-foreground">
                  Göz yormayan renkler ve ferah bir düzen ile bu yazıyı keyifle okuyabilirsiniz.
                </p>
                <div className="mt-6 space-y-4 text-sm text-muted-foreground">
                  <div className="flex items-center justify-between rounded-xl bg-background/80 px-4 py-3 ring-1 ring-border/70">
                    <span className="font-medium text-foreground">Kategori</span>
                    <span>{post.categoryName}</span>
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

              <div className="rounded-[2rem] border border-border/70 bg-gradient-to-br from-primary/10 via-primary/5 to-secondary/30 p-8 shadow-lg">
                <h3 className="text-lg font-semibold text-foreground">Yeni hikayeleri keşfet</h3>
                <p className="mt-2 text-sm text-muted-foreground">
                  Okumaya ara verdiğinizde bile sonraki veya önceki yazıya kolayca geçebilirsiniz.
                </p>
                <div className="mt-6 space-y-3 text-sm text-muted-foreground">
                  <div className="flex items-center justify-between rounded-xl bg-background/80 px-4 py-3 ring-1 ring-border/70">
                    <span>Önceki yazıya git</span>
                    <ArrowLeft className="h-4 w-4 text-primary" />
                  </div>
                  <div className="flex items-center justify-between rounded-xl bg-background/80 px-4 py-3 ring-1 ring-border/70">
                    <span>Sonraki yazıya git</span>
                    <ArrowRight className="h-4 w-4 text-primary" />
                  </div>
                </div>
              </div>
            </div>
          </aside>
        </div>
      </motion.section>

      <motion.nav
        className="mx-auto w-full max-w-6xl space-y-6 rounded-[2.75rem] border border-border/50 bg-background/90 p-8 shadow-2xl shadow-primary/10 backdrop-blur"
        initial={{ opacity: 0, y: 24 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 0.2, duration: 0.4 }}
      >
        <div className="grid gap-6 lg:grid-cols-2">
          {previousPost ? (
            <Button
              variant="outline"
              asChild
              className="group flex h-full flex-col items-start gap-3 rounded-2xl border border-border/60 bg-background/90 p-6 text-left shadow-lg transition-all hover:-translate-y-1 hover:border-primary/50 hover:bg-primary/10 hover:text-foreground"
            >
              <Link to={`/posts/${previousPost.id}`}>
                <span className="text-xs font-semibold uppercase tracking-wider text-muted-foreground group-hover:text-primary">
                  Bir önceki yazı
                </span>
                <span className="text-lg font-semibold text-foreground group-hover:text-primary/90">{previousPost.title}</span>
                <span className="mt-1 inline-flex items-center gap-2 text-sm font-medium text-primary/80 group-hover:text-primary">
                  <ArrowLeft className="h-4 w-4 transition-transform duration-300 group-hover:-translate-x-1" />
                  Önceki gönderiye git
                </span>
              </Link>
            </Button>
          ) : (
            <div className="flex h-full flex-col justify-center rounded-2xl border border-dashed border-border/60 bg-muted/30 p-6 text-sm text-muted-foreground">
              Daha eski bir yazı bulunamadı.
            </div>
          )}

          {nextPost ? (
            <Button
              variant="outline"
              asChild
              className="group flex h-full flex-col items-start gap-3 rounded-2xl border border-border/60 bg-background/90 p-6 text-left shadow-lg transition-all hover:-translate-y-1 hover:border-primary/50 hover:bg-primary/10 hover:text-foreground"
            >
              <Link to={`/posts/${nextPost.id}`}>
                <span className="text-xs font-semibold uppercase tracking-wider text-muted-foreground group-hover:text-primary">
                  Bir sonraki yazı
                </span>
                <span className="text-lg font-semibold text-foreground group-hover:text-primary/90">{nextPost.title}</span>
                <span className="mt-1 inline-flex items-center gap-2 text-sm font-medium text-primary/80 group-hover:text-primary">
                  Sonraki gönderiye git
                  <ArrowRight className="h-4 w-4 transition-transform duration-300 group-hover:translate-x-1" />
                </span>
              </Link>
            </Button>
          ) : (
            <div className="flex h-full flex-col justify-center rounded-2xl border border-dashed border-border/60 bg-muted/30 p-6 text-sm text-muted-foreground">
              Daha yeni bir yazı bulunamadı.
            </div>
          )}
        </div>
      </motion.nav>
    </div>
  );
}
