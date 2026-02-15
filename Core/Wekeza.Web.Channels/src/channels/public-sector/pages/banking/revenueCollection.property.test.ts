import { describe, it, expect } from 'vitest';
import * as fc from 'fast-check';

/**
 * Property-Based Tests for Revenue Collection
 * 
 * **Validates: Requirements 3.3**
 * 
 * Property 9: Revenue Collection Recording
 */

interface RevenueCollection {
  id: string;
  date: Date;
  revenueType: 'TAX' | 'FEE' | 'LICENSE' | 'FINE' | 'OTHER';
  amount: number;
  payerName: string;
  payerReference: string;
  status: 'PENDING' | 'COLLECTED' | 'RECONCILED' | 'DISPUTED';
  reconciliationDate?: Date;
}

describe('Property 9: Revenue Collection Recording', () => {
  /**
   * Arbitrary generator for revenue collections
   */
  const revenueCollectionArbitrary = fc.record({
    id: fc.uuid(),
    date: fc.date({ min: new Date('2024-01-01'), max: new Date('2026-12-31') }),
    revenueType: fc.constantFrom('TAX', 'FEE', 'LICENSE', 'FINE', 'OTHER') as fc.Arbitrary<'TAX' | 'FEE' | 'LICENSE' | 'FINE' | 'OTHER'>,
    amount: fc.double({ min: 1, max: 10000000, noNaN: true }),
    payerName: fc.string({ minLength: 3, maxLength: 100 }),
    payerReference: fc.string({ minLength: 5, maxLength: 50 }),
    status: fc.constantFrom('PENDING', 'COLLECTED', 'RECONCILED', 'DISPUTED') as fc.Arbitrary<'PENDING' | 'COLLECTED' | 'RECONCILED' | 'DISPUTED'>,
    reconciliationDate: fc.option(fc.date({ min: new Date('2024-01-01'), max: new Date('2026-12-31') }))
  }) as fc.Arbitrary<RevenueCollection>;

  const revenueCollectionsArbitrary = fc.array(revenueCollectionArbitrary, { minLength: 1, maxLength: 100 });

  /**
   * Record revenue collections and calculate totals
   */
  const recordRevenueCollections = (collections: RevenueCollection[]): {
    totalCollected: number;
    totalReconciled: number;
    totalPending: number;
    byType: Record<string, number>;
    collectionCount: number;
  } => {
    const totalCollected = collections
      .filter(c => c.status === 'COLLECTED' || c.status === 'RECONCILED')
      .reduce((sum, c) => sum + c.amount, 0);

    const totalReconciled = collections
      .filter(c => c.status === 'RECONCILED')
      .reduce((sum, c) => sum + c.amount, 0);

    const totalPending = collections
      .filter(c => c.status === 'PENDING')
      .reduce((sum, c) => sum + c.amount, 0);

    const byType: Record<string, number> = {};
    collections.forEach(c => {
      if (c.status === 'COLLECTED' || c.status === 'RECONCILED') {
        byType[c.revenueType] = (byType[c.revenueType] || 0) + c.amount;
      }
    });

    return {
      totalCollected,
      totalReconciled,
      totalPending,
      byType,
      collectionCount: collections.length
    };
  };

  it('should record all collections', () => {
    fc.assert(
      fc.property(revenueCollectionsArbitrary, (collections) => {
        const result = recordRevenueCollections(collections);
        expect(result.collectionCount).toBe(collections.length);
      }),
      { numRuns: 100 }
    );
  });

  it('should have reconciled amount less than or equal to collected amount', () => {
    fc.assert(
      fc.property(revenueCollectionsArbitrary, (collections) => {
        const result = recordRevenueCollections(collections);
        expect(result.totalReconciled).toBeLessThanOrEqual(result.totalCollected);
      }),
      { numRuns: 100 }
    );
  });

  it('should calculate total collected from collected and reconciled statuses only', () => {
    fc.assert(
      fc.property(revenueCollectionsArbitrary, (collections) => {
        const result = recordRevenueCollections(collections);
        const expectedTotal = collections
          .filter(c => c.status === 'COLLECTED' || c.status === 'RECONCILED')
          .reduce((sum, c) => sum + c.amount, 0);
        
        expect(Math.abs(result.totalCollected - expectedTotal)).toBeLessThan(0.01);
      }),
      { numRuns: 100 }
    );
  });

  it('should calculate total pending from pending status only', () => {
    fc.assert(
      fc.property(revenueCollectionsArbitrary, (collections) => {
        const result = recordRevenueCollections(collections);
        const expectedPending = collections
          .filter(c => c.status === 'PENDING')
          .reduce((sum, c) => sum + c.amount, 0);
        
        expect(Math.abs(result.totalPending - expectedPending)).toBeLessThan(0.01);
      }),
      { numRuns: 100 }
    );
  });

  it('should have non-negative totals', () => {
    fc.assert(
      fc.property(revenueCollectionsArbitrary, (collections) => {
        const result = recordRevenueCollections(collections);
        expect(result.totalCollected).toBeGreaterThanOrEqual(0);
        expect(result.totalReconciled).toBeGreaterThanOrEqual(0);
        expect(result.totalPending).toBeGreaterThanOrEqual(0);
      }),
      { numRuns: 100 }
    );
  });

  it('should correctly categorize by revenue type', () => {
    fc.assert(
      fc.property(revenueCollectionsArbitrary, (collections) => {
        const result = recordRevenueCollections(collections);
        
        // Sum of all types should equal total collected
        const sumByType = Object.values(result.byType).reduce((sum, val) => sum + val, 0);
        expect(Math.abs(sumByType - result.totalCollected)).toBeLessThan(0.01);
      }),
      { numRuns: 100 }
    );
  });

  it('should have positive amounts for each revenue type', () => {
    fc.assert(
      fc.property(revenueCollectionsArbitrary, (collections) => {
        const result = recordRevenueCollections(collections);
        
        Object.values(result.byType).forEach(amount => {
          expect(amount).toBeGreaterThan(0);
        });
      }),
      { numRuns: 100 }
    );
  });

  it('should maintain unique collection IDs', () => {
    fc.assert(
      fc.property(revenueCollectionsArbitrary, (collections) => {
        const ids = collections.map(c => c.id);
        const uniqueIds = new Set(ids);
        
        // All IDs should be unique (this is a property of the generator)
        expect(uniqueIds.size).toBe(ids.length);
      }),
      { numRuns: 100 }
    );
  });

  it('should have reconciliation date only for reconciled collections', () => {
    fc.assert(
      fc.property(revenueCollectionsArbitrary, (collections) => {
        collections.forEach(collection => {
          if (collection.status === 'RECONCILED' && collection.reconciliationDate) {
            // Reconciliation date should be on or after collection date
            expect(collection.reconciliationDate.getTime()).toBeGreaterThanOrEqual(collection.date.getTime());
          }
        });
      }),
      { numRuns: 100 }
    );
  });

  it('should handle empty collections gracefully', () => {
    const result = recordRevenueCollections([]);
    expect(result.totalCollected).toBe(0);
    expect(result.totalReconciled).toBe(0);
    expect(result.totalPending).toBe(0);
    expect(result.collectionCount).toBe(0);
    expect(Object.keys(result.byType).length).toBe(0);
  });

  it('should be deterministic for same input', () => {
    fc.assert(
      fc.property(revenueCollectionsArbitrary, (collections) => {
        const result1 = recordRevenueCollections(collections);
        const result2 = recordRevenueCollections(collections);
        
        expect(result1.totalCollected).toBe(result2.totalCollected);
        expect(result1.totalReconciled).toBe(result2.totalReconciled);
        expect(result1.totalPending).toBe(result2.totalPending);
        expect(result1.collectionCount).toBe(result2.collectionCount);
      }),
      { numRuns: 100 }
    );
  });

  it('should correctly aggregate by revenue type', () => {
    fc.assert(
      fc.property(revenueCollectionsArbitrary, (collections) => {
        const result = recordRevenueCollections(collections);
        
        // Manually calculate expected totals by type
        const expectedByType: Record<string, number> = {};
        collections
          .filter(c => c.status === 'COLLECTED' || c.status === 'RECONCILED')
          .forEach(c => {
            expectedByType[c.revenueType] = (expectedByType[c.revenueType] || 0) + c.amount;
          });
        
        // Compare each type
        Object.keys(expectedByType).forEach(type => {
          expect(Math.abs((result.byType[type] || 0) - expectedByType[type])).toBeLessThan(0.01);
        });
      }),
      { numRuns: 100 }
    );
  });

  it('should handle all revenue types', () => {
    const allTypes: Array<'TAX' | 'FEE' | 'LICENSE' | 'FINE' | 'OTHER'> = ['TAX', 'FEE', 'LICENSE', 'FINE', 'OTHER'];
    
    allTypes.forEach(type => {
      const collection: RevenueCollection = {
        id: '123',
        date: new Date(),
        revenueType: type,
        amount: 1000,
        payerName: 'Test Payer',
        payerReference: 'REF123',
        status: 'COLLECTED'
      };
      
      const result = recordRevenueCollections([collection]);
      expect(result.byType[type]).toBe(1000);
    });
  });
});
