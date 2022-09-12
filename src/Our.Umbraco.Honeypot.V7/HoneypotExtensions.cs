using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Our.Umbraco.Honeypot
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

            HoneypotService service = DependencyResolver.Current.GetService<HoneypotService>();
            var isTrapped = service.IsTrapped(httpContext, out _, out _);

            return isTrapped;
        }

        public static IHtmlString HoneypotTimeField(this HtmlHelper helper, HoneypotOptions options = null)
        {
            if (options == null)
            {
                options = DependencyResolver.Current.GetService<HoneypotOptions>();
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
                options = DependencyResolver.Current.GetService<HoneypotOptions>();
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

    }
}
