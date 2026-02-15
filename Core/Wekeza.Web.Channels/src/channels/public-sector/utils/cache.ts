// Caching utility for performance optimization

interface CacheEntry<T> {
  data: T;
  timestamp: number;
  expiresIn: number; // milliseconds
}

class CacheManager {
  private cache: Map<string, CacheEntry<any>> = new Map();

  /**
   * Get cached data if it exists and hasn't expired
   */
  get<T>(key: string): T | null {
    const entry = this.cache.get(key);
    
    if (!entry) return null;
    
    const now = Date.now();
    const age = now - entry.timestamp;
    
    if (age > entry.expiresIn) {
      // Cache expired, remove it
      this.cache.delete(key);
      return null;
    }
    
    return entry.data as T;
  }

  /**
   * Set data in cache with expiration time
   */
  set<T>(key: string, data: T, expiresIn: number): void {
    this.cache.set(key, {
      data,
      timestamp: Date.now(),
      expiresIn
    });
  }

  /**
   * Clear specific cache entry
   */
  clear(key: string): void {
    this.cache.delete(key);
  }

  /**
   * Clear all cache entries
   */
  clearAll(): void {
    this.cache.clear();
  }

  /**
   * Clear expired entries
   */
  clearExpired(): void {
    const now = Date.now();
    
    for (const [key, entry] of this.cache.entries()) {
      const age = now - entry.timestamp;
      if (age > entry.expiresIn) {
        this.cache.delete(key);
      }
    }
  }
}

// Singleton instance
const cacheManager = new CacheManager();

// Clear expired entries every minute
setInterval(() => {
  cacheManager.clearExpired();
}, 60 * 1000);

// Cache duration constants (in milliseconds)
export const CACHE_DURATION = {
  DASHBOARD_METRICS: 5 * 60 * 1000,      // 5 minutes
  SECURITIES_PRICES: 30 * 1000,          // 30 seconds
  USER_PERMISSIONS: Infinity,             // Session (never expires in memory)
  LOAN_PORTFOLIO: 2 * 60 * 1000,         // 2 minutes
  GRANT_PROGRAMS: 10 * 60 * 1000,        // 10 minutes
  ACCOUNTS: 1 * 60 * 1000,               // 1 minute
};

/**
 * Fetch data with caching
 */
export async function fetchWithCache<T>(
  key: string,
  fetcher: () => Promise<T>,
  expiresIn: number = CACHE_DURATION.DASHBOARD_METRICS
): Promise<T> {
  // Check cache first
  const cached = cacheManager.get<T>(key);
  if (cached !== null) {
    return cached;
  }

  // Fetch fresh data
  const data = await fetcher();
  
  // Store in cache
  cacheManager.set(key, data, expiresIn);
  
  return data;
}

/**
 * Clear cache for specific key
 */
export function clearCache(key: string): void {
  cacheManager.clear(key);
}

/**
 * Clear all cache
 */
export function clearAllCache(): void {
  cacheManager.clearAll();
}

/**
 * Helper functions for common cache operations
 */

export function cacheDashboardMetrics<T>(data: T): void {
  cacheManager.set('dashboard:metrics', data, CACHE_DURATION.DASHBOARD_METRICS);
}

export function getCachedDashboardMetrics<T>(): T | null {
  return cacheManager.get<T>('dashboard:metrics');
}

export function cacheSecuritiesPrices<T>(data: T): void {
  cacheManager.set('securities:prices', data, CACHE_DURATION.SECURITIES_PRICES);
}

export function getCachedSecuritiesPrices<T>(): T | null {
  return cacheManager.get<T>('securities:prices');
}

export function cacheUserPermissions<T>(userId: string, permissions: T): void {
  cacheManager.set(`user:${userId}:permissions`, permissions, CACHE_DURATION.USER_PERMISSIONS);
}

export function getCachedUserPermissions<T>(userId: string): T | null {
  return cacheManager.get<T>(`user:${userId}:permissions`);
}

export function clearUserCache(userId: string): void {
  cacheManager.clear(`user:${userId}:permissions`);
}

// Clear all cache on logout
export function clearCacheOnLogout(): void {
  cacheManager.clearAll();
}
