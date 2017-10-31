using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Servises;
using Krista.FM.ServerLibrary;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas 
{
    public class InvestAreasExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        public InvestAreasExtensionInstaller() 
            : base(typeof(InvestAreasExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.EO14InvestAreas.Config.xml")
        {
        }

        public string Identifier
        {
            get { return "Krista.FM.RIA.Extensions.EO14InvestAreas.InvestAreasExtensionInstaller, Krista.FM.RIA.Extensions.EO14InvestAreas"; }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            navigationService.AddNavigation(base.ConfigureNavigation(parametersService));
        }

        public override void RegisterTypes(IUnityContainer container)
        {
            base.RegisterTypes(container);
            container.RegisterType<IAreaService, AreaService>();
            container.RegisterType<IFilesService, FilesService>();
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            base.ConfigureClientExtension(clientExtensionService);

            var extension = new UserCredentials(
                Resolver.Get<IScheme>(),
                Resolver.Get<IRepository<Users>>(),
                Resolver.Get<ILinqRepository<Memberships>>());
            extension.Initialize();

            Resolver.RegisterInstance<IUserCredentials>(extension, LifetimeManagerType.Session);
        }
    }
}
