using SubstrateCore.Datas;
using SubstrateCore.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateCore.Repository
{
    public class SQLiteDataRepository : DataRepositoryBase
    {
        public SQLiteDataRepository(string connectionString)
            : base(new SQLiteDb(connectionString))
        {

        }
    }
}
