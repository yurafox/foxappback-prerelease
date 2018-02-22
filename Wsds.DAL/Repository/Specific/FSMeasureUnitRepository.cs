using System.Collections.Generic;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSMeasureUnitRepository : IMeasureUnitRepository
    {
        private ICacheService<Measure_Unit_DTO> _csmu;
        public FSMeasureUnitRepository(ICacheService<Measure_Unit_DTO> csmu) => _csmu = csmu;
        public IEnumerable<Measure_Unit_DTO> MeasureUnits => _csmu.Items.Values;

        public Measure_Unit_DTO MeasureUnit(long id) => _csmu.Item(id);
    }
}
