using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSPollRepository : IPollRepository
    {
        private readonly IConfiguration _configuration;
        private string OraConnectionString { get; }

        public FSPollRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            OraConnectionString = _configuration.GetConnectionString("MainDataConnection");
        }

        public Poll_DTO GetPollById(long id)
        {
            var plCnfg = EntityConfigDictionary.GetConfig("polls");
            var prov = new EntityProvider<Poll_DTO>(plCnfg);
            return prov.GetItem(id);
        }

        public IEnumerable<ClientPollAnswer_DTO> GetClientPollAnswersByPollId(long pollId, long clientId)
        {
            var cnfg = EntityConfigDictionary.GetConfig("client_poll_answers");
            var prov = new EntityProvider<ClientPollAnswer_DTO>(cnfg);
            return prov.GetItems("id_poll=:pollId and id_client=:clientId", new OracleParameter("pollId", pollId),
                                     new OracleParameter("clientId", clientId));
        
        }

        public ClientPollAnswer_DTO GetClientPollAnswersById(long id)
        {
            var cnfg = EntityConfigDictionary.GetConfig("client_poll_answers");
            var prov = new EntityProvider<ClientPollAnswer_DTO>(cnfg);
            return prov.GetItem(id);
        }


        public IEnumerable<PollQuestion_DTO> GetPollQuestionsByPollId(long pollId)
        {
            var cnfg = EntityConfigDictionary.GetConfig("poll_questions");
            var prov = new EntityProvider<PollQuestion_DTO>(cnfg);
            return prov.GetItems("id_poll=:pollId", new OracleParameter("pollId", pollId));
        }

        public IEnumerable<PollQuestionAnswer_DTO> GetPollAnswersByQuestionId(long pollQuestionId)
        {
            var cnfg = EntityConfigDictionary.GetConfig("poll_question_answers");
            var prov = new EntityProvider<PollQuestionAnswer_DTO>(cnfg);
            return prov.GetItems("id_poll_questions=:pollQuestionId", new OracleParameter("id_poll_questions", pollQuestionId));
        }

        public ClientPollAnswer_DTO SaveClientAnswers(ClientPoolAnswers pollAnswers, long clientId)
        {
            if (pollAnswers?.PollResult == null)
                return null;

            ClientPollAnswer_DTO lastInsertAnswer = null;

            using (IDbConnection db = new OracleConnection(OraConnectionString))
            {
                foreach (var clientPoolResult in pollAnswers.PollResult.Values)
                {
                    var oraQuery = "insert into client_poll_answers(id_poll, id_poll_question, id_client, client_answer) " +
                                   "VALUES(:id_poll, :id_poll_question, :id_client, :client_answer)";
                    db.Execute(oraQuery, new
                    {
                        id_poll = pollAnswers.PollId,
                        id_poll_question = clientPoolResult.QuestionId,
                        id_client = clientId,
                        client_answer = clientPoolResult.AnswerValue
                    });
                   
                }

                var param = new DynamicParameters();
                param.Add(name: "Id", dbType: DbType.Int64, direction: ParameterDirection.Output);
                var currSeqQuery = "begin select client_poll_answers_seq.nextval into :Id from dual; end;";
                db.Execute(currSeqQuery, param);
                var id = param.Get<long>("Id");

                lastInsertAnswer = GetClientPollAnswersById(id-1);
                 
            }

            return lastInsertAnswer;
        }
    }
}
