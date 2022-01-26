using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubstrateCore.Services
{
    public interface ISearchPathService
    {
        Task<List<string>> GetAll();
        Task Save(string path);
    }
}