using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Wsds.DAL.Services.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSNoveltyRepository:INoveltyRepository
    {
        private readonly IConfiguration _configuration;
        private string OraConnectionString { get; }

        public FSNoveltyRepository(ICacheService<NoveltyDetails_DTO> csNoveltyDetails, IConfiguration configuration)
        {
            _configuration = configuration;
            OraConnectionString = _configuration.GetConnectionString("MainDataConnection");
        }

        public IEnumerable<Novelty_DTO> GetNovelties()
        {
            IEnumerable<Novelty_DTO> novelties = new List<Novelty_DTO>();
            using (IDbConnection db = new OracleConnection(OraConnectionString))
            {
                novelties = db.Query<Novelty_DTO>("SELECT t.id, t.id_product as idProduct, t.name, t.img_url, t.priority, t.sketch_content, t.novelty_content FROM Novelties t WHERE t.is_active = 1").ToList();
            }
            return novelties;
        }

        public Novelty_DTO GetNoveltyById(long id)
        {
            Novelty_DTO novelty = null;
            using (IDbConnection db = new OracleConnection(OraConnectionString))
            {
                novelty = db.Query<Novelty_DTO>("SELECT t.id, t.id_product as idProduct, t.name, t.img_url, t.priority, t.sketch_content, t.novelty_content FROM Novelties t WHERE t.is_active = 1 and t.id = :id", new { id }).FirstOrDefault();
            }
            return novelty;
        }

        public IEnumerable<NoveltyDetails_DTO> GetNoveltyDetailsByNoveltyId(long id)
        {
            var cnfg = EntityConfigDictionary.GetConfig("novelty_details");
            var prov = new EntityProvider<NoveltyDetails_DTO>(cnfg);
            var noveltyDetails = prov.GetItems("id_novelty = :id", new OracleParameter("a", id));
            return noveltyDetails;
        }
    }
}
