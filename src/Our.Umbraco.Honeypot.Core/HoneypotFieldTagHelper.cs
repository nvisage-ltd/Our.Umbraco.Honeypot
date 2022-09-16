#if NET5_0_OR_GREATER
#nullable enable
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Our.Umbraco.Honeypot.Core
{
    public class HoneypotFieldTagHelper : TagHelper
    {
        public HoneypotFieldTagHelper(IOptions<HoneypotOptions> options)
        {
            Name = string.Empty;
            Type = "text";
            Options = options.Value;
        }
        public HoneypotOptions Options { get; }

        public string Name { get; set; }

        public string Type { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext? ViewContext { get; set; }



        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (!Options.HoneypotEnableFieldCheck)
            {
                return;
            }

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";

            var value = string.Empty;

            if (ViewContext?.FormContext.HasFormData == true)
            {
                foreach (var inputKey in ViewContext.FormContext.FormData.Keys)
                {
                    if (Options.HoneypotIsFieldName(inputKey))
                    {
                        Name = inputKey;
                        value = Convert.ToString(ViewContext.FormContext.FormData[Name]);
                        break;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = Options.RandomName();
            }

            string fieldName = Options.HoneypotGetFieldName(Name);


            output.Attributes.Add("class", $"{Options.HoneypotFieldClass} {fieldName}");
            output.Attributes.Add("style", Options.HoneypotFieldStyles);
            if (Type == "text")
            {
                output.Content.AppendHtml(
                    $"<label for=\"{fieldName}\" class=\"{Options.HoneypotFieldClass} {fieldName}\" title=\"{fieldName}\" placeholder=\"\" style=\"{Options.HoneypotFieldStyles}\">&nbsp;</label>");
            }

            output.Content.AppendHtml($"<input type=\"{Type}\" name=\"{fieldName}\" id=\"{fieldName}\" value=\"{value}\" />");
        }
    }
}
#endif