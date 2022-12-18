using NetworkEquipmentStore.Models.DB;
using System;
using System.Collections.Generic;
using System.Data;

namespace NetworkEquipmentStore.Models.DAO
{
    public class CartDAO
    {
        public Cart GetUserCart(int userID)
        {
            string query = $"SELECT id, order_date FROM ShopOrder WHERE user_id = {userID} AND is_cart = TRUE;";
            DataTable table = Database.Request(query);

            int id = int.Parse(table.Rows[0]["id"].ToString());
            DateTime data = DateTime.Parse(table.Rows[0]["order_date"].ToString());
            OrderDAO orderDAO = new OrderDAO();
            IEnumerable<ProductOrderInfo> products = orderDAO.GetAllProductsFromOrder(id);

            return new Cart
            {
                Lines = products,
                LastUpdatedTime = data
            };
        }

        public Cart InsertCart(User user)
        {
            int userID = user.ID;
            DateTime date = DateTime.Now;

            string query = $"INSERT INTO ShopOrder(user_id, order_date, is_cart) VALUES ({userID}, '{date}', TRUE);";
            Database.Execute(query);

            return new Cart();
        }

        public Cart UpdateCart(Cart cart, User user)
        {
            int userID = user.ID;
            DateTime date = DateTime.Now;

            string query = $"UPDATE ShopOrder SET order_date = '{date}' WHERE user_id = {userID} AND is_cart = TRUE RETURNING id;";
            int id = int.Parse(Database.Request(query).Rows[0]["id"].ToString());
            string clearQuery = $"DELETE FROM ShopProductsList WHERE order_id = {id};";
            Database.Execute(clearQuery);

            foreach (ProductOrderInfo line in cart.Lines)
            {
                int productID = line.Product.ID;
                int quantity = line.Quantity;

                string productsListQuery = $"INSERT INTO ShopProductsList(order_id, product_id, quantity) VALUES ({id}, {productID}, {quantity});";
                Database.Execute(productsListQuery);
            }

            return cart;
        }

        public void DeleteCart(User user)
        {
            int userID = user.ID;

            string orderIDQuery = $"SELECT id FROM ShopOrder WHERE user_id = {userID} AND is_cart = TRUE;";
            int orderID = int.Parse(Database.Request(orderIDQuery).Rows[0]["id"].ToString());

            string productsListQuery = $"DELETE FROM ShopProductsList WHERE order_id = {orderID};";
            Database.Execute(productsListQuery);
            string query = $"DELETE FROM ShopOrder WHERE id = ${orderID};";
            Database.Execute(query);
        }
    }
}
