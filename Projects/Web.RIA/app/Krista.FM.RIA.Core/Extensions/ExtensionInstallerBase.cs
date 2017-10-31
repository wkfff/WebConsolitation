using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.ExtensionModule;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Gui;
using Microsoft.Practices.Unity;
using ActionDescriptor = Krista.FM.RIA.Core.ViewModel.ActionDescriptor;

namespace Krista.FM.RIA.Core.Extensions
{
    public class ExtensionInstallerBase : IExtensionConfig
    {
        private readonly Assembly assembly;
        private readonly string configName;

        public ExtensionInstallerBase(Assembly assembly, string configName)
        {
            this.assembly = assembly;
            this.configName = configName;
        }

        public void RegisterParameters(IParametersService parametersService)
        {
            Stream stream = assembly.GetManifestResourceStream(configName);
            if (stream == null)
            {
                throw new InvalidOperationException("Не найден ресурс {0}.".FormatWith(configName));                
            }

            parametersService.RegisterExtensionConfigParameters(stream);
        }

        /// <summary>
        /// Установка маршрутизации.
        /// </summary>
        public virtual void InstallRoutes(RouteCollection routes)
        {
            XDocument doc = LoadXConfig();
            XElement e = doc.Root.Element("Routes");

            if (e != null)
            {
                // IgnoreRoute
                foreach (XElement ignoreRoute in e.Elements("IgnoreRoute"))
                {
                    routes.IgnoreRoute(ignoreRoute.Attribute("url").Value, ignoreRoute.Value);
                }

                // MapRoute
                foreach (XElement route in e.Elements("MapRoute"))
                {
                    RouteValueDictionary defaults = new RouteValueDictionary();
                    foreach (XElement param in route.Descendants("Param"))
                    {
                        defaults.Add(param.Attribute("name").Value, param.Value);
                    }

                    routes.MapRoute(
                        route.Attribute("name").Value,
                        route.Attribute("url").Value, 
                        defaults);
                }
            }
        }

        /// <summary>
        /// Регистрация типов в IoC контейнере.
        /// </summary>
        public virtual void RegisterTypes(IUnityContainer container)
        {
            XDocument doc = LoadXConfig();
            XElement e = doc.Root.Element("Types");

            if (e != null)
            {
                foreach (XElement type in e.Elements("Register"))
                {
                    LifetimeManager lifetimeManager = Activator.CreateInstance(
                        Type.GetType(type.Attribute("lifetimeManager").Value)) as LifetimeManager;

                    container.RegisterType(
                        Type.GetType(type.Attribute("from").Value),
                        Type.GetType(type.Attribute("to").Value),
                        null,
                        lifetimeManager,
                        new InjectionMember[0]);
                }
            }
        }

        /// <summary>
        /// Настройка области навигации.
        /// </summary>
        public virtual Navigation ConfigureNavigation(IParametersService parametersService)
        {
            XDocument doc = LoadXConfig();
            XElement e = doc.Root.Element("Navigation");

            if (e != null)
            {
                NavigationFactoty factory = new NavigationFactoty();
                Navigation navigation = factory.CreateNavigation(
                    e.Attribute("type").Value,
                    parametersService);

                navigation.Id = e.Attribute("ID").Value;
                navigation.Title = e.Attribute("title").Value;
                navigation.Group = e.Attribute("group").Value;
                navigation.Icon = (Icon)Enum.Parse(typeof(Icon), e.Attribute("icon").Value);
                navigation.DashboardIcon = e.Attribute("dashboardIcon").Value;
                navigation.DefaultItemId = e.Attribute("defaultItemId").Value;

                XElement itemsElement = e.Element("Items");
                if (itemsElement != null)
                {
                    navigation.Items = ConfigureNavigationItems(itemsElement, parametersService);
                }

                XElement commandsElement = e.Element("Commands");
                if (commandsElement != null)
                {
                    navigation.Commands = ConfigureNavigationCommands(commandsElement, parametersService);
                }

                return navigation;
            }

            return null;
        }

        public virtual void ConfigureViews(ViewService viewService, IParametersService parametersService)
        {
            XDocument doc = LoadXConfig();
            XElement xViews = doc.Root.Element("Views");

            if (xViews != null)
            {
                foreach (XElement xView in xViews.Elements("View"))
                {
                    if (new Condition(parametersService).Test(xView))
                    {
                        View view = new View();
                        view.Id = xView.Attribute("id").Value;
                        view.Title = xView.Attribute("title").Value;
                        if (xView.Attribute("url") != null)
                        {
                            view.Url = xView.Attribute("url").Value;
                        }

                        view.Type = Type.GetType(xView.Attribute("type").Value);

                        view.Config = xView.ToString();

                        viewService.AddView(view.Id, view);
                    }
                }
            }
        }

        public virtual void ConfigureWindows(WindowService windowService)
        {
        }

        public virtual void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
        }

        private List<NavigationItem> ConfigureNavigationItems(XElement itemsElement, IParametersService parametersService)
        {
            List<NavigationItem> list = new List<NavigationItem>();

            foreach (XElement item in itemsElement.Elements("Item"))
            {
                if (new Condition(parametersService).Test(item))
                {
                    NavigationItem navigationItem = new NavigationItem();
                    
                    // Основные свойства
                    navigationItem.ID = item.Attribute("ID").Value;
                    navigationItem.Title = item.Attribute("title").Value;
                    navigationItem.Icon = (Icon)Enum.Parse(
                        typeof(Icon), item.Attribute("icon").Value);

                    // Параметры
                    XElement parameters = item.Element("Params");
                    if (parameters != null)
                    {
                        foreach (XElement xParam in parameters.Elements("Param"))
                        {
                            navigationItem.Params.Add(new NavigationItemParameter
                            {
                                Name = xParam.Attribute("name").Value,
                                Value = xParam.Value
                            });
                        }
                    }

                    // Вложенные элементы
                    var childItems = item.Element("Items");
                    if (childItems != null)
                    {
                        navigationItem.Items = ConfigureNavigationItems(childItems, parametersService);
                    }

                    list.Add(navigationItem);
                }
            }

            return list;
        }

        private List<ActionDescriptor> ConfigureNavigationCommands(XElement commandsElement, IParametersService parametersService)
        {
            List<ActionDescriptor> list = new List<ActionDescriptor>();

            foreach (XElement xCommand in commandsElement.Elements("Command"))
            {
                if (new Condition(parametersService).Test(xCommand))
                {
                    CommandDescriptor command = new CommandDescriptor();
                    
                    // Основные свойства
                    XAttribute xAttribute = xCommand.Attribute("id");
                    command.Id = xAttribute.Value;

                    xAttribute = xCommand.Attribute("title");
                    if (xAttribute != null)
                    {
                        command.Title = xAttribute.Value;
                    }

                    command.Icon = (Icon)Enum.Parse(
                        typeof(Icon), xCommand.Attribute("icon").Value);

                    command.Handler = xCommand.Element("Handler").Value;

                    list.Add(command);
                }
            }

            return list;
        }

        private XDocument LoadXConfig()
        {
            try
            {
                Stream stream = assembly.GetManifestResourceStream(configName);
                return XDocument.Load(new XmlTextReader(stream));
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Ошибка при обработке конфигурационного файла расширения: {0}", configName), e);
            }
        }
    }
}