using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace WebApp.Services
{
    public static class dbConnection
    {
        //Azure
        //private static string ConnectionString = "Server=tcp:iforest9dbserver.database.windows.net,1433;Initial Catalog=iForest9;Persist Security Info=False;User ID=iForest9DBadmin;Password=iForest9@2021;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        
        //MS SQL
        //private static string ConnectionString = "Data Source=FYAIS-MASTER;Initial Catalog=iForest9;User ID=iForest9DBadmin;Password=password@123;";

        //MySql
        private static string ConnectionString = "server=127.0.0.1;database=tsd;uid=root;pwd=root;";

        private static MySqlConnection conn = new MySqlConnection(ConnectionString);
        public static ConnectionState GetConnectionStatus { get { return conn.State; } }
        public static void OpenConnection() { conn.Open(); }
        public static void CloseConnection() { conn.Close(); }
        public static MySqlConnection GetConnection { get { return conn; } }
    }
}
