using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
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
        private SecureString login;
        private SecureString password;

        private SecureString connectionString;

        public SQLConnector()
        {
            try
            {
                m2Config = new SQLDataConfig("Configs/M2SQLConfig.xml");
            }
            catch (Exception e) { MessageBox.Show(e.ToString()); throw; }
            try
            {
                wmoConfig = new SQLDataConfig("Configs/WMOSQLConfig.xml");
            }
            catch (Exception e) { MessageBox.Show(e.ToString()); throw; }
        }

        /// <summary>
        /// Sets connection string based on provided input.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="database"></param>
        /// <param name="table"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public void SetConnectionInformation(string host, int port, string database, string table, SecureString login, SecureString password)
        {
            this.host = host;
            this.port = port;
            this.database = database;
            this.table = table;
            this.login = login;
            this.password = password;
            connectionString = Utilities.ToSecureString(string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}; SslMode=none;",
                host, port, database, Utilities.ToInsecureString(login), Utilities.ToInsecureString(password)));
        }

        /// <summary>
        /// Tests wheter database connection with provided data can be successfuly established.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="database"></param>
        /// <param name="table"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public void TestConnection(string host, int port, string database, string table, SecureString login, SecureString password)
        {
            SetConnectionInformation(host, port, database, table, login, password);
            try
            {
                connection = new MySqlConnection(Utilities.ToInsecureString(connectionString));
                connection.Open();
                MessageBox.Show("Connection was succesful.");
                connection.Close();
            }
            catch (Exception e) { throw new Exception("Error occured while establishing database connection.\n\n" + e.Message); }
        }

        /// <summary>
        /// Gets amount of gameobjects in range defined by given start and end entries from database.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public int AmountInRange(int start, int end)
        {
            if (start > end)
                throw new ArgumentException(string.Format("Start entry value ({0}) has to be lower or equal to end value ({1}).", start, end));
            
            connection = new MySqlConnection(Utilities.ToInsecureString(connectionString));
            connection.Open();
            var query = new MySqlCommand(string.Format("SELECT COUNT({0}) FROM {1} WHERE {0} BETWEEN {2} AND {3};", m2Config.entryColName, table, start, end), connection);
            using (var r = query.ExecuteReader())
            {
                if (r.Read())
                    return Convert.ToInt32(r[0]);
            }
            connection.Close();
            return 0;
        }

        /// <summary>
        /// Imports generated gameobjects into database. Runs as one transaction.
        /// </summary>
        /// <param name="m2DisplayIDs">List of (displayID, gob name) for M2s</param>
        /// <param name="wmoDisplayIDs">List of (displayID, gob name) for WMOs</param>
        /// <param name="baseEntry">gameobject_template.entry = baseEntry+gameobject_template.displayId</param>
        /// <param name="useInsert">If false, use replace method instead</param>
        public void CreateGameobjects(List<Tuple<int, string>> m2DisplayIDs, List<Tuple<int, string>> wmoDisplayIDs, int baseEntry, bool useInsert)
        {
            connection = new MySqlConnection(Utilities.ToInsecureString(connectionString));
            connection.Open();


            var query = new StringBuilder("START TRANSACTION;\r\n");
            if (m2DisplayIDs.Count > 0)
            {
                if (useInsert)
                    query.Append("INSERT");
                else
                    query.Append("REPLACE");

                var m2Cols = new StringBuilder();
                m2Cols.AppendFormat("`{0}`, `{1}`, `{2}`", m2Config.entryColName, m2Config.displayIDColName, m2Config.nameColName);
                foreach (var col in m2Config.defaultValues)
                    m2Cols.AppendFormat(", `{0}`", col.Key);

                query.AppendFormat(" INTO `{0}` ({1}) VALUES\r\n", table, m2Cols);
                bool firstM = true;
                foreach (var displayID in m2DisplayIDs)
                {
                    if (!firstM)
                        query.Append(",\r\n");
                    firstM = false;
                    query.AppendFormat("({0}, {1}, \"{2}\"", baseEntry + displayID.Item1, displayID.Item1, displayID.Item2);
                    foreach (var col in m2Config.defaultValues)
                        query.AppendFormat(", \"{0}\"", col.Value);
                    query.Append(")");
                }
                query.Append(";\n");
            }
            if (wmoDisplayIDs.Count > 0)
            {
                if (useInsert)
                    query.Append("INSERT");
                else
                    query.Append("REPLACE");

                var wmoCols = new StringBuilder();
                wmoCols.AppendFormat("`{0}`, `{1}`, `{2}`", wmoConfig.entryColName, wmoConfig.displayIDColName, wmoConfig.nameColName);
                foreach (var col in wmoConfig.defaultValues)
                    wmoCols.AppendFormat(", `{0}`", col.Key);

                query.AppendFormat(" INTO `{0}` ({1}) VALUES\r\n", table, wmoCols);
                bool firstW = true;
                foreach (var displayID in wmoDisplayIDs)
                {
                    if (!firstW)
                        query.Append(",\r\n");
                    firstW = false;
                    query.AppendFormat("({0}, {1}, \"{2}\"", baseEntry + displayID.Item1, displayID.Item1, displayID.Item2);
                    foreach (var col in wmoConfig.defaultValues)
                        query.AppendFormat(", \"{0}\"", col.Value);
                    query.Append(")");
                }
                query.Append(";\n");
            }
            query.Append("COMMIT;");

            using (var sw = new StreamWriter("SQLQueryBackup.sql"))
            {
                sw.Write(query);
            }


            var command = new MySqlCommand(query.ToString(), connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
