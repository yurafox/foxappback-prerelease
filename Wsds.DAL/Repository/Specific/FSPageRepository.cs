using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSPageRepository:IPageRepository
    {
        private readonly ICacheService<Page_DTO> _csPage;

        public FSPageRepository(ICacheService<Page_DTO> csPage)
        {
            _csPage = csPage;
        }

        public Page_DTO GetPageById(long id)
        {
            return _csPage.Item(id);
        }
    }
}
