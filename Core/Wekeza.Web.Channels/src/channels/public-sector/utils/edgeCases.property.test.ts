import { describe, it, expect } from 'vitest';
import * as fc from 'fast-check';

/**
 * Property-Based Tests for Edge Cases
 * 
 * **Validates: Requirements 1.7, 3.2, 4.2**
 */

interface Portfolio {
  securities: Array<{
    id: string;
    type: string;
    quantity: number;
    value: number;
  }>;
  totalValue: number;
}

interface Transaction {
  id: string;
  amount: number;
  type: 'DEBIT' | 'CREDIT';
  status: 'PENDING' | 'COMPLETED' | 'FAILED';
}

interface GrantProgram {
  id: string;
  name: string;
  startDate: Date;
  endDate: Date;
  budget: number;
  status: 'ACTIVE' | 'EXPIRED' | 'SUSPENDED';
}

describe('Property 23: Empty Portfolio Handling', () => {
  /**
   * Handle empty portfolio gracefully
   */
  const calculatePortfolioMetrics = (portfolio: Portfolio): {
    totalValue: number;
    securityCount: number;
    averageValue: number;
    isEmpty: boolean;
  } => {
    const isEmpty = portfolio.securities.length === 0;
    const totalValue = isEmpty ? 0 : portfolio.securities.reduce((sum, s) => sum + s.value, 0);
    const averageValue = isEmpty ? 0 : totalValue / portfolio.securities.length;

    return {
      totalValue,
      securityCount: portfolio.securities.length,
      averageValue,
      isEmpty
    };
  };

  it('should handle empty portfolio without errors', () => {
    const emptyPortfolio: Portfolio = {
      securities: [],
      totalValue: 0
    };

    const metrics = calculatePortfolioMetrics(emptyPortfolio);
    
    expect(metrics.isEmpty).toBe(true);
    expect(metrics.totalValue).toBe(0);
    expect(metrics.securityCount).toBe(0);
    expect(metrics.averageValue).toBe(0);
  });

  it('should return zero for all metrics on empty portfolio', () => {
    const emptyPortfolio: Portfolio = {
      securities: [],
      totalValue: 0
    };

    const metrics = calculatePortfolioMetrics(emptyPortfolio);
    
    expect(metrics.totalValue).toBe(0);
    expect(metrics.averageValue).toBe(0);
    expect(Number.isFinite(metrics.averageValue)).toBe(true);
  });

  it('should not throw division by zero error', () => {
    const emptyPortfolio: Portfolio = {
      securities: [],
      totalValue: 0
    };

    expect(() => calculatePortfolioMetrics(emptyPortfolio)).not.toThrow();
  });

  it('should correctly identify empty vs non-empty portfolios', () => {
    const emptyPortfolio: Portfolio = {
      securities: [],
      totalValue: 0
    };

    const nonEmptyPortfolio: Portfolio = {
      securities: [{ id: '1', type: 'BOND', quantity: 1, value: 1000 }],
      totalValue: 1000
    };

    expect(calculatePortfolioMetrics(emptyPortfolio).isEmpty).toBe(true);
    expect(calculatePortfolioMetrics(nonEmptyPortfolio).isEmpty).toBe(false);
  });

  it('should handle transition from empty to non-empty', () => {
    const portfolio: Portfolio = {
      securities: [],
      totalValue: 0
    };

    const metrics1 = calculatePortfolioMetrics(portfolio);
    expect(metrics1.isEmpty).toBe(true);

    // Add a security
    portfolio.securities.push({ id: '1', type: 'BOND', quantity: 1, value: 1000 });
    portfolio.totalValue = 1000;

    const metrics2 = calculatePortfolioMetrics(portfolio);
    expect(metrics2.isEmpty).toBe(false);
    expect(metrics2.totalValue).toBe(1000);
  });

  it('should handle portfolio with zero-value securities', () => {
    const portfolio: Portfolio = {
      securities: [
        { id: '1', type: 'BOND', quantity: 0, value: 0 },
        { id: '2', type: 'STOCK', quantity: 0, value: 0 }
      ],
      totalValue: 0
    };

    const metrics = calculatePortfolioMetrics(portfolio);
    expect(metrics.totalValue).toBe(0);
    expect(metrics.securityCount).toBe(2);
    expect(metrics.averageValue).toBe(0);
  });
});

