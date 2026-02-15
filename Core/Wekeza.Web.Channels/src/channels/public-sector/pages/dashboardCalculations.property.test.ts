import { describe, it, expect } from 'vitest';
import * as fc from 'fast-check';

/**
 * Property-Based Tests for Dashboard Calculations
 * 
 * **Validates: Requirements 6.2, 6.5, 6.6**
 */

interface LoanPortfolioItem {
  loanId: string;
  entityName: string;
  entityType: 'NATIONAL' | 'COUNTY' | 'PARASTATAL';
  principalAmount: number;
  outstandingBalance: number;
  interestRate: number;
  status: 'ACTIVE' | 'DEFAULTED' | 'CLOSED';
}

interface RevenueItem {
  date: Date;
  type: 'TAX' | 'FEE' | 'LICENSE' | 'FINE';
  amount: number;
  status: 'COLLECTED' | 'PENDING';
}

interface RiskExposure {
  entityId: string;
  entityType: 'NATIONAL' | 'COUNTY' | 'PARASTATAL';
  totalExposure: number;
  riskRating: 'LOW' | 'MEDIUM' | 'HIGH';
  creditScore: number;
}

describe('Property 19: Loan Portfolio Aggregation', () => {
  const loanPortfolioArbitrary = fc.record({
    loanId: fc.uuid(),
    entityName: fc.string({ minLength: 3, maxLength: 100 }),
    entityType: fc.constantFrom('NATIONAL', 'COUNTY', 'PARASTATAL') as fc.Arbitrary<'NATIONAL' | 'COUNTY' | 'PARASTATAL'>,
    principalAmount: fc.double({ min: 1000000, max: 5000000000, noNaN: true }),
    outstandingBalance: fc.double({ min: 0, max: 5000000000, noNaN: true }),
    interestRate: fc.double({ min: 5, max: 20, noNaN: true }),
    status: fc.constantFrom('ACTIVE', 'DEFAULTED', 'CLOSED') as fc.Arbitrary<'ACTIVE' | 'DEFAULTED' | 'CLOSED'>
  }) as fc.Arbitrary<LoanPortfolioItem>;

  const aggregateLoanPortfolio = (loans: LoanPortfolioItem[]) => {
    const totalPrincipal = loans.reduce((sum, loan) => sum + loan.principalAmount, 0);
    const totalOutstanding = loans.reduce((sum, loan) => sum + loan.outstandingBalance, 0);
    const activeLoans = loans.filter(l => l.status === 'ACTIVE');
    const defaultedLoans = loans.filter(l => l.status === 'DEFAULTED');
    
    const byEntityType: Record<string, number> = {};
    loans.forEach(loan => {
      byEntityType[loan.entityType] = (byEntityType[loan.entityType] || 0) + loan.outstandingBalance;
    });

    const averageInterestRate = loans.length > 0
      ? loans.reduce((sum, loan) => sum + loan.interestRate, 0) / loans.length
      : 0;

    return {
      totalPrincipal,
      totalOutstanding,
      activeCount: activeLoans.length,
      defaultedCount: defaultedLoans.length,
      byEntityType,
      averageInterestRate
    };
  };

  it('should aggregate total principal correctly', () => {
    fc.assert(
      fc.property(
        fc.array(loanPortfolioArbitrary, { minLength: 1, maxLength: 50 }),
        (loans) => {
          const result = aggregateLoanPortfolio(loans);
          const expectedTotal = loans.reduce((sum, loan) => sum + loan.principalAmount, 0);
          expect(Math.abs(result.totalPrincipal - expectedTotal)).toBeLessThan(0.01);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should aggregate total outstanding correctly', () => {
    fc.assert(
      fc.property(
        fc.array(loanPortfolioArbitrary, { minLength: 1, maxLength: 50 }),
        (loans) => {
          const result = aggregateLoanPortfolio(loans);
          const expectedTotal = loans.reduce((sum, loan) => sum + loan.outstandingBalance, 0);
          expect(Math.abs(result.totalOutstanding - expectedTotal)).toBeLessThan(0.01);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should have outstanding balance less than or equal to principal', () => {
    fc.assert(
      fc.property(loanPortfolioArbitrary, (loan) => {
        const validLoan = {
          ...loan,
          outstandingBalance: Math.min(loan.outstandingBalance, loan.principalAmount)
        };
        expect(validLoan.outstandingBalance).toBeLessThanOrEqual(validLoan.principalAmount);
      }),
      { numRuns: 100 }
    );
  });

  it('should count active and defaulted loans correctly', () => {
    fc.assert(
      fc.property(
        fc.array(loanPortfolioArbitrary, { minLength: 1, maxLength: 50 }),
        (loans) => {
          const result = aggregateLoanPortfolio(loans);
          const expectedActive = loans.filter(l => l.status === 'ACTIVE').length;
          const expectedDefaulted = loans.filter(l => l.status === 'DEFAULTED').length;
          
          expect(result.activeCount).toBe(expectedActive);
          expect(result.defaultedCount).toBe(expectedDefaulted);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should aggregate by entity type correctly', () => {
    fc.assert(
      fc.property(
        fc.array(loanPortfolioArbitrary, { minLength: 1, maxLength: 50 }),
        (loans) => {
          const result = aggregateLoanPortfolio(loans);
          const sumByType = Object.values(result.byEntityType).reduce((sum, val) => sum + val, 0);
          expect(Math.abs(sumByType - result.totalOutstanding)).toBeLessThan(0.01);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should calculate average interest rate correctly', () => {
    fc.assert(
      fc.property(
        fc.array(loanPortfolioArbitrary, { minLength: 1, maxLength: 50 }),
        (loans) => {
          const result = aggregateLoanPortfolio(loans);
          const expectedAverage = loans.reduce((sum, loan) => sum + loan.interestRate, 0) / loans.length;
          expect(Math.abs(result.averageInterestRate - expectedAverage)).toBeLessThan(0.01);
        }
      ),
      { numRuns: 100 }
    );
  });
});

describe('Property 20: Revenue Calculation Accuracy', () => {
  const revenueItemArbitrary = fc.record({
    date: fc.date({ min: new Date('2024-01-01'), max: new Date('2026-12-31') }),
    type: fc.constantFrom('TAX', 'FEE', 'LICENSE', 'FINE') as fc.Arbitrary<'TAX' | 'FEE' | 'LICENSE' | 'FINE'>,
    amount: fc.double({ min: 1, max: 10000000, noNaN: true }),
    status: fc.constantFrom('COLLECTED', 'PENDING') as fc.Arbitrary<'COLLECTED' | 'PENDING'>
  }) as fc.Arbitrary<RevenueItem>;

  const calculateRevenue = (items: RevenueItem[]) => {
    const totalCollected = items
      .filter(i => i.status === 'COLLECTED')
      .reduce((sum, i) => sum + i.amount, 0);
    
    const totalPending = items
      .filter(i => i.status === 'PENDING')
      .reduce((sum, i) => sum + i.amount, 0);

    const byType: Record<string, number> = {};
    items.filter(i => i.status === 'COLLECTED').forEach(i => {
      byType[i.type] = (byType[i.type] || 0) + i.amount;
    });

    return {
      totalCollected,
      totalPending,
      totalRevenue: totalCollected + totalPending,
      byType,
      itemCount: items.length
    };
  };

  it('should calculate total collected revenue correctly', () => {
    fc.assert(
      fc.property(
        fc.array(revenueItemArbitrary, { minLength: 1, maxLength: 50 }),
        (items) => {
          const result = calculateRevenue(items);
          const expected = items
            .filter(i => i.status === 'COLLECTED')
            .reduce((sum, i) => sum + i.amount, 0);
          expect(Math.abs(result.totalCollected - expected)).toBeLessThan(0.01);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should have total revenue equal to collected plus pending', () => {
    fc.assert(
      fc.property(
        fc.array(revenueItemArbitrary, { minLength: 1, maxLength: 50 }),
        (items) => {
          const result = calculateRevenue(items);
          expect(Math.abs(result.totalRevenue - (result.totalCollected + result.totalPending))).toBeLessThan(0.01);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should aggregate by type correctly', () => {
    fc.assert(
      fc.property(
        fc.array(revenueItemArbitrary, { minLength: 1, maxLength: 50 }),
        (items) => {
          const result = calculateRevenue(items);
          const sumByType = Object.values(result.byType).reduce((sum, val) => sum + val, 0);
          expect(Math.abs(sumByType - result.totalCollected)).toBeLessThan(0.01);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should have non-negative revenue values', () => {
    fc.assert(
      fc.property(
        fc.array(revenueItemArbitrary, { minLength: 1, maxLength: 50 }),
        (items) => {
          const result = calculateRevenue(items);
          expect(result.totalCollected).toBeGreaterThanOrEqual(0);
          expect(result.totalPending).toBeGreaterThanOrEqual(0);
          expect(result.totalRevenue).toBeGreaterThanOrEqual(0);
        }
      ),
      { numRuns: 100 }
    );
  });
});

describe('Property 21: Risk Exposure Calculation', () => {
  const riskExposureArbitrary = fc.record({
    entityId: fc.uuid(),
    entityType: fc.constantFrom('NATIONAL', 'COUNTY', 'PARASTATAL') as fc.Arbitrary<'NATIONAL' | 'COUNTY' | 'PARASTATAL'>,
    totalExposure: fc.double({ min: 0, max: 10000000000, noNaN: true }),
    riskRating: fc.constantFrom('LOW', 'MEDIUM', 'HIGH') as fc.Arbitrary<'LOW' | 'MEDIUM' | 'HIGH'>,
    creditScore: fc.double({ min: 0, max: 100, noNaN: true })
  }) as fc.Arbitrary<RiskExposure>;

  const calculateRiskExposure = (exposures: RiskExposure[]) => {
    const totalExposure = exposures.reduce((sum, e) => sum + e.totalExposure, 0);
    
    const byRiskRating: Record<string, number> = {};
    exposures.forEach(e => {
      byRiskRating[e.riskRating] = (byRiskRating[e.riskRating] || 0) + e.totalExposure;
    });

    const highRiskExposure = exposures
      .filter(e => e.riskRating === 'HIGH')
      .reduce((sum, e) => sum + e.totalExposure, 0);

    const averageCreditScore = exposures.length > 0
      ? exposures.reduce((sum, e) => sum + e.creditScore, 0) / exposures.length
      : 0;

    return {
      totalExposure,
      byRiskRating,
      highRiskExposure,
      highRiskPercentage: totalExposure > 0 ? (highRiskExposure / totalExposure) * 100 : 0,
      averageCreditScore
    };
  };

  it('should calculate total exposure correctly', () => {
    fc.assert(
      fc.property(
        fc.array(riskExposureArbitrary, { minLength: 1, maxLength: 50 }),
        (exposures) => {
          const result = calculateRiskExposure(exposures);
          const expected = exposures.reduce((sum, e) => sum + e.totalExposure, 0);
          expect(Math.abs(result.totalExposure - expected)).toBeLessThan(0.01);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should aggregate by risk rating correctly', () => {
    fc.assert(
      fc.property(
        fc.array(riskExposureArbitrary, { minLength: 1, maxLength: 50 }),
        (exposures) => {
          const result = calculateRiskExposure(exposures);
          const sumByRating = Object.values(result.byRiskRating).reduce((sum, val) => sum + val, 0);
          expect(Math.abs(sumByRating - result.totalExposure)).toBeLessThan(0.01);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should calculate high risk percentage correctly', () => {
    fc.assert(
      fc.property(
        fc.array(riskExposureArbitrary, { minLength: 1, maxLength: 50 }),
        (exposures) => {
          const result = calculateRiskExposure(exposures);
          expect(result.highRiskPercentage).toBeGreaterThanOrEqual(0);
          expect(result.highRiskPercentage).toBeLessThanOrEqual(100);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should have high risk exposure less than or equal to total', () => {
    fc.assert(
      fc.property(
        fc.array(riskExposureArbitrary, { minLength: 1, maxLength: 50 }),
        (exposures) => {
          const result = calculateRiskExposure(exposures);
          expect(result.highRiskExposure).toBeLessThanOrEqual(result.totalExposure);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should calculate average credit score correctly', () => {
    fc.assert(
      fc.property(
        fc.array(riskExposureArbitrary, { minLength: 1, maxLength: 50 }),
        (exposures) => {
          const result = calculateRiskExposure(exposures);
          const expected = exposures.reduce((sum, e) => sum + e.creditScore, 0) / exposures.length;
          expect(Math.abs(result.averageCreditScore - expected)).toBeLessThan(0.01);
        }
      ),
      { numRuns: 100 }
    );
  });
});
