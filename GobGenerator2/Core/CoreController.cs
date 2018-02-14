using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;

namespace GobGenerator2.Core
{
    class CoreController
    {
        private static CoreController instance;

        private CoreController() { }

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

        /// <summary>
        /// Attempts to establish MySQL connection with currently stored input from user.
        /// </summary>
        public void TestConnection()
        {
            try
            {
                sql.TestConnection(usi.host, usi.port, usi.database, usi.table, usi.login, usi.password);
            }
            catch (Exception e) { MessageBox.Show("Connection to MySQL server was not successful.\n\n" + e.Message); }
        }

        /// <summary>
        /// Checks how many records would be created with currently stored user input, and determines how many collisions would occur in DBC and how many in MySQL.
        /// </summary>
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
                if (amountForImport > 0)
                {
                    int firstDisplayID = usi.startDisplayID;
                    int lastDisplayID = firstDisplayID + amountForImport - 1;

                    int firstEntry = usi.baseEntry + firstDisplayID;
                    int lastEntry = firstEntry + amountForImport - 1;

                    MessageBox.Show(string.Format("Filtered amount of files in input listfile: {0}.\n\nFound {1} collisions on displayIDs {2} - {3} in DBC.\nFound {4} collisions on entries {5} - {6} in database.",
                        amountForImport,
                        dbc.AmountInRange(firstDisplayID, lastDisplayID), firstDisplayID, lastDisplayID,
                        sql.AmountInRange(firstEntry, lastEntry), firstEntry, lastEntry));
                }
                else
                    MessageBox.Show("Filtered amount of files in input listfile is 0. There is nothing to generate, and thus no collisions.");
            }
            catch (Exception e) { MessageBox.Show("Couldn't check for collisions, following error occured.:\n\n" + e.Message); }
        }

        /// <summary>
        /// Executes displayID and then gameobject generation with currently stored user input.
        /// </summary>
        public void Generate()
        {
            //try
            {
                sql.SetConnectionInformation(usi.host, usi.port, usi.database, usi.table, usi.login, usi.password);
                dbc.SetDBCFile(usi.dbcPath);
                HashSet<string> alreadyThere = new HashSet<string>();
                if (usi.avoidDuplicates)
                    alreadyThere = dbc.AlreadyThere();
                var listfile = lc.ReadListfile(usi.listfilePath, usi.exportM2, usi.exportWMO, alreadyThere);
                dbc.CreateDisplayIDs(listfile, usi.startDisplayID, usi.useInsert);
                sql.CreateGameobjects(dbc.GetM2DisplayIDsFromRange(usi.startDisplayID, usi.startDisplayID + listfile.Count - 1, usi.prefix, usi.postfix),
                    dbc.GetWMODisplayIDsFromRange(usi.startDisplayID, usi.startDisplayID + listfile.Count - 1, usi.prefix, usi.postfix), usi.baseEntry, usi.useInsert);
            }
            //catch (Exception e) { MessageBox.Show("Generation process was not successful. Following error occured.:\n\n" + e.Message); }
        }

        /// <summary>
        /// Generates gameobjects in MySQL out of displayIDs found in DBC within currently specified range - for workflow not including creation of new displayIDs.
        /// </summary>
        public void DisplayIDToDB()
        {
            try
            {
                sql.SetConnectionInformation(usi.host, usi.port, usi.database, usi.table, usi.login, usi.password);

                dbc.SetDBCFile(usi.dbcPath);
                List<Tuple<int, string>> m2s = new List<Tuple<int, string>>();
                if (usi.exportM2)
                    dbc.GetM2DisplayIDsFromRange(usi.minDisplayID, usi.maxDisplayID, usi.prefix, usi.postfix);
                List<Tuple<int, string>> wmos = new List<Tuple<int, string>>();
                if (usi.exportWMO)
                    dbc.GetWMODisplayIDsFromRange(usi.minDisplayID, usi.maxDisplayID, usi.prefix, usi.postfix);
                if (m2s.Count + wmos.Count > 0)
                    sql.CreateGameobjects(m2s, wmos, usi.baseEntry, usi.useInsert);
                else
                    MessageBox.Show("There was nothing found within specified range to import into database.");
            }
            catch (Exception e) { MessageBox.Show("Generation process was not successful. Following error occured.:\n\n" + e.Message); }
        }

        /// <summary>
        /// Saves current user input data into XML for loading on next app's startup.
        /// </summary>
        public void SaveUserSettings()
        {
            usi.SaveUserSettings();
        }

        /// <summary>
        /// Opens issue tracker in default web browser.
        /// </summary>
        public void Help()
        {
            System.Diagnostics.Process.Start("https://github.com/Amaroth/GobGenerator2/issues");
        }

        /// <summary>
        /// Gets next free displayID (highest ID + 1) from DBC.
        /// </summary>
        /// <returns></returns>
        public int SuggestStartDisplayID()
        {
            int result = usi.startDisplayID;
            if (File.Exists(usi.dbcPath))
            {
                try
                {
                    dbc.SetDBCFile(usi.dbcPath);
                    result = dbc.SuggestStartDisplayID();
                }
                catch (Exception e) { MessageBox.Show("WARNING: Could not determine next free displayID in DBC. \n\n" + e.Message); }
            }
            return result;
        }
    }
}
