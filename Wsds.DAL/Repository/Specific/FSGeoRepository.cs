using Oracle.ManagedDataAccess.Client;
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
        private ICacheService<Region_DTO> _csRegion;
        public FSGeoRepository(ICacheService<City_DTO> csCity, 
                               ICacheService<Country_DTO> csCountry,
                               ICacheService<Region_DTO> csRegion) {
            _csCity = csCity;
            _csCountry = csCountry;
            _csRegion = csRegion;
        }

        public IEnumerable<City_DTO> Cities => _csCity.Items.Values;

        public IEnumerable<Country_DTO> Countries => _csCountry.Items.Values;

        public IEnumerable<Region_DTO> Regions => _csRegion.Items.Values;

        public IEnumerable<City_DTO> CitiesWithStores()
        {
            var clCnfg = EntityConfigDictionary.GetConfig("city_with_store");
            var prov = new EntityProvider<City_DTO>(clCnfg);
            return prov.GetItems();
        } 

        public City_DTO City(long id) => _csCity.Item(id);

        public Country_DTO Country(long id) => _csCountry.Item(id);

        public Region_DTO Region(long id) => _csRegion.Item(id);

        public IEnumerable<City_DTO> SearchCities(string srhString)
        {
            if (srhString != null)
            {
                var ctCnfg = EntityConfigDictionary.GetConfig("city");
                var prov = new EntityProvider<City_DTO>(ctCnfg);

                OracleParameter parameter = new OracleParameter
                {
                    ParameterName = "cname",
                    OracleDbType = OracleDbType.Varchar2,
                    Value = srhString.ToLower() + '%'
                };

                return prov.GetItems("lower(t.name) like :cname", parameter)
                    .OrderBy(x => x.name);
            }
            else
            {
                return new List<City_DTO>();
            };
        }
    }
}
