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
        Task<Dictionary<string, Project>> LoadProduces(bool reload = false);
        Task Save(Dictionary<string, Project> projectSet);
    }
}
