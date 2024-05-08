using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace SQL_Publisher
{
    class DBUtils
    {
        public static MySqlConnection GetDBConnection()
        {
            string host = "localhost";
            int port = 3306;
            string database = "publisher";
            string username = "anastasia";
            string password = "123456";

            return DBMySQLUtils.GetDBConnection(host, port,
                database, username, password);
        }
    }
}
