using NetworkEquipmentStore.Models.DB;
using System;
using System.Collections.Generic;
using System.Data;

namespace NetworkEquipmentStore.Models.DAO
{
    internal class CartDAO
    {
        private List<CartLine> GetAllCartLines(int cartID)
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
                $"  scl.quantity AS cart_quantity " +
                $"FROM ShopProduct AS sp JOIN ShopCartLine AS scl ON sp.id = scl.product_id " +
                $"WHERE scl.cart_id = {cartID};";

            DataTable table = Database.Request(query);
            List<CartLine> cartLines = new List<CartLine>();

            foreach (DataRow row in table.Rows)
            {
                CartLine cartLine = new CartLine
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
                    Quantity = int.Parse(row["cart_quantity"].ToString())
                };

                cartLines.Add(cartLine);
            }

            return cartLines;
        }

        internal Cart GetUserCart(int userID)
        {
            string userQuery = $"SELECT cart_id FROM ShopUser WHERE id = {userID};";
            int cartID = int.Parse(Database.Request(userQuery).Rows[0]["cart_id"].ToString());
            string query = $"SELECT * FROM ShopCart WHERE id = {cartID};";
            DataTable table = Database.Request(query);

            DateTime lastUpdate = DateTime.Parse(table.Rows[0]["last_update"].ToString());
            List<CartLine> cartLines = GetAllCartLines(cartID);

            return new Cart
            {
                ID = cartID,
                LastUpdate = lastUpdate,
                Lines = cartLines
            };
        }

        internal Cart InsertCart(Cart cart)
        {
            DateTime lastUpdate = DateTime.Now;

            string query = $"INSERT INTO ShopCart(last_update) VALUES ('{lastUpdate}') RETURNING id;";
            int id = int.Parse(Database.Request(query).Rows[0]["id"].ToString());

            cart.ID = id;
            return cart;
        }

        internal Cart UpdateCart(Cart cart)
        {
            int id = cart.ID;
            DateTime lastUpdate = DateTime.Now;

            string query = $"UPDATE ShopCart SET last_update = '{lastUpdate}' WHERE id = {id};";
            Database.Execute(query);
            string clearQuery = $"DELETE FROM ShopCartLine WHERE cart_id = {id};";
            Database.Execute(clearQuery);

            foreach (CartLine cartLine in cart.Lines)
            {
                int productID = cartLine.Product.ID;
                int quantity = cartLine.Quantity;

                string cartLineQuery = $"INSERT INTO ShopCartLine(cart_id, product_id, quantity) VALUES ({id}, {productID}, {quantity});";
                Database.Execute(cartLineQuery);
            }

            cart.LastUpdate = lastUpdate;
            return cart;
        }

        internal void DeleteCart(Cart cart)
        {
            int id = cart.ID;

            string cartLineQuery = $"DELETE FROM ShopCartLine WHERE cart_id = {id};";
            Database.Execute(cartLineQuery);
            string query = $"DELETE FROM ShopCart WHERE id = {id};";
            Database.Execute(query);
        }
    }
}
