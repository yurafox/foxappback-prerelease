using System.Collections.Generic;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IMeasureUnitRepository
    {
        IEnumerable<Measure_Unit_DTO> MeasureUnits { get; }
        Measure_Unit_DTO MeasureUnit(long id);

    }
}
