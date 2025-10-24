import { useMemo } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { motion } from 'framer-motion';
import { ArrowLeft } from 'lucide-react';
import { getPostById } from '../../features/posts/api';
import { Badge } from '../../components/ui/badge';
import { Button } from '../../components/ui/button';
import { sanitizeHtml } from '../../lib/sanitize-html';

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

    return sanitizeHtml(post.body);
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
    <div className="space-y-12">
      <Button variant="ghost" className="group h-auto px-0 text-sm" asChild>
        <Link to="/" className="inline-flex items-center gap-2 text-muted-foreground transition-colors group-hover:text-primary">
          <ArrowLeft className="h-4 w-4 transition-transform duration-300 group-hover:-translate-x-1" />
          Ana sayfaya dön
        </Link>
      </Button>

      <motion.section
        className="relative overflow-hidden rounded-[2.75rem] border border-border/50 bg-gradient-to-br from-background via-background to-muted/40"
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
        <div className="relative z-10 space-y-6 px-6 py-14 text-center sm:px-10 lg:px-16">
          <Badge className="mx-auto w-fit rounded-full bg-primary/80 px-4 py-1 text-xs uppercase tracking-wider text-primary-foreground">
            {post.categoryName}
          </Badge>
          <h1 className="text-4xl font-semibold tracking-tight text-foreground sm:text-5xl">{post.title}</h1>
          <p className="mx-auto max-w-2xl text-lg text-muted-foreground">{post.summary}</p>
        </div>
      </motion.section>

      <motion.section
        className="mx-auto w-full max-w-4xl overflow-hidden rounded-[2.5rem] border border-border/70 bg-card/95 shadow-lg backdrop-blur"
        initial={{ opacity: 0, y: 24 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 0.1, duration: 0.4 }}
      >
        <div className="bg-gradient-to-b from-background/80 via-background to-background/95 px-6 py-10 sm:px-10 sm:py-12 lg:px-16 lg:py-16">
          {sanitizedContent ? (
            <article className="blog-content" dangerouslySetInnerHTML={{ __html: sanitizedContent }} />
          ) : (
            <article className="blog-content">
              <p>{post.summary ?? 'Bu gönderi için içerik bulunamadı.'}</p>
            </article>
          )}
        </div>
      </motion.section>
    </div>
  );
}
