using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Repository.Abstract;
using Wsds.WebApp.Filters;
using Wsds.WebApp.WebExtensions;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Poll")]
    public class PollController : Controller
    {
        private readonly IPollRepository _pollRepository;

        public PollController(IPollRepository pollRepository)
        {
            _pollRepository = pollRepository;
        }

        [HttpGet("{id}")]
        public IActionResult GetPoll(long id)
        {
            return Ok(_pollRepository.GetPollById(id));
        }

        [HttpGet("pollQuestions/{idPoll}")]
        public IActionResult GetPollQuestions(long idPoll)
        {
            return Ok(_pollRepository.GetPollQuestionsByPollId(idPoll));
        }

        [HttpGet("pollAnswers/{idPollQuestion}")]
        public IActionResult GetPollAnswers(long idPollQuestion)
        {
            return Ok(_pollRepository.GetPollAnswersByQuestionId(idPollQuestion));
        }
       
        [Authorize]
        [HttpGet("ClientPollAnswer/{id}")]
        [PullToken]
        public IActionResult ClientPollAnswer(long id)
        {
            var tokenModel = HttpContext.GeTokenModel();
            return Ok(_pollRepository.GetClientPollAnswersByPollId(id,tokenModel.ClientId));
        }

        [Authorize]
        [HttpPost]
        [PullToken]
        public IActionResult ClientAnswers([FromBody] ClientPoolAnswers pollAnswers)
        {
            var tokenData = HttpContext.GeTokenModel();
            var data=_pollRepository.SaveClientAnswers(pollAnswers, tokenData.ClientId);
            return (data != null) ? Ok(data) : (IActionResult) BadRequest();
        }
    }
}