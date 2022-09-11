using Our.Umbraco.Honeypot.Core;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Forms.Mvc;
using Umbraco.Forms.Web.Controllers;
using Current = Umbraco.Web.Composing.Current;

namespace Our.Umbraco.Honeypot.V8
{
    public class HoneypotComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            UmbracoFormsController.FormValidate += FormsController_FormValidate;
        }

        private void FormsController_FormValidate(object sender, FormValidationEventArgs e)
        {
            HoneypotOptions options = Current.Factory.GetInstance<HoneypotOptions>();

            if (!options.HoneypotEnableFieldCheck && !options.HoneypotEnableTimeCheck)
            {
                return;
            }

            var controller = sender as UmbracoFormsController;

            if (e.Context.IsHoneypotTrapped())
            {
                controller?.ModelState.AddModelError("error", options.HoneypotMessage);
            }
        }
    }
}