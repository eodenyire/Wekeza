import { AlertTriangle, CheckCircle, Info } from 'lucide-react';
import { ReactNode } from 'react';

interface ConfirmDialogProps {
  isOpen: boolean;
  title: string;
  message: string;
  type?: 'warning' | 'danger' | 'info' | 'success';
  confirmText?: string;
  cancelText?: string;
  onConfirm: () => void;
  onCancel: () => void;
  loading?: boolean;
  children?: ReactNode;
  confirmColor?: string; // For backward compatibility
}

export default function ConfirmDialog({
  isOpen,
  title,
  message,
  type = 'warning',
  confirmText = 'Confirm',
  cancelText = 'Cancel',
  onConfirm,
  onCancel,
  loading = false,
  children,
  confirmColor
}: ConfirmDialogProps) {
  // Map confirmColor to type for backward compatibility
  const effectiveType = confirmColor === 'green' ? 'success' :
                        confirmColor === 'red' ? 'danger' :
                        confirmColor === 'blue' ? 'info' :
                        type;
  if (!isOpen) return null;

  const getIcon = () => {
    switch (effectiveType) {
      case 'danger':
        return <AlertTriangle className="w-12 h-12 text-red-600" />;
      case 'warning':
        return <AlertTriangle className="w-12 h-12 text-yellow-600" />;
      case 'success':
        return <CheckCircle className="w-12 h-12 text-green-600" />;
      case 'info':
        return <Info className="w-12 h-12 text-blue-600" />;
    }
  };

  const getIconBg = () => {
    switch (effectiveType) {
      case 'danger':
        return 'bg-red-100';
      case 'warning':
        return 'bg-yellow-100';
      case 'success':
        return 'bg-green-100';
      case 'info':
        return 'bg-blue-100';
    }
  };

  const getConfirmButtonStyle = () => {
    switch (effectiveType) {
      case 'danger':
        return 'bg-red-600 hover:bg-red-700';
      case 'warning':
        return 'bg-yellow-600 hover:bg-yellow-700';
      case 'success':
        return 'bg-green-600 hover:bg-green-700';
      case 'info':
        return 'bg-blue-600 hover:bg-blue-700';
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-lg shadow-xl max-w-md w-full p-6 animate-fade-in">
        <div className={`w-16 h-16 ${getIconBg()} rounded-full flex items-center justify-center mx-auto mb-4`}>
          {getIcon()}
        </div>

        <h2 className="text-xl font-bold text-gray-900 text-center mb-2">
          {title}
        </h2>

        <p className="text-gray-600 text-center mb-6">
          {message}
        </p>

        {children && (
          <div className="mb-6">
            {children}
          </div>
        )}

        <div className="flex gap-3">
          <button
            onClick={onCancel}
            disabled={loading}
            className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {cancelText}
          </button>
          <button
            onClick={onConfirm}
            disabled={loading}
            className={`flex-1 px-4 py-2 text-white rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed ${getConfirmButtonStyle()}`}
          >
            {loading ? 'Processing...' : confirmText}
          </button>
        </div>
      </div>
    </div>
  );
}

// Hook for using confirm dialog
export function useConfirmDialog() {
  const [dialogState, setDialogState] = React.useState<{
    isOpen: boolean;
    title: string;
    message: string;
    type: 'warning' | 'danger' | 'info' | 'success';
    onConfirm: () => void;
  }>({
    isOpen: false,
    title: '',
    message: '',
    type: 'warning',
    onConfirm: () => {}
  });

  const showConfirm = (
    title: string,
    message: string,
    onConfirm: () => void,
    type: 'warning' | 'danger' | 'info' | 'success' = 'warning'
  ) => {
    setDialogState({
      isOpen: true,
      title,
      message,
      type,
      onConfirm
    });
  };

  const hideConfirm = () => {
    setDialogState(prev => ({ ...prev, isOpen: false }));
  };

  const handleConfirm = () => {
    dialogState.onConfirm();
    hideConfirm();
  };

  return {
    dialogState,
    showConfirm,
    hideConfirm,
    handleConfirm
  };
}

// Import React for the hook
import React from 'react';
