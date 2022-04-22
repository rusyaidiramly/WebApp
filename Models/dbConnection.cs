using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace WebApp.Models
{
    public static class dbConnection
    {
        

        //MySql
        private static string ConnectionString = "server=127.0.0.1;database=tsd;uid=root;pwd=root;";

        private static MySqlConnection conn = new MySqlConnection(ConnectionString);
        public static ConnectionState GetConnectionStatus { get { return conn.State; } }
        public static void OpenConnection() { conn.Open(); }
        public static void CloseConnection() { conn.Close(); }
        public static MySqlConnection GetConnection { get { return conn; } }
    }
}
