import { Link } from 'react-router-dom';

function Navbar() {
  return (
    <nav className="bg-blue-600 text-white shadow-lg">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          <Link to="/" className="text-xl font-bold">
            Sales Order System
          </Link>
          <div className="flex space-x-4">
            <Link to="/customers" className="hover:bg-blue-700 px-3 py-2 rounded">
              Customers
            </Link>
            <Link to="/products" className="hover:bg-blue-700 px-3 py-2 rounded">
              Products
            </Link>
            <Link to="/orders" className="hover:bg-blue-700 px-3 py-2 rounded">
              Orders
            </Link>
          </div>
        </div>
      </div>
    </nav>
  );
}

export default Navbar;