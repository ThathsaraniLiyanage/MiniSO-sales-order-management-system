import { useState, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { createOrder, updateOrder, fetchOrders } from '../../store/slices/orderSlice';
import { fetchCustomers } from '../../store/slices/customerSlice';
import { fetchProducts } from '../../store/slices/productSlice';

function OrderForm({ order, onClose }) {
    const dispatch = useDispatch();
    const customers = useSelector((state) => state.customers.items);
    const products = useSelector((state) => state.products.items);

    const [formData, setFormData] = useState({
        clientId: order?.clientId || '',
        customerName: order?.customerName || '',
        address1: order?.address1 || '',
        address2: order?.address2 || '',
        address3: order?.address3 || '',
        state: order?.state || '',
        postCode: order?.postCode || '',
        invoiceNo: order?.invoiceNo || '',
        invoiceDate: order?.invoiceDate?.split('T')[0] || new Date().toISOString().split('T')[0],
        referenceNo: order?.referenceNo || '',
        orderDetails: order?.orderDetails || [],
    });

    useEffect(() => {
        dispatch(fetchCustomers());
        dispatch(fetchProducts());
    }, [dispatch]);

    const handleChange = (e) => {
        const { name, value } = e.target;

        // If customer is changed, auto-fill address fields
        if (name === 'clientId') {
            const selectedCustomer = customers.find(c => c.clientId === parseInt(value));
            if (selectedCustomer) {
                setFormData({
                    ...formData,
                    clientId: value,
                    customerName: selectedCustomer.customerName,
                    address1: selectedCustomer.address1,
                    address2: selectedCustomer.address2,
                    address3: selectedCustomer.address3,
                    state: selectedCustomer.state,
                    postCode: selectedCustomer.postCode,
                });
                return;
            }
        }

        setFormData({
            ...formData,
            [name]: value,
        });
    };

    const addOrderLine = () => {
        setFormData({
            ...formData,
            orderDetails: [
                ...formData.orderDetails,
                { itemId: '', quantity: 1, unitPrice: 0, taxRate: 0.07, note: '' },
            ],
        });
    };

    const removeOrderLine = (index) => {
        setFormData({
            ...formData,
            orderDetails: formData.orderDetails.filter((_, i) => i !== index),
        });
    };

    const updateOrderLine = (index, field, value) => {
        const newOrderDetails = [...formData.orderDetails];
        newOrderDetails[index][field] = value;

        if (field === 'itemId') {
            const product = products.find(p => p.itemId === parseInt(value));
            if (product) {
                newOrderDetails[index].unitPrice = product.unitPrice;
                newOrderDetails[index].description = product.description;
                newOrderDetails[index].itemCode = product.itemCode;
            }
        }

        setFormData({
            ...formData,
            orderDetails: newOrderDetails,
        });
    };

    const calculateLineTotal = (line) => {
        const exclAmount = line.quantity * line.unitPrice;
        const taxAmount = exclAmount * (line.taxRate || 0);
        const inclAmount = exclAmount + taxAmount;
        return { exclAmount, taxAmount, inclAmount };
    };

    const calculateTotal = () => {
        return formData.orderDetails.reduce((totals, line) => {
            const lineTotal = calculateLineTotal(line);
            return {
                exclAmount: totals.exclAmount + lineTotal.exclAmount,
                taxAmount: totals.taxAmount + lineTotal.taxAmount,
                inclAmount: totals.inclAmount + lineTotal.inclAmount,
            };
        }, { exclAmount: 0, taxAmount: 0, inclAmount: 0 });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        const totals = calculateTotal();

        const data = {
            clientId: parseInt(formData.clientId),
            customerName: formData.customerName,
            address1: formData.address1,
            address2: formData.address2,
            address3: formData.address3,
            state: formData.state,
            postCode: formData.postCode,
            invoiceNo: formData.invoiceNo,
            invoiceDate: formData.invoiceDate,
            referenceNo: formData.referenceNo,
            totalExclAmount: totals.exclAmount,
            totalTaxAmount: totals.taxAmount,
            totalInclAmount: totals.inclAmount,
            orderDetails: formData.orderDetails.map((line, index) => {
                const lineTotal = calculateLineTotal(line);
                return {
                    itemId: parseInt(line.itemId),
                    itemCode: line.itemCode,
                    description: line.description,
                    note: line.note || '',
                    quantity: parseFloat(line.quantity),
                    unitPrice: parseFloat(line.unitPrice),
                    taxRate: parseFloat(line.taxRate || 0.07),
                    exclAmount: lineTotal.exclAmount,
                    taxAmount: lineTotal.taxAmount,
                    inclAmount: lineTotal.inclAmount,
                    lineNumber: index + 1,
                };
            }),
        };

        if (order) {
            await dispatch(updateOrder({ id: order.salesOrderId, data }));
            await dispatch(fetchOrders());
        } else {
            await dispatch(createOrder(data));
        }

        onClose();
    };

    const totals = calculateTotal();
    const isReadOnly = !!order;

    return (
        <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
                <div>
                    <label className="block text-sm font-medium mb-1">Customer *</label>
                    <select
                        name="clientId"
                        value={formData.clientId}
                        onChange={handleChange}
                        required
                        disabled={isReadOnly}
                        className="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:bg-gray-100"
                    >
                        <option value="">Select Customer</option>
                        {customers.map((customer) => (
                            <option key={customer.clientId} value={customer.clientId}>
                                {customer.customerName}
                            </option>
                        ))}
                    </select>
                </div>

                <div>
                    <label className="block text-sm font-medium mb-1">Invoice Date *</label>
                    <input
                        type="date"
                        name="invoiceDate"
                        value={formData.invoiceDate}
                        onChange={handleChange}
                        required
                        disabled={isReadOnly}
                        className="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:bg-gray-100"
                    />
                </div>
            </div>

            {formData.clientId && (
                <div className="col-span-2 bg-gray-50 p-4 rounded border">
                    <h3 className="font-semibold mb-2">Customer Details</h3>
                    <div className="space-y-1 text-sm">
                        <p><span className="font-medium">Name:</span> {formData.customerName}</p>
                        <p><span className="font-medium">Address:</span> {formData.address1}</p>
                        {formData.address2 && <p className="ml-16">{formData.address2}</p>}
                        {formData.address3 && <p className="ml-16">{formData.address3}</p>}
                        <p><span className="font-medium">State:</span> {formData.state}</p>
                        <p><span className="font-medium">Post Code:</span> {formData.postCode}</p>
                    </div>
                </div>
            )}

            <div className="grid grid-cols-2 gap-4">
                <div>
                    <label className="block text-sm font-medium mb-1">Invoice No *</label>
                    <input
                        type="text"
                        name="invoiceNo"
                        value={formData.invoiceNo}
                        onChange={handleChange}
                        required
                        disabled={isReadOnly}
                        className="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:bg-gray-100"
                    />
                </div>

                <div>
                    <label className="block text-sm font-medium mb-1">Reference No</label>
                    <input
                        type="text"
                        name="referenceNo"
                        value={formData.referenceNo}
                        onChange={handleChange}
                        disabled={isReadOnly}
                        className="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:bg-gray-100"
                    />
                </div>
            </div>

            <div>
                <div className="flex justify-between items-center mb-2">
                    <label className="block text-sm font-medium">Order Items *</label>
                    {!isReadOnly && (
                        <button
                            type="button"
                            onClick={addOrderLine}
                            className="text-blue-600 hover:text-blue-800 text-sm"
                        >
                            + Add Item
                        </button>
                    )}
                </div>

                <div className="space-y-2">
                    {formData.orderDetails.map((line, index) => (
                        <div key={index} className="flex gap-2 items-start border p-2 rounded">
                            <div className="flex-1">
                                <select
                                    value={line.itemId}
                                    onChange={(e) => updateOrderLine(index, 'itemId', e.target.value)}
                                    required
                                    disabled={isReadOnly}
                                    className="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm disabled:bg-gray-100"
                                >
                                    <option value="">Select Product</option>
                                    {products.map((product) => (
                                        <option key={product.itemId} value={product.itemId}>
                                            {product.itemCode} - {product.description}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <input
                                type="number"
                                value={line.quantity}
                                onChange={(e) => updateOrderLine(index, 'quantity', e.target.value)}
                                min="1"
                                step="0.01"
                                required
                                placeholder="Qty"
                                disabled={isReadOnly}
                                className="w-20 px-3 py-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm disabled:bg-gray-100"
                            />

                            <input
                                type="number"
                                value={line.unitPrice}
                                onChange={(e) => updateOrderLine(index, 'unitPrice', e.target.value)}
                                step="0.01"
                                min="0"
                                required
                                placeholder="Price"
                                disabled={isReadOnly}
                                className="w-24 px-3 py-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm disabled:bg-gray-100"
                            />

                            <input
                                type="number"
                                value={line.taxRate}
                                onChange={(e) => updateOrderLine(index, 'taxRate', e.target.value)}
                                step="0.01"
                                min="0"
                                max="1"
                                placeholder="Tax %"
                                disabled={isReadOnly}
                                className="w-20 px-3 py-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm disabled:bg-gray-100"
                            />

                            {!isReadOnly && (
                                <button
                                    type="button"
                                    onClick={() => removeOrderLine(index)}
                                    className="px-3 py-2 text-red-600 hover:text-red-800"
                                >
                                    ×
                                </button>
                            )}
                        </div>
                    ))}
                </div>

                {formData.orderDetails.length === 0 && (
                    <p className="text-sm text-gray-500 italic">No items added yet</p>
                )}
            </div>

            <div className="bg-gray-50 p-4 rounded space-y-2">
                <div className="flex justify-between">
                    <span>Subtotal (Excl Tax):</span>
                    <span className="font-semibold">${totals.exclAmount.toFixed(2)}</span>
                </div>
                <div className="flex justify-between">
                    <span>Tax:</span>
                    <span className="font-semibold">${totals.taxAmount.toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-lg font-bold border-t pt-2">
                    <span>Total (Incl Tax):</span>
                    <span>${totals.inclAmount.toFixed(2)}</span>
                </div>
            </div>

            <div className="flex justify-end space-x-3">
                <button
                    type="button"
                    onClick={onClose}
                    className="px-4 py-2 border rounded hover:bg-gray-50"
                >
                    Close
                </button>
                {!isReadOnly && (
                    <button
                        type="submit"
                        className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
                    >
                        {order ? 'Update Order' : 'Create Order'}
                    </button>
                )}
            </div>
        </form>
    );
}

export default OrderForm;