import BottomNav from './BottomNav'

interface AppLayoutProps {
  children: React.ReactNode
  title?: string
  showBack?: boolean
  onBack?: () => void
}

export default function AppLayout({ children, title, showBack, onBack }: AppLayoutProps) {
  return (
    <div className="flex flex-col h-screen bg-gray-50 max-w-md mx-auto relative">
      {/* Status bar spacer */}
      <div className="bg-primary-800 pt-safe-top" />

      {/* Header */}
      {title && (
        <header className="flex-none bg-primary-800 px-4 pb-4 flex items-center gap-3">
          {showBack && (
            <button
              onClick={onBack}
              className="w-9 h-9 flex items-center justify-center rounded-full bg-white/10 text-white"
            >
              <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
              </svg>
            </button>
          )}
          <h1 className="text-white font-semibold text-lg">{title}</h1>
        </header>
      )}

      {/* Scrollable content */}
      <main className="flex-1 overflow-y-auto scroll-smooth-ios pb-20">
        {children}
      </main>

      {/* Bottom navigation */}
      <BottomNav />
    </div>
  )
}
