using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IDeviceDataRepository
    {
        DeviceData_DTO SaveDeviceData(DeviceData_DTO data, long idClient);
    }
}
