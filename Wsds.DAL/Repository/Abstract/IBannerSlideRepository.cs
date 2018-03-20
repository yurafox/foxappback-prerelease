using System.Collections.Generic;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IBannerSlideRepository
    {
        IEnumerable<BannerSlide_DTO> BannerSlides { get; }
    }
}
