# Week 9: Treasury & Markets Module - Implementation Plan

## 🎯 Module Overview: Treasury & Markets Implementation

**Status**: 🚧 **IN PROGRESS** - Domain Layer Implementation  
**Industry Alignment**: Finacle Treasury & T24 Treasury & Capital Markets  
**Implementation Date**: January 17, 2026  
**Priority**: HIGH - Critical for bank liquidity and profitability management

---

## 📋 Week 9 Implementation Plan

### **Phase 1: Domain Layer** (Days 1-2)

#### 1. **Treasury & Markets Aggregates** ⭐
- **MoneyMarketDeal** - Call money, term deposits, repos
- **FXDeal** - Spot, forward, swap transactions  
- **SecurityDeal** - Government bonds, corporate bonds, equities
- **LiquidityPosition** - Daily liquidity management
- **InterestRatePosition** - Rate risk management
- **TreasuryPortfolio** - Investment portfolio management

#### 2. **Value Objects**
- **DealNumber** - Unique deal identification
- **ExchangeRate** - FX rate with spread
- **YieldRate** - Bond yield calculations
- **MaturityPeriod** - Deal maturity handling
- **RiskMetrics** - VaR, duration, convexity

#### 3. **Domain Events**
- **MoneyMarketDealBookedDomainEvent**
- **FXDealExecutedDomainEvent**
- **SecurityTradeSettledDomainEvent**
- **LiquidityShortfallDomainEvent**
- **RateLimitBreachedDomainEvent**

### **Phase 2: Application Layer** (Days 3-4)

#### 1. **Money Market Commands**
- **BookCallMoneyCommand** - Overnight lending/borrowing
- **BookTermDepositCommand** - Fixed term investments
- **BookRepoCommand** - Repurchase agreements
- **RolloverDealCommand** - Deal renewal

#### 2. **FX Trading Commands**
- **ExecuteSpotFXCommand** - Spot FX transactions
- **BookForwardFXCommand** - Forward contracts
- **ExecuteFXSwapCommand** - FX swap deals
- **UpdateFXRatesCommand** - Rate updates

#### 3. **Securities Commands**
- **BuySecurityCommand** - Security purchases
- **SellSecurityCommand** - Security sales
- **ReceiveCouponCommand** - Coupon payments
- **MarkToMarketCommand** - MTM valuations

#### 4. **Treasury Queries**
- **GetLiquidityPositionQuery** - Current liquidity
- **GetFXPositionQuery** - FX exposure
- **GetPortfolioValuationQuery** - Portfolio MTM
- **GetTreasuryDashboardQuery** - Treasury metrics

### **Phase 3: Infrastructure Layer** (Days 5-6)

#### 1. **Repository Implementations**
- **MoneyMarketDealRepository**
- **FXDealRepository**
- **SecurityDealRepository**
- **LiquidityPositionRepository**

#### 2. **EF Core Configurations**
- **MoneyMarketDealConfiguration**
- **FXDealConfiguration**
- **SecurityDealConfiguration**

#### 3. **Database Migration**
- **AddTreasuryMarketsTables** migration

### **Phase 4: API Layer** (Day 7)

#### 1. **TreasuryController**
- Money market deal endpoints
- FX trading endpoints
- Securities trading endpoints
- Liquidity management endpoints

#### 2. **Market Data Integration**
- Real-time rate feeds
- Security price updates
- Yield curve management
- Risk metric calculations

---

## 🏗️ Technical Architecture

### Treasury & Markets Domain Model

```
Treasury & Markets
├── MoneyMarketDeal
│   ├── DealNumber (Value Object)
│   ├── Counterparty
│   ├── DealType (Call, Term, Repo)
│   ├── Principal (Money)
│   ├── Rate (InterestRate)
│   ├── MaturityDate
│   └── Status (Booked, Settled, Matured)
├── FXDeal
│   ├── DealNumber (Value Object)
│   ├── BaseCurrency/QuoteCurrency
│   ├── ExchangeRate (Value Object)
│   ├── Amount (Money)
│   ├── ValueDate
│   ├── DealType (Spot, Forward, Swap)
│   └── Status (Pending, Executed, Settled)
├── SecurityDeal
│   ├── DealNumber (Value Object)
│   ├── SecurityId (ISIN/Ticker)
│   ├── Quantity
│   ├── Price (Money)
│   ├── YieldRate (Value Object)
│   ├── TradeDate/SettlementDate
│   └── Status (Pending, Executed, Settled)
└── LiquidityPosition
    ├── Date
    ├── Currency
    ├── OpeningBalance
    ├── Inflows/Outflows
    ├── ClosingBalance
    └── RequiredReserves
```

