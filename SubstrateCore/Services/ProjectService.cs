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
        private List<string> SearchedProjectFile { get; set; }
        public ProjectSet ProjectSet { get; private set; } = new ProjectSet();

        private SubstrateData _substrateData = new SubstrateData();

        public void UpdateSearchedProjectFile(string add)
        {
            if (SearchedProjectFile == null)
            {
                SearchedProjectFile = new List<string>();
            }

            if (!SearchedProjectFile.Contains(add))
            {
                SearchedProjectFile.Insert(0, add);
            }

            if (SearchedProjectFile.Count > 10)
            {
                SearchedProjectFile.RemoveAt(SearchedProjectFile.Count - 1);
            }
        }

        public async Task<ProjectSet> LoadProduces(bool reload = false)
        {
            await Load();
            if (!reload && ProjectSet.Count > 0)
            {
                return ProjectSet;
            }

            XElement projectRestoreEntryXml = await XmlUtil.LoadAsync("NonCoreXTProjectRestoreEntry\\dirs.proj");

            var producedPaths = projectRestoreEntryXml.Descendants(ProjectConst.ProjectFile)
                .Select(x => x.Attribute(ProjectConst.Include).Value.ReplaceIgnoreCase("$(InetRoot)\\", "").Trim()).ToList();

            foreach (var producedPath in producedPaths)
            {
                var p = PathUtil.GetPhysicalPath(producedPath);
                XElement xml = await XmlUtil.LoadAsync(producedPath);

                var assembliesName = ProjectUtil.InferAssemblyName(p, xml) ?? Path.GetFileNameWithoutExtension(p);

                string framework = xml.GetFirst(ProjectConst.TargetFramework)?.Value
                  ?? MSBuildUtils.InferFrameworkByPath(p);

                var project = new Project(assembliesName, PathUtil.TrimToRelativePath(p), ProjectTypeEnum.Substrate, framework);

                ProjectSet.AddProject(project);
                _substrateData.AllProjects.Add(project);
            };
            await Save();
            return ProjectSet;
        }

        public async Task<SubstrateData> Load()
        {
            if (_substrateData != null)
            {
                return _substrateData;
            }

            StorageFile file = await GetSubstrateStorageFile();

            using (var fs = await file.OpenStreamForReadAsync())
            {
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                DataContractSerializer ser = new DataContractSerializer(typeof(SubstrateData));
                var readobject = (SubstrateData)ser.ReadObject(reader, true);
                _substrateData = readobject ?? new SubstrateData();
                reader.Close();
            }
            return _substrateData;
        }

        public async Task Save()
        {
            StorageFile file = await GetSubstrateStorageFile();

            using (var fs = await file.OpenStreamForWriteAsync())
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(SubstrateData));
                ser.WriteObject(fs, _substrateData);
            }
        }

        private static async Task<StorageFile> GetSubstrateStorageFile()
        {
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file;
            try
            {
                file = await storageFolder.GetFileAsync("\\local\\SubstrateData.xml");
            }
            catch (Exception e)
            {
                await storageFolder.CreateFileAsync("\\local\\SubstrateData.xml", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                file = await storageFolder.GetFileAsync("\\local\\SubstrateData.xml");
            }

            return file;
        }
    }
}
