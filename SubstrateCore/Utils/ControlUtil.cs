using SubstrateCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateCore.Utils
{
    public class ControlUtil
    {
        /// <summary>
        /// no repeat,no empty, ignore case
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static HashSet<SearchInput> GetProjectNamesFromTextBox(string text)
        {
            HashSet<SearchInput> projectNames = new HashSet<SearchInput>();

            HashSet<string> result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string str in text.Split('\r'))
            {
                if (str.Contains("\\") || str.EndsWithIgnoreCase("proj"))
                {
                    projectNames.Add(new SearchInput(PathUtil.TrimToRelativePath(str), true));
                    continue;
                }

                var s = str.Replace(".dll", "").Trim();
                if (!string.IsNullOrEmpty(s))
                {
                    projectNames.Add(new SearchInput(s, false));
                }
            }
            return projectNames;
        }
    }
}
