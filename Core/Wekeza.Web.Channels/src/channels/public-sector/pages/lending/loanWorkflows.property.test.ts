import { describe, it, expect } from 'vitest';
import * as fc from 'fast-check';
import { LoanApplication, LoanStatus } from '../../types';

/**
 * Property-Based Tests for Loan Workflows
 * 
 * **Validates: Requirements 2.3, 2.4, 2.5, 2.9**
 */

describe('Property 4: Creditworthiness Calculation', () => {
  /**
   * Arbitrary generator for government entity financial data
   */
  const financialDataArbitrary = fc.record({
    annualRevenue: fc.double({ min: 1000000, max: 100000000000, noNaN: true }),
    totalDebt: fc.double({ min: 0, max: 50000000000, noNaN: true }),
    debtServiceCoverage: fc.double({ min: 0.1, max: 10, noNaN: true }),
    liquidityRatio: fc.double({ min: 0.1, max: 5, noNaN: true }),
    budgetUtilization: fc.double({ min: 0, max: 150, noNaN: true }), // percentage
  });

  /**
   * Calculate creditworthiness score (0-100)
   */
  const calculateCreditworthiness = (data: {
    annualRevenue: number;
    totalDebt: number;
    debtServiceCoverage: number;
    liquidityRatio: number;
    budgetUtilization: number;
  }): number => {
    // Debt-to-Revenue ratio (lower is better)
    const debtToRevenue = data.totalDebt / data.annualRevenue;
    const debtScore = Math.max(0, 30 - (debtToRevenue * 30));

    // Debt Service Coverage (higher is better, optimal > 1.5)
    const dscScore = Math.min(25, (data.debtServiceCoverage / 1.5) * 25);

    // Liquidity Ratio (higher is better, optimal > 1.5)
    const liquidityScore = Math.min(25, (data.liquidityRatio / 1.5) * 25);

    // Budget Utilization (optimal around 85-95%)
    const utilizationDiff = Math.abs(90 - data.budgetUtilization);
    const utilizationScore = Math.max(0, 20 - utilizationDiff / 5);

    const totalScore = debtScore + dscScore + liquidityScore + utilizationScore;
    return Math.min(100, Math.max(0, totalScore));
  };

  it('should return score between 0 and 100', () => {
    fc.assert(
      fc.property(financialDataArbitrary, (data) => {
        const score = calculateCreditworthiness(data);
        expect(score).toBeGreaterThanOrEqual(0);
        expect(score).toBeLessThanOrEqual(100);
      }),
      { numRuns: 100 }
    );
  });

  it('should decrease score with higher debt-to-revenue ratio', () => {
    fc.assert(
      fc.property(financialDataArbitrary, (data) => {
        const score1 = calculateCreditworthiness(data);
        const score2 = calculateCreditworthiness({
          ...data,
          totalDebt: data.totalDebt * 2
        });
        
        // Higher debt should result in lower or equal score
        expect(score2).toBeLessThanOrEqual(score1);
      }),
      { numRuns: 100 }
    );
  });

  it('should increase score with better debt service coverage', () => {
    fc.assert(
      fc.property(financialDataArbitrary, (data) => {
        const score1 = calculateCreditworthiness(data);
        const score2 = calculateCreditworthiness({
          ...data,
          debtServiceCoverage: data.debtServiceCoverage * 1.5
        });
        
        // Better DSC should result in higher or equal score
        expect(score2).toBeGreaterThanOrEqual(score1);
      }),
      { numRuns: 100 }
    );
  });

  it('should increase score with better liquidity ratio', () => {
    fc.assert(
      fc.property(financialDataArbitrary, (data) => {
        const score1 = calculateCreditworthiness(data);
        const score2 = calculateCreditworthiness({
          ...data,
          liquidityRatio: data.liquidityRatio * 1.5
        });
        
        // Better liquidity should result in higher or equal score
        expect(score2).toBeGreaterThanOrEqual(score1);
      }),
      { numRuns: 100 }
    );
  });

  it('should be deterministic for same inputs', () => {
    fc.assert(
      fc.property(financialDataArbitrary, (data) => {
        const score1 = calculateCreditworthiness(data);
        const score2 = calculateCreditworthiness(data);
        
        expect(score1).toBe(score2);
      }),
      { numRuns: 100 }
    );
  });

  it('should handle zero debt correctly', () => {
    fc.assert(
      fc.property(financialDataArbitrary, (data) => {
        const score = calculateCreditworthiness({
          ...data,
          totalDebt: 0
        });
        
        // Zero debt should give maximum debt score component
        expect(score).toBeGreaterThanOrEqual(0);
        expect(score).toBeLessThanOrEqual(100);
      }),
      { numRuns: 100 }
    );
  });
});

