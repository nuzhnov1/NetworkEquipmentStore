using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text.RegularExpressions;

namespace NetworkEquipmentStore.Models.DAO
{
    using Database;


    public class ProductDAO
    {
        public Product GetProductByID(int id)
        {
            string query = $"SELECT * FROM ShopProduct WHERE id = {id};";
            DataTable table = Database.Request(query);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = table.Rows[0];

            return new Product
            {
                ID = id,
                Name = row["name"].ToString(),
                Description = row["description"].ToString(),
                Category = (ProductCategory)Enum.Parse(typeof(ProductCategory), row["category"].ToString()),
                ImagePath = $"Content/images/{row["image"]}.png",
                Price = decimal.Parse(row["price"].ToString(), System.Globalization.NumberStyles.Currency),
                Quantity = int.Parse(row["quantity"].ToString())
            };
        }

        public IEnumerable<Product> GetAllProducts()
        {
            string query = $"SELECT * FROM ShopProduct;";
            DataTable table = Database.Request(query);
            List<Product> products = new List<Product>();

            foreach (DataRow row in table.Rows)
            {
                Product product = new Product
                {
                    ID = int.Parse(row["id"].ToString()),
                    Name = row["name"].ToString(),
                    Description = row["description"].ToString(),
                    Category = (ProductCategory)Enum.Parse(typeof(ProductCategory), row["category"].ToString()),
                    ImagePath = $"/Content/images/{row["image"]}.png",
                    Price = decimal.Parse(row["price"].ToString(), System.Globalization.NumberStyles.Currency),
                    Quantity = int.Parse(row["quantity"].ToString())
                };

                products.Add(product);
            }

            return products;
        }

        public int GetAllProductsCountByCategory(ProductCategory category)
        {
            if (category != ProductCategory.NONE)
            {
                string query = $"SELECT COUNT(*) AS result FROM ShopProduct WHERE category = '{category}';";
                return int.Parse(Database.Request(query).Rows[0]["result"].ToString());
            }
            else
            {
                string query = $"SELECT COUNT(*) AS result FROM ShopProduct;";
                return int.Parse(Database.Request(query).Rows[0]["result"].ToString());
            }
        }

        public void InsertProducts(params Product[] products)
        {
            foreach (Product product in products)
            {
                string name = product.Name;
                string description = product.Description;
                ProductCategory category = product.Category;
                string imageGUID = Regex.Match(product.Name, "Content/images/(.*).png").Groups[1].Value;
                decimal price = product.Price;
                int quantity = product.Quantity;

                string query = $"INSERT INTO ShopProduct(name, description, category, image, price, quantity) VALUES ('{name}', '{description}', '{category}', '{imageGUID}', {price}, {quantity});";
                Database.Execute(query);
            }
        }

        public void UpdateProducts(params Product[] products)
        {
            foreach (Product product in products)
            {
                int id = product.ID;
                string name = product.Name;
                string description = product.Description;
                ProductCategory category = product.Category;
                string imageGUID = Regex.Match(product.Name, "Content/images/(.*).png").Groups[1].Value;
                decimal price = product.Price;
                int quantity = product.Quantity;

                string query = $"UPDATE ShopProduct SET name = '{name}', description = '{description}', category = '{category}', image = '{imageGUID}', price = {price}, quantity = {quantity} WHERE id = {id};";
                Database.Execute(query);
            }
        }

        public void DeleteProductByID(int id)
        {
            string queryShopProductsList = $"DELETE FROM ShopProductsList WHERE product_id = {id};";
            Database.Execute(queryShopProductsList);
            string query = $"DELETE FROM ShopProduct WHERE id = {id};";
            Database.Execute(query);
        }

        public void DeleteProducts(params Product[] products)
        {
            foreach (Product product in products)
            {
                int id = product.ID;

                string queryShopProductsList = $"DELETE FROM ShopProductsList WHERE product_id = {id};";
                Database.Execute(queryShopProductsList);
                string query = $"DELETE FROM ShopProduct WHERE id = {id};";
                Database.Execute(query);
            }
        }
    }
}
