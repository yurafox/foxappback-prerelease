using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IActionRepository
    {
        IEnumerable<Action_DTO> GetActions();
        Action_DTO GetActionById(long id);
    }
}
