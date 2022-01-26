using SubstrateCore.Common;
using SubstrateCore.Models;
using SubstrateCore.Repository;
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
        public IDataRepositoryFactory DataRepositoryFactory { get; }

        public ProjectService(IDataRepositoryFactory dataRepositoryFactory)
        {
            DataRepositoryFactory = dataRepositoryFactory;
        }


        public async Task<ProjectInfo> GetAll()
        {
            using (var dataService = DataRepositoryFactory.CreateDataRepo())
            {
                return await dataService.GetAllProjectInfo();
            }
        }


        public Dictionary<string, Project> ProjectSet { get; private set; } = new Dictionary<string, Project>(StringComparer.OrdinalIgnoreCase);

        public async Task<Dictionary<string, Project>> LoadProduces(bool reload = false)
        {
            //await Load(reload);
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

                var project = new ProjectInfo(assembliesName, PathUtil.TrimToRelativePath(p), ProjectTypeEnum.Substrate, framework);

                AddProject(project);
            };
            //await Save(ProjectSet);
            return ProjectSet;
        }

        private bool AddProject(ProjectInfo obj)
        {
            string key = obj.Name;
            if (!ProjectSet.ContainsKey(key))
            {
                ProjectSet.Add(key, new Project(key, obj.ProjectType));
            }

            if (obj.ProjectType == ProjectTypeEnum.Substrate)
            {
                switch (obj.Framework)
                {
                    case FrameworkConst.NetCore:
                        ProjectSet[key].NetCore = obj;
                        break;
                    case FrameworkConst.NetStd:
                        ProjectSet[key].NetStd = obj;
                        break;
                    default:
                        ProjectSet[key].NetFramework = obj;
                        break;
                }
            }

            return true;
        }

        public async Task<Dictionary<string, Project>> Load(bool force)
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
                        DataContractSerializer ser = new DataContractSerializer(typeof(Dictionary<string, Project>));
                        var readobject = (Dictionary<string, Project>)ser.ReadObject(reader, true);
                        ProjectSet = readobject ?? ProjectSet;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return ProjectSet;
        }

        public async Task Save(Dictionary<string, Project> projectSet)
        {
            StorageFile file = await GetSubstrateStorageFile();

            using (var fs = await file.OpenStreamForWriteAsync())
            {
                fs.SetLength(0);
                DataContractSerializer ser = new DataContractSerializer(typeof(Dictionary<string, Project>));
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
