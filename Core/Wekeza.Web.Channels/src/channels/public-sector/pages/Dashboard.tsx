import { useState, useEffect } from 'react';
import { DollarSign, TrendingUp, Building2, Gift, PieChart as PieChartIcon, BarChart3, Activity, Download } from 'lucide-react';
import { DashboardMetrics, ApiResponse } from '../types';
import { PieChart, Pie, Cell, BarChart, Bar, LineChart, Line, AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

export default function Dashboard() {
  const [metrics, setMetrics] = useState<DashboardMetrics | null>(null);
  const [revenueTrends, setRevenueTrends] = useState<any[]>([]);
  const [grantTrends, setGrantTrends] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem('auth_token');
      const apiUrl = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';
      
      // Fetch dashboard metrics
      const metricsResponse = await fetch(`${apiUrl}/public-sector/dashboard/metrics`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      if (!metricsResponse.ok) {
        throw new Error(`HTTP error! status: ${metricsResponse.status}`);
      }
      
      const metricsData: ApiResponse<DashboardMetrics> = await metricsResponse.json();
      
      if (metricsData.success && metricsData.data) {
        setMetrics(metricsData.data);
      }

      // Fetch revenue trends
      const revenueTrendsResponse = await fetch(`${apiUrl}/public-sector/dashboard/revenue-trends`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      if (revenueTrendsResponse.ok) {
        const revenueTrendsData = await revenueTrendsResponse.json();
        if (revenueTrendsData.success && revenueTrendsData.data) {
          setRevenueTrends(revenueTrendsData.data);
        }
      }

      // Fetch grant trends
      const grantTrendsResponse = await fetch(`${apiUrl}/public-sector/dashboard/grant-trends`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      if (grantTrendsResponse.ok) {
        const grantTrendsData = await grantTrendsResponse.json();
        if (grantTrendsData.success && grantTrendsData.data) {
          setGrantTrends(grantTrendsData.data);
        }
      }
    } catch (err) {
      console.error('Error fetching dashboard data:', err);
    } finally {
      setLoading(false);
    }
  };

  if (loading || !metrics) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  const portfolioComposition = [
    { name: 'T-Bills', value: metrics.securitiesPortfolio.tbillsValue, color: '#3B82F6' },
    { name: 'Bonds', value: metrics.securitiesPortfolio.bondsValue, color: '#8B5CF6' },
    { name: 'Stocks', value: metrics.securitiesPortfolio.stocksValue, color: '#10B981' }
  ];

  const loanExposure = [
    { name: 'National Govt', value: metrics.loanPortfolio.nationalGovernment },
    { name: 'County Govts', value: metrics.loanPortfolio.countyGovernments }
  ];

  // Use real revenue data from API or fallback to empty array
  const revenueData = revenueTrends.length > 0 
    ? revenueTrends.map(trend => ({
        month: trend.month,
        revenue: trend.revenue / 1000000 // Convert to millions for better chart display
      }))
    : [];

  // Use real grant data from API or fallback to empty array
  const grantImpact = grantTrends.length > 0
    ? grantTrends.map(trend => ({
        month: trend.month,
        disbursed: trend.disbursed / 1000000 // Convert to millions for better chart display
      }))
    : [];

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Public Sector Dashboard</h1>
          <p className="text-gray-600 mt-1">Overview of government banking activities</p>
        </div>
        <button className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
          <Download className="w-4 h-4" />
          Export Report
        </button>
      </div>

      {/* Key Metrics Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div className="bg-white rounded-lg shadow-md p-6">
          <div className="flex items-center justify-between mb-2">
            <span className="text-gray-600">Securities Portfolio</span>
            <PieChartIcon className="w-5 h-5 text-blue-600" />
          </div>
          <p className="text-3xl font-bold text-gray-900">
            KES {metrics.securitiesPortfolio.totalValue.toLocaleString()}
          </p>
          <p className="text-sm text-gray-600 mt-2">
            YTM: {metrics.securitiesPortfolio.yieldToMaturity.toFixed(2)}%
          </p>
        </div>

        <div className="bg-white rounded-lg shadow-md p-6">
          <div className="flex items-center justify-between mb-2">
            <span className="text-gray-600">Loan Portfolio</span>
            <TrendingUp className="w-5 h-5 text-green-600" />
          </div>
          <p className="text-3xl font-bold text-gray-900">
            KES {metrics.loanPortfolio.totalOutstanding.toLocaleString()}
          </p>
          <p className="text-sm text-gray-600 mt-2">
            NPL Ratio: {metrics.loanPortfolio.nplRatio.toFixed(1)}%
          </p>
        </div>

        <div className="bg-white rounded-lg shadow-md p-6">
          <div className="flex items-center justify-between mb-2">
            <span className="text-gray-600">Account Balance</span>
            <Building2 className="w-5 h-5 text-purple-600" />
          </div>
          <p className="text-3xl font-bold text-gray-900">
            KES {metrics.banking.totalBalance.toLocaleString()}
          </p>
          <p className="text-sm text-gray-600 mt-2">
            {metrics.banking.totalAccounts} Accounts
          </p>
        </div>

        <div className="bg-white rounded-lg shadow-md p-6">
          <div className="flex items-center justify-between mb-2">
            <span className="text-gray-600">Grants Disbursed</span>
            <Gift className="w-5 h-5 text-orange-600" />
          </div>
          <p className="text-3xl font-bold text-gray-900">
            KES {metrics.grants.totalDisbursed.toLocaleString()}
          </p>
          <p className="text-sm text-gray-600 mt-2">
            {metrics.grants.beneficiaries} Beneficiaries
          </p>
        </div>
      </div>

      {/* Charts Row 1 */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Securities Portfolio Composition */}
        <div className="bg-white rounded-lg shadow-md p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Securities Portfolio Composition</h2>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={portfolioComposition}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                outerRadius={80}
                fill="#8884d8"
                dataKey="value"
              >
                {portfolioComposition.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={entry.color} />
                ))}
              </Pie>
              <Tooltip formatter={(value: number) => `KES ${value.toLocaleString()}`} />
            </PieChart>
          </ResponsiveContainer>
        </div>

        {/* Loan Portfolio by Entity */}
        <div className="bg-white rounded-lg shadow-md p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Loan Portfolio by Government Entity</h2>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={loanExposure}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis />
              <Tooltip formatter={(value: number) => `KES ${value.toLocaleString()}`} />
              <Bar dataKey="value" fill="#10B981" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Charts Row 2 */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Revenue Trends */}
        <div className="bg-white rounded-lg shadow-md p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Revenue Collection Trends</h2>
          {revenueData.length > 0 ? (
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={revenueData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="month" />
                <YAxis label={{ value: 'KES (Millions)', angle: -90, position: 'insideLeft' }} />
                <Tooltip formatter={(value: number) => `KES ${value.toLocaleString()}M`} />
                <Legend />
                <Line type="monotone" dataKey="revenue" stroke="#3B82F6" strokeWidth={2} name="Revenue" />
              </LineChart>
            </ResponsiveContainer>
          ) : (
            <div className="flex items-center justify-center h-[300px] text-gray-500">
              No revenue data available
            </div>
          )}
        </div>

        {/* Grant Impact */}
        <div className="bg-white rounded-lg shadow-md p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Grant Impact Metrics</h2>
          {grantImpact.length > 0 ? (
            <ResponsiveContainer width="100%" height={300}>
              <AreaChart data={grantImpact}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="month" />
                <YAxis label={{ value: 'KES (Millions)', angle: -90, position: 'insideLeft' }} />
                <Tooltip formatter={(value: number) => `KES ${value.toLocaleString()}M`} />
                <Legend />
                <Area type="monotone" dataKey="disbursed" stackId="1" stroke="#8B5CF6" fill="#8B5CF6" name="Disbursed" />
              </AreaChart>
            </ResponsiveContainer>
          ) : (
            <div className="flex items-center justify-center h-[300px] text-gray-500">
              No grant data available
            </div>
          )}
        </div>
      </div>

      {/* Risk Metrics */}
      <div className="bg-white rounded-lg shadow-md p-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Risk & Compliance Metrics</h2>
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
          <div>
            <p className="text-sm text-gray-600 mb-1">Exposure Utilization</p>
            <p className="text-2xl font-bold text-gray-900">
              {metrics.loanPortfolio.exposureUtilization.toFixed(1)}%
            </p>
            <div className="w-full bg-gray-200 rounded-full h-2 mt-2">
              <div
                className="bg-blue-600 h-2 rounded-full"
                style={{ width: `${metrics.loanPortfolio.exposureUtilization}%` }}
              ></div>
            </div>
          </div>
          <div>
            <p className="text-sm text-gray-600 mb-1">Grant Compliance Rate</p>
            <p className="text-2xl font-bold text-green-600">
              {metrics.grants.complianceRate.toFixed(1)}%
            </p>
            <div className="w-full bg-gray-200 rounded-full h-2 mt-2">
              <div
                className="bg-green-600 h-2 rounded-full"
                style={{ width: `${metrics.grants.complianceRate}%` }}
              ></div>
            </div>
          </div>
          <div>
            <p className="text-sm text-gray-600 mb-1">Monthly Transactions</p>
            <p className="text-2xl font-bold text-gray-900">
              {metrics.banking.monthlyTransactions.toLocaleString()}
            </p>
          </div>
          <div>
            <p className="text-sm text-gray-600 mb-1">Active Grants</p>
            <p className="text-2xl font-bold text-gray-900">
              {metrics.grants.activeGrants}
            </p>
          </div>
        </div>
      </div>

      {/* Recent Activities */}
      <div className="bg-white rounded-lg shadow-md p-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
          <Activity className="w-5 h-5" />
          Recent Activities
        </h2>
        <div className="space-y-3">
          {[
            { action: 'Loan Disbursed', entity: 'Nairobi County', amount: 50000000, time: '2 hours ago' },
            { action: 'T-Bill Order Placed', entity: 'National Treasury', amount: 100000000, time: '5 hours ago' },
            { action: 'Grant Approved', entity: 'Education Initiative', amount: 5000000, time: '1 day ago' },
            { action: 'Bulk Payment Processed', entity: 'Mombasa County', amount: 25000000, time: '2 days ago' }
          ].map((activity, index) => (
            <div key={index} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
              <div>
                <p className="font-medium text-gray-900">{activity.action}</p>
                <p className="text-sm text-gray-600">{activity.entity}</p>
              </div>
              <div className="text-right">
                <p className="font-semibold text-gray-900">KES {activity.amount.toLocaleString()}</p>
                <p className="text-sm text-gray-500">{activity.time}</p>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
