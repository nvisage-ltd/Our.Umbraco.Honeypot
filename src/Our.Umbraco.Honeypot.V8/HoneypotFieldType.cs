﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Our.Umbraco.Honeypot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;

namespace Our.Umbraco.Honeypot
{
    public class HoneypotFieldType : global::Umbraco.Forms.Core.FieldType
    {
        public HoneypotFieldType()
        {
            this.Id = new Guid("efa3f7a1-b603-4060-b416-6449f1a029db");
            this.Category = "Spam";
            this.Name = "𝖍𝖔𝖓𝖊𝖞𝖕𝖔𝖙";
            this.Description = "This will render hidden fields to trap bots";
            this.Icon = "icon-handprint";
            this.DataType = FieldDataType.Integer;
            this.SortOrder = 10;
            this.FieldTypeViewName = "FieldType.Honeypot.cshtml";
            this.HideLabel = true;
        }

        private HoneypotOptions Options => global::Umbraco.Web.Composing.Current.Factory.GetInstance<HoneypotOptions>();

        public override string GetDesignView()
        {
            return "~/App_Plugins/Our.Umbraco.Honeypot/FieldTypes/Honeypot.html";
        }

        public override IEnumerable<string> ValidateField(Form form, Field field, IEnumerable<object> postedValues, HttpContextBase context, IFormStorage formStorage)
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
