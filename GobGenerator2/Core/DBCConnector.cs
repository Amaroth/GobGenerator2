using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDBXLib;
using WDBXLib.Reader;
using WDBXLib.Definitions.WotLK;
using System.Windows;
using System.IO;
using System.Xml;
using WDBXLib.Storage;

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
            catch (Exception e) { throw new Exception("Error occured while attempting to read and parse M2DBCConfig.xml.\n\n" + e.ToString()); }
            try
            {
                wmoConfig = new DBCDataConfig("WMODBCConfig.xml");
            }
            catch (Exception e) { throw new Exception("Error occured while attempting to read and parse WMODBCConfig.xml.\n\n" + e.ToString()); }
        }

        public void SetDBCFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                this.filePath = filePath;
                dbc = DBReader.Read<GameObjectDisplayInfo>(filePath);
            }
        }

        public HashSet<string> AlreadyThere()
        {
            HashSet<string> result = new HashSet<string>();
            try
            {
                foreach (var row in dbc.Rows)
                {
                        
                    if (row.ModelName.Length > 3 && row.ModelName.ToLower().EndsWith(".mdx"))
                        result.Add(row.ModelName.ToLower().Substring(0, row.ModelName.Length - 3) + "m2");
                    else
                        result.Add(row.ModelName.ToLower());
                }
            }
            catch (Exception e) { throw new Exception("Error while attempting to read DBC file.\n\n" + e.ToString()); }
            return result;
        }

        public void CreateDisplayIDs(HashSet<string> modelPaths, int displayID)
        {
            if (AmountInRange(displayID, displayID + modelPaths.Count - 1) == 0)
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
                    dbc.Rows.Add(newRecord);
                    displayID++;
                }
                DBReader.Write(dbc, filePath);
            }
            else
                throw new ArgumentException(string.Format("Couldn't insert displayIDs into range {0} - {1}; {2} displayIDs in that range are already taken!",
                    displayID, displayID + modelPaths.Count - 1, AmountInRange(displayID, displayID + modelPaths.Count - 1)));
        }

        public int SuggestStartDisplayID()
        {
            return dbc.Rows.NextKey;
        }

        public List<Tuple<int, string>> GetM2DisplayIDsFromRange(int start, int end, string namePrefix, string namePostfix)
        {
            List<Tuple<int, string>> result = new List<Tuple<int, string>>();
            foreach (var row in dbc.Rows)
                if (row.ID >= start && row.ID <= end && !row.ModelName.ToLower().EndsWith(".wmo"))
                    result.Add(new Tuple<int, string>(row.ID, namePrefix + Utilities.GetModelName(row.ModelName) + namePostfix));
            return result;
        }

        public List<Tuple<int, string>> GetWMODisplayIDsFromRange(int start, int end, string namePrefix, string namePostfix)
        {
            List<Tuple<int, string>> result = new List<Tuple<int, string>>();
            foreach (var row in dbc.Rows)
                if (row.ID >= start && row.ID <= end && row.ModelName.ToLower().EndsWith(".wmo"))
                    result.Add(new Tuple<int, string>(row.ID, namePrefix + Utilities.GetModelName(row.ModelName) + namePostfix));
            return result;
        }

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