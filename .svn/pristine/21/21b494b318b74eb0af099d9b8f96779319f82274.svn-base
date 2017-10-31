using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.MinSport
{
    public class MinSportExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        public MinSportExtensionInstaller()
            : base(typeof(MinSportExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.MinSport.Config.xml")
        {
        }
        
        public string Identifier
        {
            get { return "Krista.FM.RIA.Extensions.MinSport.ExtensionInstaller, Krista.FM.RIA.Extensions.MinSport"; }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
             navigationService.AddNavigation(base.ConfigureNavigation(parametersService));    
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            base.ConfigureClientExtension(clientExtensionService);

            var extension = new MinSportExtension(Resolver.Get<IScheme>(), Resolver.Get<IRepository<Users>>());
            extension.Initialize();

            Resolver.RegisterInstance<IMinSportExtension>(extension, LifetimeManagerType.Session);
        }
    }
}
