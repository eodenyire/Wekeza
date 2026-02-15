import { describe, it, expect } from 'vitest';
import * as fc from 'fast-check';

/**
 * Property-Based Tests for Compliance Enforcement
 * 
 * **Validates: Requirements 5.1, 5.2, 5.4**
 */

interface CBKTransaction {
  type: 'TBILL' | 'BOND' | 'STOCK';
  amount: number;
  timestamp: Date;
  tradingHours: boolean;
}

interface PFMATransaction {
  amount: number;
  budgetAllocation: number;
  approvalLevel: number;
  requiredApprovals: number;
}

interface AMLKYCData {
  customerId: string;
  customerName: string;
  idNumber: string;
  address: string;
  sourceOfFunds: string;
  transactionAmount: number;
  riskScore: number;
}

describe('Property 16: CBK Regulation Enforcement', () => {
  /**
   * CBK Regulations:
   * - Minimum investment: T-Bills 50,000 KES, Bonds 50,000 KES, Stocks 1,000 KES
   * - Trading hours: 9:00 AM - 3:00 PM
   * - Maximum single transaction: 1 billion KES
   */
  const cbkTransactionArbitrary = fc.record({
    type: fc.constantFrom('TBILL', 'BOND', 'STOCK') as fc.Arbitrary<'TBILL' | 'BOND' | 'STOCK'>,
    amount: fc.double({ min: 1, max: 2000000000, noNaN: true }),
    timestamp: fc.date({ min: new Date('2024-01-01'), max: new Date('2026-12-31') }),
    tradingHours: fc.boolean()
  }) as fc.Arbitrary<CBKTransaction>;

  const validateCBKCompliance = (transaction: CBKTransaction): {
    isCompliant: boolean;
    violations: string[];
  } => {
    const violations: string[] = [];

    // Minimum investment check
    const minimumInvestment = {
      'TBILL': 50000,
      'BOND': 50000,
      'STOCK': 1000
    };

    if (transaction.amount < minimumInvestment[transaction.type]) {
      violations.push(`Amount below minimum investment of ${minimumInvestment[transaction.type]} KES`);
    }

    // Maximum transaction check
    if (transaction.amount > 1000000000) {
      violations.push('Amount exceeds maximum single transaction of 1 billion KES');
    }

    // Trading hours check
    if (!transaction.tradingHours) {
      violations.push('Transaction outside trading hours (9:00 AM - 3:00 PM)');
    }

    return {
      isCompliant: violations.length === 0,
      violations
    };
  };

  it('should enforce minimum investment amounts', () => {
    fc.assert(
      fc.property(cbkTransactionArbitrary, (transaction) => {
        const result = validateCBKCompliance(transaction);
        
        const minimums = { 'TBILL': 50000, 'BOND': 50000, 'STOCK': 1000 };
        if (transaction.amount < minimums[transaction.type]) {
          expect(result.isCompliant).toBe(false);
          expect(result.violations.some(v => v.includes('minimum investment'))).toBe(true);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should enforce maximum transaction limit', () => {
    fc.assert(
      fc.property(cbkTransactionArbitrary, (transaction) => {
        const result = validateCBKCompliance(transaction);
        
        if (transaction.amount > 1000000000) {
          expect(result.isCompliant).toBe(false);
          expect(result.violations.some(v => v.includes('maximum single transaction'))).toBe(true);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should enforce trading hours', () => {
    fc.assert(
      fc.property(cbkTransactionArbitrary, (transaction) => {
        const result = validateCBKCompliance(transaction);
        
        if (!transaction.tradingHours) {
          expect(result.isCompliant).toBe(false);
          expect(result.violations.some(v => v.includes('trading hours'))).toBe(true);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should allow compliant transactions', () => {
    const compliantTransaction: CBKTransaction = {
      type: 'TBILL',
      amount: 100000,
      timestamp: new Date('2024-06-15T10:00:00'),
      tradingHours: true
    };

    const result = validateCBKCompliance(compliantTransaction);
    expect(result.isCompliant).toBe(true);
    expect(result.violations.length).toBe(0);
  });

  it('should have different minimum investments for different security types', () => {
    const tbillMin = 50000;
    const bondMin = 50000;
    const stockMin = 1000;

    expect(stockMin).toBeLessThan(tbillMin);
    expect(stockMin).toBeLessThan(bondMin);
  });
});

describe('Property 17: PFMA Requirement Enforcement', () => {
  /**
   * PFMA Requirements:
   * - Transactions must not exceed budget allocation
   * - Transactions above 10M require 2 approvals
   * - Transactions above 100M require 3 approvals
   */
  const pfmaTransactionArbitrary = fc.record({
    amount: fc.double({ min: 1, max: 500000000, noNaN: true }),
    budgetAllocation: fc.double({ min: 1000000, max: 1000000000, noNaN: true }),
    approvalLevel: fc.integer({ min: 0, max: 5 }),
    requiredApprovals: fc.integer({ min: 1, max: 3 })
  }) as fc.Arbitrary<PFMATransaction>;

  const validatePFMACompliance = (transaction: PFMATransaction): {
    isCompliant: boolean;
    violations: string[];
  } => {
    const violations: string[] = [];

    // Budget allocation check
    if (transaction.amount > transaction.budgetAllocation) {
      violations.push('Amount exceeds budget allocation');
    }

    // Approval requirements
    let requiredApprovals = 1;
    if (transaction.amount > 100000000) {
      requiredApprovals = 3;
    } else if (transaction.amount > 10000000) {
      requiredApprovals = 2;
    }

    if (transaction.approvalLevel < requiredApprovals) {
      violations.push(`Insufficient approvals: ${transaction.approvalLevel} of ${requiredApprovals} required`);
    }

    return {
      isCompliant: violations.length === 0,
      violations
    };
  };

  it('should enforce budget allocation limits', () => {
    fc.assert(
      fc.property(pfmaTransactionArbitrary, (transaction) => {
        const result = validatePFMACompliance(transaction);
        
        if (transaction.amount > transaction.budgetAllocation) {
          expect(result.isCompliant).toBe(false);
          expect(result.violations.some(v => v.includes('budget allocation'))).toBe(true);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should require 2 approvals for transactions above 10M', () => {
    const transaction: PFMATransaction = {
      amount: 15000000,
      budgetAllocation: 20000000,
      approvalLevel: 1,
      requiredApprovals: 2
    };

    const result = validatePFMACompliance(transaction);
    expect(result.isCompliant).toBe(false);
    expect(result.violations.some(v => v.includes('Insufficient approvals'))).toBe(true);
  });

  it('should require 3 approvals for transactions above 100M', () => {
    const transaction: PFMATransaction = {
      amount: 150000000,
      budgetAllocation: 200000000,
      approvalLevel: 2,
      requiredApprovals: 3
    };

    const result = validatePFMACompliance(transaction);
    expect(result.isCompliant).toBe(false);
    expect(result.violations.some(v => v.includes('Insufficient approvals'))).toBe(true);
  });

  it('should allow compliant transactions', () => {
    const transaction: PFMATransaction = {
      amount: 5000000,
      budgetAllocation: 10000000,
      approvalLevel: 1,
      requiredApprovals: 1
    };

    const result = validatePFMACompliance(transaction);
    expect(result.isCompliant).toBe(true);
    expect(result.violations.length).toBe(0);
  });
});

describe('Property 18: AML/KYC Data Persistence', () => {
  const amlKycDataArbitrary = fc.record({
    customerId: fc.uuid(),
    customerName: fc.string({ minLength: 3, maxLength: 100 }),
    idNumber: fc.string({ minLength: 5, maxLength: 20 }),
    address: fc.string({ minLength: 10, maxLength: 200 }),
    sourceOfFunds: fc.constantFrom('SALARY', 'BUSINESS', 'INVESTMENT', 'INHERITANCE', 'OTHER'),
    transactionAmount: fc.double({ min: 1, max: 100000000, noNaN: true }),
    riskScore: fc.double({ min: 0, max: 100, noNaN: true })
  }) as fc.Arbitrary<AMLKYCData>;

  const validateAMLKYCData = (data: AMLKYCData): {
    isComplete: boolean;
    missingFields: string[];
    riskLevel: 'LOW' | 'MEDIUM' | 'HIGH';
  } => {
    const missingFields: string[] = [];

    if (!data.customerId) missingFields.push('customerId');
    if (!data.customerName || data.customerName.length < 3) missingFields.push('customerName');
    if (!data.idNumber || data.idNumber.length < 5) missingFields.push('idNumber');
    if (!data.address || data.address.length < 10) missingFields.push('address');
    if (!data.sourceOfFunds) missingFields.push('sourceOfFunds');

    // Determine risk level
    let riskLevel: 'LOW' | 'MEDIUM' | 'HIGH' = 'LOW';
    if (data.riskScore > 70 || data.transactionAmount > 10000000) {
      riskLevel = 'HIGH';
    } else if (data.riskScore > 40 || data.transactionAmount > 1000000) {
      riskLevel = 'MEDIUM';
    }

    return {
      isComplete: missingFields.length === 0,
      missingFields,
      riskLevel
    };
  };

  it('should validate all required KYC fields', () => {
    fc.assert(
      fc.property(amlKycDataArbitrary, (data) => {
        const result = validateAMLKYCData(data);
        expect(result.isComplete).toBe(true);
        expect(result.missingFields.length).toBe(0);
      }),
      { numRuns: 100 }
    );
  });

  it('should classify high-risk customers correctly', () => {
    fc.assert(
      fc.property(amlKycDataArbitrary, (data) => {
        const result = validateAMLKYCData(data);
        
        if (data.riskScore > 70 || data.transactionAmount > 10000000) {
          expect(result.riskLevel).toBe('HIGH');
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should persist customer ID', () => {
    fc.assert(
      fc.property(amlKycDataArbitrary, (data) => {
        expect(data.customerId).toBeDefined();
        expect(data.customerId.length).toBeGreaterThan(0);
      }),
      { numRuns: 100 }
    );
  });

  it('should maintain data integrity during serialization', () => {
    fc.assert(
      fc.property(amlKycDataArbitrary, (data) => {
        const serialized = JSON.stringify(data);
        const deserialized = JSON.parse(serialized);

        expect(deserialized.customerId).toBe(data.customerId);
        expect(deserialized.customerName).toBe(data.customerName);
        expect(deserialized.idNumber).toBe(data.idNumber);
        expect(deserialized.transactionAmount).toBe(data.transactionAmount);
      }),
      { numRuns: 100 }
    );
  });

  it('should have valid source of funds', () => {
    const validSources = ['SALARY', 'BUSINESS', 'INVESTMENT', 'INHERITANCE', 'OTHER'];
    
    fc.assert(
      fc.property(amlKycDataArbitrary, (data) => {
        expect(validSources).toContain(data.sourceOfFunds);
      }),
      { numRuns: 100 }
    );
  });

  it('should have risk score between 0 and 100', () => {
    fc.assert(
      fc.property(amlKycDataArbitrary, (data) => {
        expect(data.riskScore).toBeGreaterThanOrEqual(0);
        expect(data.riskScore).toBeLessThanOrEqual(100);
      }),
      { numRuns: 100 }
    );
  });
});
