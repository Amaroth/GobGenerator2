using System;
using System.Text;
using System.IO;
using System.Windows;
using System.Xml;
using System.Security;

namespace GobGenerator2.Core
{
    class UserSettings
    {
        private static UserSettings instance;

        private UserSettings()
        {
            DefaultSettings();
            LoadSettings();
        }

        public static UserSettings Instance
        {
            get
            {
                if (instance == null)
                    instance = new UserSettings();
                return instance;
            }
        }

        private XmlDocument xml = new XmlDocument();

        public string listfilePath = "(listfile)";
        public string dbcPath = "GameObjectDisplayInfo.dbc";
        public bool exportM2 = true;
        public bool exportWMO = true;

        public string host = "127.0.0.1";
        public SecureString login = Utilities.ToSecureString("root");
        public SecureString password = Utilities.ToSecureString("");
        public bool savePassword = false;
        public string database = "world";
        public string table = "gameobject_template";
        public int port = 3306;

        public int startDisplayID = 1;
        public int baseEntry = 400000;
        public string prefix = "[Gen]";
        public string postfix = " [Do not edit]";
        public bool useInsert = true;
        public bool avoidDuplicates = false;

        public int minDisplayID = 0;
        public int maxDisplayID = 16777215;

        /// <summary>
        /// Generates default user settings XML.
        /// </summary>
        private void DefaultSettings()
        {
            xml = new XmlDocument();
            
            XmlDeclaration declaration = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = xml.DocumentElement;
            xml.InsertBefore(declaration, root);

            XmlElement configs = xml.CreateElement("GobGenerator2Config");
            XmlComment comment = xml.CreateComment("Please, do not edit this XML directly unless you really have to. Otherwise use Save settings button.");
            xml.AppendChild(comment);
            xml.AppendChild(configs);

            Utilities.XmlAddElement(xml, xml.DocumentElement, "listfilePath", listfilePath, null);
            Utilities.XmlAddElement(xml, xml.DocumentElement, "dbcPath", dbcPath, null);
            Utilities.XmlAddElement(xml, xml.DocumentElement, "exportM2", exportM2.ToString(), null);
            Utilities.XmlAddElement(xml, xml.DocumentElement, "exportWMO", exportWMO.ToString(), null);

            Utilities.XmlAddElement(xml, xml.DocumentElement, "host", host, null);
            Utilities.XmlAddElement(xml, xml.DocumentElement, "login", Utilities.EncryptString(login), null);
            Utilities.XmlAddElement(xml, xml.DocumentElement, "password", Utilities.EncryptString(password), null);
            Utilities.XmlAddElement(xml, xml.DocumentElement, "savePassword", savePassword.ToString(), null);
            Utilities.XmlAddElement(xml, xml.DocumentElement, "database", database, null);
            Utilities.XmlAddElement(xml, xml.DocumentElement, "table", table, null);
            Utilities.XmlAddElement(xml, xml.DocumentElement, "port", port.ToString(), null);

            Utilities.XmlAddElement(xml, xml.DocumentElement, "baseEntry", baseEntry.ToString(), null);
            Utilities.XmlAddElement(xml, xml.DocumentElement, "prefix", prefix, null);
            Utilities.XmlAddElement(xml, xml.DocumentElement, "postfix", postfix, null);
            Utilities.XmlAddElement(xml, xml.DocumentElement, "useInsert", useInsert.ToString(), null);
            Utilities.XmlAddElement(xml, xml.DocumentElement, "avoidDuplicates", avoidDuplicates.ToString(), null);
        }

