using System;
using System.Linq;

namespace NetworkEquipmentStore.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public PermissionsLevel Level { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public bool IsBanned { get; set; }
        public Cart Cart { get; set; }
    }

    public static class UserExtensions
    {
        public static Order FormOrder(this User user,
            string customerName, string customerPhone,
            string customerEmail, string customerAddress
        ) => new Order
        {
            ID = 0,
            User = user,
            OrderDate = DateTime.Now,
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            CustomerEmail = customerEmail,
            CustomerAddress = customerAddress,
            ProductsInfo = user.Cart.Lines.Select(line => new ProductOrderInfo
            {
                ProductName = line.Product.Name,
                ProductCategory = line.Product.Category,
                ProductPrice = line.Product.Price,
                Quantity = line.Quantity
            })
        };
    }
}
