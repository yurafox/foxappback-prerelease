using System;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class PollQuestionAnswer_DTO
    {
        public long? id { get; set; }
        public long? idPollQuestions { get; set; }
        public string answer { get; set; }
    }
}
