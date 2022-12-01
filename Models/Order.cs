using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetworkEquipmentStore.Models
{
    public class ProductOrderInfo
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }

    public class Order
    {
        public int ID { get; set; }
        public User User { get; set; }
        public IEnumerable<ProductOrderInfo> ProductsInfo { get; set; }
        public DateTime Date { get; set; }
        public bool IsPaid { get; set; }
    }
}
