using SubstrateCore.Common;
using SubstrateCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

namespace SubstrateCore.Utils
{
    public static class PathUtil
    {

        public static string SubstrateDir
        {
            get
            {
                return ApplicationData.Current.LocalSettings.Values["SubstrateDir"] as string;
            }
        }

        public static string TrimToRelativePath(string path)
        {
            path = path.Replace(SubstrateDir, "").Trim();
            while (path.StartsWith("\\") || path.StartsWith("/"))
            {
                path = path.Remove(0, 1);
            }
            path = path.Replace("\\\\", "\\");
            return path;
        }

        public static string GetPhysicalPath(string path)
        {
            path = path.Trim();
            while (path.StartsWith("\\") || path.StartsWith("/"))
            {
                path = path.Remove(0, 1);
            }
            path = path.Replace("/", "\\");

            path = path.Replace("\\\\", "\\");

            if (!path.Contains(":"))
            {
                path = Path.Combine(SubstrateDir, path);
            }

            return path;
        }

        public static string GetTargetPath(string assemblyName, string physicalPath)
        {
            // full path:
            // eg: D:\xxx\xxx\sources\dev\assistants\src\Assistants.NetCore\Microsoft.Exchange.Assistants.NetCore.csproj

            // dev\assistants\src\Assistants.NetCore\Microsoft.Exchange.Assistants.NetCore.csproj
            string devPath = physicalPath.Split(@"sources\").Last();

            // dev\assistants\
            string rootPath = Regex.Split(devPath, "src", RegexOptions.IgnoreCase).First();

            // Microsoft.Exchange.Assistants.NetCore
            string folderName = Path.GetFileNameWithoutExtension(physicalPath);

            // $(TargetPathDir)dev\assistants\Microsoft.Exchange.Assistants.NetCore\$(FlavorPlatformDir)\{assemblyName}.dll
            return $"$(TargetPathDir){rootPath}{folderName}\\$(FlavorPlatformDir)\\{assemblyName}.dll";
        }

        public static string GetTargetPathNew(string assemblyName, string physicalPath)
        {
            // full path:
            // eg: D:\xxx\xxx\sources\dev\assistants\src\Assistants.NetCore\Microsoft.Exchange.Assistants.NetCore.csproj
            // dev\assistants\src\Assistants.NetCore\Microsoft.Exchange.Assistants.NetCore.csproj
            string devPath = physicalPath.Split(@"sources\").Last();

            // dev\assistants\
            string rootPath = Regex.Split(devPath, "src", RegexOptions.IgnoreCase).First();

            // Microsoft.Exchange.Assistants.NetCore
            string folderName = Path.GetFileNameWithoutExtension(physicalPath);

            return $"$(TargetPathDir){rootPath}{folderName}\\bin\\$(Configuration)\\netcoreapp3.1\\{assemblyName}.dll";
        }

        public static string ReplacePathVirable(string path)
        {
            return path.Replace(SubstrateConst.FlavorPlatformDir, SubstrateConst.DebugAmd64)
            .Replace(SubstrateConst.Configuration, SubstrateConst.Debug)
            .Replace(SubstrateConst.BuildArchitecture, SubstrateConst.DebugAmd64)
            .Replace(SubstrateConst.TargetPathDir + "\\", "")
            .Replace(SubstrateConst.TargetPathDir, "");
        }
    }
}
