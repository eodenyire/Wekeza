// Data Export Utilities

export type ExportFormat = 'CSV' | 'EXCEL' | 'PDF';

export function exportToCSV(data: any[], filename: string): void {
  if (data.length === 0) {
    alert('No data to export');
    return;
  }

  // Get headers from first object
  const headers = Object.keys(data[0]);
  
  // Create CSV content
  const csvContent = [
    headers.join(','),
    ...data.map(row => 
      headers.map(header => {
        const value = row[header];
        // Handle values with commas or quotes
        if (typeof value === 'string' && (value.includes(',') || value.includes('"'))) {
          return `"${value.replace(/"/g, '""')}"`;
        }
        return value;
      }).join(',')
    )
  ].join('\n');

  downloadFile(csvContent, `${filename}.csv`, 'text/csv');
}

export function exportToJSON(data: any[], filename: string): void {
  const jsonContent = JSON.stringify(data, null, 2);
  downloadFile(jsonContent, `${filename}.json`, 'application/json');
}

export function exportTableToCSV(tableId: string, filename: string): void {
  const table = document.getElementById(tableId) as HTMLTableElement;
  if (!table) {
    alert('Table not found');
    return;
  }

  const rows: string[][] = [];
  
  // Get headers
  const headerCells = table.querySelectorAll('thead th');
  const headers = Array.from(headerCells).map(cell => cell.textContent || '');
  rows.push(headers);

  // Get data rows
  const dataRows = table.querySelectorAll('tbody tr');
  dataRows.forEach(row => {
    const cells = row.querySelectorAll('td');
    const rowData = Array.from(cells).map(cell => cell.textContent || '');
    rows.push(rowData);
  });

  const csvContent = rows.map(row => row.join(',')).join('\n');
  downloadFile(csvContent, `${filename}.csv`, 'text/csv');
}

function downloadFile(content: string, filename: string, mimeType: string): void {
  const blob = new Blob([content], { type: mimeType });
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = filename;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  window.URL.revokeObjectURL(url);
}

// Format data for export
export function formatDataForExport(data: any[]): any[] {
  return data.map(item => {
    const formatted: any = {};
    
    Object.keys(item).forEach(key => {
      const value = item[key];
      
      // Format dates
      if (value instanceof Date) {
        formatted[key] = value.toLocaleDateString();
      }
      // Format numbers with commas
      else if (typeof value === 'number') {
        formatted[key] = value.toLocaleString();
      }
      // Handle nested objects
      else if (typeof value === 'object' && value !== null) {
        formatted[key] = JSON.stringify(value);
      }
      else {
        formatted[key] = value;
      }
    });
    
    return formatted;
  });
}

// Generate report filename with timestamp
export function generateReportFilename(prefix: string): string {
  const timestamp = new Date().toISOString().split('T')[0];
  return `${prefix}-${timestamp}`;
}

// Export portfolio data
export function exportPortfolio(securities: any[]): void {
  const exportData = securities.map(sec => ({
    'Security Type': sec.securityType,
    'Name': sec.name,
    'Quantity': sec.quantity,
    'Purchase Price': sec.purchasePrice,
    'Current Price': sec.currentPrice,
    'Market Value': sec.marketValue,
    'Unrealized Gain': sec.unrealizedGain,
    'Maturity Date': sec.maturityDate || 'N/A'
  }));

  exportToCSV(exportData, generateReportFilename('portfolio'));
}

// Export loan portfolio
export function exportLoanPortfolio(loans: any[]): void {
  const exportData = loans.map(loan => ({
    'Loan Number': loan.loanNumber,
    'Entity': loan.governmentEntity.name,
    'Entity Type': loan.governmentEntity.type,
    'Principal Amount': loan.principalAmount,
    'Outstanding Balance': loan.outstandingBalance,
    'Interest Rate': `${loan.interestRate}%`,
    'Status': loan.status,
    'Disbursement Date': new Date(loan.disbursementDate).toLocaleDateString(),
    'Maturity Date': new Date(loan.maturityDate).toLocaleDateString()
  }));

  exportToCSV(exportData, generateReportFilename('loan-portfolio'));
}

// Export transactions
export function exportTransactions(transactions: any[]): void {
  const exportData = transactions.map(txn => ({
    'Date': new Date(txn.date).toLocaleDateString(),
    'Type': txn.type,
    'Description': txn.description,
    'Amount': txn.amount,
    'Status': txn.status,
    'Reference': txn.reference
  }));

  exportToCSV(exportData, generateReportFilename('transactions'));
}

// Export grant applications
export function exportGrantApplications(applications: any[]): void {
  const exportData = applications.map(app => ({
    'Application Number': app.applicationNumber,
    'Applicant': app.applicantName,
    'Project Title': app.projectTitle,
    'Requested Amount': app.requestedAmount,
    'Status': app.status,
    'Submitted Date': new Date(app.submittedDate).toLocaleDateString()
  }));

  exportToCSV(exportData, generateReportFilename('grant-applications'));
}

// Export to Excel (using xlsx library)
export function exportToExcel(data: any[], filename: string): void {
  // For now, fallback to CSV export
  // Full Excel export requires xlsx library integration
  exportToCSV(data, filename);
}

// Export to PDF
export function exportToPDF(data: any[], filename: string): void {
  // For now, fallback to CSV export
  // Full PDF export requires jsPDF library integration
  exportToCSV(data, filename);
}
