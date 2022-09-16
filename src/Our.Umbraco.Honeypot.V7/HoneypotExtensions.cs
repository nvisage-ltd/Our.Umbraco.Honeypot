using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Our.Umbraco.Honeypot
{
    public static class HoneypotExtensions
    {
        public const string HttpContextItemName = "Our.Umbraco.Honeypot.IsHoneypotTrapped";
        private static readonly Regex HoneypotNoTagsRegEx;
        //private static readonly Regex HoneypotNoLinksRegEx;

        static HoneypotExtensions()
        {
            HoneypotNoTagsRegEx = new Regex("<.*?>", RegexOptions.None, TimeSpan.FromSeconds(1));

            // disable link checking for now, matches email addresses
            //HoneypotNoLinksRegEx = new Regex("([a-zA-Z0-9]+://)?([a-zA-Z0-9_]+:[a-zA-Z0-9_]+@)?([a-zA-Z0-9.-]+\\.[A-Za-z]{2,4})(:[0-9]+)?([^ ])+", RegexOptions.None, TimeSpan.FromSeconds(1));
        }

        public static bool IsTrapped(HttpContext httpContext, out bool fieldTrap, out bool timeTrap)
        {
            fieldTrap = false;
            timeTrap = false;

            if (!httpContext.Items.Contains(HttpContextItemName) || (httpContext.Items[HttpContextItemName] is bool value) == false)
            {

                var trapped = false;

                if (HoneypotOptions.For.HoneypotEnableFieldCheck)
                {
                    //check fields
                    foreach (var inputKey in httpContext.Request.Form.AllKeys)
                    {
                        if (HoneypotOptions.For.HoneypotIsFieldName(inputKey) && !string.IsNullOrEmpty(httpContext.Request.Form[inputKey]))
                        {
                            fieldTrap = true;
                            trapped = true;
                            break;
                        }
                        if (HoneypotOptions.For.HoneypotNoTags && httpContext.Request.Form[inputKey] != null)
                        {
                            var isValid = !HoneypotNoTagsRegEx.IsMatch(httpContext.Request.Form[inputKey]);
                            if (!isValid)
                            {
                                fieldTrap = true;
                                trapped = true;
                                break;
                            }
                        }
                        //if (HoneypotOptions.For.HoneypotNoLinks && httpContext.Request.Form[inputKey] != null)
                        //{
                        //    var isValid = !HoneypotNoLinksRegEx.IsMatch(httpContext.Request.Form[inputKey]);
                        //    if (!isValid)
                        //    {
                        //        fieldTrap = true;
                        //        trapped = true;
                        //        break;
                        //    }
                        //}
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
            else
            {
                return value;
            }
        }

        /// <summary>
        /// IsHoneypotTrapped
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        ///
        public static bool IsHoneypotTrapped(this HttpContext httpContext)
        {

            var isTrapped = IsTrapped(httpContext, out _, out _);

            return isTrapped;
        }

        public static IHtmlString HoneypotTimeField(this HtmlHelper helper, HttpRequestBase httpRequestBase, HoneypotOptions options = null)
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

        public static IHtmlString HoneypotField(this HtmlHelper helper, HttpRequestBase httpRequestBase, HoneypotOptions options = null)
        {
            var name = string.Empty;
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
            _ = html.AppendLine($"<label for=\"{fieldName}\" class=\"{options.HoneypotFieldClass} {fieldName}\" title=\"{fieldName}\" placeholder=\"\" style=\"{options.HoneypotFieldStyles}\">&nbsp;</label>");
            _ = html.AppendLine($"<input type=\"text\" name=\"{fieldName}\" id=\"{fieldName}\" value=\"{value}\" />");
            _ = html.AppendLine(" </div>");

            return new HtmlString(html.ToString());
        }

    }
}
