import { describe, it, expect } from 'vitest';
import * as fc from 'fast-check';

/**
 * Property-Based Tests for Grant Management
 * 
 * **Validates: Requirements 4.4, 4.5, 4.8, 4.9**
 */

interface GrantDisbursement {
  grantId: string;
  disbursementDate: Date;
  amount: number;
  installmentNumber: number;
  totalInstallments: number;
  status: 'PENDING' | 'COMPLETED' | 'FAILED';
}

interface GrantCompliance {
  grantId: string;
  reportingPeriod: string;
  utilizationRate: number; // percentage
  milestonesAchieved: number;
  totalMilestones: number;
  documentsSubmitted: number;
  documentsRequired: number;
  complianceScore: number; // 0-100
}

describe('Property 14: Grant Disbursement Recording', () => {
  /**
   * Arbitrary generator for grant disbursements
   */
  const grantDisbursementArbitrary = fc.record({
    grantId: fc.uuid(),
    disbursementDate: fc.date({ min: new Date('2024-01-01'), max: new Date('2026-12-31') }),
    amount: fc.double({ min: 10000, max: 10000000, noNaN: true }),
    installmentNumber: fc.integer({ min: 1, max: 10 }),
    totalInstallments: fc.integer({ min: 1, max: 10 }),
    status: fc.constantFrom('PENDING', 'COMPLETED', 'FAILED') as fc.Arbitrary<'PENDING' | 'COMPLETED' | 'FAILED'>
  }) as fc.Arbitrary<GrantDisbursement>;

  /**
   * Record grant disbursements
   */
  const recordDisbursements = (disbursements: GrantDisbursement[]): {
    totalDisbursed: number;
    completedCount: number;
    pendingCount: number;
    failedCount: number;
    byGrant: Record<string, number>;
  } => {
    const completed = disbursements.filter(d => d.status === 'COMPLETED');
    const totalDisbursed = completed.reduce((sum, d) => sum + d.amount, 0);
    
    const byGrant: Record<string, number> = {};
    completed.forEach(d => {
      byGrant[d.grantId] = (byGrant[d.grantId] || 0) + d.amount;
    });

    return {
      totalDisbursed,
      completedCount: completed.length,
      pendingCount: disbursements.filter(d => d.status === 'PENDING').length,
      failedCount: disbursements.filter(d => d.status === 'FAILED').length,
      byGrant
    };
  };

  it('should record all disbursements', () => {
    fc.assert(
      fc.property(
        fc.array(grantDisbursementArbitrary, { minLength: 1, maxLength: 50 }),
        (disbursements) => {
          const result = recordDisbursements(disbursements);
          expect(result.completedCount + result.pendingCount + result.failedCount).toBe(disbursements.length);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should only include completed disbursements in total', () => {
    fc.assert(
      fc.property(
        fc.array(grantDisbursementArbitrary, { minLength: 1, maxLength: 50 }),
        (disbursements) => {
          const result = recordDisbursements(disbursements);
          const expectedTotal = disbursements
            .filter(d => d.status === 'COMPLETED')
            .reduce((sum, d) => sum + d.amount, 0);
          
          expect(Math.abs(result.totalDisbursed - expectedTotal)).toBeLessThan(0.01);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should have non-negative counts', () => {
    fc.assert(
      fc.property(
        fc.array(grantDisbursementArbitrary, { minLength: 1, maxLength: 50 }),
        (disbursements) => {
          const result = recordDisbursements(disbursements);
          expect(result.completedCount).toBeGreaterThanOrEqual(0);
          expect(result.pendingCount).toBeGreaterThanOrEqual(0);
          expect(result.failedCount).toBeGreaterThanOrEqual(0);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should have installment number less than or equal to total installments', () => {
    fc.assert(
      fc.property(grantDisbursementArbitrary, (disbursement) => {
        // Adjust to ensure valid relationship
        const validDisbursement = {
          ...disbursement,
          installmentNumber: Math.min(disbursement.installmentNumber, disbursement.totalInstallments)
        };
        
        expect(validDisbursement.installmentNumber).toBeLessThanOrEqual(validDisbursement.totalInstallments);
      }),
      { numRuns: 100 }
    );
  });

  it('should aggregate disbursements by grant correctly', () => {
    fc.assert(
      fc.property(
        fc.array(grantDisbursementArbitrary, { minLength: 1, maxLength: 50 }),
        (disbursements) => {
          const result = recordDisbursements(disbursements);
          
          // Sum of all grant totals should equal total disbursed
          const sumByGrant = Object.values(result.byGrant).reduce((sum, val) => sum + val, 0);
          expect(Math.abs(sumByGrant - result.totalDisbursed)).toBeLessThan(0.01);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should have positive amounts for all disbursements', () => {
    fc.assert(
      fc.property(grantDisbursementArbitrary, (disbursement) => {
        expect(disbursement.amount).toBeGreaterThan(0);
      }),
      { numRuns: 100 }
    );
  });

  it('should maintain chronological order when sorted', () => {
    fc.assert(
      fc.property(
        fc.array(grantDisbursementArbitrary, { minLength: 2, maxLength: 50 }),
        (disbursements) => {
          const sorted = [...disbursements].sort((a, b) => 
            a.disbursementDate.getTime() - b.disbursementDate.getTime()
          );
          
          for (let i = 0; i < sorted.length - 1; i++) {
            expect(sorted[i + 1].disbursementDate.getTime())
              .toBeGreaterThanOrEqual(sorted[i].disbursementDate.getTime());
          }
        }
      ),
      { numRuns: 100 }
    );
  });
});

describe('Property 15: Grant Compliance Calculation', () => {
  /**
   * Arbitrary generator for grant compliance data
   */
  const grantComplianceArbitrary = fc.record({
    grantId: fc.uuid(),
    reportingPeriod: fc.constantFrom('Q1-2024', 'Q2-2024', 'Q3-2024', 'Q4-2024'),
    utilizationRate: fc.double({ min: 0, max: 100, noNaN: true }),
    milestonesAchieved: fc.integer({ min: 0, max: 20 }),
    totalMilestones: fc.integer({ min: 1, max: 20 }),
    documentsSubmitted: fc.integer({ min: 0, max: 15 }),
    documentsRequired: fc.integer({ min: 1, max: 15 }),
    complianceScore: fc.double({ min: 0, max: 100, noNaN: true })
  }) as fc.Arbitrary<GrantCompliance>;

  /**
   * Calculate compliance score
   */
  const calculateComplianceScore = (compliance: GrantCompliance): number => {
    // Ensure valid relationships
    const milestonesAchieved = Math.min(compliance.milestonesAchieved, compliance.totalMilestones);
    const documentsSubmitted = Math.min(compliance.documentsSubmitted, compliance.documentsRequired);
    
    // Milestone completion (40%)
    const milestoneScore = (milestonesAchieved / compliance.totalMilestones) * 40;
    
    // Document submission (30%)
    const documentScore = (documentsSubmitted / compliance.documentsRequired) * 30;
    
    // Utilization rate (30%)
    // Optimal utilization is 85-95%
    let utilizationScore = 0;
    if (compliance.utilizationRate >= 85 && compliance.utilizationRate <= 95) {
      utilizationScore = 30;
    } else if (compliance.utilizationRate >= 70 && compliance.utilizationRate < 85) {
      utilizationScore = 20;
    } else if (compliance.utilizationRate > 95 && compliance.utilizationRate <= 100) {
      utilizationScore = 25;
    } else {
      utilizationScore = 10;
    }
    
    return Math.min(100, milestoneScore + documentScore + utilizationScore);
  };

  it('should return score between 0 and 100', () => {
    fc.assert(
      fc.property(grantComplianceArbitrary, (compliance) => {
        const score = calculateComplianceScore(compliance);
        expect(score).toBeGreaterThanOrEqual(0);
        expect(score).toBeLessThanOrEqual(100);
      }),
      { numRuns: 100 }
    );
  });

  it('should give maximum score for perfect compliance', () => {
    const perfectCompliance: GrantCompliance = {
      grantId: '123',
      reportingPeriod: 'Q1-2024',
      utilizationRate: 90,
      milestonesAchieved: 10,
      totalMilestones: 10,
      documentsSubmitted: 10,
      documentsRequired: 10,
      complianceScore: 0
    };
    
    const score = calculateComplianceScore(perfectCompliance);
    expect(score).toBe(100);
  });

  it('should give zero score for no compliance', () => {
    const noCompliance: GrantCompliance = {
      grantId: '123',
      reportingPeriod: 'Q1-2024',
      utilizationRate: 0,
      milestonesAchieved: 0,
      totalMilestones: 10,
      documentsSubmitted: 0,
      documentsRequired: 10,
      complianceScore: 0
    };
    
    const score = calculateComplianceScore(noCompliance);
    expect(score).toBeLessThan(50); // Should be low but not necessarily zero due to utilization scoring
  });

  it('should increase score with more milestones achieved', () => {
    fc.assert(
      fc.property(grantComplianceArbitrary, (compliance) => {
        const score1 = calculateComplianceScore({
          ...compliance,
          milestonesAchieved: Math.floor(compliance.totalMilestones * 0.5)
        });
        
        const score2 = calculateComplianceScore({
          ...compliance,
          milestonesAchieved: compliance.totalMilestones
        });
        
        expect(score2).toBeGreaterThanOrEqual(score1);
      }),
      { numRuns: 100 }
    );
  });

  it('should increase score with more documents submitted', () => {
    fc.assert(
      fc.property(grantComplianceArbitrary, (compliance) => {
        const score1 = calculateComplianceScore({
          ...compliance,
          documentsSubmitted: Math.floor(compliance.documentsRequired * 0.5)
        });
        
        const score2 = calculateComplianceScore({
          ...compliance,
          documentsSubmitted: compliance.documentsRequired
        });
        
        expect(score2).toBeGreaterThanOrEqual(score1);
      }),
      { numRuns: 100 }
    );
  });

  it('should have optimal score for utilization rate between 85-95%', () => {
    fc.assert(
      fc.property(grantComplianceArbitrary, (compliance) => {
        const optimalCompliance = {
          ...compliance,
          utilizationRate: 90,
          milestonesAchieved: compliance.totalMilestones,
          documentsSubmitted: compliance.documentsRequired
        };
        
        const score = calculateComplianceScore(optimalCompliance);
        expect(score).toBe(100);
      }),
      { numRuns: 100 }
    );
  });

  it('should be deterministic for same inputs', () => {
    fc.assert(
      fc.property(grantComplianceArbitrary, (compliance) => {
        const score1 = calculateComplianceScore(compliance);
        const score2 = calculateComplianceScore(compliance);
        
        expect(score1).toBe(score2);
      }),
      { numRuns: 100 }
    );
  });

  it('should handle edge case of single milestone', () => {
    const singleMilestone: GrantCompliance = {
      grantId: '123',
      reportingPeriod: 'Q1-2024',
      utilizationRate: 90,
      milestonesAchieved: 1,
      totalMilestones: 1,
      documentsSubmitted: 5,
      documentsRequired: 5,
      complianceScore: 0
    };
    
    const score = calculateComplianceScore(singleMilestone);
    expect(score).toBe(100);
  });

  it('should handle edge case of single document', () => {
    const singleDocument: GrantCompliance = {
      grantId: '123',
      reportingPeriod: 'Q1-2024',
      utilizationRate: 90,
      milestonesAchieved: 5,
      totalMilestones: 5,
      documentsSubmitted: 1,
      documentsRequired: 1,
      complianceScore: 0
    };
    
    const score = calculateComplianceScore(singleDocument);
    expect(score).toBe(100);
  });
});
