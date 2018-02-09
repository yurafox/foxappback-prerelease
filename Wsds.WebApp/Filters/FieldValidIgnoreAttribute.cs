using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace wsdsCoreApi.Filters
{
    public class FieldValidIgnoreAttribute:Attribute,IAsyncActionFilter
    {
        private readonly string _fieldName;
        public FieldValidIgnoreAttribute(string fieldName)
        {
            _fieldName = fieldName;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var modelState = context.ModelState;
            if (modelState.ContainsKey(_fieldName)
                && modelState[_fieldName].ValidationState != ModelValidationState.Valid)
            {
                modelState[_fieldName].ValidationState=ModelValidationState.Valid;
            }

            await next();

        }
    }
}
