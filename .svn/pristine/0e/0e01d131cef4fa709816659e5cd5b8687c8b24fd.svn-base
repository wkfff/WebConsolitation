using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Common;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Controllers.ViewModels;
using Krista.FM.RIA.Core.ExtensionModule;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Core.Controllers
{
    [HandleError]
    [Authorize]
    [ControllerSessionState(ControllerSessionState.Default)]
    public class HomeController : SchemeBoundController
    {
        private readonly NavigationService navigationService;
        private readonly IParametersService parametersService;

        public HomeController(NavigationService navigationService, IParametersService parametersService)
        {
            this.navigationService = navigationService;
            this.parametersService = parametersService;
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult Index()
        {
            // Инициализируем системный модуль
            var coreExtensionInstaller = Resolver.Get<CoreExtensionInstaller>(typeof(CoreExtensionInstaller).Name);
            coreExtensionInstaller.ConfigureClientExtension(null);

            // Добавляем системный модуль первым в список разрешонных модулей
            List<IExtensionInstaller> allowedExtentions = new List<IExtensionInstaller> { coreExtensionInstaller };

            // Получаем все доступные инсталляторы пользовательских модулей
            var extentions = Resolver.GetAll<IExtensionInstaller>();

            // Получаем инсталляторы модулей доступные текущему пользователю
            string[] allowedObjects = Resolver.Get<CoreExtension>().AllowedObjects;
            foreach (var extensionInstaller in extentions)
            {
                if (allowedObjects.Contains(extensionInstaller.Identifier))
                {
                    allowedExtentions.Add(extensionInstaller);
                }
            }

            // Регистрируем параметры расширений
            parametersService.Clear();
            foreach (var extensionInstaller in allowedExtentions)
            {
                extensionInstaller.RegisterParameters(parametersService);
            }
            
            return View(new HomeIndexViewModel
            {
                AppName = Scheme.Server.GetConfigurationParameter("ProductName"),
                ////AppNameWithVersion = "<b>{0}</b> (v{1})".FormatWith(Scheme.Server.GetConfigurationParameter("ProductName"), Scheme.Server.GetConfigurationParameter("AssemblyBaseVersion")),
                AppNameWithVersion = "<b>{0}</b> (v{1})".FormatWith(
                                                            Scheme.Server.GetConfigurationParameter("ProductName"), 
                                                            AppVersionControl.GetAssemblyBaseVersion(Scheme.UsersManager.ServerLibraryVersion())),
                UserName = HttpContext.User.Identity.Name,
                ServerName = "{0}:{1}".FormatWith(
                    Scheme.Server.Machine, 
                    Scheme.Server.GetConfigurationParameter("ServerPort")),
                SchemeName = Scheme.Name,
                AllowedExtentions = allowedExtentions,
                ParametersService = parametersService
            });
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            List<string> items = new List<string>();

            items.Add("test");

            this.ViewData["Data"] = items;

            return this.View();
        }

        public AjaxStoreResult GetHomeSchema()
        {
            var groups = new Dictionary<string, List<Navigation>>();
            foreach (var navigation in navigationService.GetNavigations())
            {
                if (!groups.ContainsKey(navigation.Group))
                {
                    groups.Add(navigation.Group, new List<Navigation>());
                }

                groups[navigation.Group].Add(navigation);
            }

            var query = from g in groups
                    select new 
                    {
                        Title = g.Key,
                        Items = from i in g.Value
                                 select new 
                                 {
                                     Title = i.Title,
                                     Icon = i.DashboardIcon,
                                     Accordion = i.Id,
                                     MenuItem = i.DefaultItemId,
                                     Action = i.Action ?? i.DefaultAction
                                 }
                    };

            return new AjaxStoreResult(query);
        }
    }
}
