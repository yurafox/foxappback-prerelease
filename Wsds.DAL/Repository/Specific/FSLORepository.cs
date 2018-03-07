using Oracle.ManagedDataAccess.Client;
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
        private ICacheService<LoSupplEntity_DTO> _csLoSupplEnt;

        public FSLORepository(ICacheService<LoEntity_DTO> csLoEnt, 
                              ICacheService<LoSupplEntity_DTO> csLoSupplEnt) {
            _csLoEnt = csLoEnt;
            _csLoSupplEnt = csLoSupplEnt;
        }

        public IEnumerable<LoEntity_DTO> LoEntities => _csLoEnt.Items.Values;

        public IEnumerable<LoSupplEntity_DTO> LoSupplEntities => _csLoSupplEnt.Items.Values;

        public IEnumerable<LoSupplEntity_DTO> GetLoEntitiesForSuppl(long supplId) =>
                    _csLoSupplEnt.Items.Values.Where(x => x.idSupplier == supplId);

        public IEnumerable<LoTrackLog> GetTrackLogForOrderSpecProd(long orderSpecProdId)
        {
            var tlCnfg = EntityConfigDictionary.GetConfig("lo_track_log");
            var prov = new EntityProvider<LoTrackLog>(tlCnfg);

            return prov.GetItems("t.ID_ORDER_SPEC_PROD = :orderSpecProdId", 
                new OracleParameter("orderSpecProdId", orderSpecProdId))
                .OrderBy(x => x.trackDate);
        }

        public LoEntity_DTO LoEntity(long id) => _csLoEnt.Item(id);

        public object GetDeliveryCost(ClientOrderProduct_DTO orderProduct, long loEntityId, long loIdClientAddress)
        {

            return new { assessedCost = 15.34 };
        }

        public object GetDeliveryDate(ClientOrderProduct_DTO orderProduct, long loEntityId, long loIdClientAddress)
        {
            return new { deliveryDate = new DateTime() };
        }
    }
}
