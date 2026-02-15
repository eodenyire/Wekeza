import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useAuth } from '@/contexts/AuthContext';
import { Building2, Lock, User, AlertCircle } from 'lucide-react';
import { AxiosError } from 'axios';

// Validation schema
const loginSchema = z.object({
  username: z
    .string()
    .min(1, 'Username is required')
    .min(3, 'Username must be at least 3 characters')
    .max(50, 'Username must not exceed 50 characters')
    .regex(/^[a-zA-Z0-9._-]+$/, 'Username can only contain letters, numbers, dots, underscores, and hyphens'),
  password: z
    .string()
    .min(1, 'Password is required')
    .min(6, 'Password must be at least 6 characters')
    .max(100, 'Password must not exceed 100 characters'),
  rememberMe: z.boolean().optional(),
});

type LoginFormData = z.infer<typeof loginSchema>;

export default function Login() {
  const [error, setError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      username: '',
      password: '',
      rememberMe: false,
    },
  });

  const onSubmit = async (data: LoginFormData) => {
    setError('');

    try {
      await login(data.username, data.password);
      navigate('/public-sector/dashboard');
    } catch (err) {
      // Handle different error types
      if (err instanceof AxiosError) {
        if (err.response?.status === 401) {
          setError('Invalid username or password. Please try again.');
        } else if (err.response?.status === 403) {
          setError('Access denied. You do not have permission to access the Public Sector Portal.');
        } else if (err.response?.status === 429) {
          setError('Too many login attempts. Please try again later.');
        } else if (err.code === 'ECONNABORTED' || err.code === 'ERR_NETWORK') {
          setError('Unable to connect to the server. Please check your internet connection.');
        } else {
          setError(err.response?.data?.message || 'An error occurred during login. Please try again.');
        }
      } else {
        setError('An unexpected error occurred. Please try again.');
      }
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-green-700 to-green-900 flex items-center justify-center px-4">
      <div className="max-w-md w-full">
        {/* Logo */}
        <div className="text-center mb-8">
          <Link to="/" className="inline-flex items-center space-x-2 text-white">
            <Building2 className="h-12 w-12" />
            <span className="text-3xl font-bold">Wekeza Bank</span>
          </Link>
          <p className="text-green-100 mt-2">Public Sector Portal</p>
        </div>

        {/* Login Card */}
        <div className="bg-white rounded-lg shadow-xl p-8">
          <h2 className="text-2xl font-bold text-center mb-6">Government Services Login</h2>

          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4 flex items-start">
              <AlertCircle className="h-5 w-5 mr-2 flex-shrink-0 mt-0.5" />
              <span className="text-sm">{error}</span>
            </div>
          )}

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div>
              <label htmlFor="username" className="block text-sm font-medium mb-2">
                Username
              </label>
              <div className="relative">
                <User className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
                <input
                  id="username"
                  type="text"
                  {...register('username')}
                  className={`input pl-10 ${errors.username ? 'border-red-500 focus:ring-red-500' : ''}`}
                  placeholder="Enter your username"
                  autoComplete="username"
                  aria-invalid={errors.username ? 'true' : 'false'}
                  aria-describedby={errors.username ? 'username-error' : undefined}
                />
              </div>
              {errors.username && (
                <p id="username-error" className="mt-1 text-sm text-red-600">
                  {errors.username.message}
                </p>
              )}
            </div>

            <div>
              <label htmlFor="password" className="block text-sm font-medium mb-2">
                Password
              </label>
              <div className="relative">
                <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
                <input
                  id="password"
                  type="password"
                  {...register('password')}
                  className={`input pl-10 ${errors.password ? 'border-red-500 focus:ring-red-500' : ''}`}
                  placeholder="Enter your password"
                  autoComplete="current-password"
                  aria-invalid={errors.password ? 'true' : 'false'}
                  aria-describedby={errors.password ? 'password-error' : undefined}
                />
              </div>
              {errors.password && (
                <p id="password-error" className="mt-1 text-sm text-red-600">
                  {errors.password.message}
                </p>
              )}
            </div>

            <div className="flex items-center justify-between text-sm">
              <label htmlFor="rememberMe" className="flex items-center cursor-pointer">
                <input
                  id="rememberMe"
                  type="checkbox"
                  {...register('rememberMe')}
                  className="mr-2 rounded border-gray-300 text-green-700 focus:ring-green-700"
                />
                <span>Remember me</span>
              </label>
              <a href="#" className="text-green-700 hover:underline">
                Forgot password?
              </a>
            </div>

            <button
              type="submit"
              disabled={isSubmitting}
              className="btn btn-primary w-full bg-green-700 hover:bg-green-800 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isSubmitting ? 'Logging in...' : 'Login'}
            </button>
          </form>

          <div className="mt-6 text-center text-sm text-gray-600">
            Need assistance?{' '}
            <Link to="/contact" className="text-green-700 hover:underline font-medium">
              Contact Support
            </Link>
          </div>
        </div>

        {/* Demo Credentials */}
        <div className="mt-4 bg-green-900 bg-opacity-50 text-white rounded-lg p-4 text-sm">
          <p className="font-semibold mb-2">Demo Credentials:</p>
          <p>Username: <span className="font-mono">admin</span></p>
          <p>Password: <span className="font-mono">any password</span></p>
        </div>
      </div>
    </div>
  );
}
