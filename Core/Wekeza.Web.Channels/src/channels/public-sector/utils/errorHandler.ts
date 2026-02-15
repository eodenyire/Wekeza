import { ApiResponse } from '../types';

export class ApiError extends Error {
  constructor(
    message: string,
    public code: string,
    public statusCode: number,
    public details?: any
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

export async function handleApiCall<T>(
  apiCall: () => Promise<Response>
): Promise<T> {
  try {
    const response = await apiCall();
    
    // Handle authentication errors
    if (response.status === 401) {
      window.location.href = '/public-sector/login';
      throw new ApiError('Unauthorized', 'UNAUTHORIZED', 401);
    }

    // Handle authorization errors
    if (response.status === 403) {
      throw new ApiError(
        'You do not have permission to perform this action',
        'FORBIDDEN',
        403
      );
    }

    // Handle not found errors
    if (response.status === 404) {
      throw new ApiError('Resource not found', 'NOT_FOUND', 404);
    }

    // Parse response
    const data: ApiResponse<T> = await response.json();

    if (data.success && data.data) {
      return data.data;
    }

    // Handle business logic errors
    throw new ApiError(
      data.error?.message || 'An error occurred',
      data.error?.code || 'UNKNOWN_ERROR',
      response.status,
      data.error?.details
    );
  } catch (error) {
    if (error instanceof ApiError) {
      throw error;
    }

    // Handle network errors
    if (error instanceof TypeError && error.message.includes('fetch')) {
      throw new ApiError(
        'Network error. Please check your connection.',
        'NETWORK_ERROR',
        0
      );
    }

    // Handle unknown errors
    throw new ApiError(
      'An unexpected error occurred',
      'UNKNOWN_ERROR',
      500
    );
  }
}

export function getErrorMessage(error: unknown): string {
  if (error instanceof ApiError) {
    return error.message;
  }
  if (error instanceof Error) {
    return error.message;
  }
  return 'An unexpected error occurred';
}

export function logError(error: unknown, context?: string): void {
  if (import.meta.env.DEV) {
    console.error(`[${context || 'Error'}]:`, error);
  }
  
  // In production, send to error tracking service
  // e.g., Sentry, LogRocket, etc.
}
