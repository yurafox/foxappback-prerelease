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
                    break;
                }

                case TemplateEnum.PayMasterResult:
                {
                    break; ;
                }
            }

            return tmpl;
        }
        #endregion

        private string MakeHomeResult()
            => "<div style = \"width: 300px; margin: 300px auto\"><h1> Foxtrot Web API</h1></div>";
    }
}
