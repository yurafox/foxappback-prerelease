using Microsoft.AspNetCore.Mvc;
using Wsds.WebApp.Models;

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


            return View("Payment", payment);
        }

        [HttpGet("result")]
        public IActionResult Result(PaymentResultModel paymentReceiver)
        {
            if (!TryValidateModel(paymentReceiver))
            {
                return BadRequest("ошибка ответа от платежной системы");
            }

            ViewBag.Result = paymentReceiver.GetResultVerification();
            return View("Result");
        }
    }
}