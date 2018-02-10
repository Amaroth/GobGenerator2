using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows;
using System.IO;

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
        private SQLConnector sql = new SQLConnector();
        private DBCConnector dbc = new DBCConnector();
        private ListfileConnector lc = new ListfileConnector();

        public void TestConnection()
        {
            sql.TestConnection(usi.host, usi.port, usi.database, usi.login, usi.password);
        }

        public void CheckForCollisions()
        {

        }

        public void Generate()
        {
            List<string> alreadyThere = new List<string>();
            if (usi.avoidDuplicates)
                alreadyThere = dbc.AlreadyThere(usi.dbcPath);
            lc.ReadListfile(usi.listfilePath, usi.exportM2, usi.exportWMO, usi.avoidDuplicates, alreadyThere);
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

        public void Test()
        {

        }
    }
}
