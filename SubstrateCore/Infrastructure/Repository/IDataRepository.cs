using SubstrateCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateCore.Repository
{
    public interface IDataRepository : IDisposable
    {
        Task<ProjectInfo> GetAllProjectInfo();
    }
}