describe('Property 24: Zero-Amount Transaction Rejection', () => {
  const transactionArbitrary = fc.record({
    id: fc.uuid(),
    amount: fc.double({ min: -1000000, max: 1000000, noNaN: true }),
    type: fc.constantFrom('DEBIT', 'CREDIT') as fc.Arbitrary<'DEBIT' | 'CREDIT'>,
    status: fc.constantFrom('PENDING', 'COMPLETED', 'FAILED') as fc.Arbitrary<'PENDING' | 'COMPLETED' | 'FAILED'>
  }) as fc.Arbitrary<Transaction>;

  /**
   * Validate transaction amount
   */
  const validateTransaction = (transaction: Transaction): {
    isValid: boolean;
    errors: string[];
  } => {
    const errors: string[] = [];

    if (transaction.amount === 0) {
      errors.push('Transaction amount cannot be zero');
    }

    if (transaction.amount < 0) {
      errors.push('Transaction amount cannot be negative');
    }

    if (!Number.isFinite(transaction.amount)) {
      errors.push('Transaction amount must be a finite number');
    }

    return {
      isValid: errors.length === 0,
      errors
    };
  };

  it('should reject zero-amount transactions', () => {
    const zeroTransaction: Transaction = {
      id: '123',
      amount: 0,
      type: 'DEBIT',
      status: 'PENDING'
    };

    const result = validateTransaction(zeroTransaction);
    expect(result.isValid).toBe(false);
    expect(result.errors.some(e => e.includes('cannot be zero'))).toBe(true);
  });

  it('should reject negative-amount transactions', () => {
    fc.assert(
      fc.property(transactionArbitrary, (transaction) => {
        if (transaction.amount < 0) {
          const result = validateTransaction(transaction);
          expect(result.isValid).toBe(false);
          expect(result.errors.some(e => e.includes('cannot be negative'))).toBe(true);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should accept positive-amount transactions', () => {
    fc.assert(
      fc.property(transactionArbitrary, (transaction) => {
        if (transaction.amount > 0 && Number.isFinite(transaction.amount)) {
          const result = validateTransaction(transaction);
          expect(result.isValid).toBe(true);
          expect(result.errors.length).toBe(0);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should reject NaN amounts', () => {
    const nanTransaction: Transaction = {
      id: '123',
      amount: NaN,
      type: 'DEBIT',
      status: 'PENDING'
    };

    const result = validateTransaction(nanTransaction);
    expect(result.isValid).toBe(false);
    expect(result.errors.some(e => e.includes('finite number'))).toBe(true);
  });

  it('should reject Infinity amounts', () => {
    const infinityTransaction: Transaction = {
      id: '123',
      amount: Infinity,
      type: 'DEBIT',
      status: 'PENDING'
    };

    const result = validateTransaction(infinityTransaction);
    expect(result.isValid).toBe(false);
    expect(result.errors.some(e => e.includes('finite number'))).toBe(true);
  });

  it('should handle very small positive amounts', () => {
    const smallTransaction: Transaction = {
      id: '123',
      amount: 0.01,
      type: 'DEBIT',
      status: 'PENDING'
    };

    const result = validateTransaction(smallTransaction);
    expect(result.isValid).toBe(true);
  });
});

describe('Property 25: Expired Grant Program Handling', () => {
  const grantProgramArbitrary = fc.record({
    id: fc.uuid(),
    name: fc.string({ minLength: 5, maxLength: 100 }),
    startDate: fc.date({ min: new Date('2020-01-01'), max: new Date('2024-12-31') }),
    endDate: fc.date({ min: new Date('2024-01-01'), max: new Date('2030-12-31') }),
    budget: fc.double({ min: 100000, max: 100000000, noNaN: true }),
    status: fc.constantFrom('ACTIVE', 'EXPIRED', 'SUSPENDED') as fc.Arbitrary<'ACTIVE' | 'EXPIRED' | 'SUSPENDED'>
  }) as fc.Arbitrary<GrantProgram>;

  /**
   * Check if grant program is available for applications
   */
  const isGrantProgramAvailable = (program: GrantProgram, currentDate: Date = new Date()): {
    isAvailable: boolean;
    reason?: string;
  } => {
    // Check if expired
    if (currentDate > program.endDate) {
      return { isAvailable: false, reason: 'Program has expired' };
    }

    // Check if not yet started
    if (currentDate < program.startDate) {
      return { isAvailable: false, reason: 'Program has not started yet' };
    }

    // Check status
    if (program.status === 'EXPIRED') {
      return { isAvailable: false, reason: 'Program status is expired' };
    }

    if (program.status === 'SUSPENDED') {
      return { isAvailable: false, reason: 'Program is suspended' };
    }

    // Check budget
    if (program.budget <= 0) {
      return { isAvailable: false, reason: 'Program has no remaining budget' };
    }

    return { isAvailable: true };
  };

  it('should reject applications to expired programs', () => {
    const expiredProgram: GrantProgram = {
      id: '123',
      name: 'Test Program',
      startDate: new Date('2020-01-01'),
      endDate: new Date('2023-12-31'),
      budget: 1000000,
      status: 'ACTIVE'
    };

    const result = isGrantProgramAvailable(expiredProgram, new Date('2024-06-01'));
    expect(result.isAvailable).toBe(false);
    expect(result.reason).toContain('expired');
  });

  it('should reject applications to programs with EXPIRED status', () => {
    const program: GrantProgram = {
      id: '123',
      name: 'Test Program',
      startDate: new Date('2024-01-01'),
      endDate: new Date('2025-12-31'),
      budget: 1000000,
      status: 'EXPIRED'
    };

    const result = isGrantProgramAvailable(program, new Date('2024-06-01'));
    expect(result.isAvailable).toBe(false);
    expect(result.reason).toContain('expired');
  });

  it('should reject applications to suspended programs', () => {
    const program: GrantProgram = {
      id: '123',
      name: 'Test Program',
      startDate: new Date('2024-01-01'),
      endDate: new Date('2025-12-31'),
      budget: 1000000,
      status: 'SUSPENDED'
    };

    const result = isGrantProgramAvailable(program, new Date('2024-06-01'));
    expect(result.isAvailable).toBe(false);
    expect(result.reason).toContain('suspended');
  });

  it('should reject applications to programs not yet started', () => {
    const futureProgram: GrantProgram = {
      id: '123',
      name: 'Test Program',
      startDate: new Date('2025-01-01'),
      endDate: new Date('2026-12-31'),
      budget: 1000000,
      status: 'ACTIVE'
    };

    const result = isGrantProgramAvailable(futureProgram, new Date('2024-06-01'));
    expect(result.isAvailable).toBe(false);
    expect(result.reason).toContain('not started');
  });

  it('should accept applications to active programs within date range', () => {
    const activeProgram: GrantProgram = {
      id: '123',
      name: 'Test Program',
      startDate: new Date('2024-01-01'),
      endDate: new Date('2025-12-31'),
      budget: 1000000,
      status: 'ACTIVE'
    };

    const result = isGrantProgramAvailable(activeProgram, new Date('2024-06-01'));
    expect(result.isAvailable).toBe(true);
  });

  it('should reject programs with zero budget', () => {
    const noBudgetProgram: GrantProgram = {
      id: '123',
      name: 'Test Program',
      startDate: new Date('2024-01-01'),
      endDate: new Date('2025-12-31'),
      budget: 0,
      status: 'ACTIVE'
    };

    const result = isGrantProgramAvailable(noBudgetProgram, new Date('2024-06-01'));
    expect(result.isAvailable).toBe(false);
    expect(result.reason).toContain('budget');
  });

  it('should handle program on exact end date', () => {
    const program: GrantProgram = {
      id: '123',
      name: 'Test Program',
      startDate: new Date('2024-01-01'),
      endDate: new Date('2024-12-31'),
      budget: 1000000,
      status: 'ACTIVE'
    };

    const result = isGrantProgramAvailable(program, new Date('2024-12-31'));
    // On the exact end date, program should still be available
    expect(result.isAvailable).toBe(true);
  });

  it('should handle program on exact start date', () => {
    const program: GrantProgram = {
      id: '123',
      name: 'Test Program',
      startDate: new Date('2024-01-01'),
      endDate: new Date('2025-12-31'),
      budget: 1000000,
      status: 'ACTIVE'
    };

    const result = isGrantProgramAvailable(program, new Date('2024-01-01'));
    // On the exact start date, program should be available
    expect(result.isAvailable).toBe(true);
  });
});
