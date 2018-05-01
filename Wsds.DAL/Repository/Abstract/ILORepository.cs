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
        object GetDeliveryCostByShipment(Shipment_DTO shpmt, long loEntityId, long loIdClientAddress);
        object GetDeliveryDateByShipment(Shipment_DTO shpmt, long loEntityId, long loIdClientAddress);

    }
}
