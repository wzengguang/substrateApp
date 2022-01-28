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
        Task<List<ProjectInfo>> GetAll();
        Task<Dictionary<string, Project>> LoadProduces();
        Task Save(Dictionary<string, Project> projectSet);
        Task InsertOrUpdateProjectInfo(ProjectInfo projectInfo);
        Task<int> CountProjectInfo();
    }
}
