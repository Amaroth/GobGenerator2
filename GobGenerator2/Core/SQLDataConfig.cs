using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GobGenerator2.Core
{
    class SQLDataConfig
    {
        private XmlDocument xml = new XmlDocument();
        string xmlPath;

        public string entryColName = "entry";
        public string displayIDColName = "displayId";
        public string nameColName = "name";
        public Dictionary<string, string> defaultValues;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlPath">Path to *SQLConfig.xml file.</param>
        public SQLDataConfig(string xmlPath)
        {
            this.xmlPath = xmlPath;
            LoadSettings();
        }

        /// <summary>
        /// Loads settings from XML specified in xmlPath.
        /// </summary>
        private void LoadSettings()
        {
            if (!File.Exists(xmlPath))
                throw new FileNotFoundException(string.Format("File {0} could not be found. Make sure its included in app's /Configs/ folder, GobDisplayGenerator can't run without it.", xmlPath));

            using (var sr = new StreamReader(xmlPath))
            {
                string xmlString = sr.ReadToEnd();
                sr.Close();
                xml.LoadXml(xmlString);
            }

            try
            {
                entryColName = xml.GetElementsByTagName("Entry")[0].InnerText;
                displayIDColName = xml.GetElementsByTagName("DisplayID")[0].InnerText;
                nameColName = xml.GetElementsByTagName("Name")[0].InnerText;
                defaultValues = new Dictionary<string, string>();

                foreach (XmlNode element in xml.GetElementsByTagName("DefaultValues")[0].ChildNodes)
                    defaultValues.Add(element.Name, element.InnerText);
            }
            catch (XmlException e) { throw new Exception(string.Format("Error occured while loading data from {0}.\n\n", xmlPath) + e.Message); }
        }   
    }
}
