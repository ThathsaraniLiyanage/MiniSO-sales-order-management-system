USE SalesOrderDB;
GO

----------------------------------------------------------
-- INSERT SAMPLE CLIENTS
----------------------------------------------------------
IF NOT EXISTS (SELECT * FROM Client)
BEGIN
    INSERT INTO [Client] 
        (CustomerName, Address1, Address2, Address3, State, PostCode, IsActive, CreatedDate) 
    VALUES
        ('ABC Corporation', '123 Main St', 'Suite 500', 'Business District', 'California', '90001', 1, GETDATE()),
        ('XYZ Enterprises', '456 Oak Ave', 'Floor 3', 'Tech Park', 'New York', '10001', 1, GETDATE()),
        ('Global Trading Co.', '789 Pine Rd', 'Building A', 'Industrial Zone', 'Texas', '73301', 1, GETDATE());
    
    PRINT 'Clients inserted successfully!';
END
ELSE
BEGIN
    PRINT 'Clients already exist.';
END
GO


----------------------------------------------------------
-- INSERT SAMPLE ITEMS
----------------------------------------------------------
IF NOT EXISTS (SELECT * FROM Item)
BEGIN
    INSERT INTO [Item] 
        (ItemCode, Description, UnitPrice, IsActive, CreatedDate) 
    VALUES
        ('ITEM001', 'Laptop Computer - Dell Latitude 5520', 899.99, 1, GETDATE()),
        ('ITEM002', 'Wireless Mouse - Logitech MX Master 3', 99.99, 1, GETDATE()),
        ('ITEM003', 'Mechanical Keyboard - Corsair K95', 179.99, 1, GETDATE()),
        ('ITEM004', 'Monitor 27"" - LG UltraWide', 449.99, 1, GETDATE());
    
    PRINT 'Items inserted successfully!';
END
ELSE
BEGIN
    PRINT 'Items already exist.';
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
