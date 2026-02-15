// Accessibility utilities and keyboard navigation

/**
 * Keyboard shortcuts configuration
 */
export const KEYBOARD_SHORTCUTS = {
  DASHBOARD: 'Alt+D',
  SECURITIES: 'Alt+S',
  LENDING: 'Alt+L',
  BANKING: 'Alt+B',
  GRANTS: 'Alt+G',
  SEARCH: 'Alt+/',
  HELP: 'Alt+H',
  LOGOUT: 'Alt+Q',
  ESCAPE: 'Escape',
  ENTER: 'Enter',
  SPACE: 'Space',
  TAB: 'Tab',
  ARROW_UP: 'ArrowUp',
  ARROW_DOWN: 'ArrowDown',
  ARROW_LEFT: 'ArrowLeft',
  ARROW_RIGHT: 'ArrowRight',
};

/**
 * Announce message to screen readers
 */
export function announceToScreenReader(message: string, priority: 'polite' | 'assertive' = 'polite'): void {
  const announcement = document.createElement('div');
  announcement.setAttribute('role', 'status');
  announcement.setAttribute('aria-live', priority);
  announcement.setAttribute('aria-atomic', 'true');
  announcement.className = 'sr-only';
  announcement.textContent = message;
  
  document.body.appendChild(announcement);
  
  // Remove after announcement
  setTimeout(() => {
    document.body.removeChild(announcement);
  }, 1000);
}

/**
 * Focus management utilities
 */
export function trapFocus(element: HTMLElement): () => void {
  const focusableElements = element.querySelectorAll<HTMLElement>(
    'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
  );
  
  const firstFocusable = focusableElements[0];
  const lastFocusable = focusableElements[focusableElements.length - 1];
  
  const handleTabKey = (e: KeyboardEvent) => {
    if (e.key !== 'Tab') return;
    
    if (e.shiftKey) {
      if (document.activeElement === firstFocusable) {
        lastFocusable.focus();
        e.preventDefault();
      }
    } else {
      if (document.activeElement === lastFocusable) {
        firstFocusable.focus();
        e.preventDefault();
      }
    }
  };
  
  element.addEventListener('keydown', handleTabKey);
  
  // Return cleanup function
  return () => {
    element.removeEventListener('keydown', handleTabKey);
  };
}

/**
 * Set focus to element with optional delay
 */
export function setFocus(element: HTMLElement | null, delay: number = 0): void {
  if (!element) return;
  
  if (delay > 0) {
    setTimeout(() => element.focus(), delay);
  } else {
    element.focus();
  }
}

/**
 * Get accessible label for form field
 */
export function getAccessibleLabel(id: string, label: string, required: boolean = false): {
  id: string;
  'aria-label': string;
  'aria-required': boolean;
} {
  return {
    id,
    'aria-label': `${label}${required ? ' (required)' : ''}`,
    'aria-required': required,
  };
}

/**
 * Get accessible error message attributes
 */
export function getAccessibleError(fieldId: string, errorMessage?: string): {
  'aria-invalid': boolean;
  'aria-describedby'?: string;
} {
  if (!errorMessage) {
    return { 'aria-invalid': false };
  }
  
  return {
    'aria-invalid': true,
    'aria-describedby': `${fieldId}-error`,
  };
}

/**
 * Keyboard navigation handler
 */
export class KeyboardNavigationHandler {
  private shortcuts: Map<string, () => void> = new Map();
  
  constructor() {
    this.handleKeyPress = this.handleKeyPress.bind(this);
    document.addEventListener('keydown', this.handleKeyPress);
  }
  
  register(key: string, callback: () => void): void {
    this.shortcuts.set(key, callback);
  }
  
  unregister(key: string): void {
    this.shortcuts.delete(key);
  }
  
  private handleKeyPress(event: KeyboardEvent): void {
    // Don't trigger shortcuts when typing in input fields
    const target = event.target as HTMLElement;
    if (target.tagName === 'INPUT' || target.tagName === 'TEXTAREA' || target.tagName === 'SELECT') {
      return;
    }
    
    const key = this.getKeyString(event);
    const callback = this.shortcuts.get(key);
    
    if (callback) {
      event.preventDefault();
      callback();
    }
  }
  
  private getKeyString(event: KeyboardEvent): string {
    const parts: string[] = [];
    
    if (event.altKey) parts.push('Alt');
    if (event.ctrlKey) parts.push('Ctrl');
    if (event.shiftKey) parts.push('Shift');
    if (event.metaKey) parts.push('Meta');
    
    parts.push(event.key);
    
    return parts.join('+');
  }
  
  destroy(): void {
    document.removeEventListener('keydown', this.handleKeyPress);
    this.shortcuts.clear();
  }
}

/**
 * Skip to main content link
 */
export function createSkipLink(): HTMLAnchorElement {
  const skipLink = document.createElement('a');
  skipLink.href = '#main-content';
  skipLink.textContent = 'Skip to main content';
  skipLink.className = 'sr-only focus:not-sr-only focus:absolute focus:top-0 focus:left-0 focus:z-50 focus:p-4 focus:bg-blue-600 focus:text-white';
  
  skipLink.addEventListener('click', (e) => {
    e.preventDefault();
    const mainContent = document.getElementById('main-content');
    if (mainContent) {
      mainContent.focus();
      mainContent.scrollIntoView();
    }
  });
  
  return skipLink;
}

/**
 * Check if element is visible to screen readers
 */
export function isVisibleToScreenReader(element: HTMLElement): boolean {
  const style = window.getComputedStyle(element);
  
  return !(
    style.display === 'none' ||
    style.visibility === 'hidden' ||
    element.getAttribute('aria-hidden') === 'true' ||
    element.hasAttribute('hidden')
  );
}

/**
 * Generate unique ID for accessibility
 */
let idCounter = 0;
export function generateAccessibleId(prefix: string = 'a11y'): string {
  return `${prefix}-${++idCounter}`;
}
