using System;
using System.Collections.Generic;
using WDBXLib.Reader;
using WDBXLib.Definitions.WotLK;
using System.IO;
using WDBXLib.Storage;
using System.Windows;

namespace GobGenerator2.Core
{
    class DBCConnector
    {
        private DBCDataConfig m2Config;
        private DBCDataConfig wmoConfig;
        private DBEntry<GameObjectDisplayInfo> dbc;

        private string filePath;

        public DBCConnector()
        {
            try
            {
                m2Config = new DBCDataConfig("M2DBCConfig.xml");
            }
            catch (Exception e) { MessageBox.Show(e.ToString()); throw; }
            try
            {
                wmoConfig = new DBCDataConfig("WMODBCConfig.xml");
            }
            catch (Exception e) { MessageBox.Show(e.ToString()); throw; }
        }

        /// <summary>
        /// Opens given DBC file for reading.
        /// </summary>
        /// <param name="filePath"></param>
        public void SetDBCFile(string filePath)
        {
            try
            {
                this.filePath = filePath;
                dbc = DBReader.Read<GameObjectDisplayInfo>(filePath);
            }
            catch (Exception e) { throw new Exception("Error occured while attempting to read provided DBC.:\n\n" + e.Message); }
        }

        /// <summary>
        /// Gets set of lowercase file paths which are already in DBC - for duplication avoidance.
        /// </summary>
        /// <returns></returns>
        public HashSet<string> AlreadyThere()
        {
            HashSet<string> result = new HashSet<string>();
            foreach (var row in dbc.Rows)
            {
                string modelPath = row.ModelName.ToLower();
                if (!modelPath.EndsWith(".wmo"))
                    modelPath = Path.ChangeExtension(modelPath, ".m2");
                result.Add(modelPath);
            }
            return result;
        }

        /// <summary>
        /// generates records into DBC.
        /// </summary>
        /// <param name="modelPaths">Model paths to be added</param>
        /// <param name="displayID">DisplayID of the first model</param>
        /// <param name="useInsert">If false, replace method is used.</param>
        public void CreateDisplayIDs(HashSet<string> modelPaths, int displayID, bool useInsert)
        {
            int amountThere = AmountInRange(displayID, displayID + modelPaths.Count - 1);
            if (amountThere == 0 || !useInsert)
            {
                foreach (string s in modelPaths)
                {
                    var newRecord = new GameObjectDisplayInfo();
                    newRecord.ID = displayID;
                    newRecord.ModelName = s;
                    newRecord.Sound = new int[10];
                    if (s.EndsWith(".wmo"))
                    {
                        newRecord.Sound[0] = wmoConfig.SoundStand;
                        newRecord.Sound[1] = wmoConfig.SoundOpen;
                        newRecord.Sound[2] = wmoConfig.SoundLoop;
                        newRecord.Sound[3] = wmoConfig.SoundClose;
                        newRecord.Sound[4] = wmoConfig.SoundDestroy;
                        newRecord.Sound[5] = wmoConfig.SoundOpened;
                        newRecord.Sound[6] = wmoConfig.SoundCustom0;
                        newRecord.Sound[7] = wmoConfig.SoundCustom1;
                        newRecord.Sound[8] = wmoConfig.SoundCustom2;
                        newRecord.Sound[9] = wmoConfig.SoundCustom3;
                        newRecord.GeoBoxMinX = wmoConfig.GeoBoxMinX;
                        newRecord.GeoBoxMinY = wmoConfig.GeoBoxMinY;
                        newRecord.GeoBoxMinZ = wmoConfig.GeoBoxMinZ;
                        newRecord.GeoBoxMaxX = wmoConfig.GeoBoxMaxX;
                        newRecord.GeoBoxMaxY = wmoConfig.GeoBoxMaxY;
                        newRecord.GeoBoxMaxZ = wmoConfig.GeoBoxMaxZ;
                        newRecord.ObjectEffectPackageID = wmoConfig.ObjectEffectPackageID;
                    }
                    else
                    {
                        newRecord.Sound[0] = m2Config.SoundStand;
                        newRecord.Sound[1] = m2Config.SoundOpen;
                        newRecord.Sound[2] = m2Config.SoundLoop;
                        newRecord.Sound[3] = m2Config.SoundClose;
                        newRecord.Sound[4] = m2Config.SoundDestroy;
                        newRecord.Sound[5] = m2Config.SoundOpened;
                        newRecord.Sound[6] = m2Config.SoundCustom0;
                        newRecord.Sound[7] = m2Config.SoundCustom1;
                        newRecord.Sound[8] = m2Config.SoundCustom2;
                        newRecord.Sound[9] = m2Config.SoundCustom3;
                        newRecord.GeoBoxMinX = m2Config.GeoBoxMinX;
                        newRecord.GeoBoxMinY = m2Config.GeoBoxMinY;
                        newRecord.GeoBoxMinZ = m2Config.GeoBoxMinZ;
                        newRecord.GeoBoxMaxX = m2Config.GeoBoxMaxX;
                        newRecord.GeoBoxMaxY = m2Config.GeoBoxMaxY;
                        newRecord.GeoBoxMaxZ = m2Config.GeoBoxMaxZ;
                        newRecord.ObjectEffectPackageID = m2Config.ObjectEffectPackageID;
                    }
                    if (dbc.Rows[newRecord.ID] != null && !useInsert)
                        dbc.Rows[newRecord.ID] = newRecord;
                    else
                        dbc.Rows.Add(newRecord);
                    displayID++;
                }
                DBReader.Write(dbc, filePath);
            }
            else
                throw new Exception(string.Format("Couldn't insert displayIDs into range {0} - {1}; {2} displayIDs in that range are already taken!",
                    displayID, displayID + modelPaths.Count - 1, amountThere));
        }

        /// <summary>
        /// Gets the highest ID in DBC + 1
        /// </summary>
        /// <returns></returns>
        public int SuggestStartDisplayID()
        {
            return dbc.Rows.NextKey;
        }

        /// <summary>
        /// Selects nonWMO DisplayID, Name(not whole path) from range.
        /// </summary>
        /// <param name="start">Min displayID</param>
        /// <param name="end">Max displaID</param>
        /// <param name="namePrefix">Prefix to be put in front of name</param>
        /// <param name="namePostfix">Postfix to be put behind name</param>
        /// <returns></returns>
        public List<Tuple<int, string>> GetM2DisplayIDsFromRange(int start, int end, string namePrefix, string namePostfix)
        {
            List<Tuple<int, string>> result = new List<Tuple<int, string>>();
            foreach (var row in dbc.Rows)
                if (row.ID >= start && row.ID <= end && !row.ModelName.ToLower().EndsWith(".wmo"))
                    result.Add(new Tuple<int, string>(row.ID, namePrefix + Utilities.GetModelName(row.ModelName) + namePostfix));
            return result;
        }

        /// <summary>
        /// Selects WMO DisplayID, Name(not whole path) from range.
        /// </summary>
        /// <param name="start">Min displayID</param>
        /// <param name="end">Max displaID</param>
        /// <param name="namePrefix">Prefix to be put in front of name</param>
        /// <param name="namePostfix">Postfix to be put behind name</param>
        /// <returns></returns>
        public List<Tuple<int, string>> GetWMODisplayIDsFromRange(int start, int end, string namePrefix, string namePostfix)
        {
            List<Tuple<int, string>> result = new List<Tuple<int, string>>();
            foreach (var row in dbc.Rows)
                if (row.ID >= start && row.ID <= end && row.ModelName.ToLower().EndsWith(".wmo"))
                    result.Add(new Tuple<int, string>(row.ID, namePrefix + Utilities.GetModelName(row.ModelName) + namePostfix));
            return result;
        }

        /// <summary>
        /// Counts amount of displayIDs already in existence within provided range.
        /// </summary>
        /// <param name="start">Min displayID</param>
        /// <param name="end">Max displayID</param>
        /// <returns></returns>
        public int AmountInRange(int start, int end)
        {
            if (start > end)
                throw new ArgumentException(string.Format("Start displayID value ({0}) has to be lower or equal to end value ({1}).", start, end));
            int c = 0;
            foreach (var row in dbc.Rows)
                if (row.ID >= start && row.ID <= end)
                    c++;
            return c;
        }
    }
}