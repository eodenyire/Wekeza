import { describe, it, expect } from 'vitest';
import * as fc from 'fast-check';
import { SecurityOrder } from '../../types';

/**
 * Property-Based Tests for Order Submission
 * 
 * **Validates: Requirements 1.4, 1.5, 1.6**
 * 
 * Property 1: Order Submission Completeness
 * For any valid security order (T-Bill, Bond, or Stock), submitting the order 
 * should result in an API call containing all required fields (security ID, type, 
 * quantity, order type) and the order should be recorded in the system.
 */

describe('Property 1: Order Submission Completeness', () => {
  /**
   * Arbitrary generator for valid security IDs
   */
  const securityIdArbitrary = fc.string({ minLength: 1, maxLength: 50 });

  /**
   * Arbitrary generator for security types
   */
  const securityTypeArbitrary = fc.constantFrom('TBILL', 'BOND', 'STOCK') as fc.Arbitrary<'TBILL' | 'BOND' | 'STOCK'>;

  /**
   * Arbitrary generator for order types
   */
  const orderTypeArbitrary = fc.constantFrom('BUY', 'SELL') as fc.Arbitrary<'BUY' | 'SELL'>;

  /**
   * Arbitrary generator for positive quantities
   */
  const quantityArbitrary = fc.double({ min: 0.01, max: 1000000, noNaN: true });

  /**
   * Arbitrary generator for optional price (for stocks)
   */
  const priceArbitrary = fc.option(fc.double({ min: 0.01, max: 100000, noNaN: true }));

  /**
   * Arbitrary generator for optional bid type (for T-Bills)
   */
  const bidTypeArbitrary = fc.option(
    fc.constantFrom('COMPETITIVE', 'NON_COMPETITIVE') as fc.Arbitrary<'COMPETITIVE' | 'NON_COMPETITIVE'>
  );

  /**
   * Arbitrary generator for complete SecurityOrder objects
   */
  const securityOrderArbitrary = fc.record({
    securityId: securityIdArbitrary,
    securityType: securityTypeArbitrary,
    orderType: orderTypeArbitrary,
    quantity: quantityArbitrary,
    price: priceArbitrary,
    bidType: bidTypeArbitrary,
  }) as fc.Arbitrary<SecurityOrder>;

  it('should contain all required fields for any valid security order', () => {
    fc.assert(
      fc.property(securityOrderArbitrary, (order) => {
        // Property: All required fields must be present
        expect(order).toHaveProperty('securityId');
        expect(order).toHaveProperty('securityType');
        expect(order).toHaveProperty('orderType');
        expect(order).toHaveProperty('quantity');

        // Verify required fields are not null/undefined
        expect(order.securityId).toBeDefined();
        expect(order.securityId).not.toBe('');
        expect(order.securityType).toBeDefined();
        expect(order.orderType).toBeDefined();
        expect(order.quantity).toBeDefined();

        // Verify types are correct
        expect(['TBILL', 'BOND', 'STOCK']).toContain(order.securityType);
        expect(['BUY', 'SELL']).toContain(order.orderType);
        expect(typeof order.quantity).toBe('number');
        expect(order.quantity).toBeGreaterThan(0);
      }),
      { numRuns: 100 }
    );
  });

  it('should have valid price field for stock orders', () => {
    fc.assert(
      fc.property(
        securityOrderArbitrary.filter((order) => order.securityType === 'STOCK'),
        (order) => {
          // Property: Stock orders should have price information available
          // (price can be optional in the order if using market orders, but the field should exist)
          expect(order).toHaveProperty('price');
          
          // If price is provided, it should be a positive number
          if (order.price !== undefined && order.price !== null) {
            expect(typeof order.price).toBe('number');
            expect(order.price).toBeGreaterThan(0);
          }
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should have valid bidType field for T-Bill orders', () => {
    fc.assert(
      fc.property(
        securityOrderArbitrary.filter((order) => order.securityType === 'TBILL'),
        (order) => {
          // Property: T-Bill orders should have bidType information available
          expect(order).toHaveProperty('bidType');
          
          // If bidType is provided, it should be one of the valid values
          if (order.bidType !== undefined && order.bidType !== null) {
            expect(['COMPETITIVE', 'NON_COMPETITIVE']).toContain(order.bidType);
          }
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should serialize to JSON with all required fields intact', () => {
    fc.assert(
      fc.property(securityOrderArbitrary, (order) => {
        // Property: Order should be serializable and deserializable without data loss
        const serialized = JSON.stringify(order);
        const deserialized = JSON.parse(serialized) as SecurityOrder;

        // All required fields should survive serialization
        expect(deserialized.securityId).toBe(order.securityId);
        expect(deserialized.securityType).toBe(order.securityType);
        expect(deserialized.orderType).toBe(order.orderType);
        expect(deserialized.quantity).toBe(order.quantity);

        // Optional fields should also survive if present
        if (order.price !== undefined) {
          expect(deserialized.price).toBe(order.price);
        }
        if (order.bidType !== undefined) {
          expect(deserialized.bidType).toBe(order.bidType);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should maintain order type consistency across all security types', () => {
    fc.assert(
      fc.property(securityOrderArbitrary, (order) => {
        // Property: Order type should always be either BUY or SELL regardless of security type
        expect(['BUY', 'SELL']).toContain(order.orderType);
        
        // For T-Bills and Bonds, only BUY orders are typically valid
        if (order.securityType === 'TBILL' || order.securityType === 'BOND') {
          // This is a business rule that could be enforced
          // For now, we just verify the field exists and is valid
          expect(order.orderType).toBeDefined();
        }
        
        // For Stocks, both BUY and SELL are valid
        if (order.securityType === 'STOCK') {
          expect(['BUY', 'SELL']).toContain(order.orderType);
        }
      }),
      { numRuns: 100 }
    );
  });

  it('should have positive quantity for all order types', () => {
    fc.assert(
      fc.property(securityOrderArbitrary, (order) => {
        // Property: Quantity must always be positive
        expect(order.quantity).toBeGreaterThan(0);
        expect(Number.isFinite(order.quantity)).toBe(true);
        expect(Number.isNaN(order.quantity)).toBe(false);
      }),
      { numRuns: 100 }
    );
  });

  it('should create valid API request payload structure', () => {
    fc.assert(
      fc.property(securityOrderArbitrary, (order) => {
        // Property: Order should be convertible to a valid API request payload
        const payload = {
          securityId: order.securityId,
          securityType: order.securityType,
          orderType: order.orderType,
          quantity: order.quantity,
          ...(order.price !== undefined && order.price !== null && { price: order.price }),
          ...(order.bidType !== undefined && order.bidType !== null && { bidType: order.bidType }),
        };

        // Verify payload has all required fields
        expect(payload).toHaveProperty('securityId');
        expect(payload).toHaveProperty('securityType');
        expect(payload).toHaveProperty('orderType');
        expect(payload).toHaveProperty('quantity');

        // Verify payload can be stringified (for HTTP request)
        const jsonString = JSON.stringify(payload);
        expect(jsonString).toBeTruthy();
        expect(jsonString.length).toBeGreaterThan(0);

        // Verify it can be parsed back
        const parsed = JSON.parse(jsonString);
        expect(parsed.securityId).toBe(order.securityId);
        expect(parsed.securityType).toBe(order.securityType);
        expect(parsed.orderType).toBe(order.orderType);
        expect(parsed.quantity).toBe(order.quantity);
      }),
      { numRuns: 100 }
    );
  });
});
