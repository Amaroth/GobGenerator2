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
        private SQLConnector sql;
        private DBCConnector dbc;
        private ListfileConnector lc = new ListfileConnector();

        public void TestConnection()
        {
            sql = new SQLConnector();
            sql.TestConnection(usi.host, usi.port, usi.database, usi.login, usi.password);
        }

        public void CheckForCollisions()
        {
            /*try
            {
                dbc = new DBCConnector();
                sql = new SQLConnector();
                HashSet<string> alreadyThere = new HashSet<string>();
                if (usi.avoidDuplicates)
                    alreadyThere = dbc.AlreadyThere(usi.dbcPath);
                int amountForImport = lc.ReadListfile(usi.listfilePath, usi.exportM2, usi.exportWMO, alreadyThere).Count;
                MessageBox.Show(string.Format("Filtered amount of files in input listfile: {0}.\n\nFound {1} collisions on displayIDs {2} - {3} in DBC.\nFound {4} collisions on entries {5} - {6} in database.",
                    amountForImport,
                    dbc.AmountInRange(usi.startDisplayID, amountForImport), usi.startDisplayID, usi.startDisplayID + amountForImport,
                    sql.AmountInRange(usi.baseEntry + usi.startDisplayID, usi.baseEntry + usi.startDisplayID + amountForImport), usi.baseEntry + usi.startDisplayID, usi.baseEntry + usi.startDisplayID + amountForImport));
            }
            catch (Exception e) { MessageBox.Show("Couldn't check for collisions, following error occured.:\n\n" + e.Message); }*/
        }

        public void Generate()
        {
            try
            {
                dbc = new DBCConnector();
                sql = new SQLConnector();
                HashSet<string> alreadyThere = new HashSet<string>();
                if (usi.avoidDuplicates)
                    alreadyThere = dbc.AlreadyThere(usi.dbcPath);
                dbc.CreateDisplayIDs(lc.ReadListfile(usi.listfilePath, usi.exportM2, usi.exportWMO, alreadyThere), usi.startDisplayID);
            }
            catch (Exception e) { MessageBox.Show("Generation process was not successful. Following error occured.:\n\n" + e.Message); }
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
