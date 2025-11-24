USE SalesOrderDB;
GO

----------------------------------------------------------
-- INSERT SAMPLE SALES ORDER DETAILS (CORRECTED)
----------------------------------------------------------
-- First, let's delete any partial data if it exists
DELETE FROM SalesOrderDetail;

DECLARE @OrderId1 INT, @OrderId2 INT, @OrderId3 INT, @OrderId4 INT, @OrderId5 INT;
DECLARE @ItemId1 INT, @ItemId2 INT, @ItemId3 INT, @ItemId4 INT;
DECLARE @ItemCode1 NVARCHAR(50), @ItemCode2 NVARCHAR(50), @ItemCode3 NVARCHAR(50), @ItemCode4 NVARCHAR(50);
DECLARE @ItemDesc1 NVARCHAR(255), @ItemDesc2 NVARCHAR(255), @ItemDesc3 NVARCHAR(255), @ItemDesc4 NVARCHAR(255);
DECLARE @UnitPrice1 DECIMAL(18,2), @UnitPrice2 DECIMAL(18,2), @UnitPrice3 DECIMAL(18,2), @UnitPrice4 DECIMAL(18,2);

-- Get Order IDs
SELECT @OrderId1 = SalesOrderId FROM SalesOrder WHERE InvoiceNo = 'INV-2024-001';
SELECT @OrderId2 = SalesOrderId FROM SalesOrder WHERE InvoiceNo = 'INV-2024-002';
SELECT @OrderId3 = SalesOrderId FROM SalesOrder WHERE InvoiceNo = 'INV-2024-003';
SELECT @OrderId4 = SalesOrderId FROM SalesOrder WHERE InvoiceNo = 'INV-2024-004';
SELECT @OrderId5 = SalesOrderId FROM SalesOrder WHERE InvoiceNo = 'INV-2024-005';

-- Get Item 1 details (Laptop)
SELECT @ItemId1 = ItemId, @ItemCode1 = ItemCode, @ItemDesc1 = Description, @UnitPrice1 = UnitPrice 
FROM Item WHERE ItemCode = 'ITEM001';

-- Get Item 2 details (Mouse)
SELECT @ItemId2 = ItemId, @ItemCode2 = ItemCode, @ItemDesc2 = Description, @UnitPrice2 = UnitPrice 
FROM Item WHERE ItemCode = 'ITEM002';

-- Get Item 3 details (Keyboard)
SELECT @ItemId3 = ItemId, @ItemCode3 = ItemCode, @ItemDesc3 = Description, @UnitPrice3 = UnitPrice 
FROM Item WHERE ItemCode = 'ITEM003';

-- Get Item 4 details (Monitor)
SELECT @ItemId4 = ItemId, @ItemCode4 = ItemCode, @ItemDesc4 = Description, @UnitPrice4 = UnitPrice 
FROM Item WHERE ItemCode = 'ITEM004';

INSERT INTO [SalesOrderDetail] 
    (SalesOrderId, ItemId, ItemCode, Description, Note, Quantity, UnitPrice, TaxRate, ExclAmount, TaxAmount, InclAmount, LineNumber) 
VALUES
    -- Order 1 Details (INV-2024-001) - 2 Laptops
    (@OrderId1, @ItemId1, @ItemCode1, @ItemDesc1, 'Bulk order for office', 2, @UnitPrice1, 0.07, 
     1799.98, 125.99, 1925.97, 1),
    
    -- Order 2 Details (INV-2024-002) - 1 Laptop
    (@OrderId2, @ItemId1, @ItemCode1, @ItemDesc1, '', 1, @UnitPrice1, 0.07, 
     899.99, 62.99, 962.98, 1),
    
    -- Order 3 Details (INV-2024-003) - Multiple items
    (@OrderId3, @ItemId2, @ItemCode2, @ItemDesc2, 'Standard wireless mouse', 3, @UnitPrice2, 0.07, 
     299.97, 21.00, 320.97, 1),
    (@OrderId3, @ItemId3, @ItemCode3, @ItemDesc3, 'Mechanical RGB keyboard', 1, @UnitPrice3, 0.07, 
     179.99, 12.60, 192.59, 2),
    (@OrderId3, @ItemId4, @ItemCode4, @ItemDesc4, 'UltraWide display', 1, @UnitPrice4, 0.07, 
     449.99, 31.50, 481.49, 3),
    
    -- Order 4 Details (INV-2024-004) - 3 Monitors
    (@OrderId4, @ItemId4, @ItemCode4, @ItemDesc4, 'Multi-monitor setup', 3, @UnitPrice4, 0.07, 
     1349.97, 94.50, 1444.47, 1),
    
    -- Order 5 Details (INV-2024-005) - 1 Monitor
    (@OrderId5, @ItemId4, @ItemCode4, @ItemDesc4, 'Single display unit', 1, @UnitPrice4, 0.07, 
     449.99, 31.50, 481.49, 1);

