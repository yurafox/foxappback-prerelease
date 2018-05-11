using System;
using System.Collections.Generic;
using System.Data;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;

namespace Wsds.DAL.Repository.Specific
{
    public class FSReviewRepository : IReviewRepository
    {
        private readonly IConfiguration _config;
        public FSReviewRepository(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<StoreReview_DTO> GetStoreReviewsByStoreId(long idStore, long idClient)
        {
            var cnfg = EntityConfigDictionary.GetConfig("store_reviews_add_vote");
            var prov = new EntityProvider<StoreReview_DTO>(cnfg);
            object clientId = (idClient == 0) ? (object)DBNull.Value : idClient;

            var reviews = prov.GetItems("id_store = :id", new OracleParameter("idClient", clientId),
                                                            new OracleParameter("id", idStore)
                                       );
            return reviews;
        }

        public IEnumerable<StoreReview_DTO> GetStoreReviews(long idClient)
        {
            var cnfg = EntityConfigDictionary.GetConfig("store_reviews_add_vote");
            var prov = new EntityProvider<StoreReview_DTO>(cnfg);
            object clientId = (idClient == 0) ? (object)DBNull.Value : idClient;

            var reviews = prov.GetItems(null,new OracleParameter("idClient", clientId));
            return reviews;
        }

        public IEnumerable<ProductReview_DTO> GetProductReviews(long idProduct, long idClient)
        {
            var cnfg = EntityConfigDictionary.GetConfig("product_reviews_add_vote");
            var prov = new EntityProvider<ProductReview_DTO>(cnfg);
            object clientId = (idClient == 0) ? (object)DBNull.Value : idClient;

            var reviews = prov.GetItems("id_product = :id", new OracleParameter("idClient", clientId),
                                                            new OracleParameter("id", idProduct)
                                       );
            return reviews;
        }

        public ProductReview_DTO SaveProductReview(ProductReview_DTO review, Client_DTO client)
        {
            var cnfg = EntityConfigDictionary.GetConfig("product_reviews");
            var prov = new EntityProvider<ProductReview_DTO>(cnfg);
            var _review = new ProductReview_DTO
            {
                idProduct = review.idProduct,
                idClient = client.id,
                user = client.name,
                reviewDate = review.reviewDate,
                reviewText = review.reviewText,
                rating = review.rating,
                advantages = review.advantages,
                disadvantages = review.disadvantages,
                upvotes = 0,
                downvotes = 0,
                idReview = review.idReview
            };
            if (_review != null)
            {
                return prov.InsertItem(_review);
            }
            else return null;
        }

        public ProductReview_DTO UpdateProductReview(ProductReview_DTO review, long idClient)
        {
            var cnfg = EntityConfigDictionary.GetConfig("product_reviews");
            var prov = new EntityProvider<ProductReview_DTO>(cnfg);

            string res = "";
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("begin :result := foxstore.pkg_review.updatereviewvote(:previewid,:pclientid,:pvote,:ptype);end;", con))
            {
                try
                {
                    cmd.Parameters.Add("result", OracleDbType.Varchar2, ParameterDirection.Output);
                    cmd.Parameters["result"].Size = 32767;
                    cmd.Parameters.Add(new OracleParameter("previewid", review.id));
                    cmd.Parameters.Add(new OracleParameter("pclientid", idClient));
                    cmd.Parameters.Add(new OracleParameter("pvote", review.vote));
                    cmd.Parameters.Add(new OracleParameter("ptype", OracleDbType.Int32)).Value = 0;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = cmd.Parameters["result"].Value.ToString();

                    if (String.IsNullOrEmpty(res))
                        return null;

                    var prevId = Convert.ToInt64(res);
                    var prevObj = prov.GetItem(prevId);
                    prevObj.vote = review.vote;

                    return prevObj;
                }
                finally
                {
                    con.Close();
                }
            }
        }


        public StoreReview_DTO SaveStoreReview(StoreReview_DTO review, Client_DTO client)
        {
            var cnfg = EntityConfigDictionary.GetConfig("store_reviews");
            var prov = new EntityProvider<StoreReview_DTO>(cnfg);
            var _review = new StoreReview_DTO
            {
                idStore = review.idStore,
                idClient = client.id,
                user = client.name,
                reviewDate = review.reviewDate,
                reviewText = review.reviewText,
                rating = review.rating,
                advantages = review.advantages,
                disadvantages = review.disadvantages,
                upvotes = 0,
                downvotes = 0,
                idReview = review.idReview
            };
            if (_review != null)
            {
                return prov.InsertItem(_review);
            }
            else return null;
        }

        public StoreReview_DTO UpdateStoreReview(StoreReview_DTO review, long idClient)
        {
            var cnfg = EntityConfigDictionary.GetConfig("store_reviews");
            var prov = new EntityProvider<StoreReview_DTO>(cnfg);

            string res = "";
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("begin :result := foxstore.pkg_review.updatereviewvote(:sreviewid,:sclientid,:svote,:stype);end;", con))
            {
                try
                {
                    cmd.Parameters.Add("result", OracleDbType.Varchar2, ParameterDirection.Output);
                    cmd.Parameters["result"].Size = 32767;
                    cmd.Parameters.Add(new OracleParameter("sreviewid", review.id));
                    cmd.Parameters.Add(new OracleParameter("sclientid", idClient));
                    cmd.Parameters.Add(new OracleParameter("svote", review.vote));
                    cmd.Parameters.Add(new OracleParameter("stype", OracleDbType.Int32)).Value = 1;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = cmd.Parameters["result"].Value.ToString();

                    if (String.IsNullOrEmpty(res))
                        return null;

                    var srevId = Convert.ToInt64(res);
                    var srevObj = prov.GetItem(srevId);
                    srevObj.vote = review.vote;

                    return srevObj;
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}
