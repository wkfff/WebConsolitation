using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class ForecastForma2pExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        public ForecastForma2pExtensionInstaller()
            : base(typeof(ForecastForma2pExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.Forecast.Forma2p.Config.xml")
        {
        }
        
        public string Identifier
        {
            get { return "Krista.FM.Client.ViewObjects.ForecastUI.ForecastNavigation, Krista.FM.Client.ViewObjects.ForecastUI"; }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            navigationService.AddNavigation(base.ConfigureNavigation(parametersService));
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            base.ConfigureClientExtension(clientExtensionService);

            var extension = new ForecastExtension(Resolver.Get<IScheme>(), Resolver.Get<IRepository<Users>>());
            extension.Initialize();

            Resolver.RegisterInstance<IForecastExtension>(extension, LifetimeManagerType.Session);
        }
    }
}
