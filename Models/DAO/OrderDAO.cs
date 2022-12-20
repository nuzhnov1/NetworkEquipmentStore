using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using NetworkEquipmentStore.Models.DB;

namespace NetworkEquipmentStore.Models.DAO
{
    public class OrderDAO
    {
        private IEnumerable<ProductOrderInfo> GetAllProductsFromOrder(int orderID)
        {
            string query = $"SELECT * FROM ShopProductOrderInfo WHERE order_id = {orderID};";

            DataTable table = Database.Request(query);
            List<ProductOrderInfo> products = new List<ProductOrderInfo>();

            foreach (DataRow row in table.Rows)
            {
                ProductOrderInfo product = new ProductOrderInfo
                {
                    ProductName = row["product_name"].ToString(),
                    ProductCategory = (ProductCategory)Enum.Parse(typeof(ProductCategory), row["product_category"].ToString()),
                    ProductPrice = decimal.Parse(row["product_price"].ToString()),
                    Quantity = int.Parse(row["quantity"].ToString()),
                };

                products.Add(product);
            }

            return products;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            string query = $"SELECT * FROM ShopOrder;";
            DataTable table = Database.Request(query);
            List<Order> orders = new List<Order>();

            foreach (DataRow row in table.Rows)
            {
                int id = int.Parse(row["id"].ToString());
                int userID = int.Parse(row["user_id"].ToString());
                UserDAO userDAO = new UserDAO();
                User user = userDAO.GetUserByID(userID);
                DateTime date = DateTime.Parse(row["order_date"].ToString());
                string customerName = row["customer_name"].ToString();
                string customerPhone = row["customer_phone"].ToString();
                string customerEmail = row["customer_email"].ToString();
                string customerAddress = row["customer_address"].ToString();
                IEnumerable<ProductOrderInfo> products = GetAllProductsFromOrder(id);

                Order order = new Order
                {
                    ID = id,
                    User = user,
                    OrderDate = date,
                    CustomerName = customerName,
                    CustomerPhone = customerPhone,
                    CustomerEmail = customerEmail,
                    CustomerAddress = customerAddress,
                    ProductsInfo = products
                };

                orders.Add(order);
            }

            return orders;
        }

        public int GetAllOrdersCount(User user = null)
        {
            string query;
            if (user != null)
            {
                query = $"SELECT COUNT(*) AS result FROM ShopOrder WHERE user_id = {user.ID};";
            }
            else
            {
                query = "SELECT COUNT(*) AS result FROM ShopOrder;";
            }

            return int.Parse(Database.Request(query).Rows[0]["result"].ToString());
        }

        public Order InsertOrder(Order order)
        {
            int userID = order.User.ID;
            DateTime orderDate = order.OrderDate;
            string customerName = order.CustomerName.Replace("'", "\\'");
            string customerPhone = order.CustomerPhone.Replace("'", "\\'");
            string customerEmail = order.CustomerEmail.Replace("'", "\\'");
            string customerAddress = order.CustomerAddress.Replace("'", "\\'");

            string query = $"INSERT INTO ShopOrder(user_id, order_date, customer_name, customer_phone, customer_email, customer_address) VALUES ({userID}, '{orderDate}', '{customerName}', '{customerPhone}', '{customerEmail}', '{customerAddress}') RETURNING id;";
            int id = int.Parse(Database.Request(query).Rows[0]["id"].ToString());

            foreach (ProductOrderInfo productOrderInfo in order.ProductsInfo)
            {
                string productName = productOrderInfo.ProductName.Replace("'", "\\'");
                ProductCategory productCategory = productOrderInfo.ProductCategory;
                string productPrice = productOrderInfo.ProductPrice.ToString().Replace(',', '.');
                int quantity = productOrderInfo.Quantity;

                string productsListQuery = $"INSERT INTO ShopProductOrderInfo(order_id, product_name, product_category, product_price, quantity) VALUES ({id}, '{productName}', '{productCategory}', {productPrice}, {quantity});";
                Database.Execute(productsListQuery);
            }

            order.ID = id;
            return order;
        }

        public void DeleteOrderByID(int orderID)
        {
            string productOrderInfoQuery = $"DELETE FROM ShopProductOrderInfo WHERE order_id = {orderID};";
            Database.Execute(productOrderInfoQuery);
            string query = $"DELETE FROM ShopOrder WHERE id = {orderID};";
            Database.Execute(query);
        }

        public void DeleteAllOrders(int? userID)
        {
            IEnumerable<Order> orders = GetAllOrders()
                .Where(order => userID == null || order.User.ID == userID);

            foreach (Order order in orders)
            {
                DeleteOrderByID(order.ID);
            }
        }
    }
}
