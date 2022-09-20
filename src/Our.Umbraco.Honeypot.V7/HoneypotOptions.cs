using System;
using System.Configuration;

namespace Our.Umbraco.Honeypot
{
    public class HoneypotOptions
    {
        private static readonly Lazy<HoneypotOptions> Lazy = new Lazy<HoneypotOptions>(() => new HoneypotOptions());
        public static HoneypotOptions For => Lazy.Value;

        public HoneypotOptions()
        {

            HoneypotEnableFieldCheck = Convert.ToBoolean(ConfigurationManager.AppSettings["HoneypotEnableFieldCheck"] ?? "true");
            HoneypotEnableTimeCheck = Convert.ToBoolean(ConfigurationManager.AppSettings["HoneypotEnableTimeCheck"] ?? "true");
            HoneypotPrefixFieldName = ConfigurationManager.AppSettings["HoneypotPrefixFieldName"] ?? "hp_";
            HoneypotSuffixFieldName = ConfigurationManager.AppSettings["HoneypotSuffixFieldName"] ?? "";
            HoneypotTimeFieldName = ConfigurationManager.AppSettings["HoneypotTimeFieldName"] ?? "__time";
            HoneypotMinTimeDuration = TimeSpan.Parse(ConfigurationManager.AppSettings["HoneypotMinTimeDuration"] ?? "00:00:02");
            HoneypotFieldStyles = ConfigurationManager.AppSettings["HoneypotFieldStyles"] ?? "display: none !important; position: absolute !important; left: -9000px !important;";
            HoneypotFieldClass = ConfigurationManager.AppSettings["HoneypotFieldClass"] ?? "hp-field";
            HoneypotFieldNames = ConfigurationManager.AppSettings["HoneypotFieldNames"]?.Split(',') ?? new[] { "Name", "Phone", "Comment", "Message", "Email", "Website" };
            HoneypotMessage = ConfigurationManager.AppSettings["HoneypotMessage"] ?? "Please check your answers, we can't accept your submission currently.";

        }

        public bool HoneypotEnableFieldCheck { get; set; }

        public string HoneypotMessage { get; set; }

        public string HoneypotFieldStyles { get; set; }

        public string HoneypotFieldClass { get; set; }

        public string[] HoneypotFieldNames { get; set; }

        public bool HoneypotEnableTimeCheck { get; set; }

        public string HoneypotPrefixFieldName { get; set; }

        public string HoneypotSuffixFieldName { get; set; }

        public string HoneypotTimeFieldName { get; set; }

        public TimeSpan HoneypotMinTimeDuration { get; set; }

        internal bool HoneypotIsFieldName(string name)
        {
            return !string.IsNullOrWhiteSpace(HoneypotPrefixFieldName)
                ? name.StartsWith($"{HoneypotPrefixFieldName}")
                : !string.IsNullOrWhiteSpace(HoneypotSuffixFieldName) && name.EndsWith($"{HoneypotSuffixFieldName}");
        }

        internal string HoneypotGetFieldName(string name)
        {
            return $"{HoneypotPrefixFieldName}{name}";
        }

        public string RandomName()
        {
            return HoneypotFieldNames[new Random().Next(0, HoneypotFieldNames.Length)];
        }

    }
}
