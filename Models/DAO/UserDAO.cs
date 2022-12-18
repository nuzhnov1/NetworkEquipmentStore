using System;
using System.Collections.Generic;
using System.Data;
using NetworkEquipmentStore.Models.DB;

namespace NetworkEquipmentStore.Models.DAO
{
    public class UserDAO
    {
        internal User GetUserByID(int id)
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

        public User InsertUser(User user)
        {
            string name = user.Name.Replace("'", "\\'");
            PermissionsLevel level = user.Level;
            string login = user.Login.Replace("'", "\\'");
            string PasswordHash = user.PasswordHash.Replace("'", "\\'");

            string query = $"INSERT INTO ShopUser(name, level, login, password_hash) VALUES ('{name}', '{level}', '{login}', '{PasswordHash}') RETURNING id;";
            int id = int.Parse(Database.Request(query).Rows[0]["id"].ToString());

            user.ID = id;
            return user;
        }

        public void DeleteUser(User user)
        {
            int id = user.ID;

            string orderQuery = $"DELETE FROM ShopOrder WHERE user_id = {id};";
            Database.Execute(orderQuery);
            string query = $"DELETE FROM ShopUser WHERE id = {id};";
            Database.Execute(query);
        }
    }
}
