# Accessibility & Internationalization Guide

## Overview

The Public Sector Portal now includes comprehensive accessibility features and multi-language support (English and Swahili).

## Accessibility Features

### Keyboard Navigation

The portal supports keyboard shortcuts for quick navigation:

| Shortcut | Action |
|----------|--------|
| `Alt+D` | Navigate to Dashboard |
| `Alt+S` | Navigate to Securities Trading |
| `Alt+L` | Navigate to Government Lending |
| `Alt+B` | Navigate to Banking Services |
| `Alt+G` | Navigate to Grants & Philanthropy |
| `Alt+H` | Show keyboard shortcuts help |
| `Alt+Q` | Logout |
| `Esc` | Close modals and dialogs |
| `Tab` | Navigate forward through elements |
| `Shift+Tab` | Navigate backward through elements |
| `Enter` | Activate buttons and links |
| `Space` | Toggle checkboxes and buttons |

### Screen Reader Support

The portal is fully compatible with screen readers:

- **NVDA** (Windows)
- **JAWS** (Windows)
- **VoiceOver** (macOS/iOS)
- **TalkBack** (Android)

Features:
- Live region announcements for dynamic content
- Proper ARIA labels and roles
- Semantic HTML structure
- Skip to main content link
- Descriptive alt text for images

### Visual Accessibility

- **Focus Indicators**: Clear visual indicators when navigating with keyboard
- **High Contrast Mode**: Automatic support for high contrast preferences
- **Reduced Motion**: Respects user's motion preferences
- **Color Contrast**: WCAG AA compliant color contrast ratios
- **Text Scaling**: Supports browser text scaling up to 200%

### Using Accessibility Features

#### For Keyboard Users

1. Press `Tab` to navigate through interactive elements
2. Use `Alt+[Key]` shortcuts for quick navigation
3. Press `Alt+H` to view all available shortcuts
4. Press `Esc` to close any open modal or dialog

#### For Screen Reader Users

1. Use your screen reader's navigation commands
2. Listen for live region announcements when content changes
3. Use the "Skip to main content" link to bypass navigation
4. Form fields have proper labels and error messages

#### For Users with Visual Impairments

1. Enable high contrast mode in your operating system
2. Use browser zoom (Ctrl/Cmd + Plus/Minus)
3. All interactive elements have clear focus indicators
4. Color is not the only means of conveying information

## Internationalization (i18n)

### Supported Languages

1. **English** (en) - Default
2. **Swahili** (sw) - Kiswahili

### Changing Language

#### Using the Language Switcher

1. Look for the globe icon (🌐) in the top navigation bar
2. Click on it to see available languages
3. Select your preferred language
4. The interface will immediately update

#### Language Persistence

Your language preference is saved in your browser and will be remembered for future sessions.

### Translation Coverage

All UI elements are translated, including:

- Navigation menus
- Form labels and placeholders
- Button text
- Error messages
- Success messages
- Validation messages
- Help text
- Keyboard shortcuts
- Dashboard metrics
- Module-specific terminology

### Adding More Languages

To add a new language:

1. Create a new translation file in `src/channels/public-sector/i18n/locales/`
2. Copy the structure from `en.json`
3. Translate all strings
4. Update `i18n/config.ts` to include the new language
5. Add the language to the `LanguageSwitcher` component

Example:
```typescript
// In i18n/config.ts
import frTranslations from './locales/fr.json';

i18n.init({
  resources: {
    en: { translation: enTranslations },
    sw: { translation: swTranslations },
    fr: { translation: frTranslations } // New language
  }
});
```

## Developer Guide

### Using Translations in Components

```typescript
import { useTranslation } from 'react-i18next';

function MyComponent() {
  const { t } = useTranslation();
  
  return (
    <div>
      <h1>{t('navigation.dashboard')}</h1>
      <button>{t('common.submit')}</button>
    </div>
  );
}
```

### Using Accessibility Utilities

