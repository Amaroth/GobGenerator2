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
        private List<Tuple<bool, string>> filePaths;

        public void ReadListfile(string filePath, bool exportM2, bool exportWMO, bool avoidDuplicates, List<string> alreadyThere)
        {
            filePaths = new List<Tuple<bool, string>>();
            try
            {
                StreamReader sr = new StreamReader(filePath);
                string line;
                Regex reg = new Regex(@".*_{1}[0-9]{3}\.wmo");

                while ((line = sr.ReadLine()) != null)
                {
                    string lowLine = line.ToLower();
                    if (!alreadyThere.Contains(lowLine) || !avoidDuplicates)
                    {
                        if (exportM2 && lowLine.EndsWith(".m2"))
                            filePaths.Add(new Tuple<bool, string>(false, line));
                        else if (exportWMO && lowLine.EndsWith(".wmo") && !reg.IsMatch(lowLine))
                            filePaths.Add(new Tuple<bool, string>(true, line));
                    }
                }
            }
            catch (Exception e)
            {
                filePaths = new List<Tuple<bool, string>>();
                MessageBox.Show("Error while attempting to read " + filePath + "\n\n" + e.ToString());
            }
        }
    }
}
