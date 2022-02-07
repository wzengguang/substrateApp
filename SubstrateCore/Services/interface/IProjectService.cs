using SubstrateCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateCore.Services
{
    public interface IProjectService
    {
        Task<List<Project>> GetAll();
        Task InsertOrUpdateProject(Project projectInfo);
        Task<int> GetProjectCount();
        Task<HashSet<string>> GetProjectReferences(Project project);
        Task<Project> GetProducedProjectByName(string name);
        Task<Project> GetProjectByPath(string path);
        Task<Project> GetProject(SearchInput search);
    }
}
