using System;
using System.ComponentModel.DataAnnotations;

namespace Wsds.WebApp.Models
{
    /* these awful field names are needed for auto-binding fields
     * from the paymaster payment system (lazy binding)
    */
    public class PaymentResultModel
    {
       [Required]
       public string LMI_MERCHANT_ID { get; set; }
       [Required]
       public string LMI_PAYMENT_NO { get; set; }
       [Required]
       public string LMI_PAYMENT_AMOUNT { get; set; }
       public string LMI_PAYMENT_DESC { get; set; }
       public string LMI_SYS_PAYMENT_ID { get; set; }
       public string LMI_PAYMENT_NOTIFICATION_URL { get; set; }
       public string LMI_PAYMENT_SYSTEM { get; set; }
       public string LMI_MODE { get; set; }
       public string LMI_PAID_AMOUN { get; set; }
       public string receiptToken { get; set; }
       public string LMI_SYS_PAYMENT_DATE { get; set; }
       public string LMI_CLIENT_MESSAGE { get; set; }
       public string ErrorCode { get; set; }
       public string ErrorPaysystemCode { get; set; }


        public string GetResultVerification()
        {
            bool tokenInfo = String.IsNullOrEmpty(receiptToken);
            return (tokenInfo && LMI_SYS_PAYMENT_DATE == null) ? "fail" : "success";
        }
    }
}