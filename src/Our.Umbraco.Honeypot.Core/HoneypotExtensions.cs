﻿#if NETFRAMEWORK
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Web.Composing;
#else
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
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
        public static IHtmlString HoneypotTimeField(this HtmlHelper helper, HoneypotOptions options = null)
        {
            if (options == null)
            {
                options = Current.Factory.GetInstance<HoneypotOptions>();
            }

            if (!options.HoneypotEnableTimeCheck)
            {
                return new HtmlString("");
            }


            var html = new StringBuilder();

            _ = html.AppendLine($"<div class=\"{options.HoneypotFieldClass} hp-{options.HoneypotTimeFieldName}\" style=\"{options.HoneypotFieldStyles}\">");
            _ = html.AppendLine($"<input type=\"hidden\" value=\"{DateTime.UtcNow.Ticks}\" name=\"{options.HoneypotTimeFieldName}\" id=\"{options.HoneypotTimeFieldName}\" />");
            _ = html.AppendLine("</div>");

            return new HtmlString(html.ToString());
        }

        public static IHtmlString HoneypotField(this HtmlHelper helper, string name = null, string type = "text", HoneypotOptions options = null)
        {
            if (options == null)
            {
                options = Current.Factory.GetInstance<HoneypotOptions>();
            }

            if (!options.HoneypotEnableFieldCheck)
            {
                return new HtmlString("");
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
                    $"<label for=\"{fieldName}\" class=\"{options.HoneypotFieldClass} {fieldName}\" title=\"{fieldName}\" placeholder=\"\" style=\"{options.HoneypotFieldStyles}\">&nbsp;</label>");
            }

            _ = html.AppendLine($"<input type=\"{type}\" name=\"{fieldName}\" id=\"{fieldName}\" />");
            _ = html.AppendLine(" </div>");

            return new HtmlString(html.ToString());
        }

#endif
    }
}
