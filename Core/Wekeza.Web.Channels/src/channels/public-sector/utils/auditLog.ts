// Audit logging utility for tracking all transactions and actions

export interface AuditLogEntry {
  userId: string;
  timestamp: string;
  actionType: string;
  transactionDetails: any;
  ipAddress?: string;
  userAgent?: string;
  module: 'SECURITIES' | 'LENDING' | 'BANKING' | 'GRANTS' | 'DASHBOARD';
  status: 'SUCCESS' | 'FAILURE';
  errorMessage?: string;
}

/**
 * Logs an audit entry for a transaction or action
 * Captures user ID, timestamp, action type, transaction details, and IP address
 * Sends audit logs to backend API and handles failures gracefully
 */
export async function logAuditEntry(entry: Omit<AuditLogEntry, 'timestamp' | 'ipAddress' | 'userAgent'>): Promise<void> {
  try {
    const auditEntry: AuditLogEntry = {
      ...entry,
      timestamp: new Date().toISOString(),
      userAgent: navigator.userAgent,
      // IP address will be captured by backend from request headers
    };

    // Send to backend audit API
    const response = await fetch('/api/public-sector/audit/log', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      },
      body: JSON.stringify(auditEntry)
    });

    if (!response.ok) {
      // Log failure but don't throw - audit logging should not break user flow
      console.error('Failed to log audit entry:', response.statusText);
    }
  } catch (error) {
    // Handle audit log failures gracefully - don't interrupt user operations
    console.error('Audit logging error:', error);
    
    // Optionally store failed audit logs locally for retry
    storeFailedAuditLog(entry);
  }
}

/**
 * Store failed audit logs in local storage for later retry
 */
function storeFailedAuditLog(entry: Omit<AuditLogEntry, 'timestamp' | 'ipAddress' | 'userAgent'>): void {
  try {
    const failedLogs = JSON.parse(localStorage.getItem('failedAuditLogs') || '[]');
    failedLogs.push({
      ...entry,
      timestamp: new Date().toISOString(),
      userAgent: navigator.userAgent
    });
    
    // Keep only last 100 failed logs
    if (failedLogs.length > 100) {
      failedLogs.shift();
    }
    
    localStorage.setItem('failedAuditLogs', JSON.stringify(failedLogs));
  } catch (error) {
    console.error('Failed to store audit log locally:', error);
  }
}

/**
 * Retry sending failed audit logs
 */
export async function retryFailedAuditLogs(): Promise<void> {
  try {
    const failedLogs = JSON.parse(localStorage.getItem('failedAuditLogs') || '[]');
    
    if (failedLogs.length === 0) return;

    const response = await fetch('/api/public-sector/audit/log/batch', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      },
      body: JSON.stringify(failedLogs)
    });

    if (response.ok) {
      // Clear failed logs on successful retry
      localStorage.removeItem('failedAuditLogs');
    }
  } catch (error) {
    console.error('Failed to retry audit logs:', error);
  }
}

/**
 * Helper functions for common audit log scenarios
 */

export function logSecurityOrder(userId: string, orderDetails: any, status: 'SUCCESS' | 'FAILURE', errorMessage?: string): void {
  logAuditEntry({
    userId,
    actionType: 'SECURITY_ORDER',
    transactionDetails: orderDetails,
    module: 'SECURITIES',
    status,
    errorMessage
  });
}

export function logLoanApproval(userId: string, loanDetails: any, status: 'SUCCESS' | 'FAILURE', errorMessage?: string): void {
  logAuditEntry({
    userId,
    actionType: 'LOAN_APPROVAL',
    transactionDetails: loanDetails,
    module: 'LENDING',
    status,
    errorMessage
  });
}

export function logLoanDisbursement(userId: string, disbursementDetails: any, status: 'SUCCESS' | 'FAILURE', errorMessage?: string): void {
  logAuditEntry({
    userId,
    actionType: 'LOAN_DISBURSEMENT',
    transactionDetails: disbursementDetails,
    module: 'LENDING',
    status,
    errorMessage
  });
}

export function logBulkPayment(userId: string, paymentDetails: any, status: 'SUCCESS' | 'FAILURE', errorMessage?: string): void {
  logAuditEntry({
    userId,
    actionType: 'BULK_PAYMENT',
    transactionDetails: paymentDetails,
    module: 'BANKING',
    status,
    errorMessage
  });
}

export function logRevenueReconciliation(userId: string, reconciliationDetails: any, status: 'SUCCESS' | 'FAILURE', errorMessage?: string): void {
  logAuditEntry({
    userId,
    actionType: 'REVENUE_RECONCILIATION',
    transactionDetails: reconciliationDetails,
    module: 'BANKING',
    status,
    errorMessage
  });
}

export function logGrantApproval(userId: string, grantDetails: any, status: 'SUCCESS' | 'FAILURE', errorMessage?: string): void {
  logAuditEntry({
    userId,
    actionType: 'GRANT_APPROVAL',
    transactionDetails: grantDetails,
    module: 'GRANTS',
    status,
    errorMessage
  });
}

export function logGrantDisbursement(userId: string, disbursementDetails: any, status: 'SUCCESS' | 'FAILURE', errorMessage?: string): void {
  logAuditEntry({
    userId,
    actionType: 'GRANT_DISBURSEMENT',
    transactionDetails: disbursementDetails,
    module: 'GRANTS',
    status,
    errorMessage
  });
}

// Initialize retry mechanism on app load
if (typeof window !== 'undefined') {
  // Retry failed logs every 5 minutes
  setInterval(retryFailedAuditLogs, 5 * 60 * 1000);
}
