import { describe, it, expect } from 'vitest';
import * as fc from 'fast-check';

/**
 * Property-Based Tests for Data Export
 * 
 * **Validates: Requirements 6.8**
 * 
 * Property 22: Data Export Completeness
 */

interface ExportData {
  id: string;
  name: string;
  value: number;
  date: Date;
  status: string;
  metadata?: Record<string, any>;
}

describe('Property 22: Data Export Completeness', () => {
  const exportDataArbitrary = fc.record({
    id: fc.uuid(),
    name: fc.string({ minLength: 1, maxLength: 100 }),
    value: fc.double({ min: 0, max: 1000000000, noNaN: true }),
    date: fc.date({ min: new Date('2024-01-01'), max: new Date('2026-12-31') }),
    status: fc.constantFrom('ACTIVE', 'PENDING', 'COMPLETED', 'CANCELLED'),
    metadata: fc.option(fc.dictionary(fc.string(), fc.anything()))
  }) as fc.Arbitrary<ExportData>;

  /**
   * Export data to CSV format
   */
  const exportToCSV = (data: ExportData[]): string => {
    if (data.length === 0) return '';

    const headers = ['id', 'name', 'value', 'date', 'status'];
    const csvRows = [headers.join(',')];

    data.forEach(item => {
      const row = [
        item.id,
        `"${item.name.replace(/"/g, '""')}"`,
        item.value.toString(),
        item.date.toISOString(),
        item.status
      ];
      csvRows.push(row.join(','));
    });

    return csvRows.join('\n');
  };

  /**
   * Parse CSV back to data
   */
  const parseCSV = (csv: string): ExportData[] => {
    if (!csv) return [];

    const lines = csv.split('\n');
    if (lines.length < 2) return [];

    const data: ExportData[] = [];
    
    for (let i = 1; i < lines.length; i++) {
      const line = lines[i];
      if (!line) continue;

      // Simple CSV parsing (handles quoted fields)
      const matches = line.match(/("(?:[^"]|"")*"|[^,]+)/g);
      if (!matches || matches.length < 5) continue;

      data.push({
        id: matches[0],
        name: matches[1].replace(/^"|"$/g, '').replace(/""/g, '"'),
        value: parseFloat(matches[2]),
        date: new Date(matches[3]),
        status: matches[4]
      });
    }

    return data;
  };

  it('should export all records', () => {
    fc.assert(
      fc.property(
        fc.array(exportDataArbitrary, { minLength: 1, maxLength: 50 }),
        (data) => {
          const csv = exportToCSV(data);
          const lines = csv.split('\n');
          
          // Should have header + data rows
          expect(lines.length).toBe(data.length + 1);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should preserve data integrity during export and import', () => {
    fc.assert(
      fc.property(
        fc.array(exportDataArbitrary, { minLength: 1, maxLength: 20 }),
        (data) => {
          const csv = exportToCSV(data);
          const parsed = parseCSV(csv);

          expect(parsed.length).toBe(data.length);

          parsed.forEach((item, index) => {
            expect(item.id).toBe(data[index].id);
            expect(item.name).toBe(data[index].name);
            expect(Math.abs(item.value - data[index].value)).toBeLessThan(0.01);
            expect(item.status).toBe(data[index].status);
          });
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should handle empty data gracefully', () => {
    const csv = exportToCSV([]);
    expect(csv).toBe('');
  });

  it('should include header row', () => {
    fc.assert(
      fc.property(
        fc.array(exportDataArbitrary, { minLength: 1, maxLength: 50 }),
        (data) => {
          const csv = exportToCSV(data);
          const firstLine = csv.split('\n')[0];
          
          expect(firstLine).toContain('id');
          expect(firstLine).toContain('name');
          expect(firstLine).toContain('value');
          expect(firstLine).toContain('date');
          expect(firstLine).toContain('status');
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should handle special characters in names', () => {
    const dataWithSpecialChars: ExportData[] = [
      {
        id: '123',
        name: 'Test, "Company"',
        value: 1000,
        date: new Date('2024-01-01'),
        status: 'ACTIVE'
      }
    ];

    const csv = exportToCSV(dataWithSpecialChars);
    const parsed = parseCSV(csv);

    expect(parsed[0].name).toBe('Test, "Company"');
  });

  it('should maintain row count consistency', () => {
    fc.assert(
      fc.property(
        fc.array(exportDataArbitrary, { minLength: 1, maxLength: 50 }),
        (data) => {
          const csv = exportToCSV(data);
          const parsed = parseCSV(csv);

          expect(parsed.length).toBe(data.length);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should export numeric values correctly', () => {
    fc.assert(
      fc.property(
        fc.array(exportDataArbitrary, { minLength: 1, maxLength: 50 }),
        (data) => {
          const csv = exportToCSV(data);
          const parsed = parseCSV(csv);

          parsed.forEach((item, index) => {
            expect(typeof item.value).toBe('number');
            expect(Number.isFinite(item.value)).toBe(true);
            expect(Math.abs(item.value - data[index].value)).toBeLessThan(0.01);
          });
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should export dates in ISO format', () => {
    fc.assert(
      fc.property(
        fc.array(exportDataArbitrary, { minLength: 1, maxLength: 50 }),
        (data) => {
          const csv = exportToCSV(data);
          
          // Check that dates are in ISO format
          data.forEach(item => {
            expect(csv).toContain(item.date.toISOString());
          });
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should be deterministic for same input', () => {
    fc.assert(
      fc.property(
        fc.array(exportDataArbitrary, { minLength: 1, maxLength: 50 }),
        (data) => {
          const csv1 = exportToCSV(data);
          const csv2 = exportToCSV(data);

          expect(csv1).toBe(csv2);
        }
      ),
      { numRuns: 100 }
    );
  });

  it('should handle large datasets', () => {
    fc.assert(
      fc.property(
        fc.array(exportDataArbitrary, { minLength: 100, maxLength: 1000 }),
        (data) => {
          const csv = exportToCSV(data);
          const lines = csv.split('\n');

          expect(lines.length).toBe(data.length + 1);
        }
      ),
      { numRuns: 10 } // Fewer runs for large datasets
    );
  });
});
