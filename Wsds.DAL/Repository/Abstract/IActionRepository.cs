using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IActionRepository
    {
        IEnumerable<Action_DTO> GetActions();
        Action_DTO GetActionById(long id);
        IEnumerable<ActionsByProduct_DTO> GetProductActions(long id);
        IEnumerable<Product_DTO> GetProductsOfDay();
        IEnumerable<Product_DTO> GetProductsSalesHits();
    }
}
