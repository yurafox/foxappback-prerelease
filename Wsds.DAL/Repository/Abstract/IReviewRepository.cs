using System.Collections.Generic;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IReviewRepository
    {
        IEnumerable<ProductReview_DTO> GetProductReviews(long id);

        IEnumerable<StoreReview_DTO> GetStoreReviews();
        IEnumerable<StoreReview_DTO> GetStoreReviewsByStoreId(long id);

        ProductReview_DTO SaveProductReview(ProductReview_DTO review, Client_DTO client);
        StoreReview_DTO SaveStoreReview(StoreReview_DTO review, Client_DTO client);
        ProductReview_DTO UpdateProductReview(ProductReview_DTO review);
        StoreReview_DTO UpdateStoreReview(StoreReview_DTO review);
    }
}
