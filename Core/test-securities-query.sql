SELECT 
    SUM(CASE WHEN s."SecurityType" = 'TBILL' THEN so."TotalAmount" ELSE 0 END) as TbillsValue,
    SUM(CASE WHEN s."SecurityType" = 'BOND' THEN so."TotalAmount" ELSE 0 END) as BondsValue,
    SUM(CASE WHEN s."SecurityType" = 'STOCK' THEN so."TotalAmount" ELSE 0 END) as StocksValue,
    SUM(so."TotalAmount") as TotalValue
FROM "SecurityOrders" so
JOIN "Securities" s ON so."SecurityId" = s."Id"
WHERE so."Status" = 'Executed' AND so."OrderType" = 'BUY';
