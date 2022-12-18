using System.Data;
using Npgsql;

namespace NetworkEquipmentStore.Models.DB
{
    internal enum Status
    {
        ConnectionNotInitialized,
        ConnectionOpen,
        ConnectionClosed,
        EmptyResult
    }

    public static class Database
    {
        const string HOST = "localhost";
        const int PORT = 5432;
        const string DATABASE = "networkshop";
        const string USERNAME = "admin_shop";
        const string PASSWORD = "ropkenro17";

        static private NpgsqlConnection Connection;
        static private Status ConnectionStatus { get; set; } = Status.ConnectionNotInitialized;


        private static void OpenConnection()
        {
            if ((ConnectionStatus == Status.ConnectionNotInitialized) || (ConnectionStatus == Status.ConnectionClosed))
            {
                string connstring = $"Host={HOST};Port={PORT};Database={DATABASE};Username={USERNAME};Password={PASSWORD};";

                Connection = new NpgsqlConnection(connstring);
                Connection.Open();
                ConnectionStatus = Status.ConnectionOpen;
            }
        }

        public static void CloseConnection()
        {
            if (ConnectionStatus == Status.ConnectionOpen)
            {
                Connection.Close();
                ConnectionStatus = Status.ConnectionClosed;
            }
        }

        public static DataTable Request(string sql)
        {
            if (ConnectionStatus != Status.ConnectionOpen)
            {
                OpenConnection();
            }

            NpgsqlCommand command = new NpgsqlCommand(sql, Connection);
            var adapter = new NpgsqlDataAdapter(command);
            DataTable dt = new DataTable();

            adapter.Fill(dt);
            return dt;
        }

        public static void Execute(string sql)
        {
            if (ConnectionStatus != Status.ConnectionOpen)
            {
                OpenConnection();
            }

            NpgsqlCommand command = new NpgsqlCommand(sql, Connection);
            command.ExecuteNonQuery();
        }
    }
}
