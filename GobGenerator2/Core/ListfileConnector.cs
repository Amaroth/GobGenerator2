using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GobGenerator2.Core
{
    class ListfileConnector
    {
        private HashSet<string> filePaths;

        /// <summary>
        /// Goes through provided file and gets set of valid file paths in it.
        /// </summary>
        /// <param name="filePath">Path to input listfile.</param>
        /// <param name="exportM2">Include M2/MDX/MDL?</param>
        /// <param name="exportWMO">Include root WMO?</param>
        /// <param name="alreadyThere">For duplicity avoidance</param>
        /// <returns>Filtered valid file paths.</returns>
        public HashSet<string> ReadListfile(string filePath, bool exportM2, bool exportWMO, HashSet<string> alreadyThere)
        {
            filePaths = new HashSet<string>();
            try
            {
                using (var sr = new StreamReader(filePath))
                {
                    string line;
                    Regex reg = new Regex(@".*_[0-9]{3}\.wmo");

                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.ToLower();
                        if (!alreadyThere.Contains(line))
                        {
                            if (line.EndsWith(".wmo"))
                            {
                                /// Avoid inserting nonroot WMO files.
                                if (exportWMO && !reg.IsMatch(line))
                                    filePaths.Add(line);
                            }
                            else if (exportM2 && (line.EndsWith(".m2") || line.EndsWith(".mdl") || line.EndsWith(".mdx")))
                                filePaths.Add(line);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                filePaths = new HashSet<string>();
                throw new Exception("Error while attempting to read input listfile " + filePath + "\n\n" + e.ToString());
            }
            return filePaths;
        }
    }
}
