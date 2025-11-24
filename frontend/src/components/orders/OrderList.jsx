import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { fetchOrders, deleteOrder } from '../../store/slices/orderSlice';
import OrderForm from './OrderForm';
import Modal from '../shared/Modal';
import LoadingSpinner from '../shared/LoadingSpinner';
import OrderPrint from './OrderPrint';

function OrderList() {
    const dispatch = useDispatch();
    const { items, loading } = useSelector((state) => state.orders);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingOrder, setEditingOrder] = useState(null);
    const [printOrder, setPrintOrder] = useState(null);

    useEffect(() => {
        dispatch(fetchOrders());
    }, [dispatch]);

    const handleEdit = (order) => {
        setEditingOrder(order);
        setIsModalOpen(true);
    };

    const handleDelete = async (id) => {
        if (window.confirm('Are you sure you want to delete this order?')) {
            await dispatch(deleteOrder(id));
        }
    };

    const handleCloseModal = () => {
        setIsModalOpen(false);
        setEditingOrder(null);
    };

    const handlePrint = (order) => {
        setPrintOrder(order);
    };

    if (loading) return <LoadingSpinner />;

    return (
        <div>
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-3xl font-bold">Sales Orders</h1>
                <button
                    onClick={() => setIsModalOpen(true)}
                    className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                >
                    Create Order
                </button>
            </div>

            <div className="bg-white rounded-lg shadow overflow-hidden">
                <table className="min-w-full">
                    <thead className="bg-gray-50">
                        <tr>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Invoice No</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Customer</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Total (Excl)</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Tax</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Total (Incl)</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Actions</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-200">
                        {items.map((order) => (
                            <tr
                                key={order.salesOrderId}
                                onDoubleClick={() => handleEdit(order)}
                                className="cursor-pointer hover:bg-gray-50"
                            >
                                <td className="px-6 py-4">{order.invoiceNo}</td>
                                <td className="px-6 py-4">{order.customerName}</td>
                                <td className="px-6 py-4">
                                    {new Date(order.invoiceDate).toLocaleDateString()}
                                </td>
                                <td className="px-6 py-4">${order.totalExclAmount.toFixed(2)}</td>
                                <td className="px-6 py-4">${order.totalTaxAmount.toFixed(2)}</td>
                                <td className="px-6 py-4 font-bold">${order.totalInclAmount.toFixed(2)}</td>
                                <td className="px-6 py-4">
                                    <button
                                        onClick={() => handleEdit(order)}
                                        className="text-blue-600 hover:text-blue-800 mr-3"
                                    >
                                        View
                                    </button>
                                    <button
                                        onClick={() => handlePrint(order)}
                                        className="text-green-600 hover:text-green-800 mr-3"
                                    >
                                        Print
                                    </button>
                                    <button
                                        onClick={() => handleDelete(order.salesOrderId)}
                                        className="text-red-600 hover:text-red-800"
                                    >
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>

                {printOrder && (
                    <OrderPrint
                        order={printOrder}
                        onClose={() => setPrintOrder(null)}
                    />
                )}
            </div>

            <Modal
                isOpen={isModalOpen}
                onClose={handleCloseModal}
                title={editingOrder ? 'View Order' : 'Create Order'}
            >
                <OrderForm
                    order={editingOrder}
                    onClose={handleCloseModal}
                />
            </Modal>
        </div>
    );
}

export default OrderList;