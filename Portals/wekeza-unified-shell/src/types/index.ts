// User and Authentication Types
export interface User {
  id: string;
  username: string;
  email: string;
  fullName: string;
  roles: UserRole[];
  permissions: string[];
  branchCode?: string;
  branchName?: string;
  department?: string;
  position?: string;
  profileImage?: string;
}

export type UserRole = 
  | 'SystemAdministrator'
  | 'ITSecurityAdmin'
  | 'CoreBankingAdmin'
  | 'CEO'
  | 'CFO'
  | 'CRO'
  | 'COO'
  | 'BoardMember'
  | 'BranchManager'
  | 'RegionalManager'
  | 'VaultOfficer'
  | 'OperationsOfficer'
  | 'Teller'
  | 'Supervisor'
  | 'AMLOfficer'
  | 'FraudAnalyst'
  | 'RiskOfficer'
  | 'ComplianceManager'
  | 'TreasuryDealer'
  | 'LiquidityManager'
  | 'TradeFinanceOfficer'
  | 'CorporateBankingOfficer'
  | 'ProductManager'
  | 'FinanceController'
  | 'PaymentsOfficer'
  | 'ClearingOfficer'
  | 'RetailCustomer'
  | 'SMECustomer';

export interface LoginCredentials {
  username: string;
  password: string;
  rememberMe?: boolean;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  user: User;
  expiresIn: number;
}

// Portal Types
export type PortalType =
  | 'admin'
  | 'executive'
  | 'branch-manager'
  | 'branch-operations'
  | 'teller'
  | 'supervisor'
  | 'compliance'
  | 'treasury'
  | 'trade-finance'
  | 'product-gl'
  | 'payments'
  | 'customer'
  | 'staff'
  | 'workflow';

export interface Portal {
  id: PortalType;
  name: string;
  description: string;
  icon: string;
  route: string;
  allowedRoles: UserRole[];
  color: string;
}

export interface MenuItem {
  key: string;
  label: string;
  icon?: string;
  path?: string;
  children?: MenuItem[];
  roles?: UserRole[];
}

// API Response Types
export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

export interface PaginatedResponse<T> {
  items: T[];
  totalItems: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// Common Data Types
export interface Dashboard {
  title: string;
  widgets: DashboardWidget[];
  lastUpdated: Date;
}

export interface DashboardWidget {
  id: string;
  type: 'metric' | 'chart' | 'table' | 'alert';
  title: string;
  data: any;
  position: { x: number; y: number; w: number; h: number };
}

export interface Notification {
  id: string;
  type: 'info' | 'success' | 'warning' | 'error';
  title: string;
  message: string;
  timestamp: Date;
  read: boolean;
  actionUrl?: string;
}
