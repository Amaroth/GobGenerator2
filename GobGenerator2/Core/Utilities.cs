using System.Xml;

namespace GobGenerator2.Core
{
    class Utilities
    {
        public static void XmlAddElement(XmlDocument xml, XmlNode parent, string name, string innerText, string comment)
        {
            XmlNode newNode = xml.CreateElement(name);
            if (comment != "" && comment != null)
            {
                XmlComment newComment = xml.CreateComment(name + ": " + comment);
                parent.AppendChild(newComment);
            }
            newNode.InnerText = innerText;
            parent.AppendChild(newNode);
        }

        public static string GetModelName(string modelPath)
        {
            return modelPath;
        }
    }
}
