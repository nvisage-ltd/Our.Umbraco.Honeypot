﻿

#if NETFRAMEWORK
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
#endif
using System.Net;

namespace Our.Umbraco.Honeypot.Core
{
    public class HoneypotAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

#if NETFRAMEWORK

            var isTrapped = context.HttpContext.ApplicationInstance.Context.IsHoneypotTrapped();

            if (isTrapped)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Result = new ContentResult() { Content = "bot detection", ContentType = "text/plain" };
            }
#else

            bool isTrapped = context.HttpContext.IsHoneypotTrapped();

            if (isTrapped)
            {
                context.Result = new ContentResult() { Content = "bot detection", ContentType = "text/plain", StatusCode = (int)HttpStatusCode.OK };
            }
#endif
        }
    }
}
