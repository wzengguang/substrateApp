using SubstrateApp.DataModel;
using SubstrateCore.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage;
using Windows.UI.Popups;

namespace SubstrateApp.Utils
{
    public class SubstrateUtil
    {
        public static string SubstrateDir { get { return SubstrateData.Instance.RootDir; } }

        private static IgnoreCaseDictionary<Project> allProduced = null;

        public static async Task<IgnoreCaseDictionary<Project>> AllProduced()
        {
            if (allProduced == null)
            {
                allProduced = new IgnoreCaseDictionary<Project>();

                XElement projectRestoreEntryXml = await XmlUtil.LoadAsync("NonCoreXTProjectRestoreEntry\\dirs.proj");
                var producedPaths = projectRestoreEntryXml.Descendants(Tags.ProjectFile).Select(x => x.Attribute("Include").Value.ReplaceIgnoreCase("$(InetRoot)\\", "").Trim()).ToList();
                foreach (var producedPath in producedPaths)
                {
                    var p = Path.Combine(SubstrateDir, producedPath);
                    XElement xml = await XmlUtil.LoadAsync(producedPath);

                    var assembliesName = InferAssemblyName(p, xml) ?? Path.GetFileNameWithoutExtension(p);

                    string framework = xml.GetFirst(Tags.TargetFramework)?.Value
                      ?? MSBuildUtils.InferFrameworkByPath(p);

                    var project = new Project(InferAssemblyName(p, xml), ProjectType.Substrate);
                    if (!allProduced.ContainsKey(project.Name))
                    {
                        allProduced.Add(project.Name, project);
                        project.NetCore = new Project(InferAssemblyName(p, xml), framework, producedPath, ProjectType.Substrate);
                    }
                };
            }

            return allProduced;

        }

        public static string InferAssemblyName(string path, XElement xml)
        {
            xml = xml.GetFirst(Tags.PropertyGroup);

            string assemblyName = xml.GetFirst(Tags.AssemblyName)?.Value;

            if (string.IsNullOrEmpty(assemblyName))
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                assemblyName = fileName;
            }
            else if (assemblyName.EqualsIgnoreCase(@"$(RootNamespace)"))
            {
                assemblyName = xml.GetFirst(Tags.RootNamespace).Value;
            }

            return assemblyName;
        }

        public static bool targetPathFileExist(string assemblyName, string targetPath)
        {
            // $(TargetPathDir)dev\assistants\Microsoft.Exchange.Assistants.NetCore\$(FlavorPlatformDir)\{assemblyName}.dll

            targetPath = targetPath.Replace("$(FlavorPlatformDir)", @"debug\\amd64");
            targetPath = targetPath.Replace("$(TargetPathDir)", SubstrateDir + "\\target\\");
            return File.Exists(targetPath);
        }

        public static string ResolveTargetPath(string assemblyName, string filePath)
        {
            return MSBuildUtils.ResolveTargetPath(assemblyName, filePath);
        }

        public static string RectifyResolveTargetPath(string assemblyName, string filePath)
        {
            // full path:
            // eg: D:\xxx\xxx\sources\dev\assistants\src\Assistants.NetCore\Microsoft.Exchange.Assistants.NetCore.csproj
            // dev\assistants\src\Assistants.NetCore\Microsoft.Exchange.Assistants.NetCore.csproj
            string devPath = filePath.Split(@"sources\").Last();

            // dev\assistants\
            string rootPath = Regex.Split(devPath, "src", RegexOptions.IgnoreCase).First();

            // Microsoft.Exchange.Assistants.NetCore
            string folderName = Path.GetFileNameWithoutExtension(filePath);

            //var xmlElement = XElement.Load(filePath);
            //if (xmlElement.Descendants("").Count() != 0)
            //{
            //    // $(TargetPathDir)dev\assistants\Microsoft.Exchange.Assistants.NetCore\$(FlavorPlatformDir)\{assemblyName}.dll
            //    return $"$(TargetPathDir){rootPath}{folderName}\\$(FlavorPlatformDir)\\{assemblyName}.dll";
            //}

            return $"$(TargetPathDir){rootPath}{folderName}\\bin\\$(Configuration)\\netcoreapp3.1\\{assemblyName}.dll";
        }
    }

}
