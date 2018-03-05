using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSGeoRepository : IGeoRepository
    {
        private ICacheService<City_DTO> _csCity;
        private ICacheService<Country_DTO> _csCountry;
        public FSGeoRepository(ICacheService<City_DTO> csCity, ICacheService<Country_DTO> csCountry) {
            _csCity = csCity;
            _csCountry = csCountry;
        }

        public IEnumerable<City_DTO> Cities => _csCity.Items.Values;

        public IEnumerable<Country_DTO> Countries => _csCountry.Items.Values;

        public IEnumerable<City_DTO> CitiesWithStores()
        {
            var clCnfg = EntityConfigDictionary.GetConfig("city_with_store");
            var prov = new EntityProvider<City_DTO>(clCnfg);
            return prov.GetItems();
        } 

        public City_DTO City(long id) => _csCity.Item(id);

        public Country_DTO Country(long id) => _csCountry.Item(id);
    }
}
