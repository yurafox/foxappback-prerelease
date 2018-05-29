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
    public class FSAppParamsRepository : IAppParamsRepository
    {

        private ICacheService<AppParam_DTO> _csAppParams;
        public FSAppParamsRepository(ICacheService<AppParam_DTO> csAppParams) {
            _csAppParams = csAppParams;
        }

        public IEnumerable<AppParam_DTO> GetAppParams()
        {
            return _csAppParams.Items.Values;
        }
    }
}
