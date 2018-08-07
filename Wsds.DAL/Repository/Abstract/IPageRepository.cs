using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IPageRepository
    {
        Page_DTO GetPageById(long id);
        string GetPageOptions(long id);
    }
}
