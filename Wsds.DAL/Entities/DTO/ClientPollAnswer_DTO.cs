using System;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class ClientPollAnswer_DTO
    {
        public long? id { get; set; }
        public long? idClient { get; set; }
        public long? idPoll { get; set; }
        public long? idPollQuestions { get; set; }
        public string clientAnswer { get; set; }
    }
}
