using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;

namespace Wsds.WebApp.Attributes
{
    public class LinkAttribute : Attribute, IActionConstraint
    {
        private readonly string _queryParam;
        public LinkAttribute(string queryParam)
        {
            _queryParam = queryParam;
        }
        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            return context.RouteContext.HttpContext.Request.Query.ContainsKey(_queryParam);
        }
    }
}
