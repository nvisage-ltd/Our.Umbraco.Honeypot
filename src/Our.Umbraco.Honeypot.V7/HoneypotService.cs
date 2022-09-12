using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Our.Umbraco.Honeypot
{
    public class HoneypotService
    {
        public const string HttpContextItemName = "Our.Umbraco.Honeypot.IsHoneypotTrapped";

        private readonly Regex _honeypotNoTagsRegEx;

        private HoneypotOptions Options { get; }


        //public bool IsTrapped(HttpContext httpContext)
        //{
        //    return httpContext.IsHoneypotTrapped();
        //}

        public HoneypotService(HoneypotOptions options)
        {
            Options = options;
            _honeypotNoTagsRegEx = new Regex("<.*?>", RegexOptions.None, TimeSpan.FromSeconds(1));
        }

        public bool IsTrapped(HttpContext httpContext, out bool fieldTrap, out bool timeTrap)
        {
            fieldTrap = false;
            timeTrap = false;

            if (!httpContext.Items.Contains(HttpContextItemName) || (httpContext.Items[HttpContextItemName] is bool value) == false)
            {

                var trapped = false;

                if (Options.HoneypotEnableFieldCheck)
                {
                    //check fields
                    foreach (var inputKey in httpContext.Request.Form.AllKeys)
                    {
                        if (Options.HoneypotIsFieldName(inputKey) && !string.IsNullOrEmpty(httpContext.Request.Form[inputKey]))
                        {
                            fieldTrap = true;
                            trapped = true;
                            break;
                        }
                        if (Options.HoneypotNoTags && httpContext.Request.Form[inputKey] != null)
                        {
                            var isValid = !_honeypotNoTagsRegEx.IsMatch(httpContext.Request.Form[inputKey]);
                            if (!isValid)
                            {
                                fieldTrap = true;
                                trapped = true;
                                break;
                            }
                        }
                    }
                }

                if (Options.HoneypotEnableTimeCheck && !trapped)
                {
                    //check time
                    if (httpContext.Request.Form[Options.HoneypotTimeFieldName] is string timeValue)
                    {
                        TimeSpan diff = DateTime.UtcNow - new DateTime(long.Parse(timeValue), DateTimeKind.Utc);

                        timeTrap = true;
                        trapped = diff < Options.HoneypotMinTimeDuration;
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

    }
}