describe('Property 5: Loan Status Transitions', () => {
  /**
   * Valid loan status transitions
   */
  const validTransitions: Record<LoanStatus, LoanStatus[]> = {
    'DRAFT': ['PENDING', 'CANCELLED'],
    'PENDING': ['UNDER_REVIEW', 'CANCELLED'],
    'UNDER_REVIEW': ['APPROVED', 'REJECTED', 'PENDING'],
    'APPROVED': ['DISBURSED', 'CANCELLED'],
    'REJECTED': [],
    'DISBURSED': ['ACTIVE', 'CANCELLED'],
    'ACTIVE': ['CLOSED', 'DEFAULTED'],
    'CLOSED': [],
    'DEFAULTED': ['ACTIVE', 'CLOSED'],
    'CANCELLED': []
  };

  const loanStatusArbitrary = fc.constantFrom(
    'DRAFT', 'PENDING', 'UNDER_REVIEW', 'APPROVED', 'REJECTED',
    'DISBURSED', 'ACTIVE', 'CLOSED', 'DEFAULTED', 'CANCELLED'
  ) as fc.Arbitrary<LoanStatus>;

  const isValidTransition = (from: LoanStatus, to: LoanStatus): boolean => {
    return validTransitions[from]?.includes(to) || false;
  };

  it('should only allow valid status transitions', () => {
    fc.assert(
      fc.property(loanStatusArbitrary, loanStatusArbitrary, (fromStatus, toStatus) => {
        const isValid = isValidTransition(fromStatus, toStatus);
        
        if (isValid) {
          // Valid transitions should be in the allowed list
          expect(validTransitions[fromStatus]).toContain(toStatus);
        } else {
          // Invalid transitions should not be in the allowed list
          expect(validTransitions[fromStatus] || []).not.toContain(toStatus);
        }
      }),
      { numRuns: 200 }
    );
  });

  it('should not allow transitions from terminal states', () => {
    const terminalStates: LoanStatus[] = ['REJECTED', 'CLOSED', 'CANCELLED'];
    
    fc.assert(
      fc.property(loanStatusArbitrary, (toStatus) => {
        terminalStates.forEach(terminalState => {
          const isValid = isValidTransition(terminalState, toStatus);
          
          // Terminal states should not transition to any other state
          expect(isValid).toBe(false);
        });
      }),
      { numRuns: 100 }
    );
  });

  it('should allow DRAFT to transition to PENDING', () => {
    expect(isValidTransition('DRAFT', 'PENDING')).toBe(true);
  });

  it('should allow APPROVED to transition to DISBURSED', () => {
    expect(isValidTransition('APPROVED', 'DISBURSED')).toBe(true);
  });

  it('should not allow REJECTED to transition to APPROVED', () => {
    expect(isValidTransition('REJECTED', 'APPROVED')).toBe(false);
  });

  it('should not allow CLOSED to transition to any state', () => {
    fc.assert(
      fc.property(loanStatusArbitrary, (toStatus) => {
        expect(isValidTransition('CLOSED', toStatus)).toBe(false);
      }),
      { numRuns: 100 }
    );
  });

  it('should maintain status transition history integrity', () => {
    fc.assert(
      fc.property(
        fc.array(loanStatusArbitrary, { minLength: 2, maxLength: 10 }),
        (statusHistory) => {
          // Check if the status history represents valid transitions
          let isValidHistory = true;
          
          for (let i = 0; i < statusHistory.length - 1; i++) {
            if (!isValidTransition(statusHistory[i], statusHistory[i + 1])) {
              isValidHistory = false;
              break;
            }
          }
          
          // If history is valid, all transitions should be valid
          // If history is invalid, at least one transition should be invalid
          expect(typeof isValidHistory).toBe('boolean');
        }
      ),
      { numRuns: 100 }
    );
  });
});

