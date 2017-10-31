using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.EO10MissivePRF
{
    public class EO10ExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        private EO10Extension extension;

        public EO10ExtensionInstaller()
            : base(typeof(EO10ExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.EO10MissivePRF.Config.xml")
        {
        }

        public string Identifier
        {
            get { return "Krista.FM.RIA.Extensions.EO10MissivePRF.EO10ExtensionInstaller, Krista.FM.RIA.Extensions.EO10MissivePRF"; }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            var navigation = base.ConfigureNavigation(parametersService);
            navigationService.AddNavigation(navigation);
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            base.ConfigureClientExtension(clientExtensionService);

            extension = new EO10Extension(
                Resolver.Get<IScheme>(), 
                Resolver.Get<IRepository<Users>>(), 
                Resolver.Get<IRepository<D_MissivePRF_Execut>>(),
                Resolver.Get<IRepository<DataSources>>());
            extension.Initialize();
            Resolver.RegisterInstance<IEO10Extension>(extension, LifetimeManagerType.Session);
        }
    }
}
