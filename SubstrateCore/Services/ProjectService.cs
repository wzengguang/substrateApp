using SubstrateApp.Utils;
using SubstrateCore.Common;
using SubstrateCore.Models;
using SubstrateCore.Repositories;
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
        public ProjectRepository _sQLiteDataRepository { get; }

        public ProjectService(ProjectRepository sQLiteDataRepository)
        {
            _sQLiteDataRepository = sQLiteDataRepository;
        }

        private List<Project> _projects;

        public async Task<List<Project>> GetAll()
        {
            if (_projects == null)
            {
                _projects = await _sQLiteDataRepository.GetProjects();
            }
            return _projects;
        }

        public async Task InsertOrUpdateProject(Project projectInfo)
        {
            await _sQLiteDataRepository.AddProject(projectInfo);
        }

        public async Task<int> GetProjectCount()
        {
            return await _sQLiteDataRepository.CountProjectInfo();
        }

        public async Task<Dictionary<string, string>> GetProjectReferences(Project project)
        {
            var result = new Dictionary<string, string>();
            var xml = await XmlUtil.LoadDocAsync(project.RelativePath);

            foreach (var element in xml.Descendants())
            {
                string path = null;
                if (element.Name.LocalName == Tags.Reference)
                {
                    var hint = element.Descendants(Tags.HintPath).FirstOrDefault();
                    if (hint != null)
                    {
                        path = hint.Value;
                    }
                    else
                    {
                        path = element.Attribute(Tags.Include).Value;
                    }
                }
                else if (element.Name.LocalName == Tags.ProjectReference)
                {
                    path = element.Attribute(Tags.Include).Value;
                }

                if (path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {

                }
                else if (path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".vcsproj", StringComparison.OrdinalIgnoreCase))
                {

                }
            }
            return result;
        }


        public async Task<KeyValuePair<string, string>> FromInetrootToRelativePath(string path)
        {
            var relative = path.Replace(SubstrateConst.Inetroot, "", StringComparison.OrdinalIgnoreCase);
            var name = (await _sQLiteDataRepository.GetProjectByPath(relative))?.Name;

            if (name == null)
            {
                var xml = await XmlUtil.LoadDocAsync(relative);
                name = XmlUtil.TryGetAssemblyName(xml, relative);
            }
            if (name == null)
            {
                return null;
            }
            else
            {
                return new KeyValuePair<string, string>(name, relative);
            }
        }

        public KeyValuePair<string, string> FromTargetPathToRelativePath(string path)
        {

        }

    }
}
