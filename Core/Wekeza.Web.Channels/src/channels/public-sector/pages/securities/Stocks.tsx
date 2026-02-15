import { useState, useEffect } from 'react';
import { TrendingUp, TrendingDown, DollarSign, AlertCircle, BarChart3, Activity } from 'lucide-react';
import { Stock, SecurityOrder, ApiResponse } from '../../types';

export default function Stocks() {
  const [stocks, setStocks] = useState<Stock[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedStock, setSelectedStock] = useState<Stock | null>(null);
  const [showOrderForm, setShowOrderForm] = useState(false);
  const [orderForm, setOrderForm] = useState({
    orderType: 'BUY' as 'BUY' | 'SELL',
    quantity: '',
    price: ''
  });
  const [submitting, setSubmitting] = useState(false);
  const [orderSuccess, setOrderSuccess] = useState(false);

  useEffect(() => {
    fetchStocks();
    
    // Set up polling for real-time price updates every 30 seconds
    const interval = setInterval(() => {
      fetchStocks();
    }, 30000);

    return () => clearInterval(interval);
  }, []);

  const fetchStocks = async () => {
    try {
      // Only show loading spinner on initial load, not on polling updates
      if (stocks.length === 0) {
        setLoading(true);
      }
      setError(null);
      
      const response = await fetch('/api/public-sector/securities/stocks');
      const data: ApiResponse<Stock[]> = await response.json();
      
      if (data.success && data.data) {
        setStocks(data.data);
      } else {
        setError(data.message || 'Failed to fetch stocks');
      }
    } catch (err) {
      setError('Failed to fetch stocks. Please try again.');
      console.error('Error fetching stocks:', err);
    } finally {
      setLoading(false);
    }
  };

  const handlePlaceOrder = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedStock) return;

    setSubmitting(true);
    setError(null);

    try {
      const order: Partial<SecurityOrder> = {
        securityId: selectedStock.id,
        securityType: 'STOCK',
        orderType: orderForm.orderType,
        quantity: parseInt(orderForm.quantity),
        price: parseFloat(orderForm.price),
        totalAmount: parseInt(orderForm.quantity) * parseFloat(orderForm.price)
      };

      const response = await fetch('/api/public-sector/securities/stocks/order', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(order)
      });

      const data: ApiResponse<SecurityOrder> = await response.json();

      if (data.success) {
        setOrderSuccess(true);
        setOrderForm({ orderType: 'BUY', quantity: '', price: '' });
        setTimeout(() => {
          setShowOrderForm(false);
          setOrderSuccess(false);
          setSelectedStock(null);
        }, 2000);
      } else {
        setError(data.message || 'Failed to place order');
      }
    } catch (err) {
      setError('Failed to place order. Please try again.');
      console.error('Error placing order:', err);
    } finally {
      setSubmitting(false);
    }
  };

  const openOrderForm = (stock: Stock, type: 'BUY' | 'SELL') => {
    setSelectedStock(stock);
    setOrderForm({ ...orderForm, orderType: type, price: stock.currentPrice.toString() });
    setShowOrderForm(true);
    setOrderSuccess(false);
    setError(null);
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600" role="status" aria-label="Loading stocks"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Government Stocks</h1>
          <p className="text-gray-600 mt-1">NSE-listed government stocks with real-time pricing</p>
        </div>
        <div className="flex items-center gap-2 text-sm text-gray-500">
          <Activity className="w-4 h-4 animate-pulse text-green-500" />
          <span>Live prices (updates every 30s)</span>
        </div>
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 flex items-start gap-3">
          <AlertCircle className="w-5 h-5 text-red-600 flex-shrink-0 mt-0.5" />
          <div>
            <h3 className="font-medium text-red-900">Error</h3>
            <p className="text-red-700 text-sm mt-1">{error}</p>
          </div>
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {stocks.map((stock) => (
          <div key={stock.id} className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
            <div className="flex justify-between items-start mb-4">
              <div>
                <h3 className="text-lg font-semibold text-gray-900">{stock.name}</h3>
                <p className="text-sm text-gray-500">{stock.symbol}</p>
              </div>
              <div className={`flex items-center gap-1 px-3 py-1 rounded-full text-sm font-medium ${
                stock.priceChange >= 0 
                  ? 'bg-green-100 text-green-700' 
                  : 'bg-red-100 text-red-700'
              }`}>
                {stock.priceChange >= 0 ? (
                  <TrendingUp className="w-4 h-4" />
                ) : (
                  <TrendingDown className="w-4 h-4" />
                )}
                {stock.priceChange >= 0 ? '+' : ''}{stock.priceChange.toFixed(2)}%
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4 mb-4">
              <div>
                <p className="text-sm text-gray-500">Current Price</p>
                <p className="text-2xl font-bold text-gray-900">
                  KES {stock.currentPrice.toLocaleString()}
                </p>
              </div>
              <div>
                <p className="text-sm text-gray-500">Volume</p>
                <p className="text-lg font-semibold text-gray-700">
                  {stock.volume.toLocaleString()}
                </p>
              </div>
            </div>

            <div className="grid grid-cols-3 gap-3 mb-4 text-sm">
              <div>
                <p className="text-gray-500">Open</p>
                <p className="font-medium text-gray-900">KES {stock.openPrice.toLocaleString()}</p>
              </div>
              <div>
                <p className="text-gray-500">High</p>
                <p className="font-medium text-green-600">KES {stock.highPrice.toLocaleString()}</p>
              </div>
              <div>
                <p className="text-gray-500">Low</p>
                <p className="font-medium text-red-600">KES {stock.lowPrice.toLocaleString()}</p>
              </div>
            </div>

            <div className="border-t pt-4">
              <div className="flex items-center justify-between mb-3">
                <div className="flex items-center gap-2">
                  <BarChart3 className="w-4 h-4 text-gray-400" />
                  <span className="text-sm font-medium text-gray-700">Order Book</span>
                </div>
              </div>
              <div className="grid grid-cols-2 gap-3 text-sm">
                <div className="bg-green-50 rounded p-2">
                  <p className="text-gray-600 mb-1">Bid</p>
                  <p className="font-semibold text-green-700">KES {stock.bidPrice.toLocaleString()}</p>
                  <p className="text-xs text-gray-500">{stock.bidVolume.toLocaleString()} shares</p>
                </div>
                <div className="bg-red-50 rounded p-2">
                  <p className="text-gray-600 mb-1">Ask</p>
                  <p className="font-semibold text-red-700">KES {stock.askPrice.toLocaleString()}</p>
                  <p className="text-xs text-gray-500">{stock.askVolume.toLocaleString()} shares</p>
                </div>
              </div>
            </div>

            <div className="flex gap-3 mt-4">
              <button
                onClick={() => openOrderForm(stock, 'BUY')}
                className="flex-1 bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors font-medium"
              >
                Buy
              </button>
              <button
                onClick={() => openOrderForm(stock, 'SELL')}
                className="flex-1 bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 transition-colors font-medium"
              >
                Sell
              </button>
            </div>
          </div>
        ))}
      </div>

      {stocks.length === 0 && !loading && (
        <div className="text-center py-12">
          <BarChart3 className="w-16 h-16 text-gray-300 mx-auto mb-4" />
          <h3 className="text-lg font-medium text-gray-900 mb-2">No stocks available</h3>
          <p className="text-gray-500">There are currently no government stocks listed.</p>
        </div>
      )}

      {/* Order Form Modal */}
      {showOrderForm && selectedStock && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg shadow-xl max-w-md w-full p-6">
            <h2 className="text-xl font-bold text-gray-900 mb-4">
              {orderForm.orderType} {selectedStock.symbol}
            </h2>

            {orderSuccess ? (
              <div className="text-center py-8">
                <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
                  <svg className="w-8 h-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                  </svg>
                </div>
                <h3 className="text-lg font-semibold text-gray-900 mb-2">Order Placed Successfully!</h3>
                <p className="text-gray-600">Your order has been submitted for processing.</p>
              </div>
            ) : (
              <form onSubmit={handlePlaceOrder} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Order Type
                  </label>
                  <div className="flex gap-2">
                    <button
                      type="button"
                      onClick={() => setOrderForm({ ...orderForm, orderType: 'BUY' })}
                      className={`flex-1 px-4 py-2 rounded-lg font-medium transition-colors ${
                        orderForm.orderType === 'BUY'
                          ? 'bg-green-600 text-white'
                          : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                      }`}
                    >
                      Buy
                    </button>
                    <button
                      type="button"
                      onClick={() => setOrderForm({ ...orderForm, orderType: 'SELL' })}
                      className={`flex-1 px-4 py-2 rounded-lg font-medium transition-colors ${
                        orderForm.orderType === 'SELL'
                          ? 'bg-red-600 text-white'
                          : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                      }`}
                    >
                      Sell
                    </button>
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Quantity (shares)
                  </label>
                  <input
                    type="number"
                    required
                    min="1"
                    value={orderForm.quantity}
                    onChange={(e) => setOrderForm({ ...orderForm, quantity: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    placeholder="Enter quantity"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Price per Share (KES)
                  </label>
                  <input
                    type="number"
                    required
                    min="0"
                    step="0.01"
                    value={orderForm.price}
                    onChange={(e) => setOrderForm({ ...orderForm, price: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    placeholder="Enter price"
                  />
                </div>

                {orderForm.quantity && orderForm.price && (
                  <div className="bg-blue-50 rounded-lg p-4">
                    <div className="flex items-center gap-2 mb-2">
                      <DollarSign className="w-5 h-5 text-blue-600" />
                      <span className="font-medium text-gray-900">Total Amount</span>
                    </div>
                    <p className="text-2xl font-bold text-blue-600">
                      KES {(parseInt(orderForm.quantity) * parseFloat(orderForm.price)).toLocaleString()}
                    </p>
                  </div>
                )}

                <div className="flex gap-3 pt-4">
                  <button
                    type="button"
                    onClick={() => {
                      setShowOrderForm(false);
                      setSelectedStock(null);
                      setError(null);
                    }}
                    className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-medium"
                    disabled={submitting}
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    disabled={submitting}
                    className={`flex-1 px-4 py-2 rounded-lg font-medium transition-colors ${
                      orderForm.orderType === 'BUY'
                        ? 'bg-green-600 hover:bg-green-700 text-white'
                        : 'bg-red-600 hover:bg-red-700 text-white'
                    } disabled:opacity-50 disabled:cursor-not-allowed`}
                  >
                    {submitting ? 'Placing Order...' : `Place ${orderForm.orderType} Order`}
                  </button>
                </div>
              </form>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
