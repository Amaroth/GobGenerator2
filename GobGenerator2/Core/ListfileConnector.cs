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

        public void ReadListfile(string filePath, bool exportM2, bool exportWMO, bool avoidDuplicates, HashSet<string> alreadyThere)
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
                    if (!alreadyThere.Contains(line) || !avoidDuplicates)
                    {
                        if (exportM2 && line.EndsWith(".m2"))
                            filePaths.Add(line);
                        else if (exportWMO && line.EndsWith(".wmo") && !reg.IsMatch(line))
                            filePaths.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                filePaths = new HashSet<string>();
                MessageBox.Show("Error while attempting to read " + filePath + "\n\n" + e.ToString());
            }
            using (StreamWriter sw = new StreamWriter("filteredListfile.txt"))
            {
                foreach (string s in filePaths)
                    sw.WriteLine(s);
            }
            using (StreamWriter sw = new StreamWriter("alreadyThere.txt"))
            {
                foreach (string s in alreadyThere)
                    sw.WriteLine(s);
            }
        }
    }
}
