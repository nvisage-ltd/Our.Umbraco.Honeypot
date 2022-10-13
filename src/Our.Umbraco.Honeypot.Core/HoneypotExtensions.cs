#if NETFRAMEWORK
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Web.Composing;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Our.Umbraco.Honeypot.Core
{
    public static class HoneypotExtensions
    {

        /// <summary>
        /// IsHoneypotTrapped
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        ///
        public static bool IsHoneypotTrapped(this HttpContext httpContext)
        {

#if NETFRAMEWORK
            HoneypotService service = Current.Factory.GetInstance<HoneypotService>();
#else
            HoneypotService service = httpContext.RequestServices.GetRequiredService<HoneypotService>();
#endif
            var isTrapped = service.IsTrapped(httpContext, out var fieldTrapped, out var timeTrapped);

            return isTrapped;
        }

#if NETFRAMEWORK
        public static IHtmlString HoneypotTimeField(this HtmlHelper helper, HttpRequestBase httpRequestBase, HoneypotOptions options = null)
        {
            var value = DateTime.UtcNow.Ticks.ToString();
            if (options == null)
            {
                options = Current.Factory.GetInstance<HoneypotOptions>();
            }

            if (!options.HoneypotEnableTimeCheck)
            {
                return new HtmlString("");
            }

            foreach (var inputKey in httpRequestBase.Form.AllKeys)
            {
                if (options.HoneypotIsFieldName(inputKey))
                {
                    value = httpRequestBase.Form?.Get(inputKey);
                    break;
                }
            }

            var html = new StringBuilder();

            _ = html.AppendLine($"<div class=\"{options.HoneypotFieldClass} hp-{options.HoneypotTimeFieldName}\" style=\"{options.HoneypotFieldStyles}\">");
            _ = html.AppendLine($"<input type=\"hidden\" value=\"{value}\" name=\"{options.HoneypotTimeFieldName}\" id=\"{options.HoneypotTimeFieldName}\" />");
            _ = html.AppendLine("</div>");

            return new HtmlString(html.ToString());
        }

        public static IHtmlString HoneypotField(this HtmlHelper helper, HttpRequestBase httpRequestBase, string name = null, string type = "text", HoneypotOptions options = null)
        {
            var value = string.Empty;

            if (options == null)
            {
                options = Current.Factory.GetInstance<HoneypotOptions>();
            }

            if (!options.HoneypotEnableFieldCheck)
            {
                return new HtmlString("");
            }

            foreach (var inputKey in httpRequestBase.Form.AllKeys)
            {
                if (options.HoneypotIsFieldName(inputKey))
                {
                    name = inputKey;
                    value = httpRequestBase.Form?.Get(name);
                    break;
                }
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                name = options.RandomName();
            }

            var fieldName = options.HoneypotGetFieldName(name);


            var html = new StringBuilder();

            _ = html.AppendLine($"<div class=\"{options.HoneypotFieldClass} {fieldName}\" style=\"{options.HoneypotFieldStyles}\">");
            if (type == "text")
            {
                _ = html.AppendLine(
                    $"<label for=\"{fieldName}\" class=\"{options.HoneypotFieldClass} {fieldName}\" title=\"{fieldName}\" style=\"{options.HoneypotFieldStyles}\">&nbsp;</label>");
            }

            _ = html.AppendLine($"<input type=\"{type}\" name=\"{fieldName}\" id=\"{fieldName}\" value=\"{value}\" />");
            _ = html.AppendLine(" </div>");

            return new HtmlString(html.ToString());
        }

#endif
    }
}
