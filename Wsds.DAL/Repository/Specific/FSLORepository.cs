using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSLORepository : ILORepository
    {
        private ICacheService<LoEntity_DTO> _csLoEnt;

        public FSLORepository(ICacheService<LoEntity_DTO> csLoEnt) {
            _csLoEnt = csLoEnt;
        }

        public IEnumerable<LoEntity_DTO> LoEntities => _csLoEnt.Items.Values;

        public LoEntity_DTO LoEntity(long id) => _csLoEnt.Item(id);
    }
}
