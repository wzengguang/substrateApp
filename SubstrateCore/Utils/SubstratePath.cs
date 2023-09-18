using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateCore.Utils
{
    public class SubstratePath
    {
        public static readonly string ROOTPATH = "C:\\o365\\SB\\src";

        public static string Combine(string path)
        {
            return Path.Combine(ROOTPATH, path);
        }
    }
}
