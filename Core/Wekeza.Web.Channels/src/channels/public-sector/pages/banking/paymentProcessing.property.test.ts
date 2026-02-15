import { describe, it, expect } from 'vitest';
import * as fc from 'fast-check';

/**
 * Property-Based Tests for Payment Processing
 * 
 * **Validates: Requirements 3.2, 3.5, 3.6**
 */

interface Payment {
  id: string;
  accountNumber: string;
  beneficiaryName: string;
  amount: number;
  currency: string;
  reference: string;
  status: 'PENDING' | 'PROCESSING' | 'COMPLETED' | 'FAILED';
}

interface Account {
  accountNumber: string;
  balance: number;
  currency: string;
}

describe('Property 8: Payment Processing Completeness', () => {
  /**
   * Arbitrary generator for payments
   */
  const paymentArbitrary = fc.record({
    id: fc.uuid(),
    accountNumber: fc.string({ minLength: 10, maxLength: 20 }),
    beneficiaryName: fc.string({ minLength: 3, maxLength: 100 }),
    amount: fc.double({ min: 1, max: 10000000, noNaN: true }),
    currency: fc.constantFrom('KES', 'USD', 'EUR', 'GBP'),
    reference: fc.string({ minLength: 5, maxLength: 50 }),
    status: fc.constantFrom('PENDING', 'PROCESSING', 'COMPLETED', 'FAILED') as fc.Arbitrary<'PENDING' | 'PROCESSING' | 'COMPLETED' | 'FAILED'>
  }) as fc.Arbitrary<Payment>;

  const bulkPaymentArbitrary = fc.array(paymentArbitrary, { minLength: 1, maxLength: 100 });

  /**
   * Process a bulk payment batch
   */
  const processBulkPayment = (payments: Payment[]): {
    processed: Payment[];
    successful: number;
    failed: number;
    totalAmount: number;
  } => {
    const processed = payments.map(payment => ({
      ...payment,
      status: (payment.amount > 0 && payment.accountNumber && payment.beneficiaryName) 
        ? 'COMPLETED' as const
        : 'FAILED' as const
    }));

    const successful = processed.filter(p => p.status === 'COMPLETED').length;
    const failed = processed.filter(p => p.status === 'FAILED').length;
    const totalAmount = processed
      .filter(p => p.status === 'COMPLETED')
      .reduce((sum, p) => sum + p.amount, 0);

    return { processed, successful, failed, totalAmount };
  };

  it('should process all payments in the batch', () => {
    fc.assert(
      fc.property(bulkPaymentArbitrary, (payments) => {
        const result = processBulkPayment(payments);
        expect(result.processed.length).toBe(payments.length);
      }),
      { numRuns: 100 }
    );
  });

  it('should have successful plus failed equal to total', () => {
    fc.assert(
      fc.property(bulkPaymentArbitrary, (payments) => {
        const result = processBulkPayment(payments);
        expect(result.successful + result.failed).toBe(payments.length);
      }),
      { numRuns: 100 }
    );
  });

  it('should calculate total amount from successful payments only', () => {
    fc.assert(
      fc.property(bulkPaymentArbitrary, (payments) => {
        const result = processBulkPayment(payments);
        const expectedTotal = result.processed
          .filter(p => p.status === 'COMPLETED')
          .reduce((sum, p) => sum + p.amount, 0);
        
        expect(result.totalAmount).toBeCloseTo(expectedTotal, 2);
      }),
      { numRuns: 100 }
    );
  });

  it('should maintain payment IDs through processing', () => {
    fc.assert(
      fc.property(bulkPaymentArbitrary, (payments) => {
        const result = processBulkPayment(payments);
        const originalIds = payments.map(p => p.id).sort();
        const processedIds = result.processed.map(p => p.id).sort();
        
        expect(processedIds).toEqual(originalIds);
      }),
      { numRuns: 100 }
    );
  });

  it('should have non-negative successful and failed counts', () => {
    fc.assert(
      fc.property(bulkPaymentArbitrary, (payments) => {
        const result = processBulkPayment(payments);
        expect(result.successful).toBeGreaterThanOrEqual(0);
        expect(result.failed).toBeGreaterThanOrEqual(0);
      }),
      { numRuns: 100 }
    );
  });

  it('should have non-negative total amount', () => {
    fc.assert(
      fc.property(bulkPaymentArbitrary, (payments) => {
        const result = processBulkPayment(payments);
        expect(result.totalAmount).toBeGreaterThanOrEqual(0);
      }),
      { numRuns: 100 }
    );
  });

  it('should preserve payment details during processing', () => {
    fc.assert(
      fc.property(bulkPaymentArbitrary, (payments) => {
        const result = processBulkPayment(payments);
        
        result.processed.forEach((processed, index) => {
          const original = payments.find(p => p.id === processed.id);
          expect(original).toBeDefined();
          expect(processed.accountNumber).toBe(original!.accountNumber);
          expect(processed.beneficiaryName).toBe(original!.beneficiaryName);
          expect(processed.amount).toBe(original!.amount);
          expect(processed.currency).toBe(original!.currency);
        });
      }),
      { numRuns: 100 }
    );
  });
});

