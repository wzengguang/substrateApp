using System.Threading.Tasks;

namespace SubstrateCore.Services
{
    public interface IScanService
    {
        Task ScanFileOfNonCoreXTProjectRestoreEntry();
    }
}