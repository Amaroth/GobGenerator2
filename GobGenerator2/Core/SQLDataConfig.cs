using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public SQLDataConfig(string xmlPath)
        {
            this.xmlPath = xmlPath;
            LoadSettings();
        }

        private void LoadSettings()
        {
            using (var sr = new StreamReader(xmlPath))
            {
                string xmlString = sr.ReadToEnd();
                sr.Close();
                xml.LoadXml(xmlString);
            }
            defaultValues = new Dictionary<string, string>();

            entryColName = xml.GetElementsByTagName("Entry")[0].InnerText;
            displayIDColName = xml.GetElementsByTagName("DisplayID")[0].InnerText;
            nameColName = xml.GetElementsByTagName("Name")[0].InnerText;

            foreach (XmlNode element in xml.GetElementsByTagName("DefaultValues")[0].ChildNodes)
                defaultValues.Add(element.Name, element.InnerText);
        }
    }
}
