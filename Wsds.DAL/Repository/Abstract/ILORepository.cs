using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface ILORepository
    {
        IEnumerable<LoEntity_DTO> LoEntities { get; }
        LoEntity_DTO LoEntity(long id);
    }
}
