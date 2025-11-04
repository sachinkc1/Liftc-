using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace CtrlElevator
{
    static class GlobalConnection
    {
        public static readonly MySqlConnection Con;

        static GlobalConnection()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultMySql"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("Connection string 'DefaultMySql' not found in App.config.");

            Con = new MySqlConnection(cs);
        }

        public static void Open()
        {
            if (Con.State != ConnectionState.Open)
                Con.Open();
        }

        public static void Close()
        {
            if (Con.State != ConnectionState.Closed)
                Con.Close();
        }
    }
}