using System;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class PollQuestion_DTO
    {
        public long? id { get; set; }
        public long? idPoll { get; set; }
        public long? order { get; set; }
        public string question { get; set; }
        public int? answerType { get; set; }
    }
}
