using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Umbraco.Forms.Core;

namespace Our.Umbraco.Honeypot
{
    public class HoneypotFieldType : FieldType
    {
        public HoneypotFieldType()
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
        }

        private HoneypotOptions Options => DependencyResolver.Current.GetService<HoneypotOptions>();

        public override string GetDesignView()
        {
            return "~/App_Plugins/Our.Umbraco.Honeypot/FieldTypes/Honeypot.html";
        }

        public override IEnumerable<string> ValidateField(Form form, Field field, IEnumerable<object> postedValues,
            HttpContextBase context)
        {
            var returnStrings = new List<string>();

            if (context.ApplicationInstance.Context.IsHoneypotTrapped())
            {
                returnStrings.Add(Options.HoneypotMessage);
            }

            return returnStrings;
        }
    }
}