import { Routes, Route, Link } from 'react-router-dom';
import HomePage from './pages/HomePage';
import ProductsPage from './pages/ProductsPage';
import AboutPage from './pages/AboutPage';
import ContactPage from './pages/ContactPage';
import OpenAccountPage from './pages/OpenAccountPage';
import { Building2, Menu, X } from 'lucide-react';
import { useState } from 'react';

export default function PublicWebsite() {
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  return (
    <div className="min-h-screen flex flex-col">
      {/* Header */}
      <header className="bg-white shadow-sm sticky top-0 z-50">
        <nav className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center">
              <Link to="/" className="flex items-center space-x-2">
                <Building2 className="h-8 w-8 text-wekeza-blue" />
                <span className="text-2xl font-bold text-wekeza-blue">Wekeza Bank</span>
              </Link>
            </div>

            {/* Desktop Navigation */}
            <div className="hidden md:flex items-center space-x-8">
              <Link to="/" className="text-gray-700 hover:text-wekeza-blue">Home</Link>
              <Link to="/products" className="text-gray-700 hover:text-wekeza-blue">Products</Link>
              <Link to="/about" className="text-gray-700 hover:text-wekeza-blue">About Us</Link>
              <Link to="/contact" className="text-gray-700 hover:text-wekeza-blue">Contact</Link>
              <Link to="/open-account" className="btn btn-primary">Open Account</Link>
              <Link to="/personal/login" className="btn btn-secondary">Login</Link>
            </div>

            {/* Mobile menu button */}
            <div className="md:hidden flex items-center">
              <button
                onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
                className="text-gray-700"
              >
                {mobileMenuOpen ? <X className="h-6 w-6" /> : <Menu className="h-6 w-6" />}
              </button>
            </div>
          </div>

          {/* Mobile Navigation */}
          {mobileMenuOpen && (
            <div className="md:hidden py-4 space-y-2">
              <Link to="/" className="block py-2 text-gray-700 hover:text-wekeza-blue">Home</Link>
              <Link to="/products" className="block py-2 text-gray-700 hover:text-wekeza-blue">Products</Link>
              <Link to="/about" className="block py-2 text-gray-700 hover:text-wekeza-blue">About Us</Link>
              <Link to="/contact" className="block py-2 text-gray-700 hover:text-wekeza-blue">Contact</Link>
              <Link to="/open-account" className="block py-2 text-wekeza-blue font-medium">Open Account</Link>
              <Link to="/personal/login" className="block py-2 text-wekeza-blue font-medium">Login</Link>
            </div>
          )}
        </nav>
      </header>

      {/* Main Content */}
      <main className="flex-grow">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/products" element={<ProductsPage />} />
          <Route path="/about" element={<AboutPage />} />
          <Route path="/contact" element={<ContactPage />} />
          <Route path="/open-account" element={<OpenAccountPage />} />
        </Routes>
      </main>

      {/* Footer */}
      <footer className="bg-gray-900 text-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
            <div>
              <h3 className="text-lg font-bold mb-4">Wekeza Bank</h3>
              <p className="text-gray-400">Banking made simple for everyone</p>
            </div>
            <div>
              <h4 className="font-semibold mb-4">Products</h4>
              <ul className="space-y-2 text-gray-400">
                <li><Link to="/products#personal">Personal Banking</Link></li>
                <li><Link to="/products#corporate">Corporate Banking</Link></li>
                <li><Link to="/products#sme">SME Banking</Link></li>
                <li><Link to="/products#loans">Loans</Link></li>
              </ul>
            </div>
            <div>
              <h4 className="font-semibold mb-4">Company</h4>
              <ul className="space-y-2 text-gray-400">
                <li><Link to="/about">About Us</Link></li>
                <li><Link to="/contact">Contact</Link></li>
                <li><a href="#">Careers</a></li>
                <li><a href="#">News</a></li>
              </ul>
            </div>
            <div>
              <h4 className="font-semibold mb-4">Legal</h4>
              <ul className="space-y-2 text-gray-400">
                <li><a href="#">Privacy Policy</a></li>
                <li><a href="#">Terms of Service</a></li>
                <li><a href="#">Security</a></li>
              </ul>
            </div>
          </div>
          <div className="border-t border-gray-800 mt-8 pt-8 text-center text-gray-400">
            <p>&copy; 2026 Wekeza Bank. All rights reserved.</p>
          </div>
        </div>
      </footer>
    </div>
  );
}
