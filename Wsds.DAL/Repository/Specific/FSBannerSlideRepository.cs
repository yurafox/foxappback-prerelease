using System.Collections.Generic;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSBannerSlideRepository: IBannerSlideRepository
    {
        private ICacheService<BannerSlide_DTO> _csb;

        public FSBannerSlideRepository(ICacheService<BannerSlide_DTO> csb)
        {
            _csb = csb;
        }

        public IEnumerable<BannerSlide_DTO> BannerSlides => _csb.Items.Values;
    }
}
