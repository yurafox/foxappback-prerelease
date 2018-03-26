using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class FavoriteStore_DTO
    {
        public long? id { get; set; }
        [FieldBinding(Field = "id_client")]
        public long idClient { get; set; }
        [FieldBinding(Field = "id_store_places")]
        public long idStore { get; set; }
    }
}
