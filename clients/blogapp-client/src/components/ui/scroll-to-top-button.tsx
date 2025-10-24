import { useEffect, useState } from 'react';
import { ArrowUp } from 'lucide-react';

import { Button } from './button';
import { cn } from '../../lib/utils';

export function ScrollToTopButton() {
  const [isVisible, setIsVisible] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      setIsVisible(window.scrollY > 200);
    };

    handleScroll();
    window.addEventListener('scroll', handleScroll, { passive: true });

    return () => {
      window.removeEventListener('scroll', handleScroll);
    };
  }, []);

  const scrollToTop = () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  return (
    <div className="pointer-events-none fixed bottom-6 right-6 z-50">
      <Button
        type="button"
        size="icon"
        variant="secondary"
        aria-label="Yukarı çık"
        onClick={scrollToTop}
        className={cn(
          'pointer-events-auto rounded-full shadow-lg transition-all duration-200',
          isVisible
            ? 'translate-y-0 opacity-100'
            : 'translate-y-4 opacity-0'
        )}
      >
        <ArrowUp className="h-5 w-5" />
      </Button>
    </div>
  );
}
