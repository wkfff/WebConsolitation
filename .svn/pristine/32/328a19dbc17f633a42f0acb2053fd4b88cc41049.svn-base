using System.Xml.Schema;

using Ext.Net;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Presentation.Views;
using Krista.FM.RIA.Extensions.E86N.Services;
using Krista.FM.RIA.Extensions.E86N.Services.DocService;
using Krista.FM.RIA.Extensions.E86N.Services.OGSService;
using Krista.FM.RIA.Extensions.E86N.Services.ParameterDocService;
using Krista.FM.RIA.Extensions.E86N.Services.Params;
using Krista.FM.RIA.Extensions.E86N.Services.PassportServices;
using Krista.FM.RIA.Extensions.E86N.Services.PfhdService;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Services.SmetaService;
using Krista.FM.ServerLibrary;

using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Extensions.E86N
{
    public class E86NExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        public E86NExtensionInstaller()
            : base(typeof(E86NExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.E86N.Config.xml")
        {
        }

        #region IExtensionInstaller Members

        public string Identifier
        {
            get { return "Krista.FM.RIA.Extensions.E86N.E86NExtensionInstaller, Krista.FM.RIA.Extensions.E86N"; }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            navigationService.AddNavigation(base.ConfigureNavigation(parametersService));
            navigationService.AddNavigation(new ReportNavigation());
        }

        public override void RegisterTypes(IUnityContainer container)
        {
            base.RegisterTypes(container);

            container.RegisterType<IOGSService, OGSService>();
            container.RegisterType<ICommonDataService, CommonDataService>();
            container.RegisterType<IParameterDocService, ParameterDocService>();
            container.RegisterType<IPfhdService, PfhdService>();
            container.RegisterType<ISmetaService, SmetaService>();
            container.RegisterType<IDocService, DocService>();
            container.RegisterType<IPassportService, PassportService>();
            container.RegisterType<IOkvedyService, OkvedyService>();
            container.RegisterType<IFilialService, FilialService>();

            if (!container.IsRegistered<XmlSchemaSet>())
            {
                container.RegisterInstance(new XmlSchemaSet(), new ContainerControlledLifetimeManager());
            }
        }

        public override void ConfigureWindows(WindowService windowService)
        {
            var w = new Window
            {
                ID = "HBWnd",
                Title = string.Empty,
                Width = 600,
                Height = 400,
                Hidden = true,
                Modal = true
            };
            w.AutoLoad.ShowMask = true;
            w.AutoLoad.ReloadOnEvent = true;
            w.AutoLoad.TriggerEvent = "show";
            w.AutoLoad.Url = "/";
            w.AutoLoad.Mode = LoadMode.IFrame;
            w.AutoLoad.MaskMsg = @"Загрузка окна...";
            w.AddAfterClientInitScript(w.ID + ".MetaData = {};");

            windowService.AddWindow(w);
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            var extension = new AuthService(
                Resolver.Get<IScheme>(),
                Resolver.Get<IRepository<Users>>(),
                Resolver.Get<ILinqRepository<Memberships>>(),
                Resolver.Get<ILinqRepository<D_Org_UserProfile>>());
            extension.Initialize();

            Resolver.RegisterInstance<IAuthService>(extension, LifetimeManagerType.Session);
            
            var cacheRepositoryService = new CacheRepositoryService();
            Resolver.RegisterInstance<ICacheRepositoryService>(cacheRepositoryService, LifetimeManagerType.Session);

            Resolver.RegisterInstance<IParamsMap>(new ParamsMap(), LifetimeManagerType.Session);

            clientExtensionService.AddClientExtension(Resource.HandBooks);
        }

        #endregion
    }
}