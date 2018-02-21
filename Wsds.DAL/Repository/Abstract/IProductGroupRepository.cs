using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IProductGroupRepository
    {
        IEnumerable<Product_Groups_DTO> ProductGroups { get; }
        Product_Groups_DTO GetProductGroupById(long id);
    }
}
