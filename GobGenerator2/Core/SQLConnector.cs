using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
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
        private string table;
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

        public void SetConnectionInformation(string host, int port, string database, string table, string login, string password)
        {
            this.host = host;
            this.port = port;
            this.database = database;
            this.table = table;
            this.login = login;
            this.password = password;
            connectionString = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}; SslMode=none;", host, port, database, login, password);
        }

        /// <summary>
        /// Sets connection string and then tests connection.
        /// </summary>
        public void TestConnection(string host, int port, string database, string table, string login, string password)
        {
            SetConnectionInformation(host, port, database, table, login, password);
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
            
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                var query = new MySqlCommand(string.Format("SELECT COUNT({0}) FROM {1} WHERE {0} BETWEEN {2} AND {3};", m2Config.entryColName, table, start, end), connection);
                using (var r = query.ExecuteReader())
                {
                    if (r.Read())
                        return Convert.ToInt32(r[0]);
                }
                connection.Close();
            }
            catch (Exception e) { throw e; }
            return 0;
        }

        public void CreateGameobjects(List<Tuple<int, string>> m2DisplayIDs, List<Tuple<int, string>> wmoDisplayIDs, int baseEntry, bool useInsert)
        {
            if ((m2DisplayIDs.Count > 0 || wmoDisplayIDs.Count > 0) && baseEntry > 0)
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();



                string query = "START TRANSACTION;\n";

                if (useInsert)
                    query += "INSERT";
                else
                    query += "REPLACE";

                string m2Cols = string.Format("`{0}`, `{1}`, `{2}`", m2Config.entryColName, m2Config.displayIDColName, m2Config.nameColName);
                foreach (var col in m2Config.defaultValues)
                    m2Cols += ", `" + col.Key + "`";

                query += string.Format(" INTO `{0}` ({1}) VALUES\n", table, m2Cols);
                bool firstM = true;
                foreach (var displayID in m2DisplayIDs)
                {
                    if (!firstM)
                        query += ",\n";
                    query += string.Format("({0}, {1}, \"{2}\"", baseEntry + displayID.Item1, displayID.Item1, displayID.Item2);
                    foreach (var col in m2Config.defaultValues)
                        query += ", \"" + col.Value + "\"";
                    query += ")";
                    firstM = false;
                }
                query += ";\n";

                if (useInsert)
                    query += "INSERT";
                else
                    query += "REPLACE";

                string wmoCols = string.Format("`{0}`, `{1}`, `{2}`", wmoConfig.entryColName, wmoConfig.displayIDColName, wmoConfig.nameColName);
                foreach (var col in wmoConfig.defaultValues)
                    wmoCols += ", `" + col.Key + "`";

                query += string.Format(" INTO `{0}` ({1}) VALUES\n", table, wmoCols);
                bool firstW = true;
                foreach (var displayID in wmoDisplayIDs)
                {
                    if (!firstW)
                        query += ",\n";
                    query += string.Format("({0}, {1}, \"{2}\"", baseEntry + displayID.Item1, displayID.Item1, displayID.Item2);
                    foreach (var col in wmoConfig.defaultValues)
                        query += ", \"" + col.Value + "\"";
                    query += ")";
                    firstW = false;
                }
                query += ";\n";

                query += "COMMIT;";

                /*using (var sw = new StreamWriter("test.txt"))
                {
                    sw.WriteLine(query);
                }*/

                var command = new MySqlCommand(query, connection);
                command.ExecuteNonQuery();

                connection.Close();
            }
        }
    }
}
