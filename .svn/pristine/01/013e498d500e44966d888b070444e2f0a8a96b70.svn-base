using System.Collections.Generic;
using System.Web;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Extensions.Messages.Services;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Extensions.Messages
{
    public class MessagesExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        internal const string AdminIdentifier = "Krista.FM.RIA.Extensions.Messages.MessagesExtensionInstaller, Krista.FM.RIA.Extensions.Messages";

        private MessagesExtension extension;

        public MessagesExtensionInstaller()
            : base(typeof(MessagesExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.Messages.Config.xml")
        {
        }

        public string Identifier
        {
            get
            {
                return
                    "Krista.FM.Client.ViewObjects.MessagesUI.MessageManagementNavigation, Krista.FM.Client.ViewObjects.MessagesUI";
            }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            Resolver.Get<IPrincipalProvider>().SetBasePrincipal();
            var user = (BasePrincipal)HttpContext.Current.User;
            if (!user.IsInAnyRoles(new[] { "krista.ru", "Администраторы", }))
            {
                return;
            }

            var navigationItem = new NavigationItem
                {
                    ID = "niAllMessages",
                    Title = "Все сообщения",
                    Icon = Icon.Email,
                    Link = "/View/MessagesForm",
                    ToolTip = "Список сообщений"
                };

            var navigationList = new NavigationList(parametersService)
                {
                    Id = "acMessages",
                    Title = "Сообщения",
                    Icon = Icon.EmailStar,
                    DashboardIcon =
                        "/Krista.FM.RIA.Extensions.Informator/Presentation/Content/Images/news.png/extention.axd",
                    DefaultItemId = "niAllMessages",
                    Group = "Сообщения",
                    OrderPosition = 310,
                    Items = new List<NavigationItem>
                        {
                            navigationItem
                        }
                };

            navigationService.AddNavigation(navigationList);
        }

        public override void RegisterTypes(IUnityContainer container)
        {
            base.RegisterTypes(container);

            container.RegisterType<IMessageService, MessageService>(new ContainerControlledLifetimeManager());
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            base.ConfigureClientExtension(clientExtensionService);

            extension = Resolver.Get<MessagesExtension>();
            Resolver.RegisterInstance<IMessageExtension>(extension, LifetimeManagerType.Session);

            if (!extension.Initialize())
            {
                var message = new Notification().Configure(new NotificationConfig
                {
                    Title = "Инициализация блока сообщения",
                    Html = "Во время инициализации произошли ошибки.",
                    HideDelay = 5000
                });

                clientExtensionService.AddClientExtension(message.ToScript());
            }

            var count = extension.GetNewMessagesCount();
            if (count > 0)
            {
                var notificationConfig = new NotificationConfig
                    {
                        Title = "Сообщения",
                        AutoHide = true,
                        Height = 150,
                        HideDelay = 5000
                    };

                notificationConfig.Listeners.Render.Fn = @"function(item){{
    var p = new Ext.Panel({{
                    border: false,
                    padding: 2,
                    height: 113,
                    width: 180,
                    html: 'У вас есть непрочитанные сообщения. Количество - {0}. Для перехода в интерфейс Сообщения нажмите Открыть.',
                    AutoDataBind:true,
                    bbar:
                    [
                        new Ext.Button({{
                            tooltip: 'Открыть интерфейс Сообщения',
                            iconCls: 'icon-email',
                            text: 'Открыть',
                            handler:function(){{ 
                                var tab = MdiTab.getComponent('190130775');
                                if (!tab) 
                                {{
                                    MdiTab.addTab({{
                                        id: '190130775',
                                        title: 'Все сообщения', 
                                        url: '/View/MessagesForm',
                                        passParentSize: false,
                                        icon: 'icon-email'
                                    }});
                                }}
                                else
                                {{
                                    MdiTab.setActiveTab(tab);
                                }}
                            }}
                    }})]
                }});     
                item.add(p);
            }}".FormatWith(count);

                var message = new Notification().Configure(notificationConfig);
                
                clientExtensionService.AddClientExtension(message.ToScript());
            }
        }
    }
}
