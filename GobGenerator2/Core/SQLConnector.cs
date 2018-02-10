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


        public void TestConnection(string host, int port, string database, string login, string password)
        {
            string conString = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}; SslMode=none;", host, port, database, login, password);

            try
            {
                connection = new MySqlConnection(conString);
                connection.Open();
                MessageBox.Show("Connection was succesful.");
            }
            catch (Exception e)
            {
                MessageBox.Show("Connection wasn't succesful. Error:\n\n" + e.ToString());
                connection = null;
            }
        }
    }
}
