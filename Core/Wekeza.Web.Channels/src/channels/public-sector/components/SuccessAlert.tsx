import { CheckCircle, X } from 'lucide-react';

interface SuccessAlertProps {
  title?: string;
  message: string;
  onDismiss?: () => void;
}

export default function SuccessAlert({ title = 'Success', message, onDismiss }: SuccessAlertProps) {
  return (
    <div className="bg-green-50 border border-green-200 rounded-lg p-4 flex items-start gap-3">
      <CheckCircle className="w-5 h-5 text-green-600 flex-shrink-0 mt-0.5" />
      <div className="flex-1">
        <h3 className="font-medium text-green-900">{title}</h3>
        <p className="text-green-700 text-sm mt-1">{message}</p>
      </div>
      {onDismiss && (
        <button
          onClick={onDismiss}
          className="text-green-400 hover:text-green-600 transition-colors"
          aria-label="Dismiss"
        >
          <X className="w-5 h-5" />
        </button>
      )}
    </div>
  );
}
