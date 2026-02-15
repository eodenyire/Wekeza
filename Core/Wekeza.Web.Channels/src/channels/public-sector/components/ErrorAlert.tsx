import { AlertCircle, X } from 'lucide-react';

interface ErrorAlertProps {
  title?: string;
  message: string;
  onDismiss?: () => void;
  onClose?: () => void;
  onRetry?: () => void;
}

export default function ErrorAlert({ title = 'Error', message, onDismiss, onClose, onRetry }: ErrorAlertProps) {
  const handleDismiss = onDismiss || onClose;
  
  return (
    <div className="bg-red-50 border border-red-200 rounded-lg p-4 flex items-start gap-3">
      <AlertCircle className="w-5 h-5 text-red-600 flex-shrink-0 mt-0.5" />
      <div className="flex-1">
        <h3 className="font-medium text-red-900">{title}</h3>
        <p className="text-red-700 text-sm mt-1">{message}</p>
        {onRetry && (
          <button
            onClick={onRetry}
            className="mt-2 text-sm text-red-600 hover:text-red-800 underline"
          >
            Retry
          </button>
        )}
      </div>
      {handleDismiss && (
        <button
          onClick={handleDismiss}
          className="text-red-400 hover:text-red-600 transition-colors"
          aria-label="Dismiss"
        >
          <X className="w-5 h-5" />
        </button>
      )}
    </div>
  );
}
