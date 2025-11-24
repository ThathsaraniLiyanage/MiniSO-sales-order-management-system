import { useRef } from 'react';

function OrderPrint({ order, onClose }) {
    const printRef = useRef();

    const handlePrint = () => {
        const printContent = printRef.current;
        const windowPrint = window.open('', '', 'width=800,height=600');

        windowPrint.document.write(`
      <html>
        <head>
          <title>Sales Order - ${order.invoiceNo}</title>
          <style>
            body { font-family: Arial, sans-serif; padding: 20px; }
            .header { text-align: center; margin-bottom: 30px; }
            .header h1 { margin: 0; color: #2563eb; }
            .info-section { margin-bottom: 20px; }
            .info-row { display: flex; margin-bottom: 5px; }
            .label { font-weight: bold; width: 150px; }
            table { width: 100%; border-collapse: collapse; margin: 20px 0; }
            th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
            th { background-color: #2563eb; color: white; }
            .totals { margin-top: 20px; text-align: right; }
            .totals-row { margin: 5px 0; }
            .total-label { font-weight: bold; margin-right: 20px; }
            .grand-total { font-size: 1.2em; font-weight: bold; border-top: 2px solid #000; padding-top: 10px; margin-top: 10px; }
            @media print {
              button { display: none; }
            }
          </style>
        </head>
        <body>
          ${printContent.innerHTML}
        </body>
      </html>
    `);

        windowPrint.document.close();
        windowPrint.focus();
        windowPrint.print();
        windowPrint.close();
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg p-6 max-w-4xl w-full mx-4 max-h-[90vh] overflow-y-auto">
                <div className="flex justify-between items-center mb-4">
                    <h2 className="text-2xl font-bold">Print Preview</h2>
                    <div className="space-x-2">
                        <button
                            onClick={handlePrint}
                            className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                        >
                            Print
                        </button>
                        <button
                            onClick={onClose}
                            className="bg-gray-300 px-4 py-2 rounded hover:bg-gray-400"
                        >
                            Close
                        </button>
                    </div>
                </div>

                <div ref={printRef} className="border p-8">
                    {/* Header */}
                    <div className="header text-center mb-8">
                        <h1 className="text-3xl font-bold text-blue-600 mb-2">SALES ORDER</h1>
                        <p className="text-lg">Sales Order Management System</p>
                    </div>

                    {/* Invoice Information */}
                    <div className="info-section grid grid-cols-2 gap-8 mb-6">
                        <div>
                            <h3 className="font-bold text-lg mb-3 border-b pb-2">Order Information</h3>
                            <div className="space-y-2">
                                <div className="flex">
                                    <span className="label font-semibold w-32">Invoice No:</span>
                                    <span>{order.invoiceNo}</span>
                                </div>
                                <div className="flex">
                                    <span className="label font-semibold w-32">Invoice Date:</span>
                                    <span>{new Date(order.invoiceDate).toLocaleDateString()}</span>
                                </div>
                                <div className="flex">
                                    <span className="label font-semibold w-32">Reference No:</span>
                                    <span>{order.referenceNo}</span>
                                </div>
                            </div>
                        </div>

                        <div>
                            <h3 className="font-bold text-lg mb-3 border-b pb-2">Customer Details</h3>
                            <div className="space-y-2">
                                <div className="flex">
                                    <span className="label font-semibold w-32">Customer:</span>
                                    <span>{order.customerName}</span>
                                </div>
                                <div className="flex">
                                    <span className="label font-semibold w-32">Address:</span>
                                    <div>
                                        <div>{order.address1}</div>
                                        {order.address2 && <div>{order.address2}</div>}
                                        {order.address3 && <div>{order.address3}</div>}
                                        <div>{order.state} {order.postCode}</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Order Items Table */}
                    <table className="w-full border-collapse mb-6">
                        <thead>
                            <tr className="bg-blue-600 text-white">
                                <th className="border p-2 text-left">#</th>
                                <th className="border p-2 text-left">Item Code</th>
                                <th className="border p-2 text-left">Description</th>
                                <th className="border p-2 text-right">Qty</th>
                                <th className="border p-2 text-right">Unit Price</th>
                                <th className="border p-2 text-right">Tax Rate</th>
                                <th className="border p-2 text-right">Amount (Excl)</th>
                                <th className="border p-2 text-right">Tax</th>
                                <th className="border p-2 text-right">Amount (Incl)</th>
                            </tr>
                        </thead>
                        <tbody>
                            {order.orderDetails.map((detail, index) => (
                                <tr key={detail.salesOrderDetailId}>
                                    <td className="border p-2">{index + 1}</td>
                                    <td className="border p-2">{detail.itemCode}</td>
                                    <td className="border p-2">{detail.description}</td>
                                    <td className="border p-2 text-right">{detail.quantity.toFixed(2)}</td>
                                    <td className="border p-2 text-right">${detail.unitPrice.toFixed(2)}</td>
                                    <td className="border p-2 text-right">{(detail.taxRate * 100).toFixed(0)}%</td>
                                    <td className="border p-2 text-right">${detail.exclAmount.toFixed(2)}</td>
                                    <td className="border p-2 text-right">${detail.taxAmount.toFixed(2)}</td>
                                    <td className="border p-2 text-right">${detail.inclAmount.toFixed(2)}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>

                    {/* Totals */}
                    <div className="totals mt-6">
                        <div className="totals-row">
                            <span className="total-label">Subtotal (Excl Tax):</span>
                            <span className="font-semibold">${order.totalExclAmount.toFixed(2)}</span>
                        </div>
                        <div className="totals-row">
                            <span className="total-label">Total Tax:</span>
                            <span className="font-semibold">${order.totalTaxAmount.toFixed(2)}</span>
                        </div>
                        <div className="grand-total totals-row">
                            <span className="total-label">GRAND TOTAL (Incl Tax):</span>
                            <span>${order.totalInclAmount.toFixed(2)}</span>
                        </div>
                    </div>

                    {/* Footer */}
                    <div className="mt-12 pt-6 border-t text-center text-sm text-gray-600">
                        <p>Thank you for your business!</p>
                        <p className="mt-2">Generated on: {new Date().toLocaleString()}</p>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default OrderPrint;