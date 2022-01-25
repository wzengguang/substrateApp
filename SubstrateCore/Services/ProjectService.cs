using SubstrateCore.Common;
using SubstrateCore.Models;
using SubstrateCore.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage;

namespace SubstrateCore.Services
{
    public class ProjectService : IProjectService
    {
        public ProjectSet ProjectSet { get; private set; } = new ProjectSet();

        public async Task<ProjectSet> LoadProduces(bool reload = false)
        {
            await Load(reload);
            if (!reload && ProjectSet.Count > 0)
            {
                return ProjectSet;
            }

            XElement projectRestoreEntryXml = await XmlUtil.LoadAsync("NonCoreXTProjectRestoreEntry\\dirs.proj");

            var producedPaths = projectRestoreEntryXml.Descendants(SubstrateConst.ProjectFile)
                .Select(x => x.Attribute(SubstrateConst.Include).Value.ReplaceIgnoreCase("$(InetRoot)\\", "").Trim()).ToList();

            foreach (var producedPath in producedPaths)
            {
                var p = PathUtil.GetPhysicalPath(producedPath);
                XElement xml = await XmlUtil.LoadAsync(producedPath);

                var assembliesName = ProjectUtil.InferAssemblyName(p, xml) ?? Path.GetFileNameWithoutExtension(p);

                string framework = xml.GetFirst(SubstrateConst.TargetFramework)?.Value
                  ?? MSBuildUtils.InferFrameworkByPath(p);

                var project = new Project(assembliesName, PathUtil.TrimToRelativePath(p), ProjectTypeEnum.Substrate, framework);

                ProjectSet.AddProject(project);
            };
            await Save(ProjectSet);
            return ProjectSet;
        }

        public async Task<ProjectSet> Load(bool force)
        {
            if (ProjectSet.Count > 0 && !force)
            {
                return ProjectSet;
            }

            StorageFile file = await GetSubstrateStorageFile();

            using (var fs = await file.OpenStreamForReadAsync())
            {
                using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
                {
                    try
                    {
                        DataContractSerializer ser = new DataContractSerializer(typeof(ProjectSet));
                        var readobject = (ProjectSet)ser.ReadObject(reader, true);
                        ProjectSet = readobject ?? ProjectSet;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return ProjectSet;
        }

        public async Task Save(ProjectSet projectSet)
        {
            StorageFile file = await GetSubstrateStorageFile();

            using (var fs = await file.OpenStreamForWriteAsync())
            {
                fs.SetLength(0);
                DataContractSerializer ser = new DataContractSerializer(typeof(ProjectSet));
                ser.WriteObject(fs, projectSet);
            }
        }

        private static async Task<StorageFile> GetSubstrateStorageFile()
        {
            var path = "\\local\\SubstrateData.xml";
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file;
            try
            {
                file = await storageFolder.GetFileAsync(path);
            }
            catch (Exception)
            {
                file = await storageFolder.CreateFileAsync(path, CreationCollisionOption.ReplaceExisting);
            }

            return file;
        }
    }
}
