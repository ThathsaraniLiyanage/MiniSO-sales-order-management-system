using System;
using System.Collections.Generic;

namespace MiniSO.Domain.Entities
{
    public class Client
    {
        public int ClientId { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<SalesOrder> SalesOrders { get; set; }

        public Client()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
            SalesOrders = new HashSet<SalesOrder>();
        }
    }
}