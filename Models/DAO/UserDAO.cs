using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace NetworkEquipmentStore.Models.DAO
{
    using Database;


    public class UserDAO
    {
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
                PasswordHash = row["password_hash"].ToString()
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

            return new User
            {
                ID = int.Parse(row["id"].ToString()),
                Name = row["name"].ToString(),
                Level = (PermissionsLevel)Enum.Parse(typeof(PermissionsLevel), row["level"].ToString()),
                Login = login,
                PasswordHash = row["password_hash"].ToString()
            };
        }

        public IEnumerable<User> GetAllUsers()
        {
            string query = $"SELECT * FROM ShopUser;";
            DataTable table = Database.Request(query);
            List<User> users = new List<User>();

            foreach (DataRow row in table.Rows)
            {
                User user = new User
                {
                    ID = int.Parse(row["id"].ToString()),
                    Name = row["name"].ToString(),
                    Level = (PermissionsLevel)Enum.Parse(typeof(PermissionsLevel), row["level"].ToString()),
                    Login = row["login"].ToString(),
                    PasswordHash = row["password_hash"].ToString()
                };

                users.Add(user);
            }

            return users;
        }

        public void InsertUsers(params User[] users)
        {
            foreach (User user in users)
            {
                string name = user.Name;
                PermissionsLevel level = user.Level;
                string login = user.Login;
                string PasswordHash = user.PasswordHash;

                string query = $"INSERT INTO ShopUser(name, level, login, password_hash) VALUES ('{name}', '{level}', '{login}', '{PasswordHash}');";
                Database.Execute(query);
            }
        }

        public void UpdateUsers(params User[] users)
        {
            foreach (User user in users)
            {
                int id = user.ID;
                string name = user.Name;
                PermissionsLevel level = user.Level;
                string login = user.Login;
                string PasswordHash = user.PasswordHash;

                string query = $"UPDATE ShopUser SET name = '{name}', level = '{level}', login = '{login}', password_hash = '{PasswordHash}' WHERE id = {id};";
                Database.Execute(query);
            }
        }

        public void DeleteUsers(params User[] users)
        {
            foreach (User user in users)
            {
                int id = user.ID;

                string orderQuery = $"DELETE FROM ShopOrder WHERE user_id = {id};";
                Database.Execute(orderQuery);
                string query = $"DELETE FROM ShopUser WHERE id = {id};";
                Database.Execute(query);
            }
        }
    }
}
