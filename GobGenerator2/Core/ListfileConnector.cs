using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows;

namespace GobGenerator2.Core
{
    class ListfileConnector
    {
        private HashSet<string> filePaths;

        public HashSet<string> ReadListfile(string filePath, bool exportM2, bool exportWMO, HashSet<string> alreadyThere)
        {
            filePaths = new HashSet<string>();
            try
            {
                StreamReader sr = new StreamReader(filePath);
                string line;
                Regex reg = new Regex(@".*_{1}[0-9]{3}\.wmo");

                while ((line = sr.ReadLine()) != null)
                {
                    line = line.ToLower();
                    if (!alreadyThere.Contains(line))
                    {
                        if (line.EndsWith(".wmo"))
                        {
                            if (exportWMO && !reg.IsMatch(line))
                                filePaths.Add(line);
                        }
                        else if (exportM2)
                            filePaths.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                filePaths = new HashSet<string>();
                throw new Exception("Error while attempting to read " + filePath + "\n\n" + e.ToString());
            }
            return filePaths;
        }
    }
}
