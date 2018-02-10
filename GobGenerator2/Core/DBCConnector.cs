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

namespace GobGenerator2.Core
{
    class DBCConnector
    {
        

        public DBCConnector()
        {

        }

        public List<string> AlreadyThere(string filePath)
        {
            List<string> result = new List<string>();
            if (File.Exists(filePath))
            {
                try
                {
                    var dbc = DBReader.Read<GameObjectDisplayInfo>(@"GameObjectDisplayInfo.dbc");
                    foreach (var row in dbc.Rows)
                    {
                        if (row.ModelName.EndsWith(".wmo"))
                            result.Add(row.ModelName.ToLower());
                        else if (row.ModelName.Length > 3)
                            result.Add(row.ModelName.ToLower().Substring(0, row.ModelName.Length - 3) + "m2");
                    }
                }
                catch (Exception e) { MessageBox.Show("Error while attempting to read DBC file.:\n\n" + e.ToString()); }
            }
            else
                MessageBox.Show("Provided DBC file not found.");
            return result;
        }

        public void ConnectDBC(string dbcPath)
        {
            var entry = DBReader.Read<GameObjectDisplayInfo>(dbcPath);
        }
    }
}
