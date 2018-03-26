using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Specific
{
    public class FSDeviceDataRepository: IDeviceDataRepository
    {
        public DeviceData_DTO SaveDeviceData(DeviceData_DTO data, long idClient)
        {
            var cnfg = EntityConfigDictionary.GetConfig("device_data");
            var prov = new EntityProvider<DeviceData_DTO>(cnfg);
            var deviceData = new DeviceData_DTO
            {
                idClient = idClient,
                model = data.model,
                os = data.os,
                height = data.height,
                width = data.width,
                pushDeviceToken = data.pushDeviceToken
            };
            var devices = prov.GetItems("t.id_client = :clientId and t.model = :model and t.operation_system = :os", new OracleParameter("clientId", idClient), new OracleParameter("model", data.model), new OracleParameter("os", data.os));
            if (devices.Count() == 0)
            {
                return prov.InsertItem(deviceData);
            }
            else return null;
        }
    }
}
