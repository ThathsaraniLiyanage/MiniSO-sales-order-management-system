import { useState } from 'react';
import { useDispatch } from 'react-redux';
import { createProduct, updateProduct } from '../../store/slices/productSlice';

function ProductForm({ product, onClose }) {
  const dispatch = useDispatch();
  const [formData, setFormData] = useState({
    itemCode: product?.itemCode || '',
    description: product?.description || '',
    unitPrice: product?.unitPrice || '',
  });

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    const data = {
      ...formData,
      unitPrice: parseFloat(formData.unitPrice),
    };

    if (product) {
      await dispatch(updateProduct({ id: product.itemId, data }));
    } else {
      await dispatch(createProduct(data));
    }
    
    onClose();
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <label className="block text-sm font-medium mb-1">Item Code *</label>
        <input
          type="text"
          name="itemCode"
          value={formData.itemCode}
          onChange={handleChange}
          required
          className="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      <div>
        <label className="block text-sm font-medium mb-1">Description *</label>
        <textarea
          name="description"
          value={formData.description}
          onChange={handleChange}
          required
          rows="3"
          className="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      <div>
        <label className="block text-sm font-medium mb-1">Unit Price *</label>
        <input
          type="number"
          name="unitPrice"
          value={formData.unitPrice}
          onChange={handleChange}
          step="0.01"
          min="0"
          required
          className="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      <div className="flex justify-end space-x-3">
        <button
          type="button"
          onClick={onClose}
          className="px-4 py-2 border rounded hover:bg-gray-50"
        >
          Cancel
        </button>
        <button
          type="submit"
          className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
        >
          {product ? 'Update' : 'Create'}
        </button>
      </div>
    </form>
  );
}

export default ProductForm;