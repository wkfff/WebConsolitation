using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Principal;

namespace Krista.FM.RIA.Extensions.Entity
{
    public class EntityExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        public EntityExtensionInstaller()
            : base(typeof (EntityExtensionInstaller).Assembly,
                   "Krista.FM.RIA.Extensions.Entity.Config.xml")
        {
        }

        #region IExtensionInstaller Members

        public string Identifier
        {
            get
            {
                return
                    "Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation.EntityNavigationControl, Krista.FM.Client.ViewObjects.AssociatedCLSUI";
            }
        }

        public override void InstallRoutes(RouteCollection routes)
        {
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            Resolver.Get<IPrincipalProvider>().SetBasePrincipal();
            var user = (BasePrincipal) HttpContext.Current.User;
            if (!user.IsInAnyRoles(new[] {"krista.ru", "Администраторы",}))
                return;

            navigationService.AddNavigation(
                new NavigationList(parametersService)
                    {
                        Id = "acEntity",
                        Title = "Справочники и таблицы",
                        Icon = Icon.FolderGo,
                        Group = "Прочее",
                        OrderPosition = 800,
                        DashboardIcon = "/Content/images/icon.png",
                        Items = new List<NavigationItem>
                                    {
                                        new NavigationItem
                                            {
                                                ID = "",
                                                Title = "Фиксированные классификаторы",
                                                Icon = Icon.Report,
                                                Link = "/EntityNav/Index?classType=2",
                                                ToolTip = "Фиксированные классификаторы"
                                            },
                                        new NavigationItem
                                            {
                                                Title = "Сопоставимые классификаторы",
                                                Icon = Icon.Report,
                                                Link = "/EntityNav/Index?classType=0",
                                                ToolTip = "Сопоставимые классификаторы"
                                            },
                                        new NavigationItem
                                            {
                                                Title = "Классификаторы данных",
                                                Icon = Icon.Report,
                                                Link = "/EntityNav/Index?classType=1",
                                                ToolTip = "Классификаторы данных"
                                            },
                                        new NavigationItem
                                            {
                                                Title = "Таблицы фактов",
                                                Icon = Icon.Report,
                                                Link = "/EntityNav/Index?classType=3",
                                                ToolTip = "Таблицы фактов"
                                            }
                                    }
                    });
        }

        public override void ConfigureWindows(WindowService windowService)
        {
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
        }

        #endregion

        #region Члены IExtensionInstaller

        public override void ConfigureViews(ViewService viewService, IParametersService parametersService)
        {
        }

        #endregion
    }
}