import { useState, useEffect } from 'react';
import { TrendingUp, TrendingDown, Calendar, DollarSign, PieChart, AlertCircle, Download } from 'lucide-react';
import { Portfolio, PortfolioSecurity, ApiResponse } from '../../types';

export default function PortfolioPage() {
  const [portfolio, setPortfolio] = useState<Portfolio | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<'ALL' | 'TBILL' | 'BOND' | 'STOCK'>('ALL');

  useEffect(() => {
    fetchPortfolio();
  }, []);

  const fetchPortfolio = async () => {
    try {
      setLoading(true);
      setError(null);
      
      const response = await fetch('/api/public-sector/securities/portfolio');
      const data: ApiResponse<Portfolio> = await response.json();
      
      if (data.success && data.data) {
        setPortfolio(data.data);
      } else {
        setError(data.error?.message || 'Failed to fetch portfolio');
      }
    } catch (err) {
      setError('Failed to fetch portfolio. Please try again.');
      console.error('Error fetching portfolio:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleExport = () => {
    if (!portfolio) return;
    
    // Create CSV content
    const headers = ['Security Type', 'Name', 'Quantity', 'Purchase Price', 'Current Price', 'Market Value', 'Unrealized Gain', 'Maturity Date'];
    const rows = portfolio.securities.map(sec => [
      sec.securityType,
      sec.name,
      sec.quantity,
      sec.purchasePrice,
      sec.currentPrice,
      sec.marketValue,
      sec.unrealizedGain,
      sec.maturityDate || 'N/A'
    ]);
    
    const csvContent = [
      headers.join(','),
      ...rows.map(row => row.join(','))
    ].join('\n');
    
    // Download CSV
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `portfolio-${new Date().toISOString().split('T')[0]}.csv`;
    a.click();
    window.URL.revokeObjectURL(url);
  };

  const filteredSecurities = portfolio?.securities.filter(sec => 
    filter === 'ALL' || sec.securityType === filter
  ) || [];

  const getSecurityTypeColor = (type: string) => {
    switch (type) {
      case 'TBILL': return 'bg-blue-100 text-blue-700';
      case 'BOND': return 'bg-purple-100 text-purple-700';
      case 'STOCK': return 'bg-green-100 text-green-700';
      default: return 'bg-gray-100 text-gray-700';
    }
  };

  const getUpcomingMaturities = () => {
    if (!portfolio) return [];
    
    const today = new Date();
    const thirtyDaysFromNow = new Date(today.getTime() + 30 * 24 * 60 * 60 * 1000);
    
    return portfolio.securities
      .filter(sec => sec.maturityDate)
      .map(sec => ({
        ...sec,
        maturityDateObj: new Date(sec.maturityDate!)
      }))
      .filter(sec => sec.maturityDateObj >= today && sec.maturityDateObj <= thirtyDaysFromNow)
      .sort((a, b) => a.maturityDateObj.getTime() - b.maturityDateObj.getTime());
  };

  const getPortfolioComposition = () => {
    if (!portfolio) return [];
    
    const composition = portfolio.securities.reduce((acc, sec) => {
      const existing = acc.find(item => item.type === sec.securityType);
      if (existing) {
        existing.value += sec.marketValue;
      } else {
        acc.push({ type: sec.securityType, value: sec.marketValue });
      }
      return acc;
    }, [] as { type: string; value: number }[]);
    
    return composition.map(item => ({
      ...item,
      percentage: (item.value / portfolio.totalValue) * 100
    }));
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600" role="status" aria-label="Loading portfolio"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 border border-red-200 rounded-lg p-4 flex items-start gap-3">
        <AlertCircle className="w-5 h-5 text-red-600 flex-shrink-0 mt-0.5" />
        <div>
          <h3 className="font-medium text-red-900">Error</h3>
          <p className="text-red-700 text-sm mt-1">{error}</p>
        </div>
      </div>
    );
  }

  if (!portfolio || portfolio.securities.length === 0) {
    return (
      <div className="text-center py-12">
        <PieChart className="w-16 h-16 text-gray-300 mx-auto mb-4" />
        <h3 className="text-lg font-medium text-gray-900 mb-2">No securities in portfolio</h3>
        <p className="text-gray-500">Start investing in government securities to build your portfolio.</p>
      </div>
    );
  }

  const upcomingMaturities = getUpcomingMaturities();
  const composition = getPortfolioComposition();

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Securities Portfolio</h1>
          <p className="text-gray-600 mt-1">Consolidated view of all government securities</p>
        </div>
        <button
          onClick={handleExport}
          className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
        >
          <Download className="w-4 h-4" />
          Export Portfolio
        </button>
      </div>

      {/* Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="bg-white rounded-lg shadow-md p-6">
          <div className="flex items-center justify-between mb-2">
            <span className="text-gray-600">Total Portfolio Value</span>
            <DollarSign className="w-5 h-5 text-blue-600" />
          </div>
          <p className="text-3xl font-bold text-gray-900">
            KES {portfolio.totalValue.toLocaleString()}
          </p>
        </div>

        <div className="bg-white rounded-lg shadow-md p-6">
          <div className="flex items-center justify-between mb-2">
            <span className="text-gray-600">Unrealized Gain/Loss</span>
            {portfolio.unrealizedGain >= 0 ? (
              <TrendingUp className="w-5 h-5 text-green-600" />
            ) : (
              <TrendingDown className="w-5 h-5 text-red-600" />
            )}
          </div>
          <p className={`text-3xl font-bold ${portfolio.unrealizedGain >= 0 ? 'text-green-600' : 'text-red-600'}`}>
            {portfolio.unrealizedGain >= 0 ? '+' : ''}KES {portfolio.unrealizedGain.toLocaleString()}
          </p>
        </div>

        <div className="bg-white rounded-lg shadow-md p-6">
          <div className="flex items-center justify-between mb-2">
            <span className="text-gray-600">Yield to Maturity</span>
            <TrendingUp className="w-5 h-5 text-purple-600" />
          </div>
          <p className="text-3xl font-bold text-gray-900">
            {portfolio.yieldToMaturity.toFixed(2)}%
          </p>
        </div>
      </div>

      {/* Portfolio Composition */}
      <div className="bg-white rounded-lg shadow-md p-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
          <PieChart className="w-5 h-5" />
          Portfolio Composition
        </h2>
        <div className="space-y-3">
          {composition.map((item) => (
            <div key={item.type}>
              <div className="flex justify-between text-sm mb-1">
                <span className="font-medium text-gray-700">{item.type}</span>
                <span className="text-gray-600">
                  {item.percentage.toFixed(1)}% (KES {item.value.toLocaleString()})
                </span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div
                  className={`h-2 rounded-full ${
                    item.type === 'TBILL' ? 'bg-blue-600' :
                    item.type === 'BOND' ? 'bg-purple-600' :
                    'bg-green-600'
                  }`}
                  style={{ width: `${item.percentage}%` }}
                ></div>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Upcoming Maturities */}
      {upcomingMaturities.length > 0 && (
        <div className="bg-white rounded-lg shadow-md p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
            <Calendar className="w-5 h-5" />
            Upcoming Maturities (Next 30 Days)
          </h2>
          <div className="space-y-3">
            {upcomingMaturities.map((sec) => (
              <div key={sec.securityId} className="flex items-center justify-between p-3 bg-yellow-50 rounded-lg">
                <div>
                  <p className="font-medium text-gray-900">{sec.name}</p>
                  <p className="text-sm text-gray-600">
                    Matures on {new Date(sec.maturityDate!).toLocaleDateString()}
                  </p>
                </div>
                <div className="text-right">
                  <p className="font-semibold text-gray-900">KES {sec.marketValue.toLocaleString()}</p>
                  <span className={`text-xs px-2 py-1 rounded-full ${getSecurityTypeColor(sec.securityType)}`}>
                    {sec.securityType}
                  </span>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Filter Tabs */}
      <div className="flex gap-2 border-b">
        {['ALL', 'TBILL', 'BOND', 'STOCK'].map((tab) => (
          <button
            key={tab}
            onClick={() => setFilter(tab as any)}
            className={`px-4 py-2 font-medium transition-colors ${
              filter === tab
                ? 'text-blue-600 border-b-2 border-blue-600'
                : 'text-gray-600 hover:text-gray-900'
            }`}
          >
            {tab === 'ALL' ? 'All Securities' : tab}
          </button>
        ))}
      </div>

      {/* Securities List */}
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Security
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Type
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Quantity
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Purchase Price
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Current Price
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Market Value
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Gain/Loss
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Maturity
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredSecurities.map((security) => (
                <tr key={security.securityId} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="font-medium text-gray-900">{security.name}</div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className={`px-2 py-1 text-xs font-medium rounded-full ${getSecurityTypeColor(security.securityType)}`}>
                      {security.securityType}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-gray-900">
                    {security.quantity.toLocaleString()}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-gray-900">
                    KES {security.purchasePrice.toLocaleString()}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-gray-900">
                    KES {security.currentPrice.toLocaleString()}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right font-medium text-gray-900">
                    KES {security.marketValue.toLocaleString()}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right">
                    <span className={`font-medium ${security.unrealizedGain >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                      {security.unrealizedGain >= 0 ? '+' : ''}KES {security.unrealizedGain.toLocaleString()}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                    {security.maturityDate ? new Date(security.maturityDate).toLocaleDateString() : 'N/A'}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {filteredSecurities.length === 0 && (
        <div className="text-center py-8 text-gray-500">
          No securities found for the selected filter.
        </div>
      )}
    </div>
  );
}