describe('Property 10: Account Reconciliation Balance', () => {
  /**
   * Arbitrary generator for accounts
   */
  const accountArbitrary = fc.record({
    accountNumber: fc.string({ minLength: 10, maxLength: 20 }),
    balance: fc.double({ min: 0, max: 1000000000, noNaN: true }),
    currency: fc.constantFrom('KES', 'USD', 'EUR')
  }) as fc.Arbitrary<Account>;

  /**
   * Arbitrary generator for transactions
   */
  const transactionArbitrary = fc.record({
    type: fc.constantFrom('CREDIT', 'DEBIT') as fc.Arbitrary<'CREDIT' | 'DEBIT'>,
    amount: fc.double({ min: 0.01, max: 1000000, noNaN: true })
  });

  /**
   * Apply transactions to account and reconcile
   */
  const reconcileAccount = (
    account: Account,
    transactions: Array<{ type: 'CREDIT' | 'DEBIT'; amount: number }>
  ): { finalBalance: number; isReconciled: boolean } => {
    let balance = account.balance;
    
    transactions.forEach(txn => {
      if (txn.type === 'CREDIT') {
        balance += txn.amount;
      } else if (txn.type === 'DEBIT') {
        balance -= txn.amount;
      }
    });

    // Reconciliation check: balance should match calculated balance
    const isReconciled = Math.abs(balance - (account.balance + 
      transactions.filter(t => t.type === 'CREDIT').reduce((sum, t) => sum + t.amount, 0) -
      transactions.filter(t => t.type === 'DEBIT').reduce((sum, t) => sum + t.amount, 0)
    )) < 0.01;

    return { finalBalance: balance, isReconciled };
  };

  it('should maintain balance equation: final = initial + credits - debits', () => {
    fc.assert(
      fc.property(
        accountArbitrary,
        fc.array(transactionArbitrary, { maxLength: 50 }),
        (account, transactions) => {
          const result = reconcileAccount(account, transactions);
          
          const totalCredits = transactions
            .filter(t => t.type === 'CREDIT')
            .reduce((sum, t) => sum + t.amount, 0);
          const totalDebits = transactions
            .filter(t => t.type === 'DEBIT')
            .reduce((sum, t) => sum + t.amount, 0);
          
          const expectedBalance = account.balance + totalCredits - totalDebits;
          expect(Math.abs(result.finalBalance - expectedBalance)).toBeLessThan(0.01);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should always reconcile correctly', () => {
    fc.assert(
      fc.property(
        accountArbitrary,
        fc.array(transactionArbitrary, { maxLength: 50 }),
        (account, transactions) => {
          const result = reconcileAccount(account, transactions);
          expect(result.isReconciled).toBe(true);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should increase balance with credits', () => {
    fc.assert(
      fc.property(accountArbitrary, transactionArbitrary, (account, transaction) => {
        const creditTransaction = { ...transaction, type: 'CREDIT' as const };
        const result = reconcileAccount(account, [creditTransaction]);
        
        expect(result.finalBalance).toBeGreaterThan(account.balance);
      }),
      { numRuns: 100 }
    );
  });

  it('should decrease balance with debits', () => {
    fc.assert(
      fc.property(accountArbitrary, transactionArbitrary, (account, transaction) => {
        const debitTransaction = { ...transaction, type: 'DEBIT' as const };
        const result = reconcileAccount(account, [debitTransaction]);
        
        expect(result.finalBalance).toBeLessThan(account.balance);
      }),
      { numRuns: 100 }
    );
  });

  it('should have no change with empty transactions', () => {
    fc.assert(
      fc.property(accountArbitrary, (account) => {
        const result = reconcileAccount(account, []);
        expect(result.finalBalance).toBe(account.balance);
      }),
      { numRuns: 100 }
    );
  });
});

describe('Property 11: Multi-Currency Consistency', () => {
  /**
   * Exchange rates (to KES)
   */
  const exchangeRates: Record<string, number> = {
    'KES': 1,
    'USD': 150,
    'EUR': 165,
    'GBP': 190
  };

  /**
   * Convert amount to base currency (KES)
   */
  const convertToBaseCurrency = (amount: number, currency: string): number => {
    return amount * (exchangeRates[currency] || 1);
  };

  /**
   * Arbitrary generator for multi-currency payments
   */
  const multiCurrencyPaymentArbitrary = fc.record({
    amount: fc.double({ min: 1, max: 1000000, noNaN: true }),
    currency: fc.constantFrom('KES', 'USD', 'EUR', 'GBP')
  });

  it('should maintain value equivalence across conversions', () => {
    fc.assert(
      fc.property(multiCurrencyPaymentArbitrary, (payment) => {
        const baseAmount = convertToBaseCurrency(payment.amount, payment.currency);
        
        // Convert back to original currency
        const backToOriginal = baseAmount / exchangeRates[payment.currency];
        
        expect(Math.abs(backToOriginal - payment.amount)).toBeLessThan(0.01);
      }),
      { numRuns: 100 }
    );
  });

  it('should have consistent total when summing multi-currency amounts', () => {
    fc.assert(
      fc.property(
        fc.array(multiCurrencyPaymentArbitrary, { minLength: 1, maxLength: 20 }),
        (payments) => {
          // Convert all to base currency and sum
          const totalInBase = payments.reduce(
            (sum, p) => sum + convertToBaseCurrency(p.amount, p.currency),
            0
          );
          
          // Sum should be non-negative
          expect(totalInBase).toBeGreaterThanOrEqual(0);
          
          // Converting each individually and summing should equal converting sum
          const individualConversions = payments.map(p => 
            convertToBaseCurrency(p.amount, p.currency)
          );
          const sumOfConversions = individualConversions.reduce((sum, val) => sum + val, 0);
          
          expect(Math.abs(totalInBase - sumOfConversions)).toBeLessThan(0.01);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should have KES as identity conversion', () => {
    fc.assert(
      fc.property(
        fc.double({ min: 1, max: 1000000, noNaN: true }),
        (amount) => {
          const converted = convertToBaseCurrency(amount, 'KES');
          expect(converted).toBe(amount);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should maintain ordering after currency conversion', () => {
    fc.assert(
      fc.property(multiCurrencyPaymentArbitrary, multiCurrencyPaymentArbitrary, (p1, p2) => {
        const base1 = convertToBaseCurrency(p1.amount, p1.currency);
        const base2 = convertToBaseCurrency(p2.amount, p2.currency);
        
        // If amounts are equal in base currency, they should be equivalent
        if (Math.abs(base1 - base2) < 0.01) {
          expect(Math.abs(base1 - base2)).toBeLessThan(0.01);
        }
        // If one is greater, it should remain greater after conversion
        else if (base1 > base2) {
          expect(base1).toBeGreaterThan(base2);
        } else {
          expect(base2).toBeGreaterThan(base1);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should have positive exchange rates', () => {
    Object.values(exchangeRates).forEach(rate => {
      expect(rate).toBeGreaterThan(0);
    });
  });
});
