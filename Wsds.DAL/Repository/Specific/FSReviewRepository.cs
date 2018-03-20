using System.Collections.Generic;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;
using Oracle.ManagedDataAccess.Client;

namespace Wsds.DAL.Repository.Specific
{
    public class FSReviewRepository : IReviewRepository
    {

        public IEnumerable<StoreReview_DTO> GetStoreReviewsByStoreId(long id)
        {
            var cnfg = EntityConfigDictionary.GetConfig("store_reviews");
            var prov = new EntityProvider<StoreReview_DTO>(cnfg);
            var reviews = prov.GetItems("id_store = :id", new OracleParameter("id", id));
            return reviews;
        }

        public IEnumerable<StoreReview_DTO> GetStoreReviews()
        {
            var cnfg = EntityConfigDictionary.GetConfig("store_reviews");
            var prov = new EntityProvider<StoreReview_DTO>(cnfg);
            var reviews = prov.GetItems();
            return reviews;
        }

        public IEnumerable<ProductReview_DTO> GetProductReviews(long id)
        {
            var cnfg = EntityConfigDictionary.GetConfig("product_reviews");
            var prov = new EntityProvider<ProductReview_DTO>(cnfg);
            var reviews = prov.GetItems("id_product = :id", new OracleParameter("id", id));
            return reviews;
        }
    }
}