```typescript
import { 
  announceToScreenReader, 
  getAccessibleLabel,
  getAccessibleError 
} from '../utils/accessibility';

// Announce to screen reader
announceToScreenReader('Form submitted successfully', 'polite');

// Get accessible label for form field
const labelProps = getAccessibleLabel('email', 'Email Address', true);
<input {...labelProps} />

// Get accessible error attributes
const errorProps = getAccessibleError('email', errorMessage);
<input {...errorProps} />
```

### Using Keyboard Navigation Hook

```typescript
import { useKeyboardNavigation } from '../hooks/useKeyboardNavigation';

function MyComponent() {
  // Automatically enables keyboard shortcuts
  useKeyboardNavigation();
  
  return <div>Content</div>;
}
```

### Using Focus Trap Hook

```typescript
import { useFocusTrap } from '../hooks/useKeyboardNavigation';
import { useRef } from 'react';

function Modal({ isOpen, onClose }) {
  const modalRef = useRef<HTMLDivElement>(null);
  
  // Trap focus within modal when open
  useFocusTrap(isOpen, modalRef);
  
  return (
    <div ref={modalRef}>
      {/* Modal content */}
    </div>
  );
}
```

## Testing Accessibility

### Manual Testing

1. **Keyboard Navigation**:
   - Unplug your mouse
   - Navigate the entire application using only keyboard
   - Ensure all interactive elements are reachable
   - Check that focus indicators are visible

2. **Screen Reader Testing**:
   - Enable a screen reader (NVDA, JAWS, VoiceOver)
   - Navigate through the application
   - Verify all content is announced properly
   - Check that dynamic content changes are announced

3. **Visual Testing**:
   - Enable high contrast mode
   - Zoom to 200%
   - Check color contrast with browser tools
   - Test with reduced motion enabled

### Automated Testing

Use tools like:
- **axe DevTools** - Browser extension for accessibility testing
- **WAVE** - Web accessibility evaluation tool
- **Lighthouse** - Chrome DevTools accessibility audit
- **Pa11y** - Automated accessibility testing

## Best Practices

### For Developers

1. Always use semantic HTML elements
2. Provide proper ARIA labels for custom components
3. Ensure keyboard navigation works for all interactive elements
4. Test with screen readers regularly
5. Use the translation function (`t()`) for all user-facing text
6. Never hardcode text strings in components
7. Maintain proper heading hierarchy (h1, h2, h3, etc.)
8. Provide alternative text for images
9. Ensure sufficient color contrast
10. Test with keyboard only

### For Content Creators

1. Write clear, concise labels
2. Provide helpful error messages
3. Use descriptive button text (avoid "Click here")
4. Keep translations culturally appropriate
5. Maintain consistent terminology
6. Provide context in form labels
7. Use plain language
8. Avoid jargon when possible

## Troubleshooting

### Language Not Changing

1. Check browser console for errors
2. Verify translation file exists
3. Clear browser cache
4. Check localStorage for 'language' key

### Keyboard Shortcuts Not Working

1. Ensure you're not in an input field
2. Check browser console for errors
3. Verify keyboard navigation hook is initialized
4. Try refreshing the page

### Screen Reader Issues

1. Ensure screen reader is properly configured
2. Check that ARIA labels are present
3. Verify live regions are working
4. Test with different screen readers

## Resources

### Accessibility

- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [MDN Accessibility](https://developer.mozilla.org/en-US/docs/Web/Accessibility)
- [WebAIM](https://webaim.org/)
- [A11y Project](https://www.a11yproject.com/)

### Internationalization

- [react-i18next Documentation](https://react.i18next.com/)
- [i18next Documentation](https://www.i18next.com/)
- [Unicode CLDR](http://cldr.unicode.org/)

## Support

For accessibility or internationalization issues:
1. Check this guide first
2. Review the code documentation
3. Test with different browsers and assistive technologies
4. Contact the development team

## Future Enhancements

Planned improvements:
- Additional language support (French, Arabic, etc.)
- Voice command support
- More keyboard shortcuts
- Enhanced screen reader announcements
- Accessibility preferences panel
- Right-to-left (RTL) language support
