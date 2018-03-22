using System;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class ClientMessage_DTO
    {
        public long? id { get; set; }
        [FieldBinding(Field = "id_client")]
        public long? idClient { get; set; }
        [FieldBinding(Field = "message_date")]
        public DateTime messageDate { get; set; }
        [FieldBinding(Field = "message_text")]
        public string messageText { get; set; }
        [FieldBinding(Field = "is_answered")]
        public long? isAnswered { get; set; }
    }
}
