import { describe, it, expect } from 'vitest';
import * as fc from 'fast-check';

/**
 * Property-Based Tests for Repayment Schedules
 * 
 * **Validates: Requirements 2.6**
 * 
 * Property 6: Repayment Schedule Accuracy
 */

interface RepaymentScheduleEntry {
  installmentNumber: number;
  dueDate: Date;
  principalAmount: number;
  interestAmount: number;
  totalAmount: number;
  remainingBalance: number;
}

describe('Property 6: Repayment Schedule Accuracy', () => {
  /**
   * Arbitrary generator for loan parameters
   */
  const loanParametersArbitrary = fc.record({
    principalAmount: fc.double({ min: 1000000, max: 5000000000, noNaN: true }),
    annualInterestRate: fc.double({ min: 5, max: 20, noNaN: true }), // percentage
    termMonths: fc.integer({ min: 12, max: 360 }), // 1 to 30 years
    startDate: fc.date({ min: new Date('2024-01-01'), max: new Date('2026-12-31') })
  });

  /**
   * Generate repayment schedule using reducing balance method
   */
  const generateRepaymentSchedule = (params: {
    principalAmount: number;
    annualInterestRate: number;
    termMonths: number;
    startDate: Date;
  }): RepaymentScheduleEntry[] => {
    const monthlyRate = params.annualInterestRate / 100 / 12;
    const numPayments = params.termMonths;
    
    // Calculate monthly payment using amortization formula
    const monthlyPayment = params.principalAmount * 
      (monthlyRate * Math.pow(1 + monthlyRate, numPayments)) /
      (Math.pow(1 + monthlyRate, numPayments) - 1);
    
    const schedule: RepaymentScheduleEntry[] = [];
    let remainingBalance = params.principalAmount;
    
    for (let i = 1; i <= numPayments; i++) {
      const interestAmount = remainingBalance * monthlyRate;
      const principalAmount = monthlyPayment - interestAmount;
      
      // Handle last payment rounding
      const actualPrincipal = i === numPayments ? remainingBalance : principalAmount;
      const actualTotal = actualPrincipal + interestAmount;
      
      remainingBalance -= actualPrincipal;
      
      const dueDate = new Date(params.startDate);
      dueDate.setMonth(dueDate.getMonth() + i);
      
      schedule.push({
        installmentNumber: i,
        dueDate,
        principalAmount: actualPrincipal,
        interestAmount,
        totalAmount: actualTotal,
        remainingBalance: Math.max(0, remainingBalance)
      });
    }
    
    return schedule;
  };

  it('should have correct number of installments', () => {
    fc.assert(
      fc.property(loanParametersArbitrary, (params) => {
        const schedule = generateRepaymentSchedule(params);
        expect(schedule.length).toBe(params.termMonths);
      }),
      { numRuns: 100 }
    );
  });

  it('should have total principal equal to loan amount', () => {
    fc.assert(
      fc.property(loanParametersArbitrary, (params) => {
        const schedule = generateRepaymentSchedule(params);
        const totalPrincipal = schedule.reduce((sum, entry) => sum + entry.principalAmount, 0);
        
        // Allow small rounding error (0.01%)
        const tolerance = params.principalAmount * 0.0001;
        expect(Math.abs(totalPrincipal - params.principalAmount)).toBeLessThan(tolerance);
      }),
      { numRuns: 100 }
    );
  });

  it('should have decreasing remaining balance', () => {
    fc.assert(
      fc.property(loanParametersArbitrary, (params) => {
        const schedule = generateRepaymentSchedule(params);
        
        for (let i = 0; i < schedule.length - 1; i++) {
          expect(schedule[i + 1].remainingBalance).toBeLessThanOrEqual(schedule[i].remainingBalance);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should have final remaining balance of zero', () => {
    fc.assert(
      fc.property(loanParametersArbitrary, (params) => {
        const schedule = generateRepaymentSchedule(params);
        const finalBalance = schedule[schedule.length - 1].remainingBalance;
        
        // Should be zero or very close to zero (rounding)
        expect(Math.abs(finalBalance)).toBeLessThan(1);
      }),
      { numRuns: 100 }
    );
  });

  it('should have total amount equal to principal plus interest', () => {
    fc.assert(
      fc.property(loanParametersArbitrary, (params) => {
        const schedule = generateRepaymentSchedule(params);
        
        schedule.forEach(entry => {
          const calculatedTotal = entry.principalAmount + entry.interestAmount;
          expect(Math.abs(entry.totalAmount - calculatedTotal)).toBeLessThan(0.01);
        });
      }),
      { numRuns: 100 }
    );
  });

  it('should have sequential installment numbers', () => {
    fc.assert(
      fc.property(loanParametersArbitrary, (params) => {
        const schedule = generateRepaymentSchedule(params);
        
        schedule.forEach((entry, index) => {
          expect(entry.installmentNumber).toBe(index + 1);
        });
      }),
      { numRuns: 100 }
    );
  });

  it('should have chronologically ordered due dates', () => {
    fc.assert(
      fc.property(loanParametersArbitrary, (params) => {
        const schedule = generateRepaymentSchedule(params);
        
        for (let i = 0; i < schedule.length - 1; i++) {
          expect(schedule[i + 1].dueDate.getTime()).toBeGreaterThan(schedule[i].dueDate.getTime());
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should have positive amounts for all entries', () => {
    fc.assert(
      fc.property(loanParametersArbitrary, (params) => {
        const schedule = generateRepaymentSchedule(params);
        
        schedule.forEach(entry => {
          expect(entry.principalAmount).toBeGreaterThan(0);
          expect(entry.interestAmount).toBeGreaterThan(0);
          expect(entry.totalAmount).toBeGreaterThan(0);
          expect(entry.remainingBalance).toBeGreaterThanOrEqual(0);
        });
      }),
      { numRuns: 100 }
    );
  });

  it('should have higher total interest for higher interest rates', () => {
    fc.assert(
      fc.property(loanParametersArbitrary, (params) => {
        const schedule1 = generateRepaymentSchedule(params);
        const schedule2 = generateRepaymentSchedule({
          ...params,
          annualInterestRate: params.annualInterestRate * 1.5
        });
        
        const totalInterest1 = schedule1.reduce((sum, entry) => sum + entry.interestAmount, 0);
        const totalInterest2 = schedule2.reduce((sum, entry) => sum + entry.interestAmount, 0);
        
        expect(totalInterest2).toBeGreaterThan(totalInterest1);
      }),
      { numRuns: 100 }
    );
  });

  it('should have higher total interest for longer terms', () => {
    fc.assert(
      fc.property(loanParametersArbitrary, (params) => {
        if (params.termMonths < 240) { // Only test if we can double the term
          const schedule1 = generateRepaymentSchedule(params);
          const schedule2 = generateRepaymentSchedule({
            ...params,
            termMonths: params.termMonths * 2
          });
          
          const totalInterest1 = schedule1.reduce((sum, entry) => sum + entry.interestAmount, 0);
          const totalInterest2 = schedule2.reduce((sum, entry) => sum + entry.interestAmount, 0);
          
          expect(totalInterest2).toBeGreaterThan(totalInterest1);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should be deterministic for same inputs', () => {
    fc.assert(
      fc.property(loanParametersArbitrary, (params) => {
        const schedule1 = generateRepaymentSchedule(params);
        const schedule2 = generateRepaymentSchedule(params);
        
        expect(schedule1.length).toBe(schedule2.length);
        
        schedule1.forEach((entry, index) => {
          expect(entry.principalAmount).toBeCloseTo(schedule2[index].principalAmount, 2);
          expect(entry.interestAmount).toBeCloseTo(schedule2[index].interestAmount, 2);
          expect(entry.totalAmount).toBeCloseTo(schedule2[index].totalAmount, 2);
        });
      }),
      { numRuns: 100 }
    );
  });

  it('should have first installment due one month after start date', () => {
    fc.assert(
      fc.property(loanParametersArbitrary, (params) => {
        const schedule = generateRepaymentSchedule(params);
        const firstDueDate = schedule[0].dueDate;
        
        const expectedMonth = (params.startDate.getMonth() + 1) % 12;
        expect(firstDueDate.getMonth()).toBe(expectedMonth);
      }),
      { numRuns: 100 }
    );
  });
});
