using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.OrgGKH
{
    public class OrgGKHExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        private OrgGKHExtension extension;

        public OrgGKHExtensionInstaller()
            : base(typeof(OrgGKHExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.OrgGKH.Config.xml")
        {
        }

        public string Identifier
        {
            get { return "Krista.FM.RIA.Extensions.OrgGKH.OrgGKHExtensionInstaller, Krista.FM.RIA.Extensions.OrgGKH"; }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            var navigation = base.ConfigureNavigation(parametersService);

            if (navigation != null)
            {
                if (extension.UserGroup == OrgGKHExtension.GroupMo || 
                    extension.UserGroup == OrgGKHExtension.GroupAudit)
                {
                    navigation.DefaultItemId = "nidbePlanProceeds";
                }
                else if (extension.UserGroup == OrgGKHExtension.GroupOrg)
                {
                    navigation.DefaultItemId = "nidbeMonthForm";
                }
                else if (extension.UserGroup == OrgGKHExtension.GroupIOGV)
                {
                    navigation.DefaultItemId = "nidbeMarksForm";
                }
                else
                {
                    navigation.DefaultItemId = string.Empty;
                }
            }

            navigationService.AddNavigation(navigation);
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            base.ConfigureClientExtension(clientExtensionService);

            extension = new OrgGKHExtension(
                Resolver.Get<IScheme>(),
                Resolver.Get<ILinqRepository<Users>>(),
                Resolver.Get<ILinqRepository<FX_Date_YearDayUNV>>(),
                Resolver.Get<ILinqRepository<DataSources>>(),
                Resolver.Get<ILinqRepository<D_Regions_Analysis>>(),
                Resolver.Get<ILinqRepository<D_Org_RegistrOrg>>());
            extension.Initialize();
            Resolver.RegisterInstance<IOrgGkhExtension>(extension, LifetimeManagerType.Session);
        }
    }
}
