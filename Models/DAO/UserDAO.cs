using System;
using System.Collections.Generic;
using System.Data;
using NetworkEquipmentStore.Models.DB;

namespace NetworkEquipmentStore.Models.DAO
{
    public class UserDAO
    {
        private readonly CartDAO CartDAO = new CartDAO();


        public User GetUserByID(int id)
        {
            string query = $"SELECT * FROM ShopUser WHERE id = {id};";
            DataTable table = Database.Request(query);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = table.Rows[0];

            return new User
            {
                ID = id,
                Name = row["name"].ToString(),
                Level = (PermissionsLevel)Enum.Parse(typeof(PermissionsLevel), row["level"].ToString()),
                Login = row["login"].ToString(),
                PasswordHash = row["password_hash"].ToString(),
                IsBanned = bool.Parse(row["is_banned"].ToString()),
                Cart = CartDAO.GetUserCart(id)
            };
        }

        public User GetUserByName(string name)
        {
            string query = $"SELECT * FROM ShopUser WHERE name = '{name}';";
            DataTable table = Database.Request(query);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = table.Rows[0];
            int id = int.Parse(row["id"].ToString());

            return new User
            {
                ID = id,
                Name = name,
                Level = (PermissionsLevel)Enum.Parse(typeof(PermissionsLevel), row["level"].ToString()),
                Login = row["login"].ToString(),
                PasswordHash = row["password_hash"].ToString(),
                IsBanned = bool.Parse(row["is_banned"].ToString()),
                Cart = CartDAO.GetUserCart(id)
            };
        }

        public User GetUserByLogin(string login)
        {
            string query = $"SELECT * FROM ShopUser WHERE login = '{login}';";
            DataTable table = Database.Request(query);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = table.Rows[0];
            int id = int.Parse(row["id"].ToString());

            return new User
            {
                ID = id,
                Name = row["name"].ToString(),
                Level = (PermissionsLevel)Enum.Parse(typeof(PermissionsLevel), row["level"].ToString()),
                Login = login,
                PasswordHash = row["password_hash"].ToString(),
                IsBanned = bool.Parse(row["is_banned"].ToString()),
                Cart = CartDAO.GetUserCart(id)
            };
        }

        public IEnumerable<User> GetAllUsers()
        {
            string query = $"SELECT * FROM ShopUser;";
            DataTable table = Database.Request(query);
            List<User> users = new List<User>();

            foreach (DataRow row in table.Rows)
            {
                int id = int.Parse(row["id"].ToString());
                User user = new User
                {
                    ID = id,
                    Name = row["name"].ToString(),
                    Level = (PermissionsLevel)Enum.Parse(typeof(PermissionsLevel), row["level"].ToString()),
                    Login = row["login"].ToString(),
                    PasswordHash = row["password_hash"].ToString(),
                    IsBanned = bool.Parse(row["is_banned"].ToString()),
                    Cart = CartDAO.GetUserCart(id)
                };

                users.Add(user);
            }

            return users;
        }

        public User InsertUser(User user)
        {
            string name = user.Name.Replace("'", "\\'");
            PermissionsLevel level = user.Level;
            string login = user.Login.Replace("'", "\\'");
            string passwordHash = user.PasswordHash.Replace("'", "\\'");
            bool isBanned = user.IsBanned;
            Cart cart = CartDAO.InsertCart(user.Cart);

            string query = $"INSERT INTO ShopUser(name, level, login, password_hash, is_banned, cart_id) VALUES ('{name}', '{level}', '{login}', '{passwordHash}', {isBanned}, {cart.ID}) RETURNING id;";
            int id = int.Parse(Database.Request(query).Rows[0]["id"].ToString());

            user.ID = id;
            user.Cart = cart;
            return user;
        }

        public User UpdateUser(User user)
        {
            int id = user.ID;
            string name = user.Name.Replace("'", "\\'");
            PermissionsLevel level = user.Level;
            string login = user.Login.Replace("'", "\\'");
            string passwordHash = user.PasswordHash.Replace("'", "\\'");
            bool isBanned = user.IsBanned;
            Cart cart = user.Cart;

            CartDAO.UpdateCart(cart);

            string query = $"UPDATE ShopUser SET name = '{name}', level = '{level}', login = '{login}', password_hash = '{passwordHash}', is_banned = {isBanned} WHERE id = {id};";
            Database.Execute(query);

            return user;
        }

        public void DeleteUser(User user)
        {
            int id = user.ID;

            OrderDAO orderDAO = new OrderDAO();
            orderDAO.DeleteAllOrders(user.ID);
            
            string query = $"DELETE FROM ShopUser WHERE id = {id};";
            Database.Execute(query);

            CartDAO.DeleteCart(user.Cart);
        }
    }
}
