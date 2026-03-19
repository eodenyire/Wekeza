import { useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import AppLayout from '../components/AppLayout'

export default function ProfilePage() {
  const navigate = useNavigate()
  const { user, logout } = useAuth()

  const handleLogout = () => {
    logout()
    navigate('/login', { replace: true })
  }

  const menuItems = [
    { icon: '🔒', label: 'Change Password', onClick: () => {} },
    { icon: '🔔', label: 'Notifications', onClick: () => {} },
    { icon: '📞', label: 'Contact Support', onClick: () => {} },
    { icon: '📋', label: 'Terms & Conditions', onClick: () => {} },
    { icon: '🛡️', label: 'Privacy Policy', onClick: () => {} },
    { icon: 'ℹ️', label: 'About Wekeza', onClick: () => {} },
  ]

  return (
    <AppLayout title="Profile">
      <div className="p-5">
        {/* User info */}
        <div className="card flex items-center gap-4 mb-6">
          <div className="w-16 h-16 bg-primary-100 rounded-2xl flex items-center justify-center text-2xl">
            {user?.firstName?.[0] ?? user?.username?.[0] ?? '👤'}
          </div>
          <div>
            <h2 className="font-bold text-gray-900 text-lg">
              {user?.firstName && user?.lastName
                ? `${user.firstName} ${user.lastName}`
                : user?.username}
            </h2>
            <p className="text-gray-400 text-sm">{user?.email}</p>
            {user?.roles?.[0] && (
              <span className="inline-block mt-1 text-xs bg-primary-100 text-primary-700 px-2.5 py-0.5 rounded-full font-medium">
                {user.roles[0]}
              </span>
            )}
          </div>
        </div>

        {/* Menu items */}
        <div className="card divide-y divide-gray-100">
          {menuItems.map((item, index) => (
            <button
              key={index}
              onClick={item.onClick}
              className="w-full flex items-center gap-4 py-3.5 text-left active:bg-gray-50 transition-colors first:rounded-t-2xl last:rounded-b-2xl"
            >
              <span className="text-xl w-7 text-center">{item.icon}</span>
              <span className="text-gray-700 font-medium text-sm flex-1">{item.label}</span>
              <svg className="w-4 h-4 text-gray-300" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
              </svg>
            </button>
          ))}
        </div>

        {/* Logout */}
        <button
          onClick={handleLogout}
          className="w-full mt-4 py-4 px-6 bg-red-50 text-red-600 font-semibold text-base rounded-xl active:bg-red-100 transition-colors flex items-center justify-center gap-2"
        >
          <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
              d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
          </svg>
          Sign Out
        </button>

        <p className="text-center text-xs text-gray-400 mt-6">
          Wekeza Mobile Web v1.0.0
        </p>
      </div>
    </AppLayout>
  )
}
