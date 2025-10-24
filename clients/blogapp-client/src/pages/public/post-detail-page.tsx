import { useMemo } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { motion } from 'framer-motion';
import { ArrowLeft, ArrowRight } from 'lucide-react';
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

  const { data: relatedPosts } = useQuery({
    queryKey: ['posts', 'published', 'related', post?.categoryId ?? 'all'],
    queryFn: () =>
      fetchPublishedPosts({
        pageIndex: 0,
        pageSize: 100,
        categoryId: post?.categoryId ?? undefined
      }),
    enabled: !!post
  });

  const { previousPost, nextPost } = useMemo(() => {
    if (!post || !relatedPosts?.items?.length) {
      return { previousPost: undefined, nextPost: undefined };
    }

    const currentIndex = relatedPosts.items.findIndex((item) => item.id === post.id);

    if (currentIndex === -1) {
      return { previousPost: undefined, nextPost: undefined };
    }

    const previous = currentIndex < relatedPosts.items.length - 1 ? relatedPosts.items[currentIndex + 1] : undefined;
    const next = currentIndex > 0 ? relatedPosts.items[currentIndex - 1] : undefined;

    return { previousPost: previous, nextPost: next };
  }, [post, relatedPosts?.items]);

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
    <div className="relative isolate space-y-14 pb-20">
      <div
        className="pointer-events-none absolute inset-0 -z-10 bg-gradient-to-br from-primary/5 via-background to-secondary/20"
        aria-hidden
      />
      <Button variant="ghost" className="group h-auto px-0 text-sm" asChild>
        <Link to="/" className="inline-flex items-center gap-2 text-muted-foreground transition-colors group-hover:text-primary">
          <ArrowLeft className="h-4 w-4 transition-transform duration-300 group-hover:-translate-x-1" />
          Ana sayfaya dön
        </Link>
      </Button>

      <motion.section
        className="relative overflow-hidden rounded-[3rem] border border-border/40 bg-gradient-to-br from-primary/10 via-background to-secondary/20 shadow-xl shadow-primary/5"
        initial={{ opacity: 0, y: 16 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.4 }}
      >
        {post.thumbnail && (
          <img
            src={post.thumbnail}
            alt={post.title}
            className="absolute inset-0 h-full w-full object-cover opacity-40"
          />
        )}
        <div className="absolute inset-0 bg-gradient-to-br from-background via-background/60 to-background" aria-hidden />
        <div className="relative z-10 space-y-6 px-6 py-14 text-center sm:px-12 lg:px-20">
          <Badge className="mx-auto w-fit rounded-full bg-primary/75 px-4 py-1 text-xs uppercase tracking-wider text-primary-foreground shadow-sm">
            {post.categoryName}
          </Badge>
          <h1 className="mx-auto max-w-4xl text-4xl font-semibold tracking-tight text-foreground sm:text-5xl">
            {post.title}
          </h1>
          <p className="mx-auto max-w-2xl text-lg text-muted-foreground/90 sm:text-xl">{post.summary}</p>
        </div>
      </motion.section>

      <motion.section
        className="mx-auto w-full max-w-5xl overflow-hidden rounded-[2.75rem] border border-border/60 bg-card/95 shadow-xl backdrop-blur"
        initial={{ opacity: 0, y: 24 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 0.1, duration: 0.4 }}
      >
        <div className="bg-gradient-to-b from-background/90 via-background to-secondary/10 px-6 py-10 sm:px-12 sm:py-14 lg:px-20 lg:py-20">
          {sanitizedContent ? (
            <article className="blog-content" dangerouslySetInnerHTML={{ __html: sanitizedContent }} />
          ) : (
            <article className="blog-content">
              <p>{post.summary ?? 'Bu gönderi için içerik bulunamadı.'}</p>
            </article>
          )}
        </div>
      </motion.section>

      <motion.nav
        className="mx-auto w-full max-w-5xl space-y-4 rounded-[2.5rem] border border-border/50 bg-background/80 p-6 shadow-lg shadow-primary/5 backdrop-blur"
        initial={{ opacity: 0, y: 24 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 0.2, duration: 0.4 }}
      >
        <div className="flex flex-col items-start gap-6 sm:flex-row sm:items-stretch sm:justify-between">
          {previousPost ? (
            <Button
              variant="outline"
              asChild
              className="group flex h-full w-full flex-1 flex-col items-start gap-3 rounded-2xl border border-border/60 bg-background/90 p-5 text-left shadow-sm transition-all hover:-translate-y-0.5 hover:border-primary/50 hover:bg-primary/10 hover:text-foreground sm:max-w-none"
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
            <div className="flex h-full w-full flex-1 flex-col justify-center rounded-2xl border border-dashed border-border/60 bg-muted/30 p-5 text-sm text-muted-foreground">
              Daha eski bir yazı bulunamadı.
            </div>
          )}

          {nextPost ? (
            <Button
              variant="outline"
              asChild
              className="group flex h-full w-full flex-1 flex-col items-start gap-3 rounded-2xl border border-border/60 bg-background/90 p-5 text-left shadow-sm transition-all hover:-translate-y-0.5 hover:border-primary/50 hover:bg-primary/10 hover:text-foreground sm:max-w-none"
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
            <div className="flex h-full w-full flex-1 flex-col justify-center rounded-2xl border border-dashed border-border/60 bg-muted/30 p-5 text-sm text-muted-foreground">
              Daha yeni bir yazı bulunamadı.
            </div>
          )}
        </div>
      </motion.nav>
    </div>
  );
}
