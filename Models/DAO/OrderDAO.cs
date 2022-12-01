using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace NetworkEquipmentStore.Models.DAO
{
    using Database;

    public class OrderDAO
    {
        private IEnumerable<ProductOrderInfo> GetAllProductsFromOrder(int OrderID)
        {
            string query = $"" +
                $"SELECT" +
                $"  sp.id AS id," +
                $"  name," +
                $"  description," +
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
                        ImagePath = $"Content/images/{row["image"]}.png",
                        Price = decimal.Parse(row["price"].ToString()),
                        Quantity = int.Parse(row["total_quantity"].ToString())
                    },

                    Quantity = int.Parse(row["order_quantity"].ToString())
                };

                products.Add(product);
            }

            return products;
        }

        public Order GetOrderByID(int id)
        {
            string query = $"SELECT * FROM ShopOrder WHERE id = {id};";
            DataRow row = Database.Request(query).Rows[0];

            int UserID = int.Parse(row["user_id"].ToString());
            DateTime date = DateTime.Parse(row["order_date"].ToString());
            bool IsPaid = bool.Parse(row["is_paid"].ToString());

            UserDAO userDAO = new UserDAO();
            User user = userDAO.GetUserByID(UserID);

            IEnumerable<ProductOrderInfo> products = GetAllProductsFromOrder(id);

            return new Order
            {
                ID = id,
                User = user,
                ProductsInfo = products,
                Date = date,
                IsPaid = IsPaid
            };
        }

        public IEnumerable<Order> GetAllOrders()
        {
            string query = $"SELECT * FROM ShopOrder;";
            DataTable table = Database.Request(query);
            List<Order> orders = new List<Order>();

            foreach (DataRow row in table.Rows)
            {
                int id = int.Parse(row["id"].ToString());
                int UserID = int.Parse(row["user_id"].ToString());
                DateTime date = DateTime.Parse(row["order_date"].ToString());
                bool IsPaid = bool.Parse(row["is_paid"].ToString());

                UserDAO userDAO = new UserDAO();
                User user = userDAO.GetUserByID(UserID);

                IEnumerable<ProductOrderInfo> products = GetAllProductsFromOrder(id);

                Order order = new Order
                {
                    ID = id,
                    User = user,
                    ProductsInfo = products,
                    Date = date,
                    IsPaid = IsPaid
                };

                orders.Add(order);
            }

            return orders;
        }

        public IEnumerable<Order> GetAllOrdersOfUser(User user)
        {
            int userID = user.ID;
            string query = $"SELECT * FROM ShopOrder WHERE user_id = {userID};";
            DataTable table = Database.Request(query);
            List<Order> orders = new List<Order>();

            foreach (DataRow row in table.Rows)
            {
                int id = int.Parse(row["id"].ToString());
                DateTime date = DateTime.Parse(row["order_date"].ToString());
                bool IsPaid = bool.Parse(row["is_paid"].ToString());

                IEnumerable<ProductOrderInfo> products = GetAllProductsFromOrder(id);

                Order order = new Order
                {
                    ID = id,
                    User = user,
                    ProductsInfo = products,
                    Date = date,
                    IsPaid = IsPaid
                };

                orders.Add(order);
            }

            return orders;
        }

        public IEnumerable<Order> GetNotPaidOrdersOfUser(User user)
        {
            int userID = user.ID;
            string query = $"SELECT * FROM ShopOrder WHERE user_id = {userID} AND is_paid = FALSE;";
            DataTable table = Database.Request(query);
            List<Order> orders = new List<Order>();

            foreach (DataRow row in table.Rows)
            {
                int id = int.Parse(row["id"].ToString());
                DateTime date = DateTime.Parse(row["order_date"].ToString());

                IEnumerable<ProductOrderInfo> products = GetAllProductsFromOrder(id);

                Order order = new Order
                {
                    ID = id,
                    User = user,
                    ProductsInfo = products,
                    Date = date,
                    IsPaid = false
                };

                orders.Add(order);
            }

            return orders;
        }

        public void InsertOrders(params Order[] orders)
        {
            foreach (Order order in orders)
            {
                int id = order.ID;
                int userID = order.User.ID;
                DateTime date = order.Date;
                bool IsPaid = order.IsPaid;

                string query = $"INSERT INTO ShopOrder(user_id, order_date, is_paid) VALUES ({userID}, '{date}', {IsPaid});";
                Database.Execute(query);

                foreach (ProductOrderInfo productOrderInfo in order.ProductsInfo)
                {
                    int productID = productOrderInfo.Product.ID;
                    int quantity = productOrderInfo.Quantity;

                    string productsListQuery = $"INSERT INTO ShopProductsList(order_id, product_id, quantity) VALUES ({id}, {productID}, {quantity});";
                    Database.Execute(productsListQuery);
                }
            }
        }

        public void UpdateOrders(params Order[] orders)
        {
            foreach (Order order in orders)
            {
                int id = order.ID;
                int userID = order.User.ID;
                DateTime date = order.Date;
                bool IsPaid = order.IsPaid;

                string query = $"UPDATE ShopOrder SET user_id = {userID}, order_date = '{date}', is_paid = {IsPaid} WHERE id = ${id};";
                Database.Execute(query);

                foreach (ProductOrderInfo productOrderInfo in order.ProductsInfo)
                {
                    int productID = productOrderInfo.Product.ID;
                    int quantity = productOrderInfo.Quantity;

                    string productsListQuery = $"UPDATE ShopProductsList SET quantity = {quantity} WHERE order_id = {id} AND product_id = {productID};";
                    Database.Execute(productsListQuery);
                }
            }
        }

        public void DeleteOrders(params Order[] orders)
        {
            foreach (Order order in orders)
            {
                int id = order.ID;

                string productsListQuery = $"DELETE FROM ShopProductsList WHERE order_id = {id};";
                Database.Execute(productsListQuery);
                string query = $"DELETE FROM ShopOrder WHERE id = ${id};";
                Database.Execute(query);
            }
        }
    }
}
