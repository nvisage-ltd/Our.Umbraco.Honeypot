using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services.Notifications;

namespace Our.Umbraco.Honeypot
{
    public class HoneypotComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            _ = builder.WithCollectionBuilder<FieldCollectionBuilder>().Add<HoneypotFieldType>();
            _ = builder.AddNotificationHandler<FormValidateNotification, HoneypotValidationNotificationHandler>();
        }
    }
}
