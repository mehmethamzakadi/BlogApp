import { Link } from 'react-router-dom';
import { ArrowRight, Calendar, Clock, BookOpen } from 'lucide-react';
import { Card } from '../ui/card';
import { Badge } from '../ui/badge';
import { cn } from '../../lib/utils';
import type { PostSummary } from '../../features/posts/types';
import { useMemo } from 'react';

interface PostCardProps {
  post: PostSummary;
  variant?: 'default' | 'horizontal' | 'featured' | 'compact';
}

export function PostCard({ post, variant = 'default' }: PostCardProps) {
  const hasThumbnail = Boolean(post.thumbnail);

  // Tarih formatı
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

  // Okuma süresi hesaplama - öncelikle body varsa onu kullan, yoksa summary'yi kullan
  const readingInfo = useMemo(() => {
    // Body varsa onu kullan, yoksa summary kullan
    const text = post.body || post.summary || '';
    
    // HTML taglerini temizle
    const plainText = text.replace(/<[^>]*>/g, ' ').replace(/\s+/g, ' ').trim();
    
    const words = plainText.split(' ').filter(Boolean);
    const wordCount = words.length;
    // Dakika başına 200 kelime okuma hızı varsayımı
    const readingMinutes = Math.max(1, Math.ceil(wordCount / 200));
    return { wordCount, readingMinutes };
  }, [post.body, post.summary]);

  return (
    <Link to={`/posts/${post.id}`} className="group block h-full">
      <Card
        className={cn(
          'flex h-full flex-col overflow-hidden rounded-3xl border border-border/60 bg-card/80 shadow-sm ring-1 ring-transparent transition-all duration-300 hover:-translate-y-1 hover:shadow-2xl hover:ring-primary/20',
          variant === 'horizontal' && 'md:flex-row',
          variant === 'featured' && 'lg:flex-row'
        )}
      >
        {hasThumbnail ? (
          <div
            className={cn(
              'relative overflow-hidden',
              variant === 'horizontal' && 'md:w-2/5',
              variant === 'featured' && 'lg:w-1/2 h-64 lg:h-auto',
              variant === 'compact' && 'h-40 w-full',
              variant === 'default' && 'h-52 w-full'
            )}
          >
            <img
              src={post.thumbnail}
              alt={post.title}
              className="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105"
            />
            <div className="absolute inset-0 bg-gradient-to-t from-background/80 via-background/10 to-transparent opacity-0 transition-opacity duration-500 group-hover:opacity-100" />
          </div>
        ) : (
          <div
            className={cn(
              'flex items-center justify-center bg-gradient-to-br from-primary/20 via-background to-secondary/30 text-primary',
              variant === 'horizontal' && 'md:w-2/5',
              variant === 'featured' && 'lg:w-1/2 h-64 lg:h-auto',
              variant === 'compact' && 'h-40 w-full',
              variant === 'default' && 'h-52 w-full'
            )}
          >
            <span className="text-sm font-medium tracking-wide">BlogApp</span>
          </div>
        )}

        <div
          className={cn(
            'flex flex-1 flex-col justify-between gap-6 p-6',
            variant === 'horizontal' && 'md:p-8',
            variant === 'featured' && 'lg:p-10',
            variant === 'compact' && 'gap-4 p-5',
            variant === 'default' && 'md:p-7'
          )}
        >
          <div className={cn('space-y-4', variant === 'compact' && 'space-y-3')}>
            <div className="flex items-center justify-between gap-3">
              <Badge variant="secondary" className="w-fit rounded-full bg-secondary/70 px-3 py-1 text-xs">
                {post.categoryName}
              </Badge>
              {formattedDate && variant !== 'compact' && (
                <div className="flex items-center gap-1.5 text-xs text-muted-foreground">
                  <Calendar className="h-3.5 w-3.5" />
                  <span>{formattedDate}</span>
                </div>
              )}
            </div>
            <h3 className={cn(
              'font-semibold leading-snug tracking-tight text-foreground transition-colors group-hover:text-primary',
              variant === 'featured' && 'text-3xl lg:text-4xl',
              variant === 'compact' && 'text-lg line-clamp-2',
              (variant === 'default' || variant === 'horizontal') && 'text-2xl'
            )}>
              {post.title}
            </h3>
            <p className={cn(
              'text-sm text-muted-foreground md:text-base',
              variant === 'featured' && 'line-clamp-4 text-base lg:text-lg',
              variant === 'compact' && 'line-clamp-2 text-sm',
              (variant === 'default' || variant === 'horizontal') && 'line-clamp-3'
            )}>
              {post.summary}
            </p>
          </div>
          
          <div className="flex items-center justify-between gap-4">
            {/* Makale Bilgileri */}
            <div className="flex items-center gap-3 text-xs text-muted-foreground">
              <div className="flex items-center gap-1">
                <Clock className="h-3.5 w-3.5" />
                <span>{readingInfo.readingMinutes} dk</span>
              </div>
              <div className="h-1 w-1 rounded-full bg-muted-foreground/30" />
              <div className="flex items-center gap-1">
                <BookOpen className="h-3.5 w-3.5" />
                <span>{readingInfo.wordCount} kelime</span>
              </div>
            </div>
            
            {/* Devamını Oku */}
            {variant !== 'compact' && (
              <div className="flex items-center gap-2 text-sm font-medium text-primary">
                Oku
                <ArrowRight className="h-4 w-4 transition-transform duration-300 group-hover:translate-x-1" />
              </div>
            )}
          </div>
        </div>
      </Card>
    </Link>
  );
}

export function PostCardSkeleton({ variant = 'default' }: { variant?: 'default' | 'horizontal' | 'featured' | 'compact' }) {
  return (
    <div
      className={cn(
        'flex h-full animate-pulse flex-col overflow-hidden rounded-3xl border border-border/60 bg-muted/30',
        variant === 'horizontal' && 'md:flex-row',
        variant === 'featured' && 'lg:flex-row'
      )}
    >
      <div className={cn(
        'bg-muted',
        variant === 'horizontal' && 'md:w-2/5',
        variant === 'featured' && 'lg:w-1/2 h-64 lg:h-auto',
        variant === 'compact' && 'h-40 w-full',
        variant === 'default' && 'h-52 w-full'
      )} />
      <div className={cn(
        'flex flex-1 flex-col justify-between gap-6 p-6',
        variant === 'compact' && 'gap-4 p-5'
      )}>
        <div className="space-y-4">
          <div className="h-3 w-20 rounded-full bg-muted" />
          <div className="h-6 w-3/4 rounded-full bg-muted" />
          <div className="h-3 w-full rounded-full bg-muted" />
          <div className="h-3 w-5/6 rounded-full bg-muted" />
        </div>
        <div className="h-3 w-24 rounded-full bg-muted" />
      </div>
    </div>
  );
}
