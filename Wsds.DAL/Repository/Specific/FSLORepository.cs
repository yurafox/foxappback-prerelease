using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.Communication;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;


namespace Wsds.DAL.Repository.Specific
{
    public class FSLORepository : ILORepository
    {
        private ICacheService<LoEntity_DTO> _csLoEnt;
        private ICacheService<LoSupplEntity_DTO> _csLoSupplEnt;
        private ICacheService<Quotation_Product_DTO> _csQProduct;
        private ICacheService<Quotation_DTO> _csQuot;
        private ICacheService<LoDeliveryType_DTO> _csDelType;
        private ICacheService<LoEntityOffice_DTO> _csEntOffice;
        private ICacheService<LoEntityDeliveryType_DTO> _csEntDelType;

        private IClientRepository _clRepo;
        private readonly IConfiguration _config;

        public FSLORepository(ICacheService<LoEntity_DTO> csLoEnt, 
                              ICacheService<LoSupplEntity_DTO> csLoSupplEnt,
                              ICacheService<Quotation_Product_DTO> csQProduct,
                              ICacheService<Quotation_DTO> csQuot,
                              ICacheService<LoDeliveryType_DTO> csDelType,
                              ICacheService<LoEntityOffice_DTO> csEntOffice,
                              ICacheService<LoEntityDeliveryType_DTO> csEntDelType,
                              IClientRepository clRepo,
                              IConfiguration config) {
            _csLoEnt = csLoEnt;
            _csLoSupplEnt = csLoSupplEnt;
            _csQProduct = csQProduct;
            _clRepo = clRepo;
            _csQuot = csQuot;
            _config = config;
            _csDelType = csDelType;
            _csEntOffice = csEntOffice;
            _csEntDelType = csEntDelType;
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

        public object GetDeliveryCostByShipment(Shipment_DTO shpmt, long loEntityId, long? loIdClientAddress, long delivTypeId)
        {
            var closCnfg = EntityConfigDictionary.GetConfig("client_order_product");
            var prov = new EntityProvider<ClientOrderProduct_DTO>(closCnfg);

            var sList = new List<DeliveryRequestT22_S_Cost>();

            foreach (var sh in shpmt.shipmentItems)
            {
                var s = new DeliveryRequestT22_S_Cost();

                var qp = prov.GetItem((long)sh.idOrderSpecProd).idQuotationProduct;
                s.g_id = _csQProduct.Item((long)qp).idProduct;
                s.price = (decimal)_csQProduct.Item((long)qp).price;
                s.qty = sh.qty;
                
                sList.Add(s);
            }

            var del22_Cost = new DeliveryRequestT22_Cost();
            del22_Cost.sht_id = loEntityId;
            del22_Cost.tcity_id = (long)_clRepo.ClientAddress((long)loIdClientAddress).idCity; //getCityIDFromClientAddress
            del22_Cost.seller_id = shpmt.idSupplier;  // getSellerIDFromQuotProduct

            del22_Cost.numfloor = 0;
            del22_Cost.type_deliv = delivTypeId;
            del22_Cost.cwh_id = shpmt.idLoEntityOffice;
            del22_Cost.spec = sList;

            var requestJson = JsonConvert.SerializeObject(del22_Cost);
            //Debug.WriteLine(requestJson);

            string res = "";
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("begin :result := foxstore.pkg_xml_json.get_deliverycost_js(:json); end;", con))
            {
                try
                {
                    cmd.Parameters.Add("result", OracleDbType.Varchar2, ParameterDirection.Output);
                    cmd.Parameters["result"].Size = 2000;
                    cmd.Parameters.Add(new OracleParameter("json", requestJson));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = cmd.Parameters["result"].Value.ToString();
                }
                finally
                {
                    con.Close();
                }
            };
            //Debug.WriteLine(res);
            var resp = JsonConvert.DeserializeObject<DeliveryResponseT22_Cost>(res);

            return new { assessedCost = resp.deliv + resp.deliv_floor };
        }
        public object GetDeliveryDateByShipment(Shipment_DTO shpmt, long loEntityId, long? loIdClientAddress, long delivTypeId) {

            var closCnfg = EntityConfigDictionary.GetConfig("client_order_product");
            var prov = new EntityProvider<ClientOrderProduct_DTO>(closCnfg);

            var sList = new List<DeliveryRequestT22_S_Date>();

            foreach (var sh in shpmt.shipmentItems) {
                var s = new DeliveryRequestT22_S_Date();

                var qp = prov.GetItem((long)sh.idOrderSpecProd).idQuotationProduct;
                s.g_id = _csQProduct.Item((long)qp).idProduct;
                s.iz_dozakupka = 0; //TODO??
                sList.Add(s);
            }

            var del22_Date = new DeliveryRequestT22_Date();

            del22_Date.sht_id = loEntityId;
            del22_Date.fcity_id = 38044; //TODO
            del22_Date.tcity_id = _clRepo.ClientAddress((long)loIdClientAddress).idCity;
            del22_Date.seller_id = shpmt.idSupplier;
            del22_Date.type_deliv = delivTypeId;
            del22_Date.cwh_id = shpmt.idLoEntityOffice;

            del22_Date.spec = sList;

            var requestJson = JsonConvert.SerializeObject(del22_Date);
            

            string res = "";
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("begin :result := foxstore.pkg_xml_json.get_DeliveryDate_JS(:json); end;", con))
            {
                try
                {
                    cmd.Parameters.Add("result", OracleDbType.Varchar2, ParameterDirection.Output);
                    cmd.Parameters["result"].Size = 2000;
                    cmd.Parameters.Add(new OracleParameter("json", requestJson));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = cmd.Parameters["result"].Value.ToString();
                }
                finally
                {
                    con.Close();
                }
            };

            var resp = JsonConvert.DeserializeObject<DeliveryResponseT22_Date>(res);


            return new { deliveryDate = DateTime.ParseExact(resp.deliv_date, "dd/MM/yyyy", CultureInfo.InvariantCulture) };
        }

        public LoDeliveryType_DTO LoDeliveryType(long id) => _csDelType.Item(id);

        public IEnumerable<LoDeliveryType_DTO> GetLoDeliveryTypesByLoEntity(long idLoEntity) {
            var res = new List<LoDeliveryType_DTO>(); 
            var lst = _csEntDelType.Items.Values.Where(x => x.idLoEntity == idLoEntity).ToList();
            foreach (var item in lst) {
                res.Add(_csDelType.Item(item.idLoDeliveryType));
            }
            return res;
        }

        public LoEntityOffice_DTO GetLoEntityOffice(long id) => _csEntOffice.Item(id);

        public IEnumerable<LoEntityOffice_DTO> GetLoEntityOfficesByLoEntityAndCity(long idLoEntity, long idCity)
        {
            return _csEntOffice.Items.Values.Where(x => x.idLoEntity == idLoEntity && x.idCity == idCity);
        }
    }
}
