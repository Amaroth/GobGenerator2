using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows;

namespace GobGenerator2.Core
{
    class CoreController
    {
        private static CoreController instance;

        private CoreController()
        {

        }

        public static CoreController Instance
        {
            get
            {
                if (instance == null)
                    instance = new CoreController();
                return instance;
            }
        }

        private UserSettings usi = UserSettings.Instance;
        MySqlConnection connection;

        public void TestConnection()
        {
            string conString = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}; SslMode=none;", usi.host, usi.port, usi.database, usi.login, usi.password);

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

        public void CheckForCollisions()
        {

        }

        public void Generate()
        {

        }

        public void OnlyDisplayToDB()
        {

        }

        public void SaveUserSettings()
        {
            usi.SaveUserSettings();
        }

        public void Help()
        {

        }
    }
}
