using SubstrateCore.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SubstrateCore.Utils
{
    public static class ProjectUtil
    {


        public static string InferAssemblyName(string path, XElement xml)
        {
            xml = xml.GetFirst(SubstrateConst.PropertyGroup);

            string assemblyName = xml.GetFirst(SubstrateConst.AssemblyName)?.Value;

            if (string.IsNullOrEmpty(assemblyName))
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                assemblyName = fileName;
            }
            else if (assemblyName.EqualsIgnoreCase(@"$(RootNamespace)"))
            {
                assemblyName = xml.GetFirst(SubstrateConst.RootNamespace).Value;
            }

            return assemblyName;
        }
    }
}
