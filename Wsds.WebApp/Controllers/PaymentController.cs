using Microsoft.AspNetCore.Mvc;
using Wsds.WebApp.Models;
using Wsds.WebApp.TempTemplate;
using Wsds.WebApp.WebExtensions;

namespace Wsds.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PaymentController : Controller
    {
        [HttpGet]
        public IActionResult Payment(PaymentModel payment)
        {
            if (payment == null || !TryValidateModel(payment))
                return BadRequest("Ошибка состояния данных в запросе к платежной системе");


            return View("Payment")
                  .GetRawContent(TemplateEnum.PayMasterPayment, payment);
        }

        [HttpGet("result")]
        public IActionResult Result(PaymentResultModel paymentReceiver)
        {
            if (!TryValidateModel(paymentReceiver))
            {
                return BadRequest("ошибка ответа от платежной системы");
            }

            var result = paymentReceiver.GetResultVerification();
            return View("Result").GetRawContent(TemplateEnum.PayMasterResult, result);
        }
    }
}