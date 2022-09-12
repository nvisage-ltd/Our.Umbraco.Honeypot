using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Our.Umbraco.Honeypot.Core;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;

namespace Our.Umbraco.Honeypot
{
    public class HoneypotFieldType : global::Umbraco.Forms.Core.FieldType
    {
        public HoneypotFieldType(IOptions<HoneypotOptions> options)
        {
            Id = new Guid("efa3f7a1-b603-4060-b416-6449f1a029db");
            Category = "Spam";
            Name = "𝖍𝖔𝖓𝖊𝖞𝖕𝖔𝖙";
            Description = "This will render hidden fields to trap bots";
            Icon = "icon-handprint";
            DataType = FieldDataType.Integer;
            SortOrder = 10;
            FieldTypeViewName = "FieldType.Honeypot.cshtml";
            HideLabel = true;

            Options = options.Value;
        }

        private HoneypotOptions Options { get; }

        public override string GetDesignView()
        {
            return "~/App_Plugins/Our.Umbraco.Honeypot/FieldTypes/Honeypot.html";
        }

        public override IEnumerable<string> ValidateField(Form form, Field field, IEnumerable<object> postedValues, HttpContext context, IPlaceholderParsingService placeholderParsingService)
        {
            var returnStrings = new List<string>();

            if (context.IsHoneypotTrapped())
            {
                returnStrings.Add(Options.HoneypotMessage);
            }

            return returnStrings;
        }
    }
}
