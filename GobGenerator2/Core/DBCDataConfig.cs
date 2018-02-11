﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace GobGenerator2.Core
{
    class DBCDataConfig
    {
        private XmlDocument xml = new XmlDocument();
        string xmlPath;

        public int SoundStand = 0;
        public int SoundOpen = 0;
        public int SoundLoop = 0;
        public int SoundClose = 0;
        public int SoundDestroy = 0;
        public int SoundOpened = 0;
        public int SoundCustom0 = 0;
        public int SoundCustom1 = 0;
        public int SoundCustom2 = 0;
        public int SoundCustom3 = 0;
        public float GeoBoxMinX = 0;
        public float GeoBoxMinY = 0;
        public float GeoBoxMinZ = 0;
        public float GeoBoxMaxX = 0;
        public float GeoBoxMaxY = 0;
        public float GeoBoxMaxZ = 0;
        public int ObjectEffectPackageID = 0;

        public DBCDataConfig(string xmlPath)
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
            SoundStand = int.Parse(xml.GetElementsByTagName("SoundStand")[0].InnerText);
            SoundOpen = int.Parse(xml.GetElementsByTagName("SoundOpen")[0].InnerText);
            SoundLoop = int.Parse(xml.GetElementsByTagName("SoundLoop")[0].InnerText);
            SoundClose = int.Parse(xml.GetElementsByTagName("SoundClose")[0].InnerText);
            SoundDestroy = int.Parse(xml.GetElementsByTagName("SoundDestroy")[0].InnerText);
            SoundOpened = int.Parse(xml.GetElementsByTagName("SoundOpened")[0].InnerText);
            SoundCustom0 = int.Parse(xml.GetElementsByTagName("SoundCustom0")[0].InnerText);
            SoundCustom1 = int.Parse(xml.GetElementsByTagName("SoundCustom1")[0].InnerText);
            SoundCustom2 = int.Parse(xml.GetElementsByTagName("SoundCustom2")[0].InnerText);
            SoundCustom3 = int.Parse(xml.GetElementsByTagName("SoundCustom3")[0].InnerText);
            GeoBoxMinX = float.Parse(xml.GetElementsByTagName("GeoBoxMinX")[0].InnerText);
            GeoBoxMinY = float.Parse(xml.GetElementsByTagName("GeoBoxMinY")[0].InnerText);
            GeoBoxMinZ = float.Parse(xml.GetElementsByTagName("GeoBoxMinZ")[0].InnerText);
            GeoBoxMaxX = float.Parse(xml.GetElementsByTagName("GeoBoxMaxX")[0].InnerText);
            GeoBoxMaxY = float.Parse(xml.GetElementsByTagName("GeoBoxMaxY")[0].InnerText);
            GeoBoxMaxZ = float.Parse(xml.GetElementsByTagName("GeoBoxMaxZ")[0].InnerText);
            ObjectEffectPackageID = int.Parse(xml.GetElementsByTagName("ObjectEffectPackageID")[0].InnerText);
        }
    }
}