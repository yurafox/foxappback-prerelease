using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wsds.DAL.Repository.Abstract;
using Wsds.WebApp.Filters;
using Wsds.WebApp.WebExtensions;
using System.Linq;
using System.Collections.Generic;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CreditCardController : Controller
    {
        private readonly ICreditCardRepository _creditCardRepository;

        public CreditCardController(ICreditCardRepository creditCardRepo) => _creditCardRepository = creditCardRepo;

        [Authorize]
        [HttpGet("CreditCards")]
        [PullToken]
        public IActionResult GetCreditCards()
        {
            var tokenModel = HttpContext.GetTokenModel();

            if (tokenModel != null)
            {
                var creditCards = _creditCardRepository.GetCreditCardsByClientId(tokenModel.ClientId);
                return Ok(creditCards?.Select(c => new { id = c.id, card_mask = c.card_mask }));

            }

            return BadRequest();
        }
    }
}
