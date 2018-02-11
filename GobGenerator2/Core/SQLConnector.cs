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
        private SQLDataConfig m2Config;
        private SQLDataConfig wmoConfig;
        private MySqlConnection connection;

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

        public void SetConnectionInformation(string host, int port, string database, string login, string password)
        {
            this.host = host;
            this.port = port;
            this.database = database;
            this.login = login;
            this.password = password;
            connectionString = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}; SslMode=none;", host, port, database, login, password);
        }

        /// <summary>
        /// Sets connection string and then tests connection.
        /// </summary>
        public void TestConnection(string host, int port, string database, string login, string password)
        {
            SetConnectionInformation(host, port, database, login, password);
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                MessageBox.Show("Connection was succesful.");
                connection.Close();
            }
            catch (Exception e) { throw new Exception("Error occured while establishing database connection.\n\n" + e.Message); }
        }

        public int AmountInRange(int start, int end)
        {
            if (start > end)
                throw new ArgumentException(string.Format("Start entry value ({0}) has to be lower or equal to end value ({1}).", start, end));
            return 0;
        }
    }
}
