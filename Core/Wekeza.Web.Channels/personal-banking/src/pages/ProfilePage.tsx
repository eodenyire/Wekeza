export default function ProfilePage() {
  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6">Profile Settings</h1>
      
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 space-y-6">
          <div className="bg-white p-6 rounded-lg shadow">
            <h2 className="text-lg font-semibold mb-4">Personal Information</h2>
            <form className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-1">First Name</label>
                  <input type="text" className="w-full px-3 py-2 border rounded" defaultValue="John" />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Last Name</label>
                  <input type="text" className="w-full px-3 py-2 border rounded" defaultValue="Doe" />
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Email</label>
                <input type="email" className="w-full px-3 py-2 border rounded" defaultValue="john.doe@example.com" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Phone Number</label>
                <input type="tel" className="w-full px-3 py-2 border rounded" defaultValue="+254712345678" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Address</label>
                <textarea className="w-full px-3 py-2 border rounded" rows={3} defaultValue="123 Main Street, Nairobi"></textarea>
              </div>
              <button className="bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700">
                Update Profile
              </button>
            </form>
          </div>

          <div className="bg-white p-6 rounded-lg shadow">
            <h2 className="text-lg font-semibold mb-4">Change Password</h2>
            <form className="space-y-4">
              <div>
                <label className="block text-sm font-medium mb-1">Current Password</label>
                <input type="password" className="w-full px-3 py-2 border rounded" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">New Password</label>
                <input type="password" className="w-full px-3 py-2 border rounded" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Confirm New Password</label>
                <input type="password" className="w-full px-3 py-2 border rounded" />
              </div>
              <button className="bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700">
                Change Password
              </button>
            </form>
          </div>
        </div>

        <div className="space-y-6">
          <div className="bg-white p-6 rounded-lg shadow">
            <h2 className="text-lg font-semibold mb-4">Account Info</h2>
            <div className="space-y-3 text-sm">
              <div>
                <p className="text-gray-600">Customer ID</p>
                <p className="font-medium">CIF-2024-001</p>
              </div>
              <div>
                <p className="text-gray-600">Account Since</p>
                <p className="font-medium">January 2024</p>
              </div>
              <div>
                <p className="text-gray-600">Account Status</p>
                <span className="px-2 py-1 bg-green-100 text-green-800 text-xs rounded">Active</span>
              </div>
            </div>
          </div>

          <div className="bg-white p-6 rounded-lg shadow">
            <h2 className="text-lg font-semibold mb-4">Security Settings</h2>
            <div className="space-y-3">
              <div className="flex items-center justify-between">
                <span className="text-sm">Two-Factor Auth</span>
                <button className="text-blue-600 text-sm hover:underline">Enable</button>
              </div>
              <div className="flex items-center justify-between">
                <span className="text-sm">SMS Alerts</span>
                <button className="text-blue-600 text-sm hover:underline">Manage</button>
              </div>
              <div className="flex items-center justify-between">
                <span className="text-sm">Email Alerts</span>
                <button className="text-blue-600 text-sm hover:underline">Manage</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
