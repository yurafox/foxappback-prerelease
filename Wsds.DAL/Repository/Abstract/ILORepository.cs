using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface ILORepository
    {
        IEnumerable<LoEntity_DTO> LoEntities { get; }
        LoEntity_DTO LoEntity(long id);

        IEnumerable<LoSupplEntity_DTO> LoSupplEntities { get; }
        IEnumerable<LoSupplEntity_DTO> GetLoEntitiesForSuppl(long supplId);
        IEnumerable<LoTrackLog> GetTrackLogForOrderSpecProd(long orderSpecProdId);
        object GetDeliveryCostByShipment(Shipment_DTO shpmt, long loEntityId, long? loIdClientAddress, long delivTypeId);
        object GetDeliveryDateByShipment(Shipment_DTO shpmt, long loEntityId, long? loIdClientAddress, long delivTypeId);
        LoDeliveryType_DTO LoDeliveryType(long id);
        IEnumerable<LoDeliveryType_DTO> GetLoDeliveryTypesByLoEntity(long idLoEntity);
        LoEntityOffice_DTO GetLoEntityOffice(long id);
        IEnumerable<LoEntityOffice_DTO> GetLoEntityOfficesByLoEntityAndCity(long idLoEntity, long idCity);
        IEnumerable<LoDeliveryTypeAttr_DTO> GetLoDeliveryTypesAttrByLoEntity(Shipment_DTO shpmt, long? loIdClientAddress);
    }
}
