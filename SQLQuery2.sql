USE SalesOrderDB;
GO

----------------------------------------------------------
-- INSERT SAMPLE SALES ORDERS
----------------------------------------------------------
IF NOT EXISTS (SELECT * FROM SalesOrder)
BEGIN
    DECLARE @ClientId1 INT, @ClientId2 INT, @ClientId3 INT;
    
    SELECT @ClientId1 = ClientId FROM Client WHERE CustomerName = 'ABC Corporation';
    SELECT @ClientId2 = ClientId FROM Client WHERE CustomerName = 'XYZ Enterprises';
    SELECT @ClientId3 = ClientId FROM Client WHERE CustomerName = 'Global Trading Co.';
    
    INSERT INTO [SalesOrder] 
        (OrderNumber, ClientId, OrderDate, Status, TotalAmount, TaxAmount, GrandTotal, CreatedDate) 
    VALUES
        ('SO-2024-001', @ClientId1, DATEADD(DAY, -10, GETDATE()), 'Completed', 1799.97, 126.00, 1925.97, DATEADD(DAY, -10, GETDATE())),
        ('SO-2024-002', @ClientId2, DATEADD(DAY, -5, GETDATE()), 'Pending', 899.99, 63.00, 962.99, DATEADD(DAY, -5, GETDATE())),
        ('SO-2024-003', @ClientId1, DATEADD(DAY, -3, GETDATE()), 'Completed', 729.97, 51.10, 781.07, DATEADD(DAY, -3, GETDATE())),
        ('SO-2024-004', @ClientId3, DATEADD(DAY, -1, GETDATE()), 'Pending', 1349.96, 94.50, 1444.46, DATEADD(DAY, -1, GETDATE())),
        ('SO-2024-005', @ClientId2, GETDATE(), 'Cancelled', 449.99, 31.50, 481.49, GETDATE());
    
    PRINT 'Sales Orders inserted successfully!';
END
ELSE
BEGIN
    PRINT 'Sales Orders already exist.';
END
GO

----------------------------------------------------------
-- INSERT SAMPLE SALES ORDER DETAILS
----------------------------------------------------------
IF NOT EXISTS (SELECT * FROM SalesOrderDetail)
BEGIN
    DECLARE @OrderId1 INT, @OrderId2 INT, @OrderId3 INT, @OrderId4 INT, @OrderId5 INT;
    DECLARE @ItemId1 INT, @ItemId2 INT, @ItemId3 INT, @ItemId4 INT;
    
    SELECT @OrderId1 = SalesOrderId FROM SalesOrder WHERE OrderNumber = 'SO-2024-001';
    SELECT @OrderId2 = SalesOrderId FROM SalesOrder WHERE OrderNumber = 'SO-2024-002';
    SELECT @OrderId3 = SalesOrderId FROM SalesOrder WHERE OrderNumber = 'SO-2024-003';
    SELECT @OrderId4 = SalesOrderId FROM SalesOrder WHERE OrderNumber = 'SO-2024-004';
    SELECT @OrderId5 = SalesOrderId FROM SalesOrder WHERE OrderNumber = 'SO-2024-005';
    
    SELECT @ItemId1 = ItemId FROM Item WHERE ItemCode = 'ITEM001';
    SELECT @ItemId2 = ItemId FROM Item WHERE ItemCode = 'ITEM002';
    SELECT @ItemId3 = ItemId FROM Item WHERE ItemCode = 'ITEM003';
    SELECT @ItemId4 = ItemId FROM Item WHERE ItemCode = 'ITEM004';
    
    INSERT INTO [SalesOrderDetail] 
        (SalesOrderId, ItemId, Quantity, UnitPrice, TotalPrice, CreatedDate) 
    VALUES
        -- Order 1 Details (SO-2024-001) - 2 Laptops
        (@OrderId1, @ItemId1, 2, 899.99, 1799.98, DATEADD(DAY, -10, GETDATE())),
        
        -- Order 2 Details (SO-2024-002) - 1 Laptop
        (@OrderId2, @ItemId1, 1, 899.99, 899.99, DATEADD(DAY, -5, GETDATE())),
        
        -- Order 3 Details (SO-2024-003) - Multiple items
        (@OrderId3, @ItemId2, 3, 99.99, 299.97, DATEADD(DAY, -3, GETDATE())),
        (@OrderId3, @ItemId3, 1, 179.99, 179.99, DATEADD(DAY, -3, GETDATE())),
        (@OrderId3, @ItemId4, 1, 449.99, 449.99, DATEADD(DAY, -3, GETDATE())),
        
        -- Order 4 Details (SO-2024-004) - 3 Monitors
        (@OrderId4, @ItemId4, 3, 449.99, 1349.97, DATEADD(DAY, -1, GETDATE())),
        
        -- Order 5 Details (SO-2024-005) - 1 Monitor (Cancelled Order)
        (@OrderId5, @ItemId4, 1, 449.99, 449.99, GETDATE());
    
    PRINT 'Sales Order Details inserted successfully!';
END
ELSE
BEGIN
    PRINT 'Sales Order Details already exist.';
END
GO

----------------------------------------------------------
-- VERIFY DATA IN ALL TABLES
----------------------------------------------------------
SELECT 'Clients' AS TableName, COUNT(*) AS RecordCount FROM Client
UNION ALL
SELECT 'Items', COUNT(*) FROM Item
UNION ALL
SELECT 'SalesOrders', COUNT(*) FROM SalesOrder
UNION ALL
SELECT 'SalesOrderDetails', COUNT(*) FROM SalesOrderDetail;
GO

----------------------------------------------------------
-- DISPLAY SAMPLE DATA
----------------------------------------------------------
PRINT '';
PRINT '========================================';
PRINT 'SAMPLE SALES ORDERS WITH DETAILS';
PRINT '========================================';
SELECT 
    so.OrderNumber,
    c.CustomerName,
    so.OrderDate,
    so.Status,
    so.TotalAmount,
    so.TaxAmount,
    so.GrandTotal
FROM SalesOrder so
INNER JOIN Client c ON so.ClientId = c.ClientId
ORDER BY so.OrderDate DESC;

PRINT '';
PRINT '========================================';
PRINT 'SALES ORDER DETAILS';
PRINT '========================================';
SELECT 
    so.OrderNumber,
    i.ItemCode,
    i.Description,
    sod.Quantity,
    sod.UnitPrice,
    sod.TotalPrice
FROM SalesOrderDetail sod
INNER JOIN SalesOrder so ON sod.SalesOrderId = so.SalesOrderId
INNER JOIN Item i ON sod.ItemId = i.ItemId
ORDER BY so.OrderNumber, i.ItemCode;
GO