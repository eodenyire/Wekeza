import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { render, screen, waitFor, fireEvent } from '@testing-library/react';
import Stocks from './Stocks';
import { Stock, ApiResponse } from '../../types';

// Mock fetch globally
global.fetch = vi.fn();

const mockStocks: Stock[] = [
  {
    id: 'stock-1',
    symbol: 'KCB',
    name: 'Kenya Commercial Bank',
    currentPrice: 45.50,
    change: 1.25,
    changePercent: 2.82,
    priceChange: 2.82,
    volume: 150000,
    marketCap: 180000000000,
    openPrice: 44.25,
    highPrice: 46.00,
    lowPrice: 44.00,
    bidPrice: 45.25,
    bidVolume: 5000,
    askPrice: 45.75,
    askVolume: 3000
  },
  {
    id: 'stock-2',
    symbol: 'EQTY',
    name: 'Equity Group Holdings',
    currentPrice: 52.00,
    change: -0.50,
    changePercent: -0.95,
    priceChange: -0.95,
    volume: 200000,
    marketCap: 195000000000,
    openPrice: 52.50,
    highPrice: 53.00,
    lowPrice: 51.50,
    bidPrice: 51.75,
    bidVolume: 8000,
    askPrice: 52.25,
    askVolume: 6000
  }
];

