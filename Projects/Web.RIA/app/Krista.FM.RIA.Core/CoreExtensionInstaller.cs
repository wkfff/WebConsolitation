using System.Web.Routing;

using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Params;

using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Core
{
    /// <summary>
    /// Отвечает за инициализацию (интеграцию) системного модуля (расширения).
    /// </summary>
    public class CoreExtensionInstaller : IExtensionInstaller
    {
        public string Identifier { get; private set; }

        public void InstallRoutes(RouteCollection routes)
        {
        }

        public void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<CurrentUserIdValueProvider, CurrentUserIdValueProvider>(new SessionLifetimeManager<CurrentUserIdValueProvider>());

            // ОКТМО и константы
            container.RegisterType<OktmoValueProvider, OktmoValueProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<OktmoUrengoyValueProvider, OktmoUrengoyValueProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<OktmoSakhalinValueProvider, OktmoSakhalinValueProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<OktmoHmaoValueProvider, OktmoHmaoValueProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<OktmoYaNAOValueProvider, OktmoYaNAOValueProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<OktmoOrenburgValueProvider, OktmoOrenburgValueProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<OktmoAltaiRegionValueProvider, OktmoAltaiRegionValueProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<OktmoKrasnodarValueProvider, OktmoKrasnodarValueProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<OktmoSaratovValueProvider, OktmoSaratovValueProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<OktmoSverdlovskValueProvider, OktmoSverdlovskValueProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<OktmoGubkinskyValueProvider, OktmoGubkinskyValueProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<OktmoTulaValueProvider, OktmoTulaValueProvider>(new ContainerControlledLifetimeManager());
        }

        public void RegisterParameters(IParametersService parametersService)
        {
            parametersService.RegisterExtensionConfigParameter("CurrentUserId", typeof(CurrentUserIdValueProvider));

            // ОКТМО и константы
            parametersService.RegisterExtensionConfigParameter("OKTMO", typeof(OktmoValueProvider));
            parametersService.RegisterExtensionConfigParameter("OKTMOUrengoy", typeof(OktmoUrengoyValueProvider));
            parametersService.RegisterExtensionConfigParameter("OKTMOSakhalin", typeof(OktmoSakhalinValueProvider));
            parametersService.RegisterExtensionConfigParameter("OKTMOHmao", typeof(OktmoHmaoValueProvider));
            parametersService.RegisterExtensionConfigParameter("OKTMOYaNAO", typeof(OktmoYaNAOValueProvider));
            parametersService.RegisterExtensionConfigParameter("OKTMOOrenburg", typeof(OktmoOrenburgValueProvider));
            parametersService.RegisterExtensionConfigParameter("OKTMOAltaiRegion", typeof(OktmoAltaiRegionValueProvider));
            parametersService.RegisterExtensionConfigParameter("OKTMOKrasnodar", typeof(OktmoKrasnodarValueProvider));
            parametersService.RegisterExtensionConfigParameter("OKTMOSaratov", typeof(OktmoSaratovValueProvider));
            parametersService.RegisterExtensionConfigParameter("OKTMOSverdlovsk", typeof(OktmoSverdlovskValueProvider));
            parametersService.RegisterExtensionConfigParameter("OKTMOGubkinsky", typeof(OktmoGubkinskyValueProvider));
            parametersService.RegisterExtensionConfigParameter("OKTMOTula", typeof(OktmoTulaValueProvider));
        }

        public void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            var extension = Resolver.Get<CoreExtension>();
            extension.Initialize();
            Resolver.RegisterInstance(extension, LifetimeManagerType.Session);
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
        }

        public void ConfigureWindows(WindowService windowService)
        {
        }

        public void ConfigureViews(ViewService viewService, IParametersService parametersService)
        {
        }
    }
}
