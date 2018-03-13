using System.Collections.Generic;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IPollRepository
    {
        Poll_DTO GetPollById(long id);
        IEnumerable<ClientPollAnswer_DTO> GetClientPollAnswersByPollId(long pollId,long clientId);
        ClientPollAnswer_DTO GetClientPollAnswersById(long id);
        IEnumerable<PollQuestion_DTO> GetPollQuestionsByPollId(long pollId);
        IEnumerable<PollQuestionAnswer_DTO> GetPollAnswersByQuestionId(long pollQuestionId);
        ClientPollAnswer_DTO SaveClientAnswers(ClientPoolAnswers pollAnswers, long clientId);
    }
}
