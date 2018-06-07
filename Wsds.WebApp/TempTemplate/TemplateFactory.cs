using System;

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
        public string GetTemplate(TemplateEnum template)
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
                    tmpl = MakePayMasterPayment();
                    break;
                }

                case TemplateEnum.PayMasterResult:
                {
                    tmpl = PayMasterResult();
                    break; ;
                }
            }

            return tmpl;
        }
        #endregion

        private string MakeHomeResult()
            => "<div style = \"width: 300px; margin: 300px auto\"><h1> Foxtrot Web API</h1></div>";

        private string MakePayMasterPayment()
            => "<div style = \"width: 250px; margin: 250px auto\"><h1>Сервис временно недоступен</h1></div>";

        private string PayMasterResult()
            => "<div style = \"width: 250px; margin: 250px auto\"><h1>Сервис временно недоступен</h1></div>";
    }
}
