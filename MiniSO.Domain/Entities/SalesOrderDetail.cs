using System;

namespace MiniSO.Domain.Entities
{
    public class SalesOrderDetail
    {
        public int SalesOrderDetailId { get; set; }
        public int SalesOrderId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        public decimal ExclAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal InclAmount { get; set; }
        public int LineNumber { get; set; }
        public virtual SalesOrder SalesOrder { get; set; }
        public virtual Item Item { get; set; }

        public void CalculateAmounts()
        {
            ExclAmount = Quantity * UnitPrice;
            TaxAmount = ExclAmount * TaxRate / 100;
            InclAmount = ExclAmount + TaxAmount;
            ExclAmount = Math.Round(ExclAmount, 2);
            TaxAmount = Math.Round(TaxAmount, 2);
            InclAmount = Math.Round(InclAmount, 2);
        }
    }
}