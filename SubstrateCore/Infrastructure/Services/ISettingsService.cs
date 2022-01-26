using SubstrateCore.Models;
using System.Threading.Tasks;

namespace SubstrateCore.Services
{
    public enum DataProviderType
    {
        SQLite,
        SQLServer,
        WebAPI
    }

    public interface ISettingsService
    {
        string Version { get; }
        string DbVersion { get; }

        string UserName { get; set; }

        DataProviderType DataProvider { get; set; }
        string PatternConnectionString { get; }
        string SQLServerConnectionString { get; set; }
        bool IsRandomErrorsEnabled { get; set; }

        Task<Result> ResetLocalDataProviderAsync();

        //Task<Result> ValidateConnectionAsync(string connectionString);
        //Task<Result> CreateDabaseAsync(string connectionString);
    }
}
