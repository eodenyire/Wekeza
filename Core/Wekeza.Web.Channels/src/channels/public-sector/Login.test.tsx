import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { BrowserRouter } from 'react-router-dom';
import Login from './Login';
import { AuthProvider } from '@/contexts/AuthContext';
import { AxiosError } from 'axios';

// Mock the API client
vi.mock('@/lib/api-client', () => ({
  apiClient: {
    login: vi.fn(),
  },
}));

// Mock useNavigate
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

const renderLogin = () => {
  return render(
    <BrowserRouter>
      <AuthProvider>
        <Login />
      </AuthProvider>
    </BrowserRouter>
  );
};

describe('Login Component', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    localStorage.clear();
  });

  describe('Rendering', () => {
    it('should render login form with all fields', () => {
      renderLogin();

      expect(screen.getByText('Government Services Login')).toBeInTheDocument();
      expect(screen.getByLabelText(/username/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/remember me/i)).toBeInTheDocument();
      expect(screen.getByRole('button', { name: /login/i })).toBeInTheDocument();
    });

    it('should render Wekeza Bank logo and title', () => {
      renderLogin();

      expect(screen.getByText('Wekeza Bank')).toBeInTheDocument();
      expect(screen.getByText('Public Sector Portal')).toBeInTheDocument();
    });

    it('should render demo credentials section', () => {
      renderLogin();

      expect(screen.getByText('Demo Credentials:')).toBeInTheDocument();
      expect(screen.getByText('admin')).toBeInTheDocument();
    });
  });

  describe('Form Validation', () => {
    it('should show validation error when username is empty', async () => {
      renderLogin();

      const submitButton = screen.getByRole('button', { name: /login/i });
      fireEvent.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText('Username is required')).toBeInTheDocument();
      });
    });

    it('should show validation error when username is too short', async () => {
      renderLogin();

      const usernameInput = screen.getByLabelText(/username/i);
      const passwordInput = screen.getByLabelText(/password/i);
      const submitButton = screen.getByRole('button', { name: /login/i });

      fireEvent.change(usernameInput, { target: { value: 'ab' } });
      fireEvent.change(passwordInput, { target: { value: 'password123' } });
      fireEvent.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText('Username must be at least 3 characters')).toBeInTheDocument();
      });
    });

    it('should show validation error when username contains invalid characters', async () => {
      renderLogin();

      const usernameInput = screen.getByLabelText(/username/i);
      const passwordInput = screen.getByLabelText(/password/i);
      const submitButton = screen.getByRole('button', { name: /login/i });

      fireEvent.change(usernameInput, { target: { value: 'user@name!' } });
      fireEvent.change(passwordInput, { target: { value: 'password123' } });
      fireEvent.click(submitButton);

      await waitFor(() => {
        expect(
          screen.getByText(/username can only contain letters, numbers/i)
        ).toBeInTheDocument();
      });
    });

    it('should show validation error when password is empty', async () => {
      renderLogin();

      const submitButton = screen.getByRole('button', { name: /login/i });
      fireEvent.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText('Password is required')).toBeInTheDocument();
      });
    });

    it('should show validation error when password is too short', async () => {
      renderLogin();

      const usernameInput = screen.getByLabelText(/username/i);
      const passwordInput = screen.getByLabelText(/password/i);
      const submitButton = screen.getByRole('button', { name: /login/i });

      fireEvent.change(usernameInput, { target: { value: 'validuser' } });
      fireEvent.change(passwordInput, { target: { value: '12345' } });
      fireEvent.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText('Password must be at least 6 characters')).toBeInTheDocument();
      });
    });

    it('should accept valid username and password', async () => {
      renderLogin();

      const usernameInput = screen.getByLabelText(/username/i);
      const passwordInput = screen.getByLabelText(/password/i);

      fireEvent.change(usernameInput, { target: { value: 'validuser' } });
      fireEvent.change(passwordInput, { target: { value: 'validpassword123' } });

      // No validation errors should be shown for error messages
      expect(screen.queryByText('Username is required')).not.toBeInTheDocument();
      expect(screen.queryByText('Password is required')).not.toBeInTheDocument();
      expect(screen.queryByText('Username must be at least 3 characters')).not.toBeInTheDocument();
      expect(screen.queryByText('Password must be at least 6 characters')).not.toBeInTheDocument();
    });
  });

  describe('Form Submission', () => {
    it('should disable submit button while submitting', async () => {
      const { apiClient } = await import('@/lib/api-client');
      
      // Mock a delayed login
      vi.mocked(apiClient.login).mockImplementation(
        () => new Promise((resolve) => setTimeout(resolve, 100))
      );

      renderLogin();

      const usernameInput = screen.getByLabelText(/username/i);
      const passwordInput = screen.getByLabelText(/password/i);
      const submitButton = screen.getByRole('button', { name: /login/i });

      fireEvent.change(usernameInput, { target: { value: 'testuser' } });
      fireEvent.change(passwordInput, { target: { value: 'password123' } });
      fireEvent.click(submitButton);

      // Button should be disabled and show loading text
      await waitFor(() => {
        expect(submitButton).toBeDisabled();
        expect(screen.getByText('Logging in...')).toBeInTheDocument();
      });
    });

    it('should call login API with correct credentials', async () => {
      const { apiClient } = await import('@/lib/api-client');
      
      vi.mocked(apiClient.login).mockResolvedValue({
        token: 'test-token',
        userId: '123',
        username: 'testuser',
        email: 'test@example.com',
        roles: ['TREASURY_OFFICER'],
      });

      renderLogin();

      const usernameInput = screen.getByLabelText(/username/i);
      const passwordInput = screen.getByLabelText(/password/i);
      const submitButton = screen.getByRole('button', { name: /login/i });

      fireEvent.change(usernameInput, { target: { value: 'testuser' } });
      fireEvent.change(passwordInput, { target: { value: 'password123' } });
      fireEvent.click(submitButton);

      await waitFor(() => {
        expect(apiClient.login).toHaveBeenCalledWith('testuser', 'password123');
      });
    });

    it('should navigate to dashboard on successful login', async () => {
      const { apiClient } = await import('@/lib/api-client');
      
      vi.mocked(apiClient.login).mockResolvedValue({
        token: 'test-token',
        userId: '123',
        username: 'testuser',
        email: 'test@example.com',
        roles: ['TREASURY_OFFICER'],
      });

      renderLogin();

      const usernameInput = screen.getByLabelText(/username/i);
      const passwordInput = screen.getByLabelText(/password/i);
      const submitButton = screen.getByRole('button', { name: /login/i });

      fireEvent.change(usernameInput, { target: { value: 'testuser' } });
      fireEvent.change(passwordInput, { target: { value: 'password123' } });
      fireEvent.click(submitButton);

      await waitFor(() => {
        expect(mockNavigate).toHaveBeenCalledWith('/public-sector/dashboard');
      });
    });
  });

  describe('Error Handling', () => {
    it('should display error message for 401 unauthorized', async () => {
      const { apiClient } = await import('@/lib/api-client');
      
      const error = new AxiosError('Unauthorized');
      error.response = { status: 401 } as any;
      vi.mocked(apiClient.login).mockRejectedValue(error);

      renderLogin();

      const usernameInput = screen.getByLabelText(/username/i);
      const passwordInput = screen.getByLabelText(/password/i);
      const submitButton = screen.getByRole('button', { name: /login/i });

      fireEvent.change(usernameInput, { target: { value: 'wronguser' } });
      fireEvent.change(passwordInput, { target: { value: 'wrongpass' } });
      fireEvent.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/invalid username or password/i)).toBeInTheDocument();
      });
    });

    it('should display error message for 403 forbidden', async () => {
      const { apiClient } = await import('@/lib/api-client');
      
      const error = new AxiosError('Forbidden');
      error.response = { status: 403 } as any;
      vi.mocked(apiClient.login).mockRejectedValue(error);

      renderLogin();

      const usernameInput = screen.getByLabelText(/username/i);
      const passwordInput = screen.getByLabelText(/password/i);
      const submitButton = screen.getByRole('button', { name: /login/i });

      fireEvent.change(usernameInput, { target: { value: 'testuser' } });
      fireEvent.change(passwordInput, { target: { value: 'password123' } });
      fireEvent.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/access denied/i)).toBeInTheDocument();
      });
    });

    it('should display error message for network errors', async () => {
      const { apiClient } = await import('@/lib/api-client');
      
      const error = new AxiosError('Network Error');
      error.code = 'ERR_NETWORK';
      vi.mocked(apiClient.login).mockRejectedValue(error);

      renderLogin();

      const usernameInput = screen.getByLabelText(/username/i);
      const passwordInput = screen.getByLabelText(/password/i);
      const submitButton = screen.getByRole('button', { name: /login/i });

      fireEvent.change(usernameInput, { target: { value: 'testuser' } });
      fireEvent.change(passwordInput, { target: { value: 'password123' } });
      fireEvent.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/unable to connect to the server/i)).toBeInTheDocument();
      });
    });

    it('should display error message for rate limiting (429)', async () => {
      const { apiClient } = await import('@/lib/api-client');
      
      const error = new AxiosError('Too Many Requests');
      error.response = { status: 429 } as any;
      vi.mocked(apiClient.login).mockRejectedValue(error);

      renderLogin();

      const usernameInput = screen.getByLabelText(/username/i);
      const passwordInput = screen.getByLabelText(/password/i);
      const submitButton = screen.getByRole('button', { name: /login/i });

      fireEvent.change(usernameInput, { target: { value: 'testuser' } });
      fireEvent.change(passwordInput, { target: { value: 'password123' } });
      fireEvent.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/too many login attempts/i)).toBeInTheDocument();
      });
    });

    it('should clear previous error when submitting again', async () => {
      const { apiClient } = await import('@/lib/api-client');
      
      // First attempt fails
      const error = new AxiosError('Unauthorized');
      error.response = { status: 401 } as any;
      vi.mocked(apiClient.login).mockRejectedValueOnce(error);

      renderLogin();

      const usernameInput = screen.getByLabelText(/username/i);
      const passwordInput = screen.getByLabelText(/password/i);
      const submitButton = screen.getByRole('button', { name: /login/i });

      // First attempt
      fireEvent.change(usernameInput, { target: { value: 'wronguser' } });
      fireEvent.change(passwordInput, { target: { value: 'wrongpass' } });
      fireEvent.click(submitButton);

      await waitFor(() => {
        expect(screen.getByText(/invalid username or password/i)).toBeInTheDocument();
      });

      // Second attempt succeeds
      vi.mocked(apiClient.login).mockResolvedValue({
        token: 'test-token',
        userId: '123',
        username: 'testuser',
        email: 'test@example.com',
        roles: ['TREASURY_OFFICER'],
      });

      fireEvent.change(usernameInput, { target: { value: 'correctuser' } });
      fireEvent.change(passwordInput, { target: { value: 'correctpass' } });
      fireEvent.click(submitButton);

      await waitFor(() => {
        expect(screen.queryByText(/invalid username or password/i)).not.toBeInTheDocument();
      });
    });
  });

  describe('Accessibility', () => {
    it('should have proper ARIA labels', () => {
      renderLogin();

      const usernameInput = screen.getByLabelText(/username/i);
      const passwordInput = screen.getByLabelText(/password/i);

      expect(usernameInput).toHaveAttribute('id', 'username');
      expect(passwordInput).toHaveAttribute('id', 'password');
    });

    it('should have proper autocomplete attributes', () => {
      renderLogin();

      const usernameInput = screen.getByLabelText(/username/i);
      const passwordInput = screen.getByLabelText(/password/i);

      expect(usernameInput).toHaveAttribute('autocomplete', 'username');
      expect(passwordInput).toHaveAttribute('autocomplete', 'current-password');
    });

    it('should mark invalid fields with aria-invalid', async () => {
      const user = userEvent.setup();
      renderLogin();

      const submitButton = screen.getByRole('button', { name: /login/i });
      await user.click(submitButton);

      await waitFor(() => {
        const usernameInput = screen.getByLabelText(/username/i);
        const passwordInput = screen.getByLabelText(/password/i);

        expect(usernameInput).toHaveAttribute('aria-invalid', 'true');
        expect(passwordInput).toHaveAttribute('aria-invalid', 'true');
      });
    });

    it('should associate error messages with inputs using aria-describedby', async () => {
      const user = userEvent.setup();
      renderLogin();

      const submitButton = screen.getByRole('button', { name: /login/i });
      await user.click(submitButton);

      await waitFor(() => {
        const usernameInput = screen.getByLabelText(/username/i);
        const passwordInput = screen.getByLabelText(/password/i);

        expect(usernameInput).toHaveAttribute('aria-describedby', 'username-error');
        expect(passwordInput).toHaveAttribute('aria-describedby', 'password-error');
      });
    });
  });
});
