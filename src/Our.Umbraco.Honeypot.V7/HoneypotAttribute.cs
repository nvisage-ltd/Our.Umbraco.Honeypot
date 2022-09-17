using System.Net;
using System.Web.Mvc;

namespace Our.Umbraco.Honeypot
{
    public class HoneypotAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);


            var isTrapped = context.HttpContext.ApplicationInstance.Context.IsHoneypotTrapped(out var honeypotResult, out var trapFieldName);

            if (isTrapped)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Result = new ContentResult() { Content = $"bot detection {honeypotResult} {trapFieldName}", ContentType = "text/plain" };
            }
        }
    }
}
