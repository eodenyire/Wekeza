import { LucideIcon, FileX } from 'lucide-react';

interface EmptyStateProps {
  icon?: LucideIcon;
  title?: string;
  description?: string;
  message?: string;
  actionLabel?: string;
  onAction?: () => void;
}

export default function EmptyState({
  icon: Icon = FileX,
  title = 'No data',
  description,
  message,
  actionLabel,
  onAction
}: EmptyStateProps) {
  const displayDescription = description || message || 'No items to display';
  
  return (
    <div className="text-center py-12">
      <div className="flex justify-center mb-4">
        <Icon className="w-16 h-16 text-gray-300" />
      </div>
      <h3 className="text-lg font-medium text-gray-900 mb-2">{title}</h3>
      <p className="text-gray-500 mb-6">{displayDescription}</p>
      {actionLabel && onAction && (
        <button
          onClick={onAction}
          className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium"
        >
          {actionLabel}
        </button>
      )}
    </div>
  );
}
