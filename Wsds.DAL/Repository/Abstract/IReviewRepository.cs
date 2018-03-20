using System.Collections.Generic;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IReviewRepository
    {
        IEnumerable<ProductReview_DTO> GetProductReviews(long id);

        IEnumerable<StoreReview_DTO> GetStoreReviews();
        IEnumerable<StoreReview_DTO> GetStoreReviewsByStoreId(long id);
    }
}
