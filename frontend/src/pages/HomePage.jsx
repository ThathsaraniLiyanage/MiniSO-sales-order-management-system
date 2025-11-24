import { Link } from 'react-router-dom';

function HomePage() {
  return (
    <div className="text-center">
      <h1 className="text-4xl font-bold mb-4">Sales Order Management System</h1>
      <p className="text-xl text-gray-600 mb-8">
        Manage your customers, products, and orders efficiently
      </p>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 max-w-4xl mx-auto">
        <Link
          to="/customers"
          className="bg-white p-8 rounded-lg shadow-lg hover:shadow-xl transition"
        >
          <div className="text-4xl mb-4">👥</div>
          <h2 className="text-2xl font-bold mb-2">Customers</h2>
          <p className="text-gray-600">Manage customer information</p>
        </Link>

        <Link
          to="/products"
          className="bg-white p-8 rounded-lg shadow-lg hover:shadow-xl transition"
        >
          <div className="text-4xl mb-4">📦</div>
          <h2 className="text-2xl font-bold mb-2">Products</h2>
          <p className="text-gray-600">Manage product catalog</p>
        </Link>

        <Link
          to="/orders"
          className="bg-white p-8 rounded-lg shadow-lg hover:shadow-xl transition"
        >
          <div className="text-4xl mb-4">🛒</div>
          <h2 className="text-2xl font-bold mb-2">Orders</h2>
          <p className="text-gray-600">Process and track orders</p>
        </Link>
      </div>
    </div>
  );
}

export default HomePage;