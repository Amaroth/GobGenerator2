using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GobGenerator2.Core
{
    class SQLConnector
    {
        MySqlConnection connection;
        SQLDataConfig m2Config;
        SQLDataConfig wmoConfig;

        private string host;
        private int port;
        private string database;
        private string login;
        private string password;

        private string connectionString;

        public SQLConnector()
        {
            try
            {
                m2Config = new SQLDataConfig("M2SQLConfig.xml");
            }
            catch (Exception e) { throw new Exception("Error occured while attempting to read and parse M2SQLConfig.xml.\n\n" + e.ToString()); }
            try
            {
                wmoConfig = new SQLDataConfig("WMOSQLConfig.xml");
            }
            catch (Exception e) { throw new Exception("Error occured while attempting to read and parse WMOSQLConfig.xml.\n\n" + e.ToString()); }
        }

        private void SetConnectionInformation(string host, int port, string database, string login, string password)
        {
            this.host = host;
            this.port = port;
            this.database = database;
            this.login = login;
            this.password = password;
            connectionString = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}; SslMode=none;", host, port, database, login, password);
        }

        /// <summary>
        /// Tests connection with current connection string available.
        /// </summary>
        public void TestConnection()
        {
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                MessageBox.Show("Connection was succesful.");
                connection.Close();
            }
            catch (Exception e) { throw new Exception("Error occured while establishing database connection.\n\n" + e.Message); }
        }

        /// <summary>
        /// Sets connection string and then tests connection.
        /// </summary>
        public void TestConnection(string host, int port, string database, string login, string password)
        {
            SetConnectionInformation(host, port, database, login, password);
            TestConnection();
        }

        public int AmountInRange(int start, int end)
        {
            return 0;
        }
    }
}
