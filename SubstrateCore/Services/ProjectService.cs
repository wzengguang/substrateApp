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


        public async Task<Project> GetProject(string name)
        {



        }

        public async Task<Project> GetProject(string path)
        {



        }
    }
}
