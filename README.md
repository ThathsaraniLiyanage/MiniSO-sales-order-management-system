# Sales Order Management System

A full-stack Sales Order Management application built with .NET Core 8 Web API and React 18, demonstrating Clean Architecture principles and modern development practices.

## 🚀 Features

- **Sales Order Management**: Complete CRUD operations for sales orders
- **Customer Management**: Add, view, edit, and delete customer records
- **Product Inventory**: Manage products with pricing and stock information
- **Automatic Calculations**: Real-time tax calculation and order totals
- **Responsive Design**: Modern UI built with React and Tailwind CSS
- **Clean Architecture**: Well-structured backend with separation of concerns
- **RESTful API**: Standard API endpoints for all operations

## 🛠️ Technology Stack

### Backend
- **.NET Core 8.0** - Web API framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database management system
- **Clean Architecture** - 4-layer architecture pattern
  - Domain Layer (Entities)
  - Application Layer (Business Logic)
  - Infrastructure Layer (Data Access)
  - API Layer (Controllers)

### Frontend
- **React 18** - UI framework
- **Redux Toolkit** - State management
- **Tailwind CSS** - Utility-first CSS framework
- **Axios** - HTTP client for API communication

## 📋 Prerequisites

Before running this application, ensure you have the following installed:

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [SQL Server 2019](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or later (or SQL Server Express)
- [Node.js 18+](https://nodejs.org/) and npm
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) (optional, for database management)

## 🔧 Configuration

### Database Connection String

⚠️ **Important**: Configure your database connection before running the application.

1. Navigate to `MiniSO.API/appsettings.Development.json`
2. Update the connection string with your SQL Server details:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=SalesOrderDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Common Server Names:**
- SQL Server Express: `localhost\\SQLEXPRESS` or `.\\SQLEXPRESS`
- LocalDB: `(localdb)\\MSSQLLocalDB`
- Default Instance: `localhost` or `.`

**Finding Your Server Name:**
- Open SQL Server Management Studio (SSMS)
- The server name appears in the "Connect to Server" dialog
- Or run `sqllocaldb info` in Command Prompt for LocalDB

**For SQL Authentication (Username/Password):**
```json
"DefaultConnection": "Server=YOUR_SERVER_NAME;Database=SalesOrderDB;User Id=your_username;Password=your_password;TrustServerCertificate=True;"
```

## 📦 Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/ThathsaraniLiyanage/MiniSO-sales-order-management-system.git
cd MiniSO-sales-order-management-system
```

### 2. Backend Setup

#### a. Restore NuGet Packages
```bash
cd MiniSO.API
dotnet restore
```

#### b. Update Database Connection String
Create or edit `appsettings.Development.json` with your actual connection string (see Configuration section above).

#### c. Apply Database Migrations
```bash
dotnet ef database update
```

This will create the `SalesOrderDB` database with all required tables.

#### d. Run the Backend API
```bash
dotnet run
```

The API will start at: **https://localhost:7242** (or the port shown in console)

### 3. Frontend Setup

#### a. Navigate to Frontend Directory
```bash
cd ../frontend
```

#### b. Install Dependencies
```bash
npm install
```

#### c. Configure API URL (if needed)
If your backend runs on a different port, update the API URL in `src/services/api.js`

#### d. Start the Frontend
```bash
npm start
```

The application will open at: **http://localhost:3000**

## 📁 Project Structure
```
MiniSO-sales-order-management-system/
├── MiniSO.Domain/                  # Domain Layer - Entities
│   ├── Entities/
│   │   ├── Customer.cs
│   │   ├── Item.cs
│   │   ├── SalesOrder.cs
│   │   └── SalesOrderDetail.cs
│   └── MiniSO.Domain.csproj
│
├── MiniSO.Application/             # Application Layer - Business Logic
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Services/
│   └── MiniSO.Application.csproj
│
├── MiniSO.Infrastructure/          # Infrastructure Layer - Data Access
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Repositories/
│   └── MiniSO.Infrastructure.csproj
│
├── MiniSO.API/                     # API Layer - Controllers
│   ├── Controllers/
│   │   ├── CustomersController.cs
│   │   ├── ItemsController.cs
│   │   └── SalesOrdersController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── MiniSO.API.csproj
│
└── frontend/                       # React Frontend
    ├── src/
    │   ├── components/
    │   ├── features/
    │   ├── services/
    │   └── App.js
    ├── package.json
    └── tailwind.config.js
```

## 🔌 API Endpoints

### Customers
- `GET /api/customers` - Get all customers
- `GET /api/customers/{id}` - Get customer by ID
- `POST /api/customers` - Create new customer
- `PUT /api/customers/{id}` - Update customer
- `DELETE /api/customers/{id}` - Delete customer

### Products (Items)
- `GET /api/items` - Get all products
- `GET /api/items/{id}` - Get product by ID
- `POST /api/items` - Create new product
- `PUT /api/items/{id}` - Update product
- `DELETE /api/items/{id}` - Delete product

### Sales Orders
- `GET /api/salesorders` - Get all sales orders
- `GET /api/salesorders/{id}` - Get sales order by ID
- `POST /api/salesorders` - Create new sales order
- `PUT /api/salesorders/{id}` - Update sales order
- `DELETE /api/salesorders/{id}` - Delete sales order

## 🗄️ Database Schema

The application uses the following main tables:
- **Customer** - Customer information
- **Item** - Product/inventory information
- **SalesOrder** - Sales order headers
- **SalesOrderDetail** - Sales order line items

All tables include proper relationships and constraints as per the technical requirements.

## 🎯 Key Features Implementation

### Clean Architecture
- **Domain Layer**: Contains entity models with no dependencies
- **Application Layer**: Business logic and service interfaces
- **Infrastructure Layer**: Data access, repositories, and EF Core context
- **API Layer**: RESTful controllers and dependency injection configuration

### Automatic Calculations
- Tax amount calculation based on configurable tax rate
- Total amount calculation (excluding and including tax)
- Real-time updates on the frontend

### State Management
- Redux Toolkit for centralized state management
- Async thunks for API calls
- Optimistic UI updates

## 🔐 Security Notes

- Connection strings with credentials should never be committed to Git
- `appsettings.Development.json` is excluded via `.gitignore`
- Use environment variables for production deployments
- Implement authentication/authorization for production use

## 🚦 Running Tests
```bash
# Backend tests (if implemented)
cd MiniSO.API
dotnet test

# Frontend tests (if implemented)
cd frontend
npm test
```

## 📝 Development Notes

- The backend runs on HTTPS by default (port 7242)
- CORS is configured to allow requests from localhost:3000
- Entity Framework migrations are used for database schema management
- Hot reload is enabled for both backend and frontend during development

## 🤝 Contributing

This is a technical assessment project. For questions or issues, please contact the repository owner.

## 👤 Author

**Thathsarani Liyanage**
- GitHub: [@ThathsaraniLiyanage](https://github.com/ThathsaraniLiyanage)

## 📄 License

This project is created as a technical assessment for SPIL Labs.

## 📧 Contact

For any questions regarding this project, please reach out through GitHub.

---

**Built with ❤️ for SPIL Labs Technical Assessment**