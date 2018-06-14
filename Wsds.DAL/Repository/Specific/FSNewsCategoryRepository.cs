using System.Collections.Generic;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

namespace Wsds.DAL.Repository.Specific
{
    public class FSNewsCategoryRepository : INewsCategoryRepository
    {
        private ICacheService<NewsCategory_DTO> _csb;

        public FSNewsCategoryRepository(ICacheService<NewsCategory_DTO> csb)
        {
            _csb = csb;
        }

        public IEnumerable<NewsCategory_DTO> NewsCategory => _csb.Items.Values;
    }
}