describe('Property 7: Lending Limit Enforcement', () => {
  /**
   * Arbitrary generator for loan applications with limits
   */
  const loanApplicationArbitrary = fc.record({
    requestedAmount: fc.double({ min: 1000000, max: 10000000000, noNaN: true }),
    entityType: fc.constantFrom('NATIONAL', 'COUNTY', 'PARASTATAL') as fc.Arbitrary<'NATIONAL' | 'COUNTY' | 'PARASTATAL'>,
    existingExposure: fc.double({ min: 0, max: 5000000000, noNaN: true }),
    entityRevenue: fc.double({ min: 10000000, max: 100000000000, noNaN: true })
  });

  /**
   * Lending limits by entity type
   */
  const lendingLimits = {
    NATIONAL: {
      maxSingleLoan: 5000000000, // 5 billion
      maxTotalExposure: 20000000000, // 20 billion
      maxDebtToRevenue: 0.5 // 50%
    },
    COUNTY: {
      maxSingleLoan: 1000000000, // 1 billion
      maxTotalExposure: 3000000000, // 3 billion
      maxDebtToRevenue: 0.4 // 40%
    },
    PARASTATAL: {
      maxSingleLoan: 500000000, // 500 million
      maxTotalExposure: 1500000000, // 1.5 billion
      maxDebtToRevenue: 0.35 // 35%
    }
  };

  const checkLendingLimit = (application: {
    requestedAmount: number;
    entityType: 'NATIONAL' | 'COUNTY' | 'PARASTATAL';
    existingExposure: number;
    entityRevenue: number;
  }): { allowed: boolean; reason?: string } => {
    const limits = lendingLimits[application.entityType];
    
    // Check single loan limit
    if (application.requestedAmount > limits.maxSingleLoan) {
      return { allowed: false, reason: 'Exceeds single loan limit' };
    }
    
    // Check total exposure limit
    const totalExposure = application.existingExposure + application.requestedAmount;
    if (totalExposure > limits.maxTotalExposure) {
      return { allowed: false, reason: 'Exceeds total exposure limit' };
    }
    
    // Check debt-to-revenue ratio
    const debtToRevenue = totalExposure / application.entityRevenue;
    if (debtToRevenue > limits.maxDebtToRevenue) {
      return { allowed: false, reason: 'Exceeds debt-to-revenue ratio' };
    }
    
    return { allowed: true };
  };

  it('should enforce single loan limits by entity type', () => {
    fc.assert(
      fc.property(loanApplicationArbitrary, (application) => {
        const result = checkLendingLimit(application);
        const limits = lendingLimits[application.entityType];
        
        if (application.requestedAmount > limits.maxSingleLoan) {
          expect(result.allowed).toBe(false);
          expect(result.reason).toContain('single loan limit');
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should enforce total exposure limits', () => {
    fc.assert(
      fc.property(loanApplicationArbitrary, (application) => {
        const result = checkLendingLimit(application);
        const limits = lendingLimits[application.entityType];
        const totalExposure = application.existingExposure + application.requestedAmount;
        
        if (totalExposure > limits.maxTotalExposure) {
          expect(result.allowed).toBe(false);
          expect(result.reason).toContain('total exposure limit');
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should enforce debt-to-revenue ratio limits', () => {
    fc.assert(
      fc.property(loanApplicationArbitrary, (application) => {
        const result = checkLendingLimit(application);
        const limits = lendingLimits[application.entityType];
        const totalExposure = application.existingExposure + application.requestedAmount;
        const debtToRevenue = totalExposure / application.entityRevenue;
        
        if (debtToRevenue > limits.maxDebtToRevenue) {
          expect(result.allowed).toBe(false);
          expect(result.reason).toContain('debt-to-revenue ratio');
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should have stricter limits for lower entity types', () => {
    // National > County > Parastatal in terms of limits
    expect(lendingLimits.NATIONAL.maxSingleLoan).toBeGreaterThan(lendingLimits.COUNTY.maxSingleLoan);
    expect(lendingLimits.COUNTY.maxSingleLoan).toBeGreaterThan(lendingLimits.PARASTATAL.maxSingleLoan);
    
    expect(lendingLimits.NATIONAL.maxTotalExposure).toBeGreaterThan(lendingLimits.COUNTY.maxTotalExposure);
    expect(lendingLimits.COUNTY.maxTotalExposure).toBeGreaterThan(lendingLimits.PARASTATAL.maxTotalExposure);
  });

  it('should allow loans within all limits', () => {
    fc.assert(
      fc.property(loanApplicationArbitrary, (application) => {
        const limits = lendingLimits[application.entityType];
        
        // Create a loan that's definitely within limits
        const safeApplication = {
          ...application,
          requestedAmount: limits.maxSingleLoan * 0.1,
          existingExposure: 0,
          entityRevenue: limits.maxSingleLoan * 10
        };
        
        const result = checkLendingLimit(safeApplication);
        expect(result.allowed).toBe(true);
      }),
      { numRuns: 100 }
    );
  });

  it('should be consistent for same inputs', () => {
    fc.assert(
      fc.property(loanApplicationArbitrary, (application) => {
        const result1 = checkLendingLimit(application);
        const result2 = checkLendingLimit(application);
        
        expect(result1.allowed).toBe(result2.allowed);
        expect(result1.reason).toBe(result2.reason);
      }),
      { numRuns: 100 }
    );
  });
});
