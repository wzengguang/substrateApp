using SubstrateApp.DataModel;
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

    public static class StorageFileUtil
    {
        private static string AmendPath(string path)
        {
            while (path.StartsWith("\\") || path.StartsWith("/"))
            {
                path = path.Remove(0, 1);
            }

            path = path.Replace("\\\\", "\\");

            if (!path.Contains(":"))
            {
                path = Path.Combine(SubstrateUtil.SubstrateDir, path);
            }

            return path;
        }

        public async static Task<XElement> LoadXmlSF(string path)
        {
            path = AmendPath(path);

            XElement xml = null;
            try
            {
                StorageFile f = await StorageFile.GetFileFromPathAsync(path);
                using (var stream = await f.OpenStreamForReadAsync())
                {
                    xml = XElement.Load(stream);
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return xml;
        }

        public static async Task SaveSF(this XElement xElement, string path)
        {
            path = AmendPath(path);

            StorageFile file = await StorageFile.GetFileFromPathAsync(path);

            using (Stream stream = await file.OpenStreamForWriteAsync())
            {
                xElement.Save(stream);
            }
        }
    }

    public class SubstrateUtil
    {
        public static string SubstrateDir { get { return SubstrateData.Instance.RootDir; } }

        private static IgnoreCaseDictionary<Project> allProduced = null;

        public static async Task<IgnoreCaseDictionary<Project>> AllProduced()
        {
            if (allProduced == null)
            {
                allProduced = new IgnoreCaseDictionary<Project>();

                XElement projectRestoreEntryXml = await StorageFileUtil.LoadXmlSF("NonCoreXTProjectRestoreEntry\\dirs.proj");
                var producedPaths = projectRestoreEntryXml.Descendants(Tags.ProjectFile).Select(x => x.Attribute("Include").Value.ReplaceIgnoreCase("$(InetRoot)\\", "").Trim()).ToList();
                foreach (var producedPath in producedPaths)
                {
                    var p = Path.Combine(SubstrateDir, producedPath);
                    XElement xml = await StorageFileUtil.LoadXmlSF(producedPath);

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

        public static string VerifyFilePath(string filePath)
        {
            string truepath;
            if (!File.Exists(filePath))
            {
                truepath = Path.Combine(SubstrateUtil.SubstrateDir, filePath);
                return truepath;
                //if (!File.Exists(truepath))
                //{
                //    new MessageDialog("文件路径: " + filePath + " 不存在");
                //    return null;
                //}
                //else
                //{
                //    return truepath;
                //}
            }

            return filePath;
        }
    }

}
