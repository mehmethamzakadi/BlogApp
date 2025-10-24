import { useEffect } from 'react';
import { useBlocker } from 'react-router-dom';

export function useUnsavedChangesWarning(shouldBlock: boolean, message: string) {
  const blocker = useBlocker(shouldBlock);

  useEffect(() => {
    if (!shouldBlock) {
      return;
    }

    const handleBeforeUnload = (event: BeforeUnloadEvent) => {
      event.preventDefault();
      event.returnValue = message;
    };

    window.addEventListener('beforeunload', handleBeforeUnload);

    return () => {
      window.removeEventListener('beforeunload', handleBeforeUnload);
    };
  }, [shouldBlock, message]);

  useEffect(() => {
    if (blocker.state !== 'blocked') {
      return;
    }

    const confirmLeave = window.confirm(message);

    if (confirmLeave) {
      blocker.proceed();
    } else {
      blocker.reset();
    }
  }, [blocker, message]);
}
