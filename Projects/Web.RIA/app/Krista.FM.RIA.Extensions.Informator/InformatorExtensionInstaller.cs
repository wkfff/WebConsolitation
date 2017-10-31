using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Extensions.Informator.Presentation.Views;
using Krista.FM.ServerLibrary;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Extensions.Informator
{
    public class InformatorExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        public InformatorExtensionInstaller()
            : base(typeof(InformatorExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.Informator.Config.xml")
        {
        }

        public string Identifier
        {
            get { return "Krista.FM.RIA.Extensions.E86N.E86NExtensionInstaller, Krista.FM.RIA.Extensions.E86N"; }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            navigationService.AddNavigation(base.ConfigureNavigation(parametersService));
        }

        public override void RegisterTypes(IUnityContainer container)
        {
            base.RegisterTypes(container);

            container.RegisterType<INewsService, NewsService>();
            ////container.RegisterType<INewsAttachmentsCleanerManager, NewsAttachmentsCleanerManager>();
        }

        public override void ConfigureWindows(WindowService windowService)
        {
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            clientExtensionService.AddClientExtension(Resource.NewsWindow);
            var extension = Resolver.Get<InformatorExtension>();
            Resolver.RegisterInstance<IInformatorExtension>(extension, LifetimeManagerType.Session);

            clientExtensionService.AddClientExtension(Resource.NewsWindow);

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

            var count = extension.NewMessagesCount;
            if (count > 0)
            {
                var notificationConfig = new NotificationConfig
                    {
                        Title = "Новости",
                        AutoHide = true,
                        CloseVisible = true,
                        Height = 150,
                        HideDelay = 5000
                    };
                notificationConfig.Listeners.Render.Fn = @"function(item){{
    var p = new Ext.Panel({{
                    border: false,
                    padding: 2,
                    height: 113,
                    width: 180,
                    html: 'У вас есть непрочитанные новости. Количество - {0}. Для перехода в интерфейс Новости нажмите Открыть.',
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
                                        title: 'Доступные новости', 
                                        url: '/View/News',
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