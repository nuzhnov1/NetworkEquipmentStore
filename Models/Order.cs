using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkEquipmentStore.Models
{
    public class ProductOrderInfo
    {
        public string ProductName { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
    }

    public class Order
    {
        public int ID { get; set; }
        public User User { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public IEnumerable<ProductOrderInfo> ProductsInfo { get; set; }
    }

    public static class OrderExtensions
    {
        public static decimal TotalCost(this Order order) =>
            order.ProductsInfo.Sum(productInfo => productInfo.Quantity * productInfo.ProductPrice);
    }
}
