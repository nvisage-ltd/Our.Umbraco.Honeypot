

#if NETFRAMEWORK
using System;
using System.Text.RegularExpressions;
using System.Web;

#else
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
#endif


namespace Our.Umbraco.Honeypot.Core
{
    public class HoneypotService
    {
        public const string HttpContextItemName = "Our.Umbraco.Honeypot.IsHoneypotTrapped";

        private readonly Regex _honeypotNoTagsRegEx;

        private HoneypotOptions Options { get; }

#if NETFRAMEWORK
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

#else
        public HoneypotService(IOptions<HoneypotOptions> options)
        {
            Options = options.Value;
            _honeypotNoTagsRegEx = new Regex("<.*?>", RegexOptions.None, TimeSpan.FromSeconds(1));

        }

        public bool IsTrapped(HttpContext httpContext, out bool fieldTrap, out bool timeTrap)
        {
            fieldTrap = false;
            timeTrap = false;

            if (httpContext.Items.TryGetValue(HttpContextItemName, out object value) == false)
            {

                bool trapped = false;

                if (Options.HoneypotEnableFieldCheck)
                {
                    //check fields
                    fieldTrap = true;
                    trapped = httpContext.Request.Form.Any(x => Options.HoneypotIsFieldName(x.Key) && x.Value.Any(v => !string.IsNullOrEmpty(v)));
                    if (!trapped && Options.HoneypotNoTags)
                    {
                        trapped = httpContext.Request.Form.Any(x => x.Value.Any(v => _honeypotNoTagsRegEx.IsMatch(v)));
                    }
                }

                if (Options.HoneypotEnableTimeCheck && !trapped)
                {
                    //check time
                    if (httpContext.Request.Form.TryGetValue(Options.HoneypotTimeFieldName, out StringValues timeValues))
                    {
                        if (timeValues.Any())
                        {
                            TimeSpan diff = DateTime.UtcNow - new DateTime(long.Parse(timeValues.First()), DateTimeKind.Utc);

                            timeTrap = true;
                            trapped = diff < Options.HoneypotMinTimeDuration;
                        }
                    }


                }

                httpContext.Items.Add(HttpContextItemName, trapped);

                return trapped;
            }
            else
            {
                return (bool)value;
            }
        }
#endif
    }
}
