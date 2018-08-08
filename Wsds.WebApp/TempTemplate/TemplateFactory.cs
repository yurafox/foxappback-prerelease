using System;
using System.Text;
using Wsds.WebApp.Models;

namespace Wsds.WebApp.TempTemplate
{
    public enum TemplateEnum{
      Home,
      PayMasterPayment,
      PayMasterResult
    }

    public class TemplateFactory
    {
        #region GetTemplate
        public string GetTemplate(TemplateEnum template, object model)
        {
            string tmpl = String.Empty;
            switch (template)
            {
                case TemplateEnum.Home:
                {
                    tmpl = MakeHomeResult();
                    break;
                }

                case TemplateEnum.PayMasterPayment:
                {
                    tmpl = MakePayMasterPayment(model as PaymentModel);
                    break;
                }

                case TemplateEnum.PayMasterResult:
                {
                    tmpl = PayMasterResult(model as string);
                    break; ;
                }
            }

            return tmpl;
        }
        #endregion

        private string MakeHomeResult()
            => "<div style = \"width: 300px; margin: 300px auto\"><h1> Foxtrot Web API</h1></div>";

        private string MakePayMasterPayment(PaymentModel model)
        {
            if (model == null)
                throw new ArgumentNullException("PaymentModel can not be null");

            string description = $"Заказ #{model.Id} на сумму {model.Total} грн";
            string versionString = typeof(Wsds.WebApp.Startup).Assembly.GetName().Version.ToString().Remove(3);
            string resultLink = $"https://fma-lb.foxtrot.com.ua/api/v{versionString}/Payment/Result";

            var sb = new StringBuilder();
            sb.Append("<form action = 'https://lmi.paysoft.solutions/' method = 'post' id = 'payment' name = 'payment'>");
            sb.Append("<input type = 'hidden' name = 'LMI_MERCHANT_ID' value = '2117'/>");
            sb.AppendFormat("<input type = 'hidden' name = 'LMI_PAYMENT_NO' value = '{0}'/>", model.Id);
            sb.AppendFormat("<input type = 'hidden' name = 'LMI_PAYMENT_AMOUNT' value = '{0}'/>", model.Total);
            sb.AppendFormat("<input type = 'hidden' name = 'LMI_PAYMENT_DESC' value = '{0}'/>", description);
            sb.AppendFormat("<input type = 'hidden' name = 'LMI_SYS_PAYMENT_ID' value = '{0}'/>", model.Id);
            sb.AppendFormat("<input type = 'hidden' name = 'LMI_SUCCESS_URL' value = '{0}'/>", resultLink);
            sb.AppendFormat("<input type = 'hidden' name = 'LMI_FAIL_URL' value = '{0}'/>", resultLink);
            sb.Append("<input type = 'hidden' name = 'LMI_PAYMENT_NOTIFICATION_URL' value = '' />");
            sb.Append("<input type = 'hidden' name = 'LMI_PAYMENT_SYSTEM' value = '21'/>");
            sb.Append("</form>");
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("document.forms['payment'].submit();");
            sb.Append("</script>");

            return sb.ToString();
        }

        private string PayMasterResult(string model)
        {
            if (model == null)
                throw new ArgumentNullException("PayMasterResult model can not be null");

            var sb = new StringBuilder();
            sb.Append("<script>");
            sb.AppendFormat("window.onload = function() {window.parent.postMessage('{0}', '*');}",model);
            sb.Append("</script>");

            return sb.ToString();
        }
    }
}
