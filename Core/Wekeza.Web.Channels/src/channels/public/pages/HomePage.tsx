import { Link } from 'react-router-dom';
import { ArrowRight, Shield, Zap, Users, TrendingUp, CreditCard, Building2 } from 'lucide-react';

export default function HomePage() {
  return (
    <div>
      {/* Hero Section */}
      <section className="bg-gradient-to-r from-wekeza-blue to-blue-600 text-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-24">
          <div className="text-center">
            <h1 className="text-5xl font-bold mb-6">Banking Made Simple</h1>
            <p className="text-xl mb-8 text-blue-100">
              Experience modern banking with Wekeza Bank - Your trusted financial partner
            </p>
            <div className="flex justify-center gap-4">
              <Link to="/open-account" className="bg-white text-wekeza-blue px-8 py-3 rounded-lg font-semibold hover:bg-gray-100 transition">
                Open Account
              </Link>
              <Link to="/personal/login" className="bg-transparent border-2 border-white text-white px-8 py-3 rounded-lg font-semibold hover:bg-white hover:text-wekeza-blue transition">
                Login
              </Link>
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-20 bg-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-16">
            <h2 className="text-4xl font-bold mb-4">Why Choose Wekeza Bank?</h2>
            <p className="text-xl text-gray-600">Modern banking solutions for everyone</p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <div className="text-center p-6">
              <div className="bg-blue-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
                <Shield className="h-8 w-8 text-wekeza-blue" />
              </div>
              <h3 className="text-xl font-semibold mb-2">Secure & Safe</h3>
              <p className="text-gray-600">Bank-grade security with 256-bit encryption and fraud protection</p>
            </div>

            <div className="text-center p-6">
              <div className="bg-green-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
                <Zap className="h-8 w-8 text-wekeza-green" />
              </div>
              <h3 className="text-xl font-semibold mb-2">Fast & Easy</h3>
              <p className="text-gray-600">Open an account in minutes and start banking instantly</p>
            </div>

            <div className="text-center p-6">
              <div className="bg-yellow-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
                <Users className="h-8 w-8 text-wekeza-gold" />
              </div>
              <h3 className="text-xl font-semibold mb-2">24/7 Support</h3>
              <p className="text-gray-600">Round-the-clock customer support whenever you need us</p>
            </div>
          </div>
        </div>
      </section>

      {/* Banking Solutions */}
      <section className="py-20 bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-16">
            <h2 className="text-4xl font-bold mb-4">Banking Solutions for Everyone</h2>
            <p className="text-xl text-gray-600">Choose the right banking solution for your needs</p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            {/* Personal Banking */}
            <div className="bg-white rounded-lg shadow-lg p-8 hover:shadow-xl transition">
              <Users className="h-12 w-12 text-wekeza-blue mb-4" />
              <h3 className="text-2xl font-bold mb-4">Personal Banking</h3>
              <p className="text-gray-600 mb-6">
                Savings accounts, current accounts, loans, and cards for individuals
              </p>
              <ul className="space-y-2 mb-6">
                <li className="flex items-center text-gray-700">
                  <ArrowRight className="h-4 w-4 mr-2 text-wekeza-green" />
                  Savings & Current Accounts
                </li>
                <li className="flex items-center text-gray-700">
                  <ArrowRight className="h-4 w-4 mr-2 text-wekeza-green" />
                  Personal Loans
                </li>
                <li className="flex items-center text-gray-700">
                  <ArrowRight className="h-4 w-4 mr-2 text-wekeza-green" />
                  Debit & Credit Cards
                </li>
              </ul>
              <Link to="/personal/login" className="btn btn-primary w-full">
                Get Started
              </Link>
            </div>

            {/* Corporate Banking */}
            <div className="bg-white rounded-lg shadow-lg p-8 hover:shadow-xl transition">
              <Building2 className="h-12 w-12 text-wekeza-blue mb-4" />
              <h3 className="text-2xl font-bold mb-4">Corporate Banking</h3>
              <p className="text-gray-600 mb-6">
                Comprehensive banking solutions for large enterprises
              </p>
              <ul className="space-y-2 mb-6">
                <li className="flex items-center text-gray-700">
                  <ArrowRight className="h-4 w-4 mr-2 text-wekeza-green" />
                  Bulk Payments
                </li>
                <li className="flex items-center text-gray-700">
                  <ArrowRight className="h-4 w-4 mr-2 text-wekeza-green" />
                  Trade Finance
                </li>
                <li className="flex items-center text-gray-700">
                  <ArrowRight className="h-4 w-4 mr-2 text-wekeza-green" />
                  Treasury Services
                </li>
              </ul>
              <Link to="/corporate/login" className="btn btn-primary w-full">
                Learn More
              </Link>
            </div>

            {/* SME Banking */}
            <div className="bg-white rounded-lg shadow-lg p-8 hover:shadow-xl transition">
              <TrendingUp className="h-12 w-12 text-wekeza-blue mb-4" />
              <h3 className="text-2xl font-bold mb-4">SME Banking</h3>
              <p className="text-gray-600 mb-6">
                Tailored solutions for small and medium enterprises
              </p>
              <ul className="space-y-2 mb-6">
                <li className="flex items-center text-gray-700">
                  <ArrowRight className="h-4 w-4 mr-2 text-wekeza-green" />
                  Business Accounts
                </li>
                <li className="flex items-center text-gray-700">
                  <ArrowRight className="h-4 w-4 mr-2 text-wekeza-green" />
                  Working Capital Loans
                </li>
                <li className="flex items-center text-gray-700">
                  <ArrowRight className="h-4 w-4 mr-2 text-wekeza-green" />
                  Payroll Services
                </li>
              </ul>
              <Link to="/sme/login" className="btn btn-primary w-full">
                Explore
              </Link>
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="bg-wekeza-blue text-white py-20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h2 className="text-4xl font-bold mb-4">Ready to Get Started?</h2>
          <p className="text-xl mb-8 text-blue-100">
            Open your account today and experience modern banking
          </p>
          <Link to="/open-account" className="bg-white text-wekeza-blue px-8 py-3 rounded-lg font-semibold hover:bg-gray-100 transition inline-flex items-center">
            Open Account Now
            <ArrowRight className="ml-2 h-5 w-5" />
          </Link>
        </div>
      </section>
    </div>
  );
}
