import { useState, useEffect } from 'react';
import { Calendar, TrendingUp, DollarSign, AlertCircle } from 'lucide-react';
import { api } from '../../utils/apiClient';

interface TreasuryBill {
  id: string;
  securitycode: string;
  name: string;
  issuedate: string;
  maturitydate: string;
  couponrate: number;
  facevalue: number;
  currentprice: number;
  status: string;
}

export default function TreasuryBills() {
  const [tBills, setTBills] = useState<TreasuryBill[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedBill, setSelectedBill] = useState<TreasuryBill | null>(null);
  const [showOrderForm, setShowOrderForm] = useState(false);
  const [orderForm, setOrderForm] = useState({
    quantity: '',
    customerId: '22222222-2222-2222-2222-222222222222' // Default to National Treasury
  });
  const [submitting, setSubmitting] = useState(false);
  const [orderSuccess, setOrderSuccess] = useState(false);

  useEffect(() => {
    fetchTreasuryBills();
  }, []);

  const fetchTreasuryBills = async () => {
    try {
      setLoading(true);
      setError(null);
      const response: any = await api.getTreasuryBills();
      
      if (response.success && response.data) {
        setTBills(response.data);
      } else {
        setError('Failed to load Treasury Bills');
      }
    } catch (err) {
      setError('Failed to connect to server');
      console.error('Error fetching T-Bills:', err);
    } finally {
      setLoading(false);
    }
  };

  const handlePlaceOrder = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedBill) return;

    try {
      setSubmitting(true);
      setError(null);

      const order = {
        customerId: orderForm.customerId,
        securityId: selectedBill.id,
        orderType: 'BUY',
        quantity: parseInt(orderForm.quantity),
        price: selectedBill.currentprice
      };

      const response: any = await api.placeSecurityOrder(order);

      if (response.success) {
        setOrderSuccess(true);
        setShowOrderForm(false);
        setOrderForm({ quantity: '', customerId: '22222222-2222-2222-2222-222222222222' });
        setSelectedBill(null);
        setTimeout(() => setOrderSuccess(false), 5000);
      } else {
        setError('Failed to place order');
      }
    } catch (err) {
      setError('Failed to submit order');
      console.error('Error placing order:', err);
    } finally {
      setSubmitting(false);
    }
  };

  const openOrderForm = (bill: TreasuryBill) => {
    setSelectedBill(bill);
    setShowOrderForm(true);
    setOrderSuccess(false);
    setError(null);
  };

  const closeOrderForm = () => {
    setShowOrderForm(false);
    setSelectedBill(null);
    setOrderForm({ quantity: '', customerId: '22222222-2222-2222-2222-222222222222' });
    setError(null);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-KE', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  const calculateDaysToMaturity = (maturityDate: string) => {
    const today = new Date();
    const maturity = new Date(maturityDate);
    const diffTime = maturity.getTime() - today.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
  };

  const getTenor = (name: string) => {
    if (name.includes('91')) return 91;
    if (name.includes('182')) return 182;
    if (name.includes('364')) return 364;
    return 91;
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-wekeza-blue"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">Treasury Bills</h1>
        <p className="text-gray-600">Invest in short-term government securities</p>
      </div>

      {/* Success Message */}
      {orderSuccess && (
        <div className="bg-green-50 border border-green-200 rounded-lg p-4 flex items-start">
          <div className="flex-shrink-0">
            <svg className="h-5 w-5 text-green-400" viewBox="0 0 20 20" fill="currentColor">
              <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
            </svg>
          </div>
          <div className="ml-3">
            <p className="text-sm font-medium text-green-800">
              Order placed successfully! Your order is being processed.
            </p>
          </div>
        </div>
      )}

      {/* Error Message */}
      {error && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 flex items-start">
          <AlertCircle className="h-5 w-5 text-red-400 flex-shrink-0" />
          <div className="ml-3">
            <p className="text-sm font-medium text-red-800">{error}</p>
          </div>
        </div>
      )}

      {/* Treasury Bills Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {tBills.map((bill) => (
          <div key={bill.id} className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
            <div className="flex justify-between items-start mb-4">
              <div>
                <h3 className="text-lg font-bold">{bill.name}</h3>
                <p className="text-sm text-gray-600">{bill.securitycode}</p>
              </div>
              <div className="bg-blue-100 px-3 py-1 rounded-full">
                <span className="text-blue-600 font-semibold">{bill.couponrate}%</span>
              </div>
            </div>

            <div className="space-y-3 mb-4">
              <div className="flex items-center text-sm">
                <Calendar className="h-4 w-4 text-gray-400 mr-2" />
                <span className="text-gray-600">Issue Date:</span>
                <span className="ml-auto font-medium">{formatDate(bill.issuedate)}</span>
              </div>
              <div className="flex items-center text-sm">
                <Calendar className="h-4 w-4 text-gray-400 mr-2" />
                <span className="text-gray-600">Maturity:</span>
                <span className="ml-auto font-medium">{formatDate(bill.maturitydate)}</span>
              </div>
              <div className="flex items-center text-sm">
                <TrendingUp className="h-4 w-4 text-gray-400 mr-2" />
                <span className="text-gray-600">Days to Maturity:</span>
                <span className="ml-auto font-medium">{calculateDaysToMaturity(bill.maturitydate)}</span>
              </div>
              <div className="flex items-center text-sm">
                <DollarSign className="h-4 w-4 text-gray-400 mr-2" />
                <span className="text-gray-600">Face Value:</span>
                <span className="ml-auto font-medium">KES {bill.facevalue.toLocaleString()}</span>
              </div>
              <div className="flex items-center text-sm">
                <DollarSign className="h-4 w-4 text-gray-400 mr-2" />
                <span className="text-gray-600">Current Price:</span>
                <span className="ml-auto font-medium">KES {bill.currentprice.toLocaleString()}</span>
              </div>
            </div>

            <button
              onClick={() => openOrderForm(bill)}
              className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-colors"
            >
              Place Order
            </button>
          </div>
        ))}
      </div>

      {/* Empty State */}
      {!loading && tBills.length === 0 && (
        <div className="card text-center py-12">
          <TrendingUp className="h-12 w-12 text-gray-400 mx-auto mb-4" />
          <h3 className="text-lg font-medium text-gray-900 mb-2">No Treasury Bills Available</h3>
          <p className="text-gray-600">There are currently no Treasury Bills available for investment.</p>
        </div>
      )}

      {/* Order Form Modal */}
      {showOrderForm && selectedBill && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg max-w-md w-full p-6">
            <h2 className="text-2xl font-bold mb-4">Place T-Bill Order</h2>
            
            <div className="bg-gray-50 p-4 rounded-lg mb-6">
              <h3 className="font-semibold mb-2">{selectedBill.name}</h3>
              <div className="space-y-1 text-sm">
                <div className="flex justify-between">
                  <span className="text-gray-600">Rate:</span>
                  <span className="font-medium">{selectedBill.couponrate}%</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Maturity:</span>
                  <span className="font-medium">{formatDate(selectedBill.maturitydate)}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Price per Unit:</span>
                  <span className="font-medium">KES {selectedBill.currentprice.toLocaleString()}</span>
                </div>
              </div>
            </div>

            <form onSubmit={handlePlaceOrder} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Quantity (Units)
                </label>
                <input
                  type="number"
                  value={orderForm.quantity}
                  onChange={(e) => setOrderForm({ ...orderForm, quantity: e.target.value })}
                  min="1"
                  step="1"
                  required
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Enter quantity"
                />
                {orderForm.quantity && (
                  <p className="mt-2 text-sm text-gray-600">
                    Total: KES {(parseInt(orderForm.quantity) * selectedBill.currentprice).toLocaleString()}
                  </p>
                )}
              </div>

              <div className="flex space-x-3 pt-4">
                <button
                  type="button"
                  onClick={closeOrderForm}
                  className="flex-1 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
                  disabled={submitting}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="flex-1 bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-colors disabled:bg-gray-400"
                  disabled={submitting}
                >
                  {submitting ? 'Submitting...' : 'Place Order'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
