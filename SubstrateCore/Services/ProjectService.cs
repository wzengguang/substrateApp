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


        public async Task<List<Project>> GetAll()
        {
            return await _sQLiteDataRepository.GetProjects();
        }

        public async Task InsertOrUpdateProject(Project projectInfo)
        {
            await _sQLiteDataRepository.AddProject(projectInfo);
        }

        public async Task<int> GetProjectCount()
        {
            return await _sQLiteDataRepository.CountProjectInfo();
        }

        public async Task<Project> GetProject(SearchInput search)
        {
            if (search.IsPath)
            {
                return await GetProjectByPath(search.Content);
            }
            else
            {
                return await GetProducedProjectByName(search.Content);
            }
        }

        public async Task<Project> GetProducedProjectByName(string name)
        {
            var ps = await _sQLiteDataRepository.GetProjectByName(name);
            if (ps.Count == 1)
            {
                return ps[0];
            }
            var project = ps.FirstOrDefault(a => a.Framework == FrameworkConst.NetCore) ?? ps.FirstOrDefault(a => a.Framework == FrameworkConst.NetStd);
            return project;
        }


        public async Task<Project> GetProjectByPath(string path)
        {
            return await _sQLiteDataRepository.GetProjectByPath(path);
        }

        public async Task<HashSet<string>> GetProjectReferences(Project project)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (project == null)
            {
                return result;
            }

            var xml = await ProjectUtil.LoadDocAsync(project.RelativePath);

            foreach (var element in xml.Descendants())
            {
                string path = null;
                if (element.Name.LocalName == SubstrateConst.Reference
                    || element.Name.LocalName == SubstrateConst.ProjectReference
                    || element.Name.LocalName == SubstrateConst.None)
                {
                    var hint = element.Descendants(SubstrateConst.HintPath).FirstOrDefault();
                    if (hint != null)
                    {
                        path = hint.Value;
                    }
                    else
                    {
                        path = element.Attribute(SubstrateConst.Include).Value;
                    }

                    if (path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(path.Split("\\").Last().Replace(".dll", "").Trim());
                    }
                    else if (path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".vcsproj", StringComparison.OrdinalIgnoreCase))
                    {
                        var r = path.ReplaceIgnoreCase(SubstrateConst.Inetroot, "");
                        var findPath = await _sQLiteDataRepository.GetProjectByPath(r);
                        if (findPath != null)
                        {
                            result.Add(findPath.Name);
                        }
                        else
                        {
                            result.Add(r);
                        }
                    }
                }
            }
            return result;
        }

        public async Task<KeyValuePair<string, string>?> FromInetrootToRelativePath(string path)
        {
            var relative = path.Replace(SubstrateConst.Inetroot, "", StringComparison.OrdinalIgnoreCase);
            var name = (await _sQLiteDataRepository.GetProjectByPath(relative))?.Name;

            if (name == null)
            {
                var xml = await ProjectUtil.LoadDocAsync(relative);
                name = ProjectUtil.TryGetAssemblyName(xml, relative);
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

        //public KeyValuePair<string, string> FromTargetPathToRelativePath(string path)
        //{

        //}

    }
}
