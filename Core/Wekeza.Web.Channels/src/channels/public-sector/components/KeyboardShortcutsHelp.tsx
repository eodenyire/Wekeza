import React, { useRef, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { X, Keyboard } from 'lucide-react';
import { useFocusTrap } from '../hooks/useKeyboardNavigation';

interface KeyboardShortcutsHelpProps {
  isOpen: boolean;
  onClose: () => void;
}

export const KeyboardShortcutsHelp: React.FC<KeyboardShortcutsHelpProps> = ({ isOpen, onClose }) => {
  const { t } = useTranslation();
  const modalRef = useRef<HTMLDivElement>(null);
  
  useFocusTrap(isOpen, modalRef);

  useEffect(() => {
    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === 'Escape' && isOpen) {
        onClose();
      }
    };

    document.addEventListener('keydown', handleEscape);
    return () => document.removeEventListener('keydown', handleEscape);
  }, [isOpen, onClose]);

  if (!isOpen) return null;

  const shortcuts = [
    { key: 'Alt+D', description: t('keyboard.dashboard') },
    { key: 'Alt+S', description: t('keyboard.securities') },
    { key: 'Alt+L', description: t('keyboard.lending') },
    { key: 'Alt+B', description: t('keyboard.banking') },
    { key: 'Alt+G', description: t('keyboard.grants') },
    { key: 'Alt+/', description: t('keyboard.search') },
    { key: 'Alt+H', description: t('keyboard.help') },
    { key: 'Alt+Q', description: t('keyboard.logout') },
    { key: 'Esc', description: 'Close dialogs and modals' },
    { key: 'Tab', description: 'Navigate between elements' },
    { key: 'Shift+Tab', description: 'Navigate backwards' },
    { key: 'Enter', description: 'Activate buttons and links' },
    { key: 'Space', description: 'Toggle checkboxes and buttons' }
  ];

  return (
    <div
      className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      onClick={onClose}
      role="dialog"
      aria-modal="true"
      aria-labelledby="keyboard-shortcuts-title"
    >
      <div
        ref={modalRef}
        className="bg-white rounded-lg p-6 max-w-2xl w-full max-h-[80vh] overflow-y-auto"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="flex justify-between items-center mb-6">
          <div className="flex items-center gap-3">
            <Keyboard className="w-6 h-6 text-blue-600" aria-hidden="true" />
            <h2 id="keyboard-shortcuts-title" className="text-2xl font-bold text-gray-900">
              {t('keyboard.shortcuts')}
            </h2>
          </div>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 p-2 rounded-lg hover:bg-gray-100 transition-colors"
            aria-label="Close keyboard shortcuts help"
          >
            <X className="w-6 h-6" aria-hidden="true" />
          </button>
        </div>

        <div className="space-y-3">
          {shortcuts.map((shortcut, index) => (
            <div
              key={index}
              className="flex justify-between items-center p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
            >
              <span className="text-gray-700">{shortcut.description}</span>
              <kbd className="px-3 py-1 bg-white border border-gray-300 rounded shadow-sm font-mono text-sm text-gray-800">
                {shortcut.key}
              </kbd>
            </div>
          ))}
        </div>

        <div className="mt-6 p-4 bg-blue-50 rounded-lg">
          <p className="text-sm text-blue-800">
            <strong>Tip:</strong> Press <kbd className="px-2 py-1 bg-white border border-blue-300 rounded text-xs">Alt+H</kbd> anytime to view this help.
          </p>
        </div>
      </div>
    </div>
  );
};
