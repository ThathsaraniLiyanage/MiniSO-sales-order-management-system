using System;
using System.Collections.Generic;

namespace MiniSO.Domain.Entities
{
    public class Item
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; }

        public Item()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
            SalesOrderDetails = new HashSet<SalesOrderDetail>();
        }
    }
}