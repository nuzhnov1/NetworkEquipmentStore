using System;
using System.Collections.Generic;
using System.Data;
using NetworkEquipmentStore.Models.DB;

namespace NetworkEquipmentStore.Models.DAO
{
    public class OrderDAO
    {
        internal IEnumerable<ProductOrderInfo> GetAllProductsFromOrder(int OrderID)
        {
            string query = $"" +
                $"SELECT" +
                $"  sp.id AS id," +
                $"  name," +
                $"  description," +
                $"  category," +
                $"  image," +
                $"  price," +
                $"  sp.quantity AS total_quantity, " +
                $"  spl.quantity AS order_quantity " +
                $"FROM ShopProduct AS sp JOIN ShopProductsList AS spl ON sp.id = spl.product_id " +
                $"WHERE spl.order_id = {OrderID};";

            DataTable table = Database.Request(query);
            List<ProductOrderInfo> products = new List<ProductOrderInfo>();

            foreach (DataRow row in table.Rows)
            {
                ProductOrderInfo product = new ProductOrderInfo
                {
                    Product = new Product
                    {
                        ID = int.Parse(row["id"].ToString()),
                        Name = row["name"].ToString(),
                        Description = row["description"].ToString(),
                        Category = (ProductCategory)Enum.Parse(typeof(ProductCategory), row["category"].ToString()),
                        ImageName = row["image"].ToString(),
                        Price = decimal.Parse(row["price"].ToString()),
                        Quantity = int.Parse(row["total_quantity"].ToString())
                    },

                    Quantity = int.Parse(row["order_quantity"].ToString())
                };

                products.Add(product);
            }

            return products;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            string query = $"SELECT * FROM ShopOrder WHERE is_cart = FALSE;";
            DataTable table = Database.Request(query);
            List<Order> orders = new List<Order>();

            foreach (DataRow row in table.Rows)
            {
                int id = int.Parse(row["id"].ToString());
                int userID = int.Parse(row["user_id"].ToString());
                UserDAO userDAO = new UserDAO();
                User user = userDAO.GetUserByID(userID);
                IEnumerable<ProductOrderInfo> products = GetAllProductsFromOrder(id);
                DateTime date = DateTime.Parse(row["order_date"].ToString());
                string customerName = row["customer_name"].ToString();
                string customerPhone = row["customer_phone"].ToString();
                string customerEmail = row["customer_email"].ToString();
                string customerAddress = row["customer_address"].ToString();
                

                Order order = new Order
                {
                    ID = id,
                    User = user,
                    ProductsInfo = products,
                    Date = date,
                    CustomerName = customerName,
                    CustomerPhone = customerPhone,
                    CustomerEmail = customerEmail,
                    CustomerAddress = customerAddress
                };

                orders.Add(order);
            }

            return orders;
        }

        public IEnumerable<Order> GetAllOrdersOfUser(int userID)
        {
            UserDAO userDAO = new UserDAO();
            User user = userDAO.GetUserByID(userID);
            string query = $"SELECT * FROM ShopOrder WHERE user_id = {userID} AND is_cart = FALSE;";
            DataTable table = Database.Request(query);
            List<Order> orders = new List<Order>();

            foreach (DataRow row in table.Rows)
            {
                int id = int.Parse(row["id"].ToString());
                IEnumerable<ProductOrderInfo> products = GetAllProductsFromOrder(id);
                DateTime date = DateTime.Parse(row["order_date"].ToString());
                string customerName = row["customer_name"].ToString();
                string customerPhone = row["customer_phone"].ToString();
                string customerEmail = row["customer_email"].ToString();
                string customerAddress = row["customer_address"].ToString();

                Order order = new Order
                {
                    ID = id,
                    User = user,
                    ProductsInfo = products,
                    Date = date,
                    CustomerName = customerName,
                    CustomerPhone = customerPhone,
                    CustomerEmail = customerEmail,
                    CustomerAddress = customerAddress
                };

                orders.Add(order);
            }

            return orders;
        }

        public Order InsertOrder(Order order)
        {
            int userID = order.User.ID;
            DateTime date = order.Date;
            string customerName = order.CustomerName;
            string customerPhone = order.CustomerPhone;
            string customerEmail = order.CustomerEmail;
            string customerAddress = order.CustomerAddress;

            string query = $"INSERT INTO ShopOrder(user_id, order_date, is_cart, customer_name, customer_phone, customer_email, customer_address) VALUES ({userID}, '{date}', FALSE, '{customerName}', '{customerPhone}', '{customerEmail}', '{customerAddress}') RETURNING id;";
            int id = int.Parse(Database.Request(query).Rows[0]["id"].ToString());

            foreach (ProductOrderInfo productOrderInfo in order.ProductsInfo)
            {
                int productID = productOrderInfo.Product.ID;
                int quantity = productOrderInfo.Quantity;

                string productsListQuery = $"INSERT INTO ShopProductsList(order_id, product_id, quantity) VALUES ({id}, {productID}, {quantity});";
                Database.Execute(productsListQuery);
            }

            order.ID = id;
            return order;
        }

        public void DeleteOrderByID(int orderID)
        {
            string productsListQuery = $"DELETE FROM ShopProductsList WHERE order_id = {orderID};";
            Database.Execute(productsListQuery);
            string query = $"DELETE FROM ShopOrder WHERE id = ${orderID};";
            Database.Execute(query);
        }
    }
}
