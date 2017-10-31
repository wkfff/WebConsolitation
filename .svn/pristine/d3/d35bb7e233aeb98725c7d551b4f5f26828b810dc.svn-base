using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;
using Krista.FM.ServerLibrary;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP
{
    public class EO15ExcCostsAIPExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        private EO15ExcCostsAIPExtension extension;

        public EO15ExcCostsAIPExtensionInstaller()
            : base(typeof(EO15ExcCostsAIPExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Config.xml")
        {
        }

        public string Identifier
        {
            get { return "Krista.FM.RIA.Extensions.EO15ExcCostsAIP.EO15ExcCostsAIPExtensionInstaller, Krista.FM.RIA.Extensions.EO15ExcCostsAIP"; }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            var navigation = base.ConfigureNavigation(parametersService);
            navigationService.AddNavigation(navigation);
        }

        public override void RegisterTypes(IUnityContainer container)
        {
            base.RegisterTypes(container);

            container.RegisterType<IConstructionService, ConstructionService>();
            container.RegisterType<IClientService, ClientService>();
        }
        
        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            base.ConfigureClientExtension(clientExtensionService);

            extension = new EO15ExcCostsAIPExtension(
                Resolver.Get<IScheme>(), 
                Resolver.Get<IClientService>(), 
                Resolver.Get<IRepository<Users>>(),
                Resolver.Get<IRepository<DataSources>>());

            extension.Initialize();
            Resolver.RegisterInstance<IEO15ExcCostsAIPExtension>(extension, LifetimeManagerType.Session);
        }
    }
}
