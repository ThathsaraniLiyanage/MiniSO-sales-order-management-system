# Database Scripts

This folder contains SQL scripts for the Sales Order Management System database.

## Execution Order

Execute the scripts in the following order:

### 1. Schema Creation
First, ensure you have run the main schema creation script:
- `01_CreateSchema.sql` - Creates tables, stored procedures, and sample data

### 2. Schema Verification (Optional)
- `00_VerifySchema.sql` - Verifies all table structures and columns
  - Use this to check if your database schema is correctly set up
  - Shows all columns, data types, and constraints

### 3. Sample Data Insertion
- `02_InsertSampleData.sql` - Inserts detailed sales order line items
  - Adds sample order details for 5 sales orders
  - Includes verification queries and reports
  - Shows data summary by item and order

## How to Run These Scripts

### Using SQL Server Management Studio (SSMS)
1. Open SSMS
2. Connect to your SQL Server instance
3. Click **File** → **Open** → **File**
4. Navigate to the script you want to run
5. Click **Execute** (or press F5)

### Using Command Line (sqlcmd)
```bash
sqlcmd -S localhost -d SalesOrderDB -i "Database/Scripts/00_VerifySchema.sql"
sqlcmd -S localhost -d SalesOrderDB -i "Database/Scripts/02_InsertSampleData.sql"
```

### Using Azure Data Studio
1. Open Azure Data Studio
2. Connect to your server
3. Open the SQL file
4. Click **Run** button

## Script Descriptions

### 00_VerifySchema.sql
- Checks all table structures (SalesOrder, SalesOrderDetail, Client, Item)
- Displays column names, data types, nullable status
- Useful for troubleshooting schema issues

### 02_InsertSampleData.sql
- Inserts sample sales order details (line items)
- Links to existing Items and Sales Orders
- Includes automatic calculations for amounts and taxes
- Provides verification queries:
  - Record count summary
  - All sales order details
  - Sales summary by item
  - Detailed order summary

## Prerequisites

- SQL Server 2019 or later
- Database `SalesOrderDB` must exist
- Tables (Client, Item, SalesOrder, SalesOrderDetail) must be created
- Sample Clients, Items, and Sales Orders must exist

## Troubleshooting

**Issue**: "Invalid object name" errors
- **Solution**: Run the schema creation script first (`01_CreateSchema.sql`)

**Issue**: Foreign key constraint errors
- **Solution**: Ensure Client and Item sample data exists before running order scripts

**Issue**: No data returned in queries
- **Solution**: Check that the InvoiceNo values match in your SalesOrder table

## Notes

- All amounts are calculated automatically based on quantity and unit price
- Tax rate is set to 7% (0.07) for all transactions
- Sample data includes various scenarios (single items, multiple items, bulk orders)
