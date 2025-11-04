using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace CtrlElevator
{
    static class DbConnectionFactory
    {
        public static MySqlConnection CreateConnection()
        {
            var cs = ConfigurationManager.ConnectionStrings["DefaultMySql"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("Connection string 'DefaultMySql' not found in App.config.");

            return new MySqlConnection(cs);
        }
    }
}