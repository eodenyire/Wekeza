import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { KeyboardNavigationHandler, KEYBOARD_SHORTCUTS, announceToScreenReader } from '../utils/accessibility';

/**
 * Hook for keyboard navigation shortcuts
 */
export function useKeyboardNavigation() {
  const navigate = useNavigate();

  useEffect(() => {
    const handler = new KeyboardNavigationHandler();

    // Register navigation shortcuts
    handler.register(KEYBOARD_SHORTCUTS.DASHBOARD, () => {
      navigate('/public-sector/dashboard');
      announceToScreenReader('Navigated to Dashboard');
    });

    handler.register(KEYBOARD_SHORTCUTS.SECURITIES, () => {
      navigate('/public-sector/securities');
      announceToScreenReader('Navigated to Securities Trading');
    });

    handler.register(KEYBOARD_SHORTCUTS.LENDING, () => {
      navigate('/public-sector/lending');
      announceToScreenReader('Navigated to Government Lending');
    });

    handler.register(KEYBOARD_SHORTCUTS.BANKING, () => {
      navigate('/public-sector/banking');
      announceToScreenReader('Navigated to Banking Services');
    });

    handler.register(KEYBOARD_SHORTCUTS.GRANTS, () => {
      navigate('/public-sector/grants');
      announceToScreenReader('Navigated to Grants & Philanthropy');
    });

    // Cleanup
    return () => {
      handler.destroy();
    };
  }, [navigate]);
}

/**
 * Hook for focus trap in modals
 */
export function useFocusTrap(isOpen: boolean, elementRef: React.RefObject<HTMLElement>) {
  useEffect(() => {
    if (!isOpen || !elementRef.current) return;

    const element = elementRef.current;
    const focusableElements = element.querySelectorAll<HTMLElement>(
      'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
    );

    const firstFocusable = focusableElements[0];
    const lastFocusable = focusableElements[focusableElements.length - 1];

    // Focus first element
    firstFocusable?.focus();

    const handleTabKey = (e: KeyboardEvent) => {
      if (e.key !== 'Tab') return;

      if (e.shiftKey) {
        if (document.activeElement === firstFocusable) {
          lastFocusable?.focus();
          e.preventDefault();
        }
      } else {
        if (document.activeElement === lastFocusable) {
          firstFocusable?.focus();
          e.preventDefault();
        }
      }
    };

    element.addEventListener('keydown', handleTabKey);

    return () => {
      element.removeEventListener('keydown', handleTabKey);
    };
  }, [isOpen, elementRef]);
}

/**
 * Hook for announcing dynamic content changes
 */
export function useAnnouncement() {
  return {
    announce: (message: string, priority: 'polite' | 'assertive' = 'polite') => {
      announceToScreenReader(message, priority);
    },
  };
}
