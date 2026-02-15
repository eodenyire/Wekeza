// CBK Regulation Compliance Utilities

export interface CBKRegulations {
  minimumTBillInvestment: number;
  minimumBondInvestment: number;
  maximumSingleTransaction: number;
  tradingStartHour: number;
  tradingEndHour: number;
}

export const CBK_REGULATIONS: CBKRegulations = {
  minimumTBillInvestment: 100000, // KES 100,000
  minimumBondInvestment: 50000, // KES 50,000
  maximumSingleTransaction: 1000000000, // KES 1 Billion
  tradingStartHour: 9, // 9:00 AM
  tradingEndHour: 15 // 3:00 PM
};

export function validateTBillInvestment(amount: number): { valid: boolean; message?: string } {
  if (amount < CBK_REGULATIONS.minimumTBillInvestment) {
    return {
      valid: false,
      message: `Minimum T-Bill investment is KES ${CBK_REGULATIONS.minimumTBillInvestment.toLocaleString()}`
    };
  }
  if (amount > CBK_REGULATIONS.maximumSingleTransaction) {
    return {
      valid: false,
      message: `Maximum single transaction is KES ${CBK_REGULATIONS.maximumSingleTransaction.toLocaleString()}`
    };
  }
  return { valid: true };
}

export function validateBondInvestment(amount: number): { valid: boolean; message?: string } {
  if (amount < CBK_REGULATIONS.minimumBondInvestment) {
    return {
      valid: false,
      message: `Minimum Bond investment is KES ${CBK_REGULATIONS.minimumBondInvestment.toLocaleString()}`
    };
  }
  if (amount > CBK_REGULATIONS.maximumSingleTransaction) {
    return {
      valid: false,
      message: `Maximum single transaction is KES ${CBK_REGULATIONS.maximumSingleTransaction.toLocaleString()}`
    };
  }
  return { valid: true };
}

export function isWithinTradingHours(): { valid: boolean; message?: string } {
  const now = new Date();
  const currentHour = now.getHours();
  
  if (currentHour < CBK_REGULATIONS.tradingStartHour || currentHour >= CBK_REGULATIONS.tradingEndHour) {
    return {
      valid: false,
      message: `Trading hours are ${CBK_REGULATIONS.tradingStartHour}:00 AM - ${CBK_REGULATIONS.tradingEndHour}:00 PM EAT`
    };
  }
  
  return { valid: true };
}

// PFMA (Public Finance Management Act) Compliance

export interface PFMARequirements {
  maxCountyExposurePercent: number;
  maxNationalExposurePercent: number;
  minimumLoanAmount: number;
  maximumLoanTenorMonths: number;
  maxGrantAmount: number;
  requiredApprovals: number;
}

export const PFMA_REQUIREMENTS: PFMARequirements = {
  maxCountyExposurePercent: 10, // 10% of bank capital
  maxNationalExposurePercent: 25, // 25% of bank capital
  minimumLoanAmount: 10000000, // KES 10 Million
  maximumLoanTenorMonths: 360, // 30 years
  maxGrantAmount: 5000000, // KES 5 Million
  requiredApprovals: 2 // Two signatories
};

export function validateLoanAmount(amount: number): { valid: boolean; message?: string } {
  if (amount < PFMA_REQUIREMENTS.minimumLoanAmount) {
    return {
      valid: false,
      message: `Minimum loan amount is KES ${PFMA_REQUIREMENTS.minimumLoanAmount.toLocaleString()}`
    };
  }
  return { valid: true };
}

export function validateLoanTenor(tenorMonths: number): { valid: boolean; message?: string } {
  if (tenorMonths > PFMA_REQUIREMENTS.maximumLoanTenorMonths) {
    return {
      valid: false,
      message: `Maximum loan tenor is ${PFMA_REQUIREMENTS.maximumLoanTenorMonths / 12} years`
    };
  }
  return { valid: true };
}

export function validateGrantAmount(amount: number): { valid: boolean; message?: string } {
  if (amount > PFMA_REQUIREMENTS.maxGrantAmount) {
    return {
      valid: false,
      message: `Maximum grant amount is KES ${PFMA_REQUIREMENTS.maxGrantAmount.toLocaleString()}`
    };
  }
  return { valid: true };
}

export function validateExposureLimit(
  entityType: 'NATIONAL' | 'COUNTY',
  currentExposure: number,
  newLoanAmount: number,
  bankCapital: number
): { valid: boolean; message?: string } {
  const maxPercent = entityType === 'NATIONAL' 
    ? PFMA_REQUIREMENTS.maxNationalExposurePercent 
    : PFMA_REQUIREMENTS.maxCountyExposurePercent;
  
  const totalExposure = currentExposure + newLoanAmount;
  const exposurePercent = (totalExposure / bankCapital) * 100;
  
  if (exposurePercent > maxPercent) {
    return {
      valid: false,
      message: `Exposure limit exceeded. Maximum ${maxPercent}% of bank capital for ${entityType} government`
    };
  }
  
  return { valid: true };
}

export function hasRequiredApprovals(approvalCount: number): { valid: boolean; message?: string } {
  if (approvalCount < PFMA_REQUIREMENTS.requiredApprovals) {
    return {
      valid: false,
      message: `Requires ${PFMA_REQUIREMENTS.requiredApprovals} approvals`
    };
  }
  return { valid: true };
}

// AML/KYC Compliance

export function validateGovernmentEntity(entity: {
  name: string;
  type: 'NATIONAL' | 'COUNTY';
  contactPerson: string;
  email: string;
  phone: string;
}): { valid: boolean; errors: string[] } {
  const errors: string[] = [];
  
  if (!entity.name || entity.name.trim().length < 3) {
    errors.push('Entity name is required (minimum 3 characters)');
  }
  
  if (!entity.contactPerson || entity.contactPerson.trim().length < 3) {
    errors.push('Contact person is required');
  }
  
  if (!entity.email || !isValidEmail(entity.email)) {
    errors.push('Valid email is required');
  }
  
  if (!entity.phone || !isValidPhone(entity.phone)) {
    errors.push('Valid phone number is required');
  }
  
  return {
    valid: errors.length === 0,
    errors
  };
}

function isValidEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
}

function isValidPhone(phone: string): boolean {
  const phoneRegex = /^(\+254|0)[17]\d{8}$/; // Kenyan phone format
  return phoneRegex.test(phone.replace(/\s/g, ''));
}

// Audit Trail

export interface AuditLogEntry {
  userId: string;
  userRole: string;
  action: string;
  entityType: string;
  entityId: string;
  timestamp: string;
  ipAddress?: string;
  details?: any;
}

export function createAuditLog(
  action: string,
  entityType: string,
  entityId: string,
  details?: any
): AuditLogEntry {
  return {
    userId: 'current-user-id', // Get from auth context
    userRole: 'current-user-role', // Get from auth context
    action,
    entityType,
    entityId,
    timestamp: new Date().toISOString(),
    ipAddress: window.location.hostname,
    details
  };
}

export async function logAuditTrail(entry: AuditLogEntry): Promise<void> {
  try {
    await fetch('/api/public-sector/audit-log', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(entry)
    });
  } catch (error) {
    console.error('Failed to log audit trail:', error);
    // Don't throw - audit logging should not break the main flow
  }
}
