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
    }
}
