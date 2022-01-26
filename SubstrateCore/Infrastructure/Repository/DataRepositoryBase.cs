using SubstrateCore.Datas;
using SubstrateCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateCore.Repository
{
    public class DataRepositoryBase : IDataRepository
    {
        private IDataSource _dataSource = null;

        public DataRepositoryBase(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public Task<ProjectInfo> GetAllProjectInfo()
        {
            throw new NotImplementedException();
        }


        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dataSource != null)
                {
                    _dataSource.Dispose();
                }
            }
        }
        #endregion
    }
}
