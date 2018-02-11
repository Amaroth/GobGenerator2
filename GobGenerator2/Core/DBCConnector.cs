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
            this.filePath = filePath;
            dbc = DBReader.Read<GameObjectDisplayInfo>(filePath);
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

        public void CreateDisplayIDs(HashSet<string> modelPaths, int start)
        {
            using (StreamWriter sw = new StreamWriter("test.txt"))
            {
                foreach (string s in modelPaths)
                {
                    if (s.EndsWith(".wmo"))
                        sw.WriteLine("{0}, {1}, {2}", start, s, wmoConfig.SoundStand);
                    else
                        sw.WriteLine("{0}, {1}, {2}", start, s, m2Config.SoundStand);
                    start++;
                }
            }
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