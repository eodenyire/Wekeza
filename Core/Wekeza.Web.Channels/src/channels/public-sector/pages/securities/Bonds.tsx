import { useState, useEffect } from 'react';
import { Calendar, TrendingUp, DollarSign, AlertCircle, Percent } from 'lucide-react';
import { Bond, SecurityOrder, ApiResponse } from '../../types';

export default function Bonds() {
  const [bonds, setBonds] = useState<Bond[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedBond, setSelectedBond] = useState<Bond | null>(null);
  const [showOrderForm, setShowOrderForm] = useState(false);
  const [orderForm, setOrderForm] = useState({
    quantity: '',
    units: ''
  });
  const [submitting, setSubmitting] = useState(false);
  const [orderSuccess, setOrderSuccess] = useState(false);

  useEffect(() => {
    fetchBonds();
  }, []);

  const fetchBonds = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await fetch('/api/public-sector/securities/bonds');
      const data: ApiResponse<Bond[]> = await response.json();
      
      if (data.success && data.data) {
        setBonds(data.data);
      } else {
        setError(data.error?.message || 'Failed to load Bonds');
      }
    } catch (err) {
      setError('Failed to connect to server');
    } finally {
      setLoading(false);
    }
  };

  const handlePlaceOrder = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedBond) return;

    try {
      setSubmitting(true);
      setError(null);

      const order: SecurityOrder = {
        securityId: selectedBond.id,
        securityType: 'BOND',
        orderType: 'BUY',
        quantity: parseFloat(orderForm.units)
      };

      const response = await fetch('/api/public-sector/securities/bonds/order', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(order)
      });

      const data: ApiResponse<any> = await response.json();

      if (data.success) {
        setOrderSuccess(true);
        setShowOrderForm(false);
        setOrderForm({ quantity: '', units: '' });
        setSelectedBond(null);
        // Refresh the list
        fetchBonds();
      } else {
        setError(data.error?.message || 'Failed to place order');
      }
    } catch (err) {
      setError('Failed to submit order');
    } finally {
      setSubmitting(false);
    }
  };

  const openOrderForm = (bond: Bond) => {
    setSelectedBond(bond);
    setShowOrderForm(true);
    setOrderSuccess(false);
    setError(null);
  };

  const closeOrderForm = () => {
    setShowOrderForm(false);
    setSelectedBond(null);
    setOrderForm({ quantity: '', units: '' });
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

  const calculateYearsToMaturity = (maturityDate: string) => {
    const days = calculateDaysToMaturity(maturityDate);
    return (days / 365).toFixed(1);
  };

  const calculateAccruedInterest = (bond: Bond, units: number) => {
    if (!units || units <= 0) return 0;
    
    const today = new Date();
    const issueDate = new Date(bond.issueDate);
    const maturityDate = new Date(bond.maturityDate);
    
    // Calculate days since last coupon payment
    // For simplicity, we'll calculate from issue date
    const daysSinceIssue = Math.floor((today.getTime() - issueDate.getTime()) / (1000 * 60 * 60 * 24));
    
    // Calculate days in coupon period
    const daysInYear = 365;
    const periodsPerYear = bond.frequency === 'SEMI_ANNUAL' ? 2 : 1;
    const daysInPeriod = daysInYear / periodsPerYear;
    
    // Calculate accrued interest
    const daysSinceLastCoupon = daysSinceIssue % daysInPeriod;
    const annualCoupon = (bond.couponRate / 100) * bond.faceValue;
    const periodCoupon = annualCoupon / periodsPerYear;
    const accruedInterest = (periodCoupon * daysSinceLastCoupon) / daysInPeriod;
    
    return accruedInterest * units;
  };

  const calculateTotalCost = (bond: Bond, units: number) => {
    if (!units || units <= 0) return 0;
    const principal = bond.faceValue * units;
    const accruedInterest = calculateAccruedInterest(bond, units);
    return principal + accruedInterest;
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
        <h1 className="text-3xl font-bold">Government Bonds</h1>
        <p className="text-gray-600">Invest in long-term government securities with regular coupon payments</p>
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
              Bond order placed successfully! Your order is being processed.
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

      {/* Bonds Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {bonds.map((bond) => (
          <div key={bond.id} className="card hover:shadow-lg transition-shadow">
            <div className="flex justify-between items-start mb-4">
              <div>
                <h3 className="text-lg font-bold">{bond.name}</h3>
                <p className="text-sm text-gray-600">ISIN: {bond.isin}</p>
              </div>
              <div className="bg-wekeza-blue/10 px-3 py-1 rounded-full">
                <span className="text-wekeza-blue font-semibold">{bond.couponRate}%</span>
              </div>
            </div>

            <div className="space-y-3 mb-4">
              <div className="flex items-center text-sm">
                <Percent className="h-4 w-4 text-gray-400 mr-2" />
                <span className="text-gray-600">Coupon Rate:</span>
                <span className="ml-auto font-medium">{bond.couponRate}%</span>
              </div>
              <div className="flex items-center text-sm">
                <DollarSign className="h-4 w-4 text-gray-400 mr-2" />
                <span className="text-gray-600">Face Value:</span>
                <span className="ml-auto font-medium">KES {bond.faceValue.toLocaleString()}</span>
              </div>
              <div className="flex items-center text-sm">
                <Calendar className="h-4 w-4 text-gray-400 mr-2" />
                <span className="text-gray-600">Issue Date:</span>
                <span className="ml-auto font-medium">{formatDate(bond.issueDate)}</span>
              </div>
              <div className="flex items-center text-sm">
                <Calendar className="h-4 w-4 text-gray-400 mr-2" />
                <span className="text-gray-600">Maturity:</span>
                <span className="ml-auto font-medium">{formatDate(bond.maturityDate)}</span>
              </div>
              <div className="flex items-center text-sm">
                <TrendingUp className="h-4 w-4 text-gray-400 mr-2" />
                <span className="text-gray-600">Years to Maturity:</span>
                <span className="ml-auto font-medium">{calculateYearsToMaturity(bond.maturityDate)}</span>
              </div>
              <div className="flex items-center text-sm">
                <Calendar className="h-4 w-4 text-gray-400 mr-2" />
                <span className="text-gray-600">Payment Frequency:</span>
                <span className="ml-auto font-medium">
                  {bond.frequency === 'SEMI_ANNUAL' ? 'Semi-Annual' : 'Annual'}
                </span>
              </div>
              <div className="flex items-center text-sm">
                <DollarSign className="h-4 w-4 text-gray-400 mr-2" />
                <span className="text-gray-600">Min. Investment:</span>
                <span className="ml-auto font-medium">KES {bond.minimumInvestment.toLocaleString()}</span>
              </div>
            </div>

            <button
              onClick={() => openOrderForm(bond)}
              className="w-full btn-primary"
            >
              Place Order
            </button>
          </div>
        ))}
      </div>

      {/* Empty State */}
      {!loading && bonds.length === 0 && (
        <div className="card text-center py-12">
          <TrendingUp className="h-12 w-12 text-gray-400 mx-auto mb-4" />
          <h3 className="text-lg font-medium text-gray-900 mb-2">No Bonds Available</h3>
          <p className="text-gray-600">There are currently no government bonds available for investment.</p>
        </div>
      )}

      {/* Order Form Modal */}
      {showOrderForm && selectedBond && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg max-w-md w-full p-6 max-h-[90vh] overflow-y-auto">
            <h2 className="text-2xl font-bold mb-4">Place Bond Order</h2>
            
            <div className="bg-gray-50 p-4 rounded-lg mb-6">
              <h3 className="font-semibold mb-2">{selectedBond.name}</h3>
              <div className="space-y-1 text-sm">
                <div className="flex justify-between">
                  <span className="text-gray-600">ISIN:</span>
                  <span className="font-medium">{selectedBond.isin}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Coupon Rate:</span>
                  <span className="font-medium">{selectedBond.couponRate}%</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Face Value:</span>
                  <span className="font-medium">KES {selectedBond.faceValue.toLocaleString()}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Maturity:</span>
                  <span className="font-medium">{formatDate(selectedBond.maturityDate)}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Payment Frequency:</span>
                  <span className="font-medium">
                    {selectedBond.frequency === 'SEMI_ANNUAL' ? 'Semi-Annual' : 'Annual'}
                  </span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Min. Investment:</span>
                  <span className="font-medium">KES {selectedBond.minimumInvestment.toLocaleString()}</span>
                </div>
              </div>
            </div>

            <form onSubmit={handlePlaceOrder} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Number of Units
                </label>
                <input
                  type="number"
                  value={orderForm.units}
                  onChange={(e) => setOrderForm({ ...orderForm, units: e.target.value })}
                  min="1"
                  step="1"
                  required
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-wekeza-blue focus:border-transparent"
                  placeholder="Enter number of units"
                />
                <p className="text-xs text-gray-500 mt-1">
                  Each unit = KES {selectedBond.faceValue.toLocaleString()}
                </p>
              </div>

              {/* Cost Breakdown */}
              {orderForm.units && parseFloat(orderForm.units) > 0 && (
                <div className="bg-blue-50 p-4 rounded-lg space-y-2">
                  <h4 className="font-semibold text-sm text-gray-900 mb-2">Cost Breakdown</h4>
                  <div className="space-y-1 text-sm">
                    <div className="flex justify-between">
                      <span className="text-gray-600">Principal Amount:</span>
                      <span className="font-medium">
                        KES {(selectedBond.faceValue * parseFloat(orderForm.units)).toLocaleString()}
                      </span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Accrued Interest:</span>
                      <span className="font-medium">
                        KES {calculateAccruedInterest(selectedBond, parseFloat(orderForm.units)).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                      </span>
                    </div>
                    <div className="border-t border-blue-200 pt-2 mt-2">
                      <div className="flex justify-between">
                        <span className="font-semibold text-gray-900">Total Cost:</span>
                        <span className="font-bold text-wekeza-blue">
                          KES {calculateTotalCost(selectedBond, parseFloat(orderForm.units)).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                        </span>
                      </div>
                    </div>
                  </div>
                  <p className="text-xs text-gray-600 mt-2">
                    Accrued interest is calculated from the last coupon payment date to today.
                  </p>
                </div>
              )}

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
                  className="flex-1 btn-primary"
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
