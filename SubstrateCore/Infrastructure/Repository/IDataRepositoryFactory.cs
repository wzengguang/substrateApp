using SubstrateCore.Repository;

namespace SubstrateCore.Repository
{
    public interface IDataRepositoryFactory
    {
        IDataRepository CreateDataRepo();
    }
}
