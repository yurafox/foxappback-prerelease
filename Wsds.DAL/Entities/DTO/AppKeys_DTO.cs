using System;
using System.ComponentModel.DataAnnotations;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class AppKeys_DTO
    {
        public long id { get; set; }

        [Required]
        [FieldBinding(Field = "KEY")]
        public string key { get; set; }

        [Required]
        [FieldBinding(Field = "DATE_START")]
        public DateTime dateStart { get; set; }

        [Required]
        [FieldBinding(Field = "DATE_END")]
        public DateTime dateEnd { get; set; }

        [Required]
        [FieldBinding(Field = "ID_CLIENT")]
        public long idClient { get; set; }
    }
}