### Market Data Integration

```
Market Data Sources
├── Central Bank Rates
├── Interbank Rates (KIBOR, LIBOR)
├── FX Rates (Reuters, Bloomberg)
├── Bond Yields (Government, Corporate)
├── Equity Prices
└── Commodity Prices
```

---

## 🎯 Business Rules & Validations

### Money Market Rules
1. **Deal Amount** must meet minimum thresholds
2. **Counterparty Limits** must not be exceeded
3. **Maturity Date** must be valid business day
4. **Interest Rate** must be within market range
5. **Collateral** required for repo deals
6. **Settlement** must follow market conventions

### FX Trading Rules
1. **Position Limits** must not be breached
2. **Value Date** must follow FX conventions
3. **Exchange Rate** must be within spread limits
4. **Counterparty Risk** limits enforced
5. **Settlement Risk** managed through PvP
6. **Regulatory Reporting** for large deals

### Securities Rules
1. **Investment Policy** compliance required
2. **Credit Rating** minimum requirements
3. **Concentration Limits** by issuer/sector
4. **Liquidity Requirements** maintained
5. **Mark-to-Market** daily valuation
6. **Regulatory Capital** impact calculated

---

## 📊 Key Features

### ✅ **Money Market Operations**
- Call money lending/borrowing
- Term deposit placements
- Repurchase agreements (Repo/Reverse Repo)
- Certificate of deposit issuance
- Commercial paper investments
- Treasury bill auctions

### ✅ **Foreign Exchange Trading**
- Spot FX transactions
- Forward FX contracts
- FX swap deals
- Currency options (basic)
- FX position management
- Real-time rate updates

### ✅ **Securities Trading**
- Government bond trading
- Corporate bond investments
- Equity trading (basic)
- Mutual fund investments
- Portfolio management
- Yield curve analysis

### ✅ **Liquidity Management**
- Daily cash flow forecasting
- Liquidity gap analysis
- Reserve requirement management
- Funding cost optimization
- Stress testing scenarios

### ✅ **Asset-Liability Management**
- Interest rate risk measurement
- Duration gap analysis
- Repricing gap reports
- Net interest income simulation
- Economic value sensitivity

### ✅ **Risk Management**
- Value at Risk (VaR) calculation
- Position limit monitoring
- Counterparty exposure tracking
- Market risk reporting
- Regulatory capital calculation

---

## 🔧 Implementation Details

### Domain Events Flow

```
Money Market Deal Flow:
1. BookCallMoneyCommand → MoneyMarketDealBookedDomainEvent
2. Validate counterparty limits
3. Update liquidity position
4. Create GL entries
5. Generate deal confirmation

FX Deal Flow:
1. ExecuteSpotFXCommand → FXDealExecutedDomainEvent
2. Check position limits
3. Update FX position
4. Create settlement entries
5. Generate trade confirmation

Securities Deal Flow:
1. BuySecurityCommand → SecurityTradeSettledDomainEvent
2. Validate investment policy
3. Update portfolio position
4. Calculate accrued interest
5. Generate settlement instructions
```

### Database Schema

