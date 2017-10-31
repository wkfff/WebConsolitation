using System.Collections.Generic;
using System.Web.Routing;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Tasks
{
    public class TasksExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        public TasksExtensionInstaller()
            : base(typeof(TasksExtensionInstaller).Assembly, 
            "Krista.FM.RIA.Extensions.Tasks.Config.xml")
        {
        }

        public string Identifier
        {
            get { return "Krista.FM.Client.ViewObjects.TasksUI.TasksNavigation, Krista.FM.Client.ViewObjects.TasksUI"; }
        }

        public override void InstallRoutes(RouteCollection routes)
        {
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            navigationService.AddNavigation(new NavigationList(parametersService)
                {
                    Id = "acTasks",
                    Title = "Задачи",
                    Icon = Icon.FolderWrench,
                    DashboardIcon = "/Krista.FM.RIA.Extensions.Tasks/Presentation/Content/images/task-72.png/extention.axd",
                    DefaultItemId = "mnTaskNav",
                    Group = "Сбор данных",
                    OrderPosition = 300,
                    Items = new List<NavigationItem>
                        {
                            new NavigationItem
                                {
                                    ID = "mnTaskNav",
                                    Title = "Список задач", 
                                    Icon = Icon.Report, 
                                    Link = "/TaskNav", 
                                    ToolTip = "Список задач"
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

        #region Члены IExtensionInstaller


        public override void ConfigureViews(ViewService viewService, IParametersService parametersService)
        {
        }

        #endregion
    }
}
