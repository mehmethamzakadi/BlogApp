import { Link } from 'react-router-dom';
import { ArrowRight } from 'lucide-react';
import { Card } from '../ui/card';
import { Badge } from '../ui/badge';
import { cn } from '../../lib/utils';
import type { PostSummary } from '../../features/posts/types';

interface PostCardProps {
  post: PostSummary;
  variant?: 'default' | 'horizontal';
}

export function PostCard({ post, variant = 'default' }: PostCardProps) {
  const hasThumbnail = Boolean(post.thumbnail);

  return (
    <Link to={`/posts/${post.id}`} className="group block h-full">
      <Card
        className={cn(
          'flex h-full flex-col overflow-hidden rounded-3xl border border-border/60 bg-card/80 shadow-sm ring-1 ring-transparent transition-all duration-300 hover:-translate-y-1 hover:shadow-2xl hover:ring-primary/20',
          variant === 'horizontal' && 'md:flex-row'
        )}
      >
        {hasThumbnail ? (
          <div
            className={cn(
              'relative overflow-hidden',
              variant === 'horizontal' ? 'md:w-2/5' : 'h-52 w-full'
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
              variant === 'horizontal' ? 'md:w-2/5' : 'h-52 w-full'
            )}
          >
            <span className="text-sm font-medium tracking-wide">BlogApp</span>
          </div>
        )}

        <div
          className={cn(
            'flex flex-1 flex-col justify-between gap-6 p-6',
            variant === 'horizontal' ? 'md:p-8' : 'md:p-7'
          )}
        >
          <div className="space-y-4">
            <Badge variant="secondary" className="w-fit rounded-full bg-secondary/70 px-3 py-1 text-xs">
              {post.categoryName}
            </Badge>
            <h3 className="text-2xl font-semibold leading-snug tracking-tight text-foreground transition-colors group-hover:text-primary">
              {post.title}
            </h3>
            <p className="line-clamp-3 text-sm text-muted-foreground md:text-base">
              {post.summary}
            </p>
          </div>
          <div className="flex items-center gap-2 text-sm font-medium text-primary">
            Devamını Oku
            <ArrowRight className="h-4 w-4 transition-transform duration-300 group-hover:translate-x-1" />
          </div>
        </div>
      </Card>
    </Link>
  );
}

export function PostCardSkeleton({ variant = 'default' }: { variant?: 'default' | 'horizontal' }) {
  return (
    <div
      className={cn(
        'flex h-full animate-pulse flex-col overflow-hidden rounded-3xl border border-border/60 bg-muted/30',
        variant === 'horizontal' && 'md:flex-row'
      )}
    >
      <div className={cn(variant === 'horizontal' ? 'md:w-2/5' : 'h-52 w-full', 'bg-muted')} />
      <div className="flex flex-1 flex-col justify-between gap-6 p-6">
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
