
#if NET5_0_OR_GREATER
#nullable enable
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Our.Umbraco.Honeypot.Core
{
    public class HoneypotTimeTagHelper : TagHelper
    {
        public HoneypotTimeTagHelper(IOptions<HoneypotOptions> options)
        {
            Options = options.Value;
        }
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext? ViewContext { get; set; }
        /// <summary>
        /// Options
        /// </summary>
        public HoneypotOptions Options { get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (!Options.HoneypotEnableTimeCheck)
            {
                return;
            }

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";


            var value = DateTime.UtcNow.Ticks.ToString();

            if (ViewContext?.FormContext.HasFormData == true)
            {
                foreach (var inputKey in ViewContext.FormContext.FormData.Keys)
                {
                    if (Options.HoneypotIsFieldName(inputKey))
                    {
                        value = Convert.ToString(ViewContext.FormContext.FormData[inputKey]);
                        break;
                    }
                }
            }
            output.Attributes.Add("class", $"{Options.HoneypotFieldClass} hp-{Options.HoneypotTimeFieldName}");
            output.Attributes.Add("style", Options.HoneypotFieldStyles);

            output.Content.AppendHtml(
                $"<input type=\"hidden\" value=\"{value}\" name=\"{Options.HoneypotTimeFieldName}\" id=\"{Options.HoneypotTimeFieldName}\" />");
        }
    }
}
#endif