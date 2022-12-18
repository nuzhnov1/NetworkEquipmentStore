using System;
using System.Collections.Generic;
using System.Data;
using NetworkEquipmentStore.Models.DB;

namespace NetworkEquipmentStore.Models.DAO
{
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
                ImageName = row["image"].ToString(),
                Price = decimal.Parse(row["price"].ToString()),
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
                    ImageName = row["image"].ToString(),
                    Price = decimal.Parse(row["price"].ToString()),
                    Quantity = int.Parse(row["quantity"].ToString())
                };

                products.Add(product);
            }

            return products;
        }

        public int GetAllProductsCountByCategory(ProductCategory category)
        {
            string query;
            if (category != ProductCategory.NONE)
            {
                query = $"SELECT COUNT(*) AS result FROM ShopProduct WHERE category = '{category}';";
            }
            else
            {
                query = $"SELECT COUNT(*) AS result FROM ShopProduct;";
            }

            return int.Parse(Database.Request(query).Rows[0]["result"].ToString());
        }

        public Product InsertProduct(Product product)
        {
            string name = product.Name.Replace("'", "\\'");
            string description = product.Description.Replace("'", "\\'");
            ProductCategory category = product.Category;
            string image = product.ImageName;
            string price = product.Price.ToString().Replace(',', '.');
            int quantity = product.Quantity;

            string query = $"INSERT INTO ShopProduct(name, description, category, image, price, quantity) VALUES ('{name}', '{description}', '{category}', '{image}', {price}, {quantity}) RETURNING id;";
            int id = int.Parse(Database.Request(query).Rows[0]["id"].ToString());

            product.ID = id;
            return product;
        }

        public Product UpdateProduct(Product product)
        {
            int id = product.ID;
            string name = product.Name.Replace("'", "\\'");
            string description = product.Description.Replace("'", "\\'");
            ProductCategory category = product.Category;
            string image = product.ImageName;
            string price = product.Price.ToString().Replace(',', '.');
            int quantity = product.Quantity;

            string query = $"UPDATE ShopProduct SET name = '{name}', description = '{description}', category = '{category}', image = '{image}', price = {price}, quantity = {quantity} WHERE id = {id};";
            Database.Execute(query);

            return product;
        }

        public void DeleteProductByID(int productID)
        {
            string queryShopProductsList = $"DELETE FROM ShopProductsList WHERE product_id = {productID};";
            Database.Execute(queryShopProductsList);
            string query = $"DELETE FROM ShopProduct WHERE id = {productID};";
            Database.Execute(query);
        }
    }
}
