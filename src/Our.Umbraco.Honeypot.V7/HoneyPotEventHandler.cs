using System.Configuration;
using Autofac;
using Our.Umbraco.IoC.Autofac;
using Umbraco.Core;

namespace Our.Umbraco.Honeypot
{
    public class HoneyPotEventHandler : IApplicationEventHandler
    {
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            if (ConfigurationManager.AppSettings["Our.Umbraco.IoC.Autofac.Enabled"] == null || !(ConfigurationManager.AppSettings["Our.Umbraco.IoC.Autofac.Enabled"] != "true"))
            {
                AutofacStartup.ContainerBuilding += (sender, args) =>
                {
                    _ = args.Builder.RegisterType<HoneypotOptions>().InstancePerLifetimeScope();
                    _ = args.Builder.RegisterType<HoneypotService>().InstancePerRequest();
                };
            }
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }
    }
}
