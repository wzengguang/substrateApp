using SubstrateCore.Configuration;
using SubstrateCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateCore.Repository
{
    public class DataRepositoryFactory : IDataRepositoryFactory
    {
        public IDataRepository CreateDataRepo()
        {
            switch (AppSettings.Current.DataProvider)
            {
                case DataProviderType.SQLite:
                    return new SQLiteDataRepository(AppSettings.Current.SQLiteConnectionString);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