PRINT 'Sales Order Details inserted successfully!';
GO

----------------------------------------------------------
-- VERIFY DATA
----------------------------------------------------------
PRINT '';
PRINT '========================================';
PRINT 'RECORD COUNT SUMMARY';
PRINT '========================================';

SELECT 'Clients' AS TableName, COUNT(*) AS RecordCount FROM Client
UNION ALL
SELECT 'Items', COUNT(*) FROM Item
UNION ALL
SELECT 'SalesOrders', COUNT(*) FROM SalesOrder
UNION ALL
SELECT 'SalesOrderDetails', COUNT(*) FROM SalesOrderDetail;
GO

----------------------------------------------------------
-- DISPLAY ALL SALES ORDER DETAILS
----------------------------------------------------------
PRINT '';
PRINT '========================================';
PRINT 'ALL SALES ORDER DETAILS';
PRINT '========================================';
SELECT 
    sod.SalesOrderDetailId,
    so.InvoiceNo,
    so.CustomerName,
    sod.ItemCode,
    sod.Description,
    sod.Note,
    sod.Quantity,
    sod.UnitPrice,
    sod.TaxRate,
    sod.ExclAmount,
    sod.TaxAmount,
    sod.InclAmount,
    sod.LineNumber
FROM SalesOrderDetail sod
INNER JOIN SalesOrder so ON sod.SalesOrderId = so.SalesOrderId
ORDER BY so.InvoiceNo, sod.LineNumber;
GO

----------------------------------------------------------
-- SUMMARY REPORT BY ITEM
----------------------------------------------------------
PRINT '';
PRINT '========================================';
PRINT 'SALES SUMMARY BY ITEM';
PRINT '========================================';
SELECT 
    sod.ItemCode,
    sod.Description,
    SUM(sod.Quantity) AS TotalQuantitySold,
    SUM(sod.ExclAmount) AS TotalExclRevenue,
    SUM(sod.TaxAmount) AS TotalTax,
    SUM(sod.InclAmount) AS TotalInclRevenue
FROM SalesOrderDetail sod
GROUP BY sod.ItemCode, sod.Description
ORDER BY TotalInclRevenue DESC;
GO

----------------------------------------------------------
-- DETAILED ORDER SUMMARY
----------------------------------------------------------
PRINT '';
PRINT '========================================';
PRINT 'DETAILED ORDER SUMMARY';
PRINT '========================================';
SELECT 
    so.InvoiceNo,
    so.InvoiceDate,
    so.ReferenceNo,
    so.CustomerName,
    so.TotalExclAmount,
    so.TotalTaxAmount,
    so.TotalInclAmount,
    COUNT(sod.SalesOrderDetailId) AS TotalLineItems,
    SUM(sod.Quantity) AS TotalQuantity
FROM SalesOrder so
LEFT JOIN SalesOrderDetail sod ON so.SalesOrderId = sod.SalesOrderId
GROUP BY so.InvoiceNo, so.InvoiceDate, so.ReferenceNo, so.CustomerName, 
         so.TotalExclAmount, so.TotalTaxAmount, so.TotalInclAmount
ORDER BY so.InvoiceDate DESC;
GO

PRINT '';
PRINT '========================================';
PRINT 'SAMPLE DATA INSERTION COMPLETED!';
PRINT '========================================';
