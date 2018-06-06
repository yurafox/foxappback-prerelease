using Microsoft.AspNetCore.Mvc;
using Wsds.WebApp.Models;

namespace Wsds.WebApp.Controllers
{
    public class PaymasterController : Controller
    {
        
        [HttpGet]
        public IActionResult Payment(PaymentModel payment)
        {
            return BadRequest();
            /*if (payment == null || !TryValidateModel(payment))
                return BadRequest("Ошибка состояния данных в запросе к платежной системе");


            return View("Payment", payment);*/
        }

        [HttpPost]
        public IActionResult Result(PaymentResultModel paymentReceiver)
        {
            return BadRequest();
            /*if (!TryValidateModel(paymentReceiver))
            {
                return BadRequest("ошибка ответа от платежной системы");
            }

            ViewBag.Result = paymentReceiver.GetResultVerification();
            return View("Result");*/

        }
    }
}