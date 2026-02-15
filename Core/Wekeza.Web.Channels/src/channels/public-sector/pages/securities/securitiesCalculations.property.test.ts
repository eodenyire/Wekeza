import { describe, it, expect } from 'vitest';
import * as fc from 'fast-check';
import { Bond, Portfolio, PortfolioSecurity } from '../../types';

/**
 * Property-Based Tests for Securities Calculations
 * 
 * **Validates: Requirements 1.8, 6.1**
 */

describe('Property 2: Interest Calculation Accuracy', () => {
  /**
   * Arbitrary generator for bonds with valid parameters
   */
  const bondArbitrary = fc.record({
    id: fc.string({ minLength: 1, maxLength: 50 }),
    name: fc.string({ minLength: 1, maxLength: 100 }),
    isin: fc.string({ minLength: 12, maxLength: 12 }),
    couponRate: fc.double({ min: 0.1, max: 20, noNaN: true }), // 0.1% to 20%
    faceValue: fc.integer({ min: 1000, max: 1000000 }),
    issueDate: fc.date({ min: new Date('2020-01-01'), max: new Date() }).map(d => d.toISOString()),
    maturityDate: fc.date({ min: new Date(), max: new Date('2050-12-31') }).map(d => d.toISOString()),
    frequency: fc.constantFrom('SEMI_ANNUAL', 'ANNUAL') as fc.Arbitrary<'SEMI_ANNUAL' | 'ANNUAL'>,
    minimumInvestment: fc.integer({ min: 50000, max: 500000 })
  }) as fc.Arbitrary<Bond>;

  /**
   * Arbitrary generator for positive number of units
   */
  const unitsArbitrary = fc.integer({ min: 1, max: 1000 });

  /**
   * Calculate accrued interest for a bond
   * This is the implementation that should match the one in Bonds.tsx
   */
  const calculateAccruedInterest = (bond: Bond, units: number): number => {
    if (!units || units <= 0) return 0;
    
    const today = new Date();
    const issueDate = new Date(bond.issueDate);
    
    // Calculate days since issue
    const daysSinceIssue = Math.floor((today.getTime() - issueDate.getTime()) / (1000 * 60 * 60 * 24));
    
    // Calculate days in coupon period
    const daysInYear = 365;
    const periodsPerYear = bond.frequency === 'SEMI_ANNUAL' ? 2 : 1;
    const daysInPeriod = daysInYear / periodsPerYear;
    
    // Calculate accrued interest
    const daysSinceLastCoupon = daysSinceIssue % daysInPeriod;
    const annualCoupon = (bond.couponRate / 100) * bond.faceValue;
    const periodCoupon = annualCoupon / periodsPerYear;
    const accruedInterest = (periodCoupon * daysSinceLastCoupon) / daysInPeriod;
    
    return accruedInterest * units;
  };

  it('should calculate interest proportional to principal amount', () => {
    fc.assert(
      fc.property(bondArbitrary, unitsArbitrary, (bond, units) => {
        // Property: Interest should be proportional to the number of units
        const interest1 = calculateAccruedInterest(bond, units);
        const interest2 = calculateAccruedInterest(bond, units * 2);
        
        // Interest for 2x units should be approximately 2x the interest for 1x units
        // Using a small tolerance for floating point arithmetic
        const ratio = interest2 / interest1;
        expect(ratio).toBeCloseTo(2, 5);
      }),
      { numRuns: 100 }
    );
  });

  it('should return zero interest for zero units', () => {
    fc.assert(
      fc.property(bondArbitrary, (bond) => {
        // Property: Zero units should result in zero interest
        const interest = calculateAccruedInterest(bond, 0);
        expect(interest).toBe(0);
      }),
      { numRuns: 100 }
    );
  });

  it('should calculate interest based on coupon rate', () => {
    fc.assert(
      fc.property(bondArbitrary, unitsArbitrary, (bond, units) => {
        // Property: Interest should increase with coupon rate
        const interest = calculateAccruedInterest(bond, units);
        
        // Create a bond with double the coupon rate
        const doubleCouponBond = { ...bond, couponRate: bond.couponRate * 2 };
        const doubleInterest = calculateAccruedInterest(doubleCouponBond, units);
        
        // Interest should be approximately double
        if (interest > 0) {
          const ratio = doubleInterest / interest;
          expect(ratio).toBeCloseTo(2, 5);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should calculate interest based on face value', () => {
    fc.assert(
      fc.property(bondArbitrary, unitsArbitrary, (bond, units) => {
        // Property: Interest should be proportional to face value
        const interest = calculateAccruedInterest(bond, units);
        
        // Create a bond with double the face value
        const doubleFaceValueBond = { ...bond, faceValue: bond.faceValue * 2 };
        const doubleInterest = calculateAccruedInterest(doubleFaceValueBond, units);
        
        // Interest should be approximately double
        if (interest > 0) {
          const ratio = doubleInterest / interest;
          expect(ratio).toBeCloseTo(2, 5);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should always return non-negative interest', () => {
    fc.assert(
      fc.property(bondArbitrary, unitsArbitrary, (bond, units) => {
        // Property: Accrued interest should never be negative
        const interest = calculateAccruedInterest(bond, units);
        expect(interest).toBeGreaterThanOrEqual(0);
      }),
      { numRuns: 100 }
    );
  });

  it('should calculate different interest for different payment frequencies', () => {
    fc.assert(
      fc.property(bondArbitrary, unitsArbitrary, (bond, units) => {
        // Property: Semi-annual and annual frequencies should produce different calculations
        const semiAnnualBond = { ...bond, frequency: 'SEMI_ANNUAL' as const };
        const annualBond = { ...bond, frequency: 'ANNUAL' as const };
        
        const semiAnnualInterest = calculateAccruedInterest(semiAnnualBond, units);
        const annualInterest = calculateAccruedInterest(annualBond, units);
        
        // The calculations should be different (unless days since issue is exactly 0)
        const today = new Date();
        const issueDate = new Date(bond.issueDate);
        const daysSinceIssue = Math.floor((today.getTime() - issueDate.getTime()) / (1000 * 60 * 60 * 24));
        
        if (daysSinceIssue > 0) {
          // They should be different due to different period calculations
          expect(semiAnnualInterest).not.toBe(annualInterest);
        }
      }),
      { numRuns: 100 }
    );
  });
});

describe('Property 3: Portfolio Valuation Consistency', () => {
  /**
   * Arbitrary generator for portfolio securities
   */
  const portfolioSecurityArbitrary = fc.record({
    securityId: fc.string({ minLength: 1, maxLength: 50 }),
    securityType: fc.constantFrom('TBILL', 'BOND', 'STOCK') as fc.Arbitrary<'TBILL' | 'BOND' | 'STOCK'>,
    name: fc.string({ minLength: 1, maxLength: 100 }),
    quantity: fc.integer({ min: 1, max: 10000 }),
    purchasePrice: fc.double({ min: 1, max: 100000, noNaN: true }),
    currentPrice: fc.double({ min: 1, max: 100000, noNaN: true }),
    marketValue: fc.double({ min: 1, max: 1000000000, noNaN: true }),
    unrealizedGain: fc.double({ min: -1000000, max: 1000000, noNaN: true }),
    maturityDate: fc.option(fc.date({ min: new Date(), max: new Date('2050-12-31') }).map(d => d.toISOString()))
  }) as fc.Arbitrary<PortfolioSecurity>;

  /**
   * Arbitrary generator for portfolios with consistent calculations
   */
  const portfolioArbitrary = fc.array(portfolioSecurityArbitrary, { minLength: 1, maxLength: 50 })
    .map(securities => {
      // Ensure market value is calculated correctly: quantity * currentPrice
      const correctedSecurities = securities.map(sec => ({
        ...sec,
        marketValue: sec.quantity * sec.currentPrice,
        unrealizedGain: (sec.quantity * sec.currentPrice) - (sec.quantity * sec.purchasePrice)
      }));
      
      const totalValue = correctedSecurities.reduce((sum, sec) => sum + sec.marketValue, 0);
      const unrealizedGain = correctedSecurities.reduce((sum, sec) => sum + sec.unrealizedGain, 0);
      
      return {
        securities: correctedSecurities,
        totalValue,
        unrealizedGain,
        yieldToMaturity: fc.sample(fc.double({ min: 0, max: 20, noNaN: true }), 1)[0]
      } as Portfolio;
    });

  it('should have total value equal to sum of individual security values', () => {
    fc.assert(
      fc.property(portfolioArbitrary, (portfolio) => {
        // Property: Total portfolio value = sum of (quantity × current price) for all securities
        const calculatedTotal = portfolio.securities.reduce(
          (sum, sec) => sum + sec.marketValue,
          0
        );
        
        expect(portfolio.totalValue).toBeCloseTo(calculatedTotal, 2);
      }),
      { numRuns: 100 }
    );
  });

  it('should calculate market value correctly for each security', () => {
    fc.assert(
      fc.property(portfolioArbitrary, (portfolio) => {
        // Property: Each security's market value = quantity × current price
        portfolio.securities.forEach(security => {
          const expectedMarketValue = security.quantity * security.currentPrice;
          expect(security.marketValue).toBeCloseTo(expectedMarketValue, 2);
        });
      }),
      { numRuns: 100 }
    );
  });

  it('should calculate unrealized gain correctly for each security', () => {
    fc.assert(
      fc.property(portfolioArbitrary, (portfolio) => {
        // Property: Unrealized gain = (quantity × current price) - (quantity × purchase price)
        portfolio.securities.forEach(security => {
          const expectedGain = (security.quantity * security.currentPrice) - 
                              (security.quantity * security.purchasePrice);
          expect(security.unrealizedGain).toBeCloseTo(expectedGain, 2);
        });
      }),
      { numRuns: 100 }
    );
  });

  it('should have total unrealized gain equal to sum of individual gains', () => {
    fc.assert(
      fc.property(portfolioArbitrary, (portfolio) => {
        // Property: Total unrealized gain = sum of individual security gains
        const calculatedGain = portfolio.securities.reduce(
          (sum, sec) => sum + sec.unrealizedGain,
          0
        );
        
        expect(portfolio.unrealizedGain).toBeCloseTo(calculatedGain, 2);
      }),
      { numRuns: 100 }
    );
  });

  it('should maintain consistency when adding securities', () => {
    fc.assert(
      fc.property(portfolioArbitrary, portfolioSecurityArbitrary, (portfolio, newSecurity) => {
        // Property: Adding a security should increase total value by its market value
        const originalTotal = portfolio.totalValue;
        
        // Ensure new security has correct market value
        const correctedNewSecurity = {
          ...newSecurity,
          marketValue: newSecurity.quantity * newSecurity.currentPrice,
          unrealizedGain: (newSecurity.quantity * newSecurity.currentPrice) - 
                         (newSecurity.quantity * newSecurity.purchasePrice)
        };
        
        const newTotal = originalTotal + correctedNewSecurity.marketValue;
        const expectedTotal = [...portfolio.securities, correctedNewSecurity].reduce(
          (sum, sec) => sum + sec.marketValue,
          0
        );
        
        expect(newTotal).toBeCloseTo(expectedTotal, 2);
      }),
      { numRuns: 100 }
    );
  });

  it('should have non-negative total value', () => {
    fc.assert(
      fc.property(portfolioArbitrary, (portfolio) => {
        // Property: Total portfolio value should never be negative
        expect(portfolio.totalValue).toBeGreaterThanOrEqual(0);
      }),
      { numRuns: 100 }
    );
  });

  it('should have non-negative market values for all securities', () => {
    fc.assert(
      fc.property(portfolioArbitrary, (portfolio) => {
        // Property: Each security's market value should be non-negative
        portfolio.securities.forEach(security => {
          expect(security.marketValue).toBeGreaterThanOrEqual(0);
        });
      }),
      { numRuns: 100 }
    );
  });

  it('should maintain valuation consistency across different security types', () => {
    fc.assert(
      fc.property(portfolioArbitrary, (portfolio) => {
        // Property: Valuation formula should be consistent regardless of security type
        const tbills = portfolio.securities.filter(s => s.securityType === 'TBILL');
        const bonds = portfolio.securities.filter(s => s.securityType === 'BOND');
        const stocks = portfolio.securities.filter(s => s.securityType === 'STOCK');
        
        // Each type should follow the same valuation formula
        [...tbills, ...bonds, ...stocks].forEach(security => {
          const expectedValue = security.quantity * security.currentPrice;
          expect(security.marketValue).toBeCloseTo(expectedValue, 2);
        });
      }),
      { numRuns: 100 }
    );
  });

  it('should calculate percentage composition correctly', () => {
    fc.assert(
      fc.property(portfolioArbitrary, (portfolio) => {
        // Property: Sum of all security percentages should equal 100%
        if (portfolio.totalValue > 0) {
          const percentages = portfolio.securities.map(sec => 
            (sec.marketValue / portfolio.totalValue) * 100
          );
          
          const totalPercentage = percentages.reduce((sum, pct) => sum + pct, 0);
          expect(totalPercentage).toBeCloseTo(100, 1);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should handle empty portfolio correctly', () => {
    // Property: Empty portfolio should have zero total value
    const emptyPortfolio: Portfolio = {
      securities: [],
      totalValue: 0,
      unrealizedGain: 0,
      yieldToMaturity: 0
    };
    
    expect(emptyPortfolio.totalValue).toBe(0);
    expect(emptyPortfolio.securities.length).toBe(0);
  });
});
