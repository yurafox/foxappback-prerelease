using System.Collections.Generic;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IReviewRepository
    {
        IEnumerable<ProductReview_DTO> GetProductReviews(long idProduct, long idClient);

        IEnumerable<StoreReview_DTO> GetStoreReviews(long idClient);
        IEnumerable<StoreReview_DTO> GetStoreReviewsByStoreId(long idStore, long idClient);

        ProductReview_DTO SaveProductReview(ProductReview_DTO review, Client_DTO client);
        StoreReview_DTO SaveStoreReview(StoreReview_DTO review, Client_DTO client);
        ProductReview_DTO UpdateProductReview(ProductReview_DTO review, long idClient);
        StoreReview_DTO UpdateStoreReview(StoreReview_DTO review, long idClient);
    }
}
