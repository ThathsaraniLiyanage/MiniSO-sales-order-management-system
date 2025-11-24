USE SalesOrderDB;
GO

----------------------------------------------------------
-- VERIFY DATABASE SCHEMA
----------------------------------------------------------
PRINT '========================================';
PRINT 'DATABASE SCHEMA VERIFICATION';
PRINT '========================================';
PRINT '';

-- Check SalesOrder table structure
PRINT 'SalesOrder Table Columns:';
PRINT '----------------------------------------';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'SalesOrder'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '';

-- Check SalesOrderDetail table structure  
PRINT 'SalesOrderDetail Table Columns:';
PRINT '----------------------------------------';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'SalesOrderDetail'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '';

-- Check Client table structure
PRINT 'Client Table Columns:';
PRINT '----------------------------------------';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Client'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '';

-- Check Item table structure
PRINT 'Item Table Columns:';
PRINT '----------------------------------------';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Item'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '========================================';
PRINT 'SCHEMA VERIFICATION COMPLETED!';
PRINT '========================================';
GO