        /// <summary>
        /// Loads saved user settings from UserSettings.xml. If file is not found default values are used.
        /// </summary>
        private void LoadSettings()
        {
            if (File.Exists("Configs/UserSettings.xml"))
            {
                try
                {
                    using (var sr = new StreamReader("Configs/UserSettings.xml"))
                    {
                        string xmlString = sr.ReadToEnd();
                        sr.Close();
                        xml.LoadXml(xmlString);
                    }

                    listfilePath = xml.GetElementsByTagName("listfilePath")[0].InnerText;
                    dbcPath = xml.GetElementsByTagName("dbcPath")[0].InnerText;
                    exportM2 = xml.GetElementsByTagName("exportM2")[0].InnerText.ToLower() == "true";
                    exportWMO = xml.GetElementsByTagName("exportWMO")[0].InnerText.ToLower() == "true";

                    host = xml.GetElementsByTagName("host")[0].InnerText;
                    LoadCredentials();
                    savePassword = xml.GetElementsByTagName("savePassword")[0].InnerText.ToLower() == "true";
                    database = xml.GetElementsByTagName("database")[0].InnerText;
                    table = xml.GetElementsByTagName("table")[0].InnerText;
                    port = int.Parse(xml.GetElementsByTagName("port")[0].InnerText);

                    baseEntry = int.Parse(xml.GetElementsByTagName("baseEntry")[0].InnerText);
                    prefix = xml.GetElementsByTagName("prefix")[0].InnerText;
                    postfix = xml.GetElementsByTagName("postfix")[0].InnerText;
                    useInsert = xml.GetElementsByTagName("useInsert")[0].InnerText.ToLower() == "true";
                    avoidDuplicates = xml.GetElementsByTagName("avoidDuplicates")[0].InnerText.ToLower() == "true";
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error while attempting to read saved user settings. Using default ones.\n\n" + e.ToString());
                    DefaultSettings();
                }
            }
            else
                DefaultSettings();
        }

        /// <summary>
        /// Saves nearly everything user has entered into form as default configuration for next startup of application.
        /// </summary>
        public void SaveUserSettings()
        {
            if (!Directory.Exists("Configs"))
                Directory.CreateDirectory("Configs");
            using (TextWriter tw = new StreamWriter("Configs/UserSettings.xml", false, Encoding.UTF8))
            {
                xml.GetElementsByTagName("listfilePath")[0].InnerText = listfilePath;
                xml.GetElementsByTagName("dbcPath")[0].InnerText = dbcPath;
                xml.GetElementsByTagName("exportM2")[0].InnerText = exportM2.ToString();
                xml.GetElementsByTagName("exportWMO")[0].InnerText = exportWMO.ToString();

                xml.GetElementsByTagName("host")[0].InnerText = host;
                SaveCredentials();
                xml.GetElementsByTagName("savePassword")[0].InnerText = savePassword.ToString();
                xml.GetElementsByTagName("database")[0].InnerText = database;
                xml.GetElementsByTagName("table")[0].InnerText = table;
                xml.GetElementsByTagName("port")[0].InnerText = port.ToString();

                xml.GetElementsByTagName("baseEntry")[0].InnerText = baseEntry.ToString();
                xml.GetElementsByTagName("prefix")[0].InnerText = prefix;
                xml.GetElementsByTagName("postfix")[0].InnerText = postfix;
                xml.GetElementsByTagName("useInsert")[0].InnerText = useInsert.ToString();
                xml.GetElementsByTagName("avoidDuplicates")[0].InnerText = avoidDuplicates.ToString();

                xml.Save(tw);
                tw.Close();
            }
        }

        /// <summary>
        /// Gets encrypted user credentials from XML.
        /// </summary>
        private void LoadCredentials()
        {
            login = Utilities.DecryptString(xml.GetElementsByTagName("login")[0].InnerText);
            password = Utilities.DecryptString(xml.GetElementsByTagName("password")[0].InnerText);
        }

        /// <summary>
        /// Puts encrypted user credentials into XML.
        /// </summary>
        private void SaveCredentials()
        {
            if (savePassword)
            {
                xml.GetElementsByTagName("password")[0].InnerText = Utilities.EncryptString(password);
            }
            else
                xml.GetElementsByTagName("password")[0].InnerText = "";
            xml.GetElementsByTagName("login")[0].InnerText = Utilities.EncryptString(login);
        }
    }
}
