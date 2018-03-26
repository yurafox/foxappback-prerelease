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

        public ProductReview_DTO UpdateProductReview(ProductReview_DTO review)
        {
            var cnfg = EntityConfigDictionary.GetConfig("product_reviews");
            var prov = new EntityProvider<ProductReview_DTO>(cnfg);
            var _review = review;
            if (review.upvotes != null) _review.upvotes = review.upvotes;
            if (review.downvotes != null) _review.downvotes = review.downvotes;
            if (_review != null)
            {
                return prov.UpdateItem(_review);
            }
            else return null;
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

        public StoreReview_DTO UpdateStoreReview(StoreReview_DTO review)
        {
            var cnfg = EntityConfigDictionary.GetConfig("store_reviews");
            var prov = new EntityProvider<StoreReview_DTO>(cnfg);
            var _review = review;
            if (review.upvotes != null) _review.upvotes = review.upvotes;
            if (review.downvotes != null) _review.downvotes = review.downvotes;
            if (_review != null)
            {
                return prov.UpdateItem(_review);
            }
            else return null;
        }
    }
}