```sql
-- Money Market Deals
CREATE TABLE MoneyMarketDeals (
    Id UUID PRIMARY KEY,
    DealNumber VARCHAR(50) UNIQUE NOT NULL,
    CounterpartyId UUID NOT NULL,
    DealType VARCHAR(20) NOT NULL, -- Call, Term, Repo
    Principal DECIMAL(18,2) NOT NULL,
    Currency VARCHAR(3) NOT NULL,
    InterestRate DECIMAL(8,4) NOT NULL,
    TradeDate DATE NOT NULL,
    ValueDate DATE NOT NULL,
    MaturityDate DATE NOT NULL,
    Status VARCHAR(20) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT NOW()
);

-- FX Deals
CREATE TABLE FXDeals (
    Id UUID PRIMARY KEY,
    DealNumber VARCHAR(50) UNIQUE NOT NULL,
    CounterpartyId UUID NOT NULL,
    DealType VARCHAR(20) NOT NULL, -- Spot, Forward, Swap
    BaseCurrency VARCHAR(3) NOT NULL,
    QuoteCurrency VARCHAR(3) NOT NULL,
    BaseAmount DECIMAL(18,2) NOT NULL,
    QuoteAmount DECIMAL(18,2) NOT NULL,
    ExchangeRate DECIMAL(12,6) NOT NULL,
    TradeDate DATE NOT NULL,
    ValueDate DATE NOT NULL,
    Status VARCHAR(20) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT NOW()
);

-- Security Deals
CREATE TABLE SecurityDeals (
    Id UUID PRIMARY KEY,
    DealNumber VARCHAR(50) UNIQUE NOT NULL,
    SecurityId VARCHAR(50) NOT NULL, -- ISIN or Ticker
    SecurityType VARCHAR(30) NOT NULL,
    TradeType VARCHAR(10) NOT NULL, -- Buy, Sell
    Quantity DECIMAL(18,4) NOT NULL,
    Price DECIMAL(18,6) NOT NULL,
    Currency VARCHAR(3) NOT NULL,
    TradeDate DATE NOT NULL,
    SettlementDate DATE NOT NULL,
    YieldRate DECIMAL(8,4),
    AccruedInterest DECIMAL(18,2),
    Status VARCHAR(20) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT NOW()
);

-- Liquidity Positions
CREATE TABLE LiquidityPositions (
    Id UUID PRIMARY KEY,
    PositionDate DATE NOT NULL,
    Currency VARCHAR(3) NOT NULL,
    OpeningBalance DECIMAL(18,2) NOT NULL,
    Inflows DECIMAL(18,2) NOT NULL,
    Outflows DECIMAL(18,2) NOT NULL,
    ClosingBalance DECIMAL(18,2) NOT NULL,
    RequiredReserves DECIMAL(18,2),
    ExcessLiquidity DECIMAL(18,2),
    CreatedAt TIMESTAMP DEFAULT NOW(),
    UNIQUE(PositionDate, Currency)
);
```

---

## 🧪 Testing Strategy

### Unit Tests (Planned: 32 tests)
- **MoneyMarketDeal Aggregate** (8 tests)
- **FXDeal Aggregate** (8 tests)
- **SecurityDeal Aggregate** (8 tests)
- **LiquidityPosition Aggregate** (4 tests)
- **ExchangeRate Value Object** (4 tests)

### Integration Tests
- **Money Market Deal Booking** end-to-end
- **FX Deal Execution** with settlement
- **Securities Trading** with portfolio update
- **Liquidity Position** calculation

---

## 📈 Success Metrics

### Functional Metrics
- ✅ Deal booking in <5 seconds
- ✅ FX rate updates in real-time
- ✅ Portfolio valuation accuracy 99.99%
- ✅ Liquidity forecasting accuracy >95%
- ✅ Risk limit monitoring real-time

### Technical Metrics
- ✅ API response time <100ms
- ✅ Database query performance <50ms
- ✅ Market data latency <1 second
- ✅ System availability 99.9%

---

## 🚀 Deployment Checklist

### Pre-deployment
- [ ] Domain model validation
- [ ] Database migration testing
- [ ] Market data connectivity
- [ ] Risk limit configuration
- [ ] Security audit

### Post-deployment
- [ ] API endpoint testing
- [ ] Market data feed testing
- [ ] Deal booking workflow
- [ ] Risk monitoring alerts
- [ ] Performance monitoring

---

## 📚 Industry Standards Compliance

### Market Standards
- ✅ ISDA documentation for derivatives
- ✅ FIX protocol for trade messaging
- ✅ ISO 20022 for payments
- ✅ SWIFT standards for confirmations

### Regulatory Compliance
- ✅ Basel III liquidity ratios (LCR, NSFR)
- ✅ Market risk capital requirements
- ✅ Large exposure regulations
- ✅ MiFID II transaction reporting

### Risk Management
- ✅ VaR methodology (Historical, Monte Carlo)
- ✅ Stress testing frameworks
- ✅ Counterparty risk measurement
- ✅ Operational risk controls

---

## 🎯 Next Steps After Week 9

### Week 10: Risk, Compliance & Controls
- AML transaction monitoring
- Sanctions screening
- Fraud detection systems
- Regulatory reporting

### Week 11: Reporting & Analytics
- Management information systems
- Regulatory returns
- Business intelligence dashboards
- Data warehouse integration

---

**Implementation Target**: Complete treasury & markets foundation by end of Week 9
**Success Criteria**: Full money market, FX, and securities trading with risk management
**Business Impact**: Enable bank profitability through treasury operations and risk management

---

*"Treasury & Markets is the profit center of modern banking - our implementation will enable sophisticated financial market operations while maintaining prudent risk management."*