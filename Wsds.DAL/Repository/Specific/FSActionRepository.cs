using System.Collections.Generic;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSActionRepository:IActionRepository
    {
        private readonly ICacheService<Action_DTO> _csAction;

        public FSActionRepository(ICacheService<Action_DTO> csAction)
        {
            _csAction = csAction;
        }

        public IEnumerable<Action_DTO> GetActions()
        {
            return _csAction.Items.Values;
        }

        public Action_DTO GetActionById(long id)
        {
            return _csAction.Item(id);
        }
    }
}
