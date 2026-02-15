import { TrendingUp, TrendingDown, DollarSign } from 'lucide-react';

interface SecurityCardProps {
  name: string;
  type: 'TBILL' | 'BOND' | 'STOCK';
  currentPrice: number;
  quantity?: number;
  priceChange?: number;
  maturityDate?: string;
  onBuy?: () => void;
  onSell?: () => void;
}

export default function SecurityCard({
  name,
  type,
  currentPrice,
  quantity,
  priceChange,
  maturityDate,
  onBuy,
  onSell
}: SecurityCardProps) {
  const getTypeColor = () => {
    switch (type) {
      case 'TBILL': return 'bg-blue-100 text-blue-700';
      case 'BOND': return 'bg-purple-100 text-purple-700';
      case 'STOCK': return 'bg-green-100 text-green-700';
      default: return 'bg-gray-100 text-gray-700';
    }
  };

  return (
    <div className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow">
      <div className="flex justify-between items-start mb-4">
        <div>
          <h3 className="text-lg font-semibold text-gray-900">{name}</h3>
          <span className={`inline-block px-2 py-1 text-xs font-medium rounded-full mt-1 ${getTypeColor()}`}>
            {type}
          </span>
        </div>
        {priceChange !== undefined && (
          <div className={`flex items-center gap-1 ${priceChange >= 0 ? 'text-green-600' : 'text-red-600'}`}>
            {priceChange >= 0 ? <TrendingUp className="w-4 h-4" /> : <TrendingDown className="w-4 h-4" />}
            <span className="font-medium">{priceChange >= 0 ? '+' : ''}{priceChange.toFixed(2)}%</span>
          </div>
        )}
      </div>

      <div className="space-y-3 mb-4">
        <div className="flex items-center justify-between">
          <span className="text-sm text-gray-600">Current Price</span>
          <span className="text-lg font-bold text-gray-900">KES {currentPrice.toLocaleString()}</span>
        </div>
        
        {quantity !== undefined && (
          <div className="flex items-center justify-between">
            <span className="text-sm text-gray-600">Quantity</span>
            <span className="font-medium text-gray-900">{quantity.toLocaleString()}</span>
          </div>
        )}

        {maturityDate && (
          <div className="flex items-center justify-between">
            <span className="text-sm text-gray-600">Maturity Date</span>
            <span className="font-medium text-gray-900">{new Date(maturityDate).toLocaleDateString()}</span>
          </div>
        )}
      </div>

      {(onBuy || onSell) && (
        <div className="flex gap-2 pt-4 border-t">
          {onBuy && (
            <button
              onClick={onBuy}
              className="flex-1 flex items-center justify-center gap-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
            >
              <DollarSign className="w-4 h-4" />
              Buy
            </button>
          )}
          {onSell && (
            <button
              onClick={onSell}
              className="flex-1 flex items-center justify-center gap-2 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
            >
              <DollarSign className="w-4 h-4" />
              Sell
            </button>
          )}
        </div>
      )}
    </div>
  );
}
