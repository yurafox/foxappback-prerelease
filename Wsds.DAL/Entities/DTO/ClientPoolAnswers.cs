using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.DTO
{
    public class ClientPoolAnswers
    {
        public long PollId { get; set; }
        public IDictionary<string, ClientPoolResult> PollResult { get; set; }
    }

    public class ClientPoolResult
    {
        public long QuestionId { get; set; }
        public string AnswerValue { get; set; }
    }
}
