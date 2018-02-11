using System.Xml;
using System.Text.RegularExpressions;

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
            string fileName;
            if (modelPath.LastIndexOf('\\') < 0)
                fileName = Regex.Escape(modelPath.ToLower());
            else
                fileName = Regex.Escape(modelPath.Substring(modelPath.LastIndexOf('\\') + 1).ToLower());
            fileName = Regex.Replace(fileName, ".mdx", ".m2");
            return Regex.Replace(fileName, ".mdl", ".m2");
        }
    }
}