describe('Stocks Component', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.restoreAllMocks();
    vi.useRealTimers();
  });

  it('should render loading state initially', () => {
    (global.fetch as any).mockImplementation(() => 
      new Promise(() => {}) // Never resolves
    );

    render(<Stocks />);
    expect(screen.getByRole('status', { hidden: true })).toBeInTheDocument();
  });

  it('should fetch and display stocks', async () => {
    const mockResponse: ApiResponse<Stock[]> = {
      success: true,
      data: mockStocks
    };

    (global.fetch as any).mockResolvedValueOnce({
      json: async () => mockResponse
    });

    render(<Stocks />);

    await waitFor(() => {
      expect(screen.getByText('Kenya Commercial Bank')).toBeInTheDocument();
      expect(screen.getByText('KCB')).toBeInTheDocument();
      expect(screen.getByText('Equity Group Holdings')).toBeInTheDocument();
      expect(screen.getByText('EQTY')).toBeInTheDocument();
    });

    expect(global.fetch).toHaveBeenCalledWith('/api/public-sector/securities/stocks');
  });

  it('should display stock prices and changes correctly', async () => {
    const mockResponse: ApiResponse<Stock[]> = {
      success: true,
      data: mockStocks
    };

    (global.fetch as any).mockResolvedValueOnce({
      json: async () => mockResponse
    });

    render(<Stocks />);

    await waitFor(() => {
      expect(screen.getByText(/KES 45.50/)).toBeInTheDocument();
      expect(screen.getByText(/\+2.82%/)).toBeInTheDocument();
      expect(screen.getByText(/KES 52.00/)).toBeInTheDocument();
      expect(screen.getByText(/-0.95%/)).toBeInTheDocument();
    });
  });

  it('should display order book information', async () => {
    const mockResponse: ApiResponse<Stock[]> = {
      success: true,
      data: mockStocks
    };

    (global.fetch as any).mockResolvedValueOnce({
      json: async () => mockResponse
    });

    render(<Stocks />);

    await waitFor(() => {
      expect(screen.getByText('Order Book')).toBeInTheDocument();
      expect(screen.getByText(/Bid/)).toBeInTheDocument();
      expect(screen.getByText(/Ask/)).toBeInTheDocument();
    });
  });

  it('should poll for updates every 30 seconds', async () => {
    const mockResponse: ApiResponse<Stock[]> = {
      success: true,
      data: mockStocks
    };

    (global.fetch as any).mockResolvedValue({
      json: async () => mockResponse
    });

    render(<Stocks />);

    // Initial fetch
    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledTimes(1);
    });

    // Advance time by 30 seconds
    vi.advanceTimersByTime(30000);

    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledTimes(2);
    });

    // Advance time by another 30 seconds
    vi.advanceTimersByTime(30000);

    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledTimes(3);
    });
  });

  it('should open buy order form when Buy button is clicked', async () => {
    const mockResponse: ApiResponse<Stock[]> = {
      success: true,
      data: mockStocks
    };

    (global.fetch as any).mockResolvedValueOnce({
      json: async () => mockResponse
    });

    render(<Stocks />);

    await waitFor(() => {
      expect(screen.getByText('Kenya Commercial Bank')).toBeInTheDocument();
    });

    const buyButtons = screen.getAllByText('Buy');
    fireEvent.click(buyButtons[0]);

    await waitFor(() => {
      expect(screen.getByText('BUY KCB')).toBeInTheDocument();
      expect(screen.getByPlaceholderText('Enter quantity')).toBeInTheDocument();
      expect(screen.getByPlaceholderText('Enter price')).toBeInTheDocument();
    });
  });

  it('should open sell order form when Sell button is clicked', async () => {
    const mockResponse: ApiResponse<Stock[]> = {
      success: true,
      data: mockStocks
    };

    (global.fetch as any).mockResolvedValueOnce({
      json: async () => mockResponse
    });

    render(<Stocks />);

    await waitFor(() => {
      expect(screen.getByText('Kenya Commercial Bank')).toBeInTheDocument();
    });

    const sellButtons = screen.getAllByText('Sell');
    fireEvent.click(sellButtons[0]);

    await waitFor(() => {
      expect(screen.getByText('SELL KCB')).toBeInTheDocument();
    });
  });

  it('should submit order with correct data', async () => {
    const mockStocksResponse: ApiResponse<Stock[]> = {
      success: true,
      data: mockStocks
    };

    const mockOrderResponse: ApiResponse<any> = {
      success: true,
      data: { orderId: 'order-123' }
    };

    (global.fetch as any)
      .mockResolvedValueOnce({
        json: async () => mockStocksResponse
      })
      .mockResolvedValueOnce({
        json: async () => mockOrderResponse
      });

    render(<Stocks />);

    await waitFor(() => {
      expect(screen.getByText('Kenya Commercial Bank')).toBeInTheDocument();
    });

    // Open buy form
    const buyButtons = screen.getAllByText('Buy');
    fireEvent.click(buyButtons[0]);

    await waitFor(() => {
      expect(screen.getByText('BUY KCB')).toBeInTheDocument();
    });

    // Fill in form
    const quantityInput = screen.getByPlaceholderText('Enter quantity');
    const priceInput = screen.getByPlaceholderText('Enter price');

    fireEvent.change(quantityInput, { target: { value: '100' } });
    fireEvent.change(priceInput, { target: { value: '45.50' } });

    // Submit form
    const placeOrderButton = screen.getByText('Place BUY Order');
    fireEvent.click(placeOrderButton);

    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledWith(
        '/api/public-sector/securities/stocks/order',
        expect.objectContaining({
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            securityId: 'stock-1',
            securityType: 'STOCK',
            orderType: 'BUY',
            quantity: 100,
            price: 45.50,
            totalAmount: 4550
          })
        })
      );
    });
  });

  it('should display error message when fetch fails', async () => {
    (global.fetch as any).mockRejectedValueOnce(new Error('Network error'));

    render(<Stocks />);

    await waitFor(() => {
      expect(screen.getByText(/Failed to fetch stocks/)).toBeInTheDocument();
    });
  });

  it('should display empty state when no stocks available', async () => {
    const mockResponse: ApiResponse<Stock[]> = {
      success: true,
      data: []
    };

    (global.fetch as any).mockResolvedValueOnce({
      json: async () => mockResponse
    });

    render(<Stocks />);

    await waitFor(() => {
      expect(screen.getByText('No stocks available')).toBeInTheDocument();
      expect(screen.getByText('There are currently no government stocks listed.')).toBeInTheDocument();
    });
  });

  it('should calculate total amount correctly in order form', async () => {
    const mockResponse: ApiResponse<Stock[]> = {
      success: true,
      data: mockStocks
    };

    (global.fetch as any).mockResolvedValueOnce({
      json: async () => mockResponse
    });

    render(<Stocks />);

    await waitFor(() => {
      expect(screen.getByText('Kenya Commercial Bank')).toBeInTheDocument();
    });

    // Open buy form
    const buyButtons = screen.getAllByText('Buy');
    fireEvent.click(buyButtons[0]);

    await waitFor(() => {
      expect(screen.getByText('BUY KCB')).toBeInTheDocument();
    });

    // Fill in form
    const quantityInput = screen.getByPlaceholderText('Enter quantity');
    const priceInput = screen.getByPlaceholderText('Enter price');

    fireEvent.change(quantityInput, { target: { value: '100' } });
    fireEvent.change(priceInput, { target: { value: '45.50' } });

    await waitFor(() => {
      expect(screen.getByText('Total Amount')).toBeInTheDocument();
      expect(screen.getByText(/KES 4,550/)).toBeInTheDocument();
    });
  });
});
