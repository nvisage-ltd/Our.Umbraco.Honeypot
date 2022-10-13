using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Honeypot
{
    public static class HoneypotExtensions
    {
        public const string HttpContextItemName = "Our.Umbraco.Honeypot.IsHoneypotTrapped";

        /// <summary>
        /// IsHoneypotTrapped
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        ///
        public static bool IsHoneypotTrapped(this HttpContext httpContext)
        {
            var isTrapped = httpContext.IsHoneypotTrapped(out _, out _);
            return isTrapped;
        }

        public static bool IsTrapped(HttpContext httpContext, out bool fieldTrap, out bool timeTrap)
        {
            fieldTrap = false;
            timeTrap = false;

            try
            {
                if (!httpContext.Items.Contains(HttpContextItemName) ||
                    httpContext.Items[HttpContextItemName] is bool value == false)
                {
                    var trapped = false;

                    if (HoneypotOptions.For.HoneypotEnableFieldCheck)
                    {
                        //check fields
                        foreach (var inputKey in httpContext.Request.Form.AllKeys)
                        {
                            if (HoneypotOptions.For.HoneypotIsFieldName(inputKey) &&
                                !string.IsNullOrEmpty(httpContext.Request.Form[inputKey]))
                            {
                                fieldTrap = true;
                                trapped = true;
                                break;
                            }
                        }
                    }

                    if (HoneypotOptions.For.HoneypotEnableTimeCheck && !trapped)
                    {
                        //check time
                        if (httpContext.Request.Form[HoneypotOptions.For.HoneypotTimeFieldName] is string timeValue)
                        {
                            TimeSpan diff = DateTime.UtcNow - new DateTime(long.Parse(timeValue), DateTimeKind.Utc);

                            timeTrap = true;
                            trapped = diff < HoneypotOptions.For.HoneypotMinTimeDuration;
                        }
                    }

                    httpContext.Items.Add(HttpContextItemName, trapped);

                    return trapped;
                }

                return value;
            }
            catch (Exception e)
            {
                LogHelper.Error<HoneypotFieldType>(e.Message, e);
                return false;
            }
        }

        /// <summary>
        ///     IsHoneypotTrapped
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="fieldTrap"></param>
        /// <param name="timeTrap"></param>
        /// <returns></returns>
        public static bool IsHoneypotTrapped(this HttpContext httpContext, out bool fieldTrap, out bool timeTrap)
        {
            var isTrapped = IsTrapped(httpContext, out fieldTrap, out timeTrap);
            return isTrapped;
        }

        public static IHtmlString HoneypotTimeField(this HtmlHelper helper, HttpRequestBase httpRequestBase,
            HoneypotOptions options = null)
        {
            if (options == null)
            {
                options = HoneypotOptions.For;
            }

            if (!options.HoneypotEnableTimeCheck)
            {
                return new HtmlString("");
            }

            var value = DateTime.UtcNow.Ticks.ToString();

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
                options = HoneypotOptions.For;
            }

            if (!options.HoneypotEnableFieldCheck)
            {
                return new HtmlString("");
            }

            foreach (var inputKey in httpRequestBase.Form.AllKeys)
            {
                if (HoneypotOptions.For.HoneypotIsFieldName(inputKey))
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
                _ = html.AppendLine($"<label for=\"{fieldName}\" class=\"{options.HoneypotFieldClass} {fieldName}\" title=\"{fieldName}\" style=\"{options.HoneypotFieldStyles}\">&nbsp;</label>");
            }

            _ = html.AppendLine($"<input type=\"{type}\" name=\"{fieldName}\" id=\"{fieldName}\" value=\"{value}\" />");
            _ = html.AppendLine(" </div>");

            return new HtmlString(html.ToString());
        }
    }
}