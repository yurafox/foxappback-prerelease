using Wsds.DAL.Entities.Communication;

namespace Wsds.DAL.Repository.Abstract
{
    public interface ISaleRmmRepository
    {
        void CreateSaleRmm(ClientOrderMQ order);
    }
}
