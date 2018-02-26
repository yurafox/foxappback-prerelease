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
        City_DTO City(long id);
        IEnumerable<Country_DTO> Countries { get; }
        Country_DTO Country(long id);
    }
}
