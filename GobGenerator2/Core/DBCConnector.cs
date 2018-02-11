﻿using System;
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

namespace GobGenerator2.Core
{
    class DBCConnector
    {
        DBCDataConfig m2Config;
        DBCDataConfig wmoConfig;

        public DBCConnector()
        {
            try
            {
                m2Config = new DBCDataConfig("M2DBCConfig.xml");
            }
            catch { throw new Exception("Error occured while attempting to read and parse M2DBCConfig.xml."); }
            try
            {
                wmoConfig = new DBCDataConfig("WMODBCConfig.xml");
            }
            catch { throw new Exception("Error occured while attempting to read and parse WMODBCConfig.xml."); }
        }

        public HashSet<string> AlreadyThere(string filePath)
        {
            HashSet<string> result = new HashSet<string>();
            if (File.Exists(filePath))
            {
                try
                {
                    var dbc = DBReader.Read<GameObjectDisplayInfo>(@"GameObjectDisplayInfo.dbc");
                    foreach (var row in dbc.Rows)
                    {
                        if (row.ModelName.ToLower().EndsWith(".wmo"))
                            result.Add(row.ModelName.ToLower());
                        else if (row.ModelName.Length > 3 && row.ModelName.ToLower().EndsWith(".mdx"))
                            result.Add(row.ModelName.ToLower().Substring(0, row.ModelName.Length - 3) + "m2");
                    }
                }
                catch{ throw new Exception("Error while attempting to read DBC file."); }
            }
            else
                MessageBox.Show("Provided DBC file not found.");
            return result;
        }
    }
}