using System;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class DeviceData_DTO
    {
        public long? id { get; set; }
        [FieldBinding(Field = "id_client")]
        public long idClient { get; set; }
        [FieldBinding(Field = "model")]
        public string model { get; set; }
        [FieldBinding(Field = "operation_system")]
        public string os { get; set; }
        [FieldBinding(Field = "screen_height")]
        public int height { get; set; }
        [FieldBinding(Field = "screen_width")]
        public int width { get; set; }
        [FieldBinding(Field = "push_device_token")]
        public string pushDeviceToken { get; set; }
    }
}
