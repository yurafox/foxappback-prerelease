using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IGeoRepository
    {
        IEnumerable<City_DTO> Cities { get; }
        IEnumerable<City_DTO> CitiesWithStores();
        IEnumerable<Region_DTO> Regions { get; }
        Region_DTO Region(long id);
        City_DTO City(long id);
        IEnumerable<Country_DTO> Countries { get; }
        Country_DTO Country(long id);
        IEnumerable<City_DTO> SearchCities(string srhString);
    }
}
