using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSActionRepository:IActionRepository
    {
        private readonly ICacheService<Action_DTO> _csAction;
        private readonly IConfiguration _config;

        public FSActionRepository(ICacheService<Action_DTO> csAction, IConfiguration config)
        {
            _csAction = csAction;
            _config = config;
        }

        public IEnumerable<Action_DTO> GetActions()
        {
            return _csAction.Items.Values;
        }

        public Action_DTO GetActionById(long id)
        {
            return _csAction.Item(id);
        }

        public IEnumerable<ActionsByProduct_DTO> GetProductActions(long id)
        {
            var stmt = "select ad.id_action, a.id_type, ad.id_quotation_product, qp.id_product, ad.id_cur, ad.action_price, qp.price, ad.bonus_qty, " +
                        "       ad.complect, ad.is_main, pig.idGroup, p.name, APP_CORE.Get_Image_Root_Url||p.image_url as image_url, a.title " +
                        "  from actions a, action_offers ad, quotations_products qp, products p, " +
                        "  (select max(z.id_group) as idGroup, z.id_product from PRODUCTS_IN_GROUPS z group by id_product) pig " +
                        "where ad.id_quotation_product = qp.id " +
                        "   and p.id = qp.id_product and a.id_type is not null " +
                        "   and a.id = ad.id_action " +
                        "   and pig.id_product = p.id " +
                        "   and ad.complect in (select ao.complect from actions a, action_offers ao, quotations_products qp " +
                        "                       where ao.id_quotation_product = qp.id " +
                        "                             and a.id = ao.id_action " +
                        "                             and qp.id_product = :id " +
                        "                             and ao.is_main = 1 " +
                        "                             and a.is_active = 1) " +
                        "union all " +
                        "select ad.id_action, a.id_type, ad.id_quotation_product, qp.id_product, ad.id_cur, ad.action_price, qp.price, ad.bonus_qty, " +
                        "       ad.complect, ad.is_main, null as idGroup, p.name, APP_CORE.Get_Image_Root_Url||p.image_url as image_url, a.title " +
                        "  from actions a, action_offers ad, quotations_products qp, products p " +
                        "where ad.id_quotation_product = qp.id " +
                        "   and p.id = qp.id_product and a.id_type is not null " +
                        "   and a.id = ad.id_action " +
                        "   and ad.complect is null " +
                        "   and p.id = :id ";

            var ConnString = _config.GetConnectionString("MainDataConnection");

            List<ActionsByProduct_DTO> res = new List<ActionsByProduct_DTO>();

            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand(stmt, con))
            {
                try
                {
                    cmd.Parameters.Add(new OracleParameter("id", id));
                    con.Open();
                    OracleDataReader dr = cmd.ExecuteReader();
                    dr.FetchSize = cmd.RowSize * 30;
                    var j = 0;
                    while (dr.Read())
                    {
                        var item = new ActionsByProduct_DTO()
                        {
                            actionId = (long?)dr["id_action"],
                            actionType = (long?)dr["id_type"],
                            idQuotationProduct = (long?)dr["id_quotation_product"],
                            idProduct = (long?)dr["id_product"],
                            idCur = (long?)dr["id_cur"],
                            actionPrice = Convert.IsDBNull(dr["action_price"]) ? (decimal?)null : (decimal?)dr["action_price"],
                            regularPrice = Convert.IsDBNull(dr["price"]) ? (decimal?)null : (decimal?)dr["price"],
                            bonusQty = Convert.IsDBNull(dr["bonus_qty"]) ? (decimal?)null : (decimal?)dr["bonus_qty"],
                            productName = Convert.IsDBNull(dr["name"]) ? null : dr["name"].ToString(),
                            complect = Convert.IsDBNull(dr["complect"]) ? null : dr["complect"].ToString(),
                            isMain = Convert.IsDBNull(dr["is_main"]) ? (int?)null : Convert.ToInt32(dr["is_main"].ToString()),
                            idGroup = Convert.IsDBNull(dr["idGroup"]) ? (long?)null : Convert.ToInt64(dr["idGroup"].ToString()),
                            imgUrl = dr["image_url"].ToString(),
                            title = Convert.IsDBNull(dr["title"]) ? null : dr["title"].ToString()
                        };
                        res.Add(item);
                    };
                }
                finally
                {
                    con.Close();
                }
            };
            return res;
        }

        public IEnumerable<long> GetProductsOfDay()
        {
            var stmt = "select qp.id_product from ACTION_OFFERS t, actions a, quotations_products qp " +
                       "where t.id_action = a.id and a.id_type = 6 and t.id_quotation_product = qp.id";
            var ConnString = _config.GetConnectionString("MainDataConnection");

            List<long> res = new List<long>();

            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand(stmt, con))
            {
                try
                {
                    con.Open();
                    OracleDataReader dr = cmd.ExecuteReader();
                    dr.FetchSize = cmd.RowSize * 10;
                    while (dr.Read())
                    {
                        res.Add((long)dr["id_product"]);
                    };
                }
                finally
                {
                    con.Close();
                }
            };

            return res;
        }

        public IEnumerable<long> GetProductsSalesHits()
        {
            var stmt = "select qp.id_product from ACTION_OFFERS t, actions a, quotations_products qp " +
                       "where t.id_action = a.id and a.id_type = 7 and t.id_quotation_product = qp.id";
            var ConnString = _config.GetConnectionString("MainDataConnection");

            List<long> res = new List<long>();

            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand(stmt, con))
            {
                try
                {
                    con.Open();
                    OracleDataReader dr = cmd.ExecuteReader();
                    dr.FetchSize = cmd.RowSize * 10;
                    while (dr.Read())
                    {
                        res.Add((long)dr["id_product"]);
                    };
                }
                finally
                {
                    con.Close();
                }
            };

            return res;
        }
    }
}
