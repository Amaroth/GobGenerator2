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
            sql.TestConnection(usi.host, usi.port, usi.database, usi.table, usi.login, usi.password);
        }

        public void CheckForCollisions()
        {
            try
            {
                sql.SetConnectionInformation(usi.host, usi.port, usi.database, usi.table, usi.login, usi.password);
                dbc.SetDBCFile(usi.dbcPath);
                HashSet<string> alreadyThere = new HashSet<string>();
                if (usi.avoidDuplicates)
                    alreadyThere = dbc.AlreadyThere();

                int amountForImport = lc.ReadListfile(usi.listfilePath, usi.exportM2, usi.exportWMO, alreadyThere).Count;

                int firstDisplayID = usi.startDisplayID;
                int lastDisplayID = firstDisplayID + amountForImport - 1;

                int firstEntry = usi.baseEntry + firstDisplayID;
                int lastEntry = firstEntry + amountForImport - 1;

                MessageBox.Show(string.Format("Filtered amount of files in input listfile: {0}.\n\nFound {1} collisions on displayIDs {2} - {3} in DBC.\nFound {4} collisions on entries {5} - {6} in database.",
                    amountForImport,
                    dbc.AmountInRange(firstDisplayID, lastDisplayID), firstDisplayID, lastDisplayID,
                    sql.AmountInRange(firstEntry, lastEntry), firstEntry, lastEntry));
            }
            catch (Exception e) { MessageBox.Show("Couldn't check for collisions, following error occured.:\n\n" + e.Message); }
        }

        public void Generate()
        {
            try
            {
                sql.SetConnectionInformation(usi.host, usi.port, usi.database, usi.table, usi.login, usi.password);
                dbc.SetDBCFile(usi.dbcPath);
                HashSet<string> alreadyThere = new HashSet<string>();
                if (usi.avoidDuplicates)
                    alreadyThere = dbc.AlreadyThere();
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

        public int SuggestStartDisplayID()
        {
            if (File.Exists(usi.dbcPath))
            {
                dbc.SetDBCFile(usi.dbcPath);
                return dbc.SuggestStartDisplayID();
            }
            else
                return usi.startDisplayID;
        }

        public void Test()
        {

        }
    }
}
