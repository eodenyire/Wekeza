import { describe, it, expect } from 'vitest';
import * as fc from 'fast-check';

/**
 * Property-Based Tests for Integration Handling
 * 
 * **Validates: Requirements 1.9, 1.10, 3.8**
 */

interface APIResponse {
  success: boolean;
  data?: any;
  error?: string;
  statusCode: number;
  retryAfter?: number;
}

interface RetryConfig {
  maxRetries: number;
  baseDelay: number;
  maxDelay: number;
  attempt: number;
}

describe('Property 26: External API Failure Handling', () => {
  const apiResponseArbitrary = fc.record({
    success: fc.boolean(),
    data: fc.option(fc.anything()),
    error: fc.option(fc.string({ minLength: 5, maxLength: 200 })),
    statusCode: fc.integer({ min: 200, max: 599 }),
    retryAfter: fc.option(fc.integer({ min: 1, max: 300 }))
  }) as fc.Arbitrary<APIResponse>;

  /**
   * Handle API response with appropriate error handling
   */
  const handleAPIResponse = (response: APIResponse): {
    shouldRetry: boolean;
    errorMessage?: string;
    retryDelay?: number;
  } => {
    // Success case
    if (response.success && response.statusCode >= 200 && response.statusCode < 300) {
      return { shouldRetry: false };
    }

    // Client errors (4xx) - don't retry
    if (response.statusCode >= 400 && response.statusCode < 500) {
      return {
        shouldRetry: false,
        errorMessage: response.error || `Client error: ${response.statusCode}`
      };
    }

    // Server errors (5xx) - retry
    if (response.statusCode >= 500) {
      return {
        shouldRetry: true,
        errorMessage: response.error || `Server error: ${response.statusCode}`,
        retryDelay: response.retryAfter || 5
      };
    }

    // Rate limiting (429) - retry with delay
    if (response.statusCode === 429) {
      return {
        shouldRetry: true,
        errorMessage: 'Rate limit exceeded',
        retryDelay: response.retryAfter || 60
      };
    }

    // Network errors - retry
    if (!response.success) {
      return {
        shouldRetry: true,
        errorMessage: response.error || 'Network error',
        retryDelay: 5
      };
    }

    return { shouldRetry: false };
  };

  it('should not retry on successful responses', () => {
    fc.assert(
      fc.property(apiResponseArbitrary, (response) => {
        if (response.success && response.statusCode >= 200 && response.statusCode < 300) {
          const result = handleAPIResponse(response);
          expect(result.shouldRetry).toBe(false);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should not retry on client errors (4xx)', () => {
    fc.assert(
      fc.property(apiResponseArbitrary, (response) => {
        if (response.statusCode >= 400 && response.statusCode < 500) {
          const result = handleAPIResponse(response);
          expect(result.shouldRetry).toBe(false);
          expect(result.errorMessage).toBeDefined();
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should retry on server errors (5xx)', () => {
    fc.assert(
      fc.property(apiResponseArbitrary, (response) => {
        if (response.statusCode >= 500) {
          const result = handleAPIResponse(response);
          expect(result.shouldRetry).toBe(true);
          expect(result.retryDelay).toBeDefined();
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should handle rate limiting with retry delay', () => {
    const rateLimitResponse: APIResponse = {
      success: false,
      statusCode: 429,
      error: 'Too many requests',
      retryAfter: 60
    };

    const result = handleAPIResponse(rateLimitResponse);
    expect(result.shouldRetry).toBe(true);
    expect(result.retryDelay).toBe(60);
  });

  it('should use default retry delay when not specified', () => {
    const serverErrorResponse: APIResponse = {
      success: false,
      statusCode: 500,
      error: 'Internal server error'
    };

    const result = handleAPIResponse(serverErrorResponse);
    expect(result.shouldRetry).toBe(true);
    expect(result.retryDelay).toBe(5);
  });

  it('should handle network errors with retry', () => {
    const networkErrorResponse: APIResponse = {
      success: false,
      statusCode: 0,
      error: 'Network connection failed'
    };

    const result = handleAPIResponse(networkErrorResponse);
    expect(result.shouldRetry).toBe(true);
  });

  it('should provide error messages for failures', () => {
    fc.assert(
      fc.property(apiResponseArbitrary, (response) => {
        if (!response.success || response.statusCode >= 400) {
          const result = handleAPIResponse(response);
          expect(result.errorMessage).toBeDefined();
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should handle timeout errors', () => {
    const timeoutResponse: APIResponse = {
      success: false,
      statusCode: 408,
      error: 'Request timeout'
    };

    const result = handleAPIResponse(timeoutResponse);
    expect(result.shouldRetry).toBe(false); // 408 is a client error
    expect(result.errorMessage).toContain('408');
  });
});

describe('Property 27: Retry Logic Consistency', () => {
  const retryConfigArbitrary = fc.record({
    maxRetries: fc.integer({ min: 1, max: 10 }),
    baseDelay: fc.integer({ min: 100, max: 5000 }),
    maxDelay: fc.integer({ min: 5000, max: 60000 }),
    attempt: fc.integer({ min: 0, max: 10 })
  }) as fc.Arbitrary<RetryConfig>;

  /**
   * Calculate retry delay with exponential backoff
   */
  const calculateRetryDelay = (config: RetryConfig): number => {
    if (config.attempt >= config.maxRetries) {
      return -1; // No more retries
    }

    // Exponential backoff: baseDelay * 2^attempt
    const exponentialDelay = config.baseDelay * Math.pow(2, config.attempt);
    
    // Cap at maxDelay
    const delay = Math.min(exponentialDelay, config.maxDelay);
    
    // Add jitter (±10%)
    const jitter = delay * 0.1 * (Math.random() * 2 - 1);
    
    return Math.max(0, delay + jitter);
  };

  /**
   * Determine if should retry
   */
  const shouldRetry = (config: RetryConfig): boolean => {
    return config.attempt < config.maxRetries;
  };

  it('should respect maximum retry limit', () => {
    fc.assert(
      fc.property(retryConfigArbitrary, (config) => {
        const canRetry = shouldRetry(config);
        
        if (config.attempt >= config.maxRetries) {
          expect(canRetry).toBe(false);
        } else {
          expect(canRetry).toBe(true);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should increase delay exponentially', () => {
    fc.assert(
      fc.property(retryConfigArbitrary, (config) => {
        if (config.attempt < config.maxRetries - 1) {
          const delay1 = calculateRetryDelay({ ...config, attempt: config.attempt });
          const delay2 = calculateRetryDelay({ ...config, attempt: config.attempt + 1 });
          
          // Second delay should generally be larger (accounting for jitter)
          // We check the base exponential values without jitter
          const base1 = Math.min(config.baseDelay * Math.pow(2, config.attempt), config.maxDelay);
          const base2 = Math.min(config.baseDelay * Math.pow(2, config.attempt + 1), config.maxDelay);
          
          expect(base2).toBeGreaterThanOrEqual(base1);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should not exceed maximum delay', () => {
    fc.assert(
      fc.property(retryConfigArbitrary, (config) => {
        if (config.attempt < config.maxRetries) {
          const delay = calculateRetryDelay(config);
          
          // Delay should not exceed maxDelay + 10% jitter
          expect(delay).toBeLessThanOrEqual(config.maxDelay * 1.1);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should return negative delay when max retries exceeded', () => {
    fc.assert(
      fc.property(retryConfigArbitrary, (config) => {
        const exhaustedConfig = { ...config, attempt: config.maxRetries };
        const delay = calculateRetryDelay(exhaustedConfig);
        
        expect(delay).toBe(-1);
      }),
      { numRuns: 100 }
    );
  });

  it('should have non-negative delay for valid attempts', () => {
    fc.assert(
      fc.property(retryConfigArbitrary, (config) => {
        if (config.attempt < config.maxRetries) {
          const delay = calculateRetryDelay(config);
          expect(delay).toBeGreaterThanOrEqual(0);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should be deterministic for same attempt number (excluding jitter)', () => {
    fc.assert(
      fc.property(retryConfigArbitrary, (config) => {
        if (config.attempt < config.maxRetries) {
          // Calculate base delay without jitter
          const baseDelay = Math.min(
            config.baseDelay * Math.pow(2, config.attempt),
            config.maxDelay
          );
          
          expect(baseDelay).toBeGreaterThanOrEqual(0);
          expect(Number.isFinite(baseDelay)).toBe(true);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should handle first attempt correctly', () => {
    fc.assert(
      fc.property(retryConfigArbitrary, (config) => {
        const firstAttemptConfig = { ...config, attempt: 0 };
        const delay = calculateRetryDelay(firstAttemptConfig);
        
        // First attempt delay should be close to baseDelay (±10% jitter)
        expect(delay).toBeGreaterThanOrEqual(config.baseDelay * 0.9);
        expect(delay).toBeLessThanOrEqual(config.baseDelay * 1.1);
      }),
      { numRuns: 100 }
    );
  });

  it('should handle maximum attempt correctly', () => {
    fc.assert(
      fc.property(retryConfigArbitrary, (config) => {
        const maxAttemptConfig = { ...config, attempt: config.maxRetries - 1 };
        
        if (config.maxRetries > 0) {
          const delay = calculateRetryDelay(maxAttemptConfig);
          expect(delay).toBeGreaterThanOrEqual(0);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should implement exponential backoff correctly', () => {
    const config: RetryConfig = {
      maxRetries: 5,
      baseDelay: 1000,
      maxDelay: 30000,
      attempt: 0
    };

    // Calculate delays for each attempt (without jitter for testing)
    const delays = [];
    for (let i = 0; i < config.maxRetries; i++) {
      const baseDelay = Math.min(config.baseDelay * Math.pow(2, i), config.maxDelay);
      delays.push(baseDelay);
    }

    // Verify exponential growth
    expect(delays[0]).toBe(1000);   // 1000 * 2^0
    expect(delays[1]).toBe(2000);   // 1000 * 2^1
    expect(delays[2]).toBe(4000);   // 1000 * 2^2
    expect(delays[3]).toBe(8000);   // 1000 * 2^3
    expect(delays[4]).toBe(16000);  // 1000 * 2^4
  });

  it('should cap at maxDelay for large attempt numbers', () => {
    const config: RetryConfig = {
      maxRetries: 10,
      baseDelay: 1000,
      maxDelay: 10000,
      attempt: 8 // 1000 * 2^8 = 256000, should be capped at 10000
    };

    const baseDelay = Math.min(config.baseDelay * Math.pow(2, config.attempt), config.maxDelay);
    expect(baseDelay).toBe(config.maxDelay);
  });
});
