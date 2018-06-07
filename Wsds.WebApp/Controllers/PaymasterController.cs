using Microsoft.AspNetCore.Mvc;
using Wsds.WebApp.Models;
using Wsds.WebApp.TempTemplate;
using Wsds.WebApp.WebExtensions;

namespace Wsds.WebApp.Controllers
{
    public class PaymasterController : Controller
    {
        
        [HttpGet]
        public IActionResult Payment(PaymentModel payment)
        {
            if (payment == null || !TryValidateModel(payment))
                return BadRequest("Ошибка состояния данных в запросе к платежной системе");


            return View("Payment", payment).GetRawContent(TemplateEnum.PayMasterPayment); ;
        }

        [HttpPost]
        public IActionResult Result(PaymentResultModel paymentReceiver)
        {
            if (!TryValidateModel(paymentReceiver))
            {
                return BadRequest("ошибка ответа от платежной системы");
            }

            ViewBag.Result = paymentReceiver.GetResultVerification();
            return View("Result").GetRawContent(TemplateEnum.PayMasterResult);

        }
    }
}