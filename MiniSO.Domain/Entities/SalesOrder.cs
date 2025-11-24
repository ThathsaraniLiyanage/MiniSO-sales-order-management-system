using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniSO.Domain.Entities
{
    public class SalesOrder
    {
        public int SalesOrderId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string ReferenceNo { get; set; }
        public int ClientId { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public decimal TotalExclAmount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalInclAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public virtual Client Client { get; set; }
        public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; }

        public SalesOrder()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
            InvoiceDate = DateTime.Now.Date;
            SalesOrderDetails = new HashSet<SalesOrderDetail>();
        }

        public void CalculateTotals()
        {
            TotalExclAmount = SalesOrderDetails?.Sum(d => d.ExclAmount) ?? 0;
            TotalTaxAmount = SalesOrderDetails?.Sum(d => d.TaxAmount) ?? 0;
            TotalInclAmount = SalesOrderDetails?.Sum(d => d.InclAmount) ?? 0;
        }
    }
}