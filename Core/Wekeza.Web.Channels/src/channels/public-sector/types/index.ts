// User Roles
export enum UserRole {
  TreasuryOfficer = 'TREASURY_OFFICER',
  CreditOfficer = 'CREDIT_OFFICER',
  GovernmentFinanceOfficer = 'GOVERNMENT_FINANCE_OFFICER',
  CSRManager = 'CSR_MANAGER',
  ComplianceOfficer = 'COMPLIANCE_OFFICER',
  SeniorManagement = 'SENIOR_MANAGEMENT'
}

// Common Types
export interface GovernmentEntity {
  id: string;
  name: string;
  type: 'NATIONAL' | 'COUNTY';
  countyCode?: string;
  contactPerson: string;
  email: string;
  phone: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  error?: {
    code: string;
    message: string;
    details?: any;
  };
  metadata?: {
    timestamp: string;
    requestId: string;
  };
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// Securities Trading Types

export interface TreasuryBill {
  id: string;
  issueNumber: string;
  tenor: 91 | 182 | 364;
  issueDate: string;
  maturityDate: string;
  rate: number;
  minimumInvestment: number;
  availableAmount: number;
}

export interface Bond {
  id: string;
  name: string;
  isin: string;
  couponRate: number;
  faceValue: number;
  issueDate: string;
  maturityDate: string;
  frequency: 'SEMI_ANNUAL' | 'ANNUAL';
  minimumInvestment: number;
}

export interface Stock {
  id: string;
  symbol: string;
  name: string;
  currentPrice: number;
  change: number;
  changePercent: number;
  priceChange: number; // Alias for changePercent for backward compatibility
  volume: number;
  marketCap: number;
  openPrice: number;
  highPrice: number;
  lowPrice: number;
  bidPrice: number;
  bidVolume: number;
  askPrice: number;
  askVolume: number;
}

export interface SecurityOrder {
  securityId: string;
  securityType: 'TBILL' | 'BOND' | 'STOCK';
  orderType: 'BUY' | 'SELL';
  quantity: number;
  price?: number; // For stocks
  bidType?: 'COMPETITIVE' | 'NON_COMPETITIVE'; // For T-Bills
  totalAmount?: number; // Total order amount
}

export interface Portfolio {
  securities: PortfolioSecurity[];
  totalValue: number;
  unrealizedGain: number;
  yieldToMaturity: number;
}

export interface PortfolioSecurity {
  securityId: string;
  securityType: 'TBILL' | 'BOND' | 'STOCK';
  name: string;
  quantity: number;
  purchasePrice: number;
  currentPrice: number;
  marketValue: number;
  unrealizedGain: number;
  maturityDate?: string;
}

// Government Lending Types

export interface LoanApplication {
  id: string;
  applicationNumber: string;
  governmentEntity: GovernmentEntity;
  loanType: 'DEVELOPMENT' | 'WORKING_CAPITAL' | 'INFRASTRUCTURE';
  requestedAmount: number;
  tenor: number; // months
  purpose: string;
  status: 'PENDING' | 'UNDER_REVIEW' | 'APPROVED' | 'REJECTED' | 'DISBURSED';
  submittedDate: string;
  creditAssessment?: CreditAssessment;
}

export interface CreditAssessment {
  sovereignRating?: string;
  revenueStreams: RevenueStream[];
  existingDebt: number;
  debtServiceRatio: number;
  recommendation: 'APPROVE' | 'REJECT' | 'CONDITIONAL';
  comments: string;
  assessedBy: string;
  assessedDate: string;
}

export interface RevenueStream {
  source: string;
  annualAmount: number;
  reliability: 'HIGH' | 'MEDIUM' | 'LOW';
}

export type LoanStatus = 'ACTIVE' | 'CLOSED' | 'DEFAULT';

export interface Loan {
  id: string;
  loanNumber: string;
  governmentEntity: GovernmentEntity;
  principalAmount: number;
  interestRate: number;
  tenor: number;
  disbursementDate: string;
  maturityDate: string;
  outstandingBalance: number;
  status: LoanStatus;
  repaymentSchedule: RepaymentSchedule[];
}

export interface RepaymentSchedule {
  installmentNumber: number;
  dueDate: string;
  principalAmount: number;
  interestAmount: number;
  totalAmount: number;
  paidAmount: number;
  status: 'PENDING' | 'PAID' | 'OVERDUE';
}

// Government Banking Types

export interface GovernmentAccount {
  id: string;
  accountNumber: string;
  accountName: string;
  governmentEntity: GovernmentEntity;
  accountType: 'CURRENT' | 'SAVINGS' | 'REVENUE_COLLECTION';
  balance: number;
  currency: string;
  status: 'ACTIVE' | 'DORMANT' | 'CLOSED';
}

export interface BulkPayment {
  id: string;
  batchNumber: string;
  fromAccountId: string;
  paymentType: 'SALARY' | 'SUPPLIER' | 'PENSION' | 'OTHER';
  totalAmount: number;
  totalCount: number;
  uploadedDate: string;
  processedDate?: string;
  status: 'UPLOADED' | 'VALIDATED' | 'PROCESSING' | 'COMPLETED' | 'FAILED';
  payments: Payment[];
}

export interface Payment {
  beneficiaryName: string;
  beneficiaryAccount: string;
  beneficiaryBank: string;
  amount: number;
  narration: string;
  status: 'PENDING' | 'SUCCESS' | 'FAILED';
  errorMessage?: string;
}

export interface RevenueCollection {
  id: string;
  collectionDate: string;
  revenueType: 'TAX' | 'FEE' | 'LICENSE' | 'FINE' | 'OTHER';
  amount: number;
  payerName: string;
  payerReference: string;
  accountId: string;
  reconciled: boolean;
}

// Grants & Philanthropy Types

export interface GrantProgram {
  id: string;
  name: string;
  description: string;
  category: 'EDUCATION' | 'HEALTH' | 'INFRASTRUCTURE' | 'ENVIRONMENT' | 'OTHER';
  maxAmount: number;
  eligibilityCriteria: string[];
  applicationDeadline: string;
  status: 'OPEN' | 'CLOSED';
}

export interface GrantApplication {
  id: string;
  applicationNumber: string;
  programId: string;
  applicantName: string;
  applicantType: 'NGO' | 'COMMUNITY_GROUP' | 'INSTITUTION' | 'INDIVIDUAL';
  requestedAmount: number;
  projectTitle: string;
  projectDescription: string;
  expectedImpact: string;
  submittedDate: string;
  status: 'SUBMITTED' | 'UNDER_REVIEW' | 'APPROVED' | 'REJECTED' | 'DISBURSED';
  approvals: Approval[];
}

export interface Approval {
  approverName: string;
  approverRole: string;
  decision: 'APPROVED' | 'REJECTED';
  comments: string;
  approvedDate: string;
}

export interface Grant {
  id: string;
  grantNumber: string;
  applicationId: string;
  approvedAmount: number;
  disbursedAmount: number;
  disbursementDate: string;
  utilizationReports: UtilizationReport[];
  complianceStatus: 'COMPLIANT' | 'NON_COMPLIANT' | 'PENDING_REPORT';
}

export interface UtilizationReport {
  reportingPeriod: string;
  amountUtilized: number;
  activities: string;
  outcomes: string;
  challenges: string;
  submittedDate: string;
}

// Dashboard Types

export interface DashboardMetrics {
  securitiesPortfolio: {
    totalValue: number;
    tbillsValue: number;
    bondsValue: number;
    stocksValue: number;
    yieldToMaturity: number;
  };
  loanPortfolio: {
    totalOutstanding: number;
    nationalGovernment: number;
    countyGovernments: number;
    nplRatio: number;
    exposureUtilization: number;
  };
  banking: {
    totalAccounts: number;
    totalBalance: number;
    monthlyTransactions: number;
    revenueCollected: number;
  };
  grants: {
    totalDisbursed: number;
    activeGrants: number;
    beneficiaries: number;
    complianceRate: number;
  };
}
