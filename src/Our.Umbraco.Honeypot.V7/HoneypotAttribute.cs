using System.Net;
using System.Web.Mvc;

namespace Our.Umbraco.Honeypot
{
    public class HoneypotAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);


            var isTrapped = context.HttpContext.ApplicationInstance.Context.IsHoneypotTrapped(out bool fieldTrap, out bool timeTrap);

            if (isTrapped)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Result = new ContentResult() { Content = $"bot detection", ContentType = "text/plain" };
            }
        }
    }
}
