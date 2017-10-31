<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Krista.FM.RIA.Core" %>
<%@ Import Namespace="Krista.FM.RIA.Core.Controllers.ViewModels" %>
<%@ Import Namespace="Krista.FM.RIA.Core.ExtensionModule" %>
<%@ Import Namespace="Krista.FM.RIA.Core.ExtensionModule.Services" %>

<%@ Register assembly="Ext.Net" namespace="Ext.Net" tagprefix="ext" %>

<%@ Register TagPrefix="a" Namespace="Ext.Net" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <style type="text/css">
        .icon-mark-active   { background-image: url(/Content/images/icon-mark-active.gif) !important; }
        .label
        {
        	font: 11px arial,tahoma,helvetica,sans-serif;
        	display: block;
        }
        .greenlabel 
        {
        	font: 11px arial,tahoma,helvetica,sans-serif;
        	background: #98FB98;
        	display: block;
        }
        .toolbar-label .x-label-value
        {
        	font-weight: bold;
        	color: #15428B;
        }
        div#DebtBookStatePanel .x-toolbar, div#DebtBookVariantPanel .x-toolbar
        {
        	background-image: url("/extjs/resources/images/default/panel/white-top-bottom-gif/ext.axd");
        }
        body
        {
            background-color: #5c87b2;
            margin: 0;
            padding: 0;
        }
    </style>
    <title><%= ((HomeIndexViewModel) Model).SchemeName %> - <%= ((HomeIndexViewModel) Model).AppName %></title>
    <script runat="server">
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var extentions = ((HomeIndexViewModel)Model).AllowedExtentions;
            var parametersService = ((HomeIndexViewModel)Model).ParametersService;

            var clientExtensionService = Resolver.Get<ClientExtensionService>();
            clientExtensionService.Clear();

#if DEBUG
            if (!MvcApplication.NHibernateInitialized)
            {
                // Ждем окончания инициализации NHibernate
                // TODO: Обработать ошибки инициализации NHibernate
                MvcApplication.NHibernateInitializedEvent.WaitOne();
            }
#endif

            foreach (var extensionInstaller in extentions)
            {
                extensionInstaller.ConfigureClientExtension(clientExtensionService);
            }

            var windowService = Resolver.Get<WindowService>();
            windowService.Clear();
            foreach (var extensionInstaller in extentions)
            {
                extensionInstaller.ConfigureWindows(windowService);
            }

            var navigationService = Resolver.Get<NavigationService>();
            navigationService.Clear();
            foreach (var extensionInstaller in extentions)
            {
                extensionInstaller.ConfigureNavigation(navigationService, parametersService);
            }

            var viewService = Resolver.Get<ViewService>();
            viewService.Clear();
            foreach (var extensionInstaller in extentions)
            {
                extensionInstaller.ConfigureViews(viewService, parametersService);
            }
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            resourceManager.RegisterScript("App.Utils", "/Content/js/App.Utils.js");
            resourceManager.RegisterScript("Workbench", "/Content/js/Workbench.js");
            resourceManager.RegisterScript("MdiTab", "/Content/js/MdiTab.js");
            resourceManager.RegisterScript("App.FxProgressManager", "/Content/js/App.FxProgressManager.js");
            resourceManager.RegisterScript("LocalStorageProvider", "/Content/js/LocalStorageProvider.js");
            
            Page.DataBind();
            
            InitializeClientExtensions();
            InitializeNavigations();
            InitializeWindows();

            // Для IE запрещаем сворачивать навигационную область, т.к. потом она не разворачивается
            panelWest.Collapsible = !RequestManager.IsIE;
            borderLayoutMain.West.Collapsible = !RequestManager.IsIE;

#if DEBUG
            if (RequestManager.IsIE)
            {
                taskManager.Tasks[0].AutoRun = false;
            }
#endif
        }

        /// <summary>
        /// Инициализация модулей расширений.
        /// </summary>
        private void InitializeClientExtensions()
        {
            var clientExtensionService = Resolver.Get<ClientExtensionService>();
            foreach (var clientExtension in clientExtensionService.GetClientExtensions())
            {
                ResourceManager.GetInstance(HttpContext.Current).RegisterOnReadyScript(clientExtension);
            }
        }
        
        /// <summary>
        /// Инициализация окон модулей расширений.
        /// </summary>
        private void InitializeWindows()
        {
            var windowService = Resolver.Get<WindowService>();
            foreach (var window in windowService.GetWindows())
            {
                Controls.Add(window);
            }
        }
        
        /// <summary>
        /// Форминование навигационного списка.
        /// </summary>
        private void InitializeNavigations()
        {
            var navigationService = Resolver.Get<NavigationService>();
            foreach (Navigation navigation in navigationService.GetNavigations())
            {
                panelWest.Items.Add(navigation.Build(this));
            }
        }

        [DirectMethod]
        public string GetThemeUrl(string theme)
        {
            var temp = (Theme)Enum.Parse(typeof(Theme), theme);

            Session["Ext.Net.Theme"] = temp;

            return (temp == Ext.Net.Theme.Default) ? "Default" : resourceManager.GetThemeUrl(temp);
        }
    </script>
    <ext:ResourcePlaceHolder ID="ResourcePlaceHolder1" runat="server" />
    <script src="/Content/js/SlidingTabs.js" type="text/javascript"></script>
     <script type="text/javascript">
         if (window.localStorage) {
             Ext.state.Manager.setProvider(
                 new Ext.ux.state.LocalStorageProvider({ prefix: 'ls' , userName: '<%# User.Identity.Name.ToLower() %>' }));
         } else {
             var thirtyDays = new Date(new Date().getTime() + (1000 * 60 * 60 * 24 * 30));
             Ext.state.Manager.setProvider(new Ext.state.CookieProvider({ expires: thirtyDays }));
         }
     </script>
</head>

<body runat="server">
    <form id="form1" runat="server">
    <ext:ResourceManager ID="resourceManager" runat="server" EnableTheming="true" DisableViewState="true">
        <Listeners>
            <DocumentReady Handler="#{panelWest}.expand();" />
        </Listeners>
    </ext:ResourceManager>
    <ext:Viewport ID="vpMain" runat="server">
        <Content>
            <ext:BorderLayout ID="borderLayoutMain" runat="server">
                <North Margins-Bottom="4">
                    <ext:Panel ID="Panel1" runat="server" AutoHeight="true" Header="false" Border="false" Title="Title">
                        <Content>
                            <ext:Toolbar ID="tbHeader" runat="server">
                                <Items>
                                    <ext:Label ID="lblTitle" runat="server" Html='<%# ((HomeIndexViewModel) Model).AppNameWithVersion %>' Icon="Eye"/>
                                    <ext:ToolbarFill ID="fill1" runat="server" />
                                    <ext:Button ID="btnUser" runat="server" Icon="StatusOnline" Text='<%# ((HomeIndexViewModel) Model).UserName %>' AutoDataBind="true">
                                        <Menu>
                                            <ext:Menu ID="menu1" runat="server">
                                                <Items>
                                                    <ext:MenuItem ID="menuProfile" Text="Профиль" Icon="Magnifier" Enabled="false" />
                                                    <ext:MenuItem ID="menuOptions" Text="Настройки" Icon="Wrench">
                                                        <Menu>
                                                            <ext:Menu ID="menu3" runat="server">
                                                                <Items>
                                                                    <ext:MenuItem ID="menuTheme" Text="Темы оформления...">
                                                                        <Menu>
                                                                            <ext:Menu ID="menu4" runat="server">
                                                                                <Items>
                                                                                    <ext:MenuItem ID="menuDefault" Text="Голубая">
                                                                                        <CustomConfig>
                                                                                            <ext:ConfigItem Name="theme" Value="Default" Mode="Value"></ext:ConfigItem>
                                                                                        </CustomConfig>
                                                                                    </ext:MenuItem>
                                                                                    <ext:MenuItem ID="MenuItem1" Text="Серая">
                                                                                        <CustomConfig>
                                                                                            <ext:ConfigItem Name="theme" Value="Gray" Mode="Value"></ext:ConfigItem>
                                                                                        </CustomConfig>
                                                                                    </ext:MenuItem>
                                                                                    <ext:MenuItem ID="MenuItem2" Text="Синевато-серая">
                                                                                        <CustomConfig>
                                                                                            <ext:ConfigItem Name="theme" Value="Slate" Mode="Value"></ext:ConfigItem>
                                                                                        </CustomConfig>
                                                                                    </ext:MenuItem>
                                                                                    <ext:MenuItem ID="MenuItem3" Text="Проникновение">
                                                                                        <CustomConfig>
                                                                                            <ext:ConfigItem Name="theme" Value="Access" Mode="Value"></ext:ConfigItem>
                                                                                        </CustomConfig>
                                                                                    </ext:MenuItem>
                                                                                </Items>
                                                                                <Listeners>
                                                                                    <ItemClick Handler="Ext.net.DirectMethods.GetThemeUrl(menuItem.theme,{
                                                                                                            success: function (result) {
                                                                                                                Ext.net.ResourceMgr.setTheme(result);
                                                                                                                tpMain.items.each(function (el) {
                                                                                                                   if (!Ext.isEmpty(el.iframe)) {
                                                                                                                        el.iframe.dom.contentWindow.Ext.net.ResourceMgr.setTheme(result);
                                                                                                                    }
                                                                                                                });
                                                                                                            }
                                                                                                        });"/>
                                                                                </Listeners>
                                                                            </ext:Menu>
                                                                        </Menu>
                                                                    </ext:MenuItem>
                                                                     <ext:MenuItem ID="MenuItemRemoveLocalStorage" Text="Очистить пользовательские настройки">
                                                                        <Listeners>
                                                                            <Click Handler="Ext.Msg.confirm('', 'Вы действительно хотите очистить пользовательские настройки?', function(btn) {
				                                                                if(btn == 'yes'){
                                                                                    var prov = Ext.state.Manager.getProvider();
                                                                                    if(prov) {
                                                                                        prov.removeAll();
                                                                                    }
                                                                                }
                                                                                })" >
                                                                            </Click>
                                                                        </Listeners>
                                                                    </ext:MenuItem>
                                                                </Items>
                                                            </ext:Menu>
                                                        </Menu>
                                                    </ext:MenuItem>
                                                </Items>
                                            </ext:Menu>
                                        </Menu>
                                    </ext:Button>
                                    <ext:Button ID="btnLogout" runat="server" Icon="LockGo" Text="Выйти">
                                        <DirectEvents>
                                            <Click Url="/Account/LogOff/" Success="vpMain.hide();">
                                                <Confirmation Message="Вы действительно хотите выйти?" ConfirmRequest="true" Title="Выход" />
                                                <EventMask ShowMask="true" Msg="Выход из системы..." />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:ToolbarSeparator ID="ToolbarSeparator1" runat="server" />
                                    <ext:Button ID="btnHelp" runat="server" Icon="Help" Text="Справка">
                                        <Menu>
                                            <ext:Menu ID="menu2" runat="server">
                                                <Items>
                                                    <ext:MenuItem ID="menuReport" Text="Сообщить об ошибке" Icon="Bug">
                                                        <Listeners>
                                                            <Click Handler="Northwind.addTab({ title: el.text, url: '/Home/Form/', icon: el.iconCls });" />
                                                        </Listeners>
                                                    </ext:MenuItem>
                                                    <ext:MenuItem ID="menuAbout" Text="О системе" Icon="Information">
                                                        <Listeners>
                                                            <Click Handler="#{winAbout}.show();" />
                                                        </Listeners>
                                                    </ext:MenuItem>
                                                </Items>
                                            </ext:Menu>
                                        </Menu>
                                    </ext:Button>
                                    <ext:Button ID="btnFeedback" runat="server" Text="Обратная связь" Icon="Email">
                                        <DirectEvents>
                                            <Click Url="/Feedback/CheckSmtpConnection"
                                                Method="GET"
                                                Success="
                                                    var wnd = Ext.getCmp('HBWnd');
                                                    if (!wnd)
                                                        Ext.MessageBox.alert('Ошибка', 'Не найдено окно: HBWnd', 1);
                                                    wnd.autoLoad.url = 'View/FeedbackView';
                                                    wnd.setTitle('Отправить письмо в техподдержку');
                                                    wnd.show();"
                                                Failure="
                                                    window.open('mailto:' + result.extraParams.redirectTo);"
                                            ></Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </Content>
                    </ext:Panel>
                </North>
                <West Collapsible="true" Split="true">
                    <ext:Panel runat="server" Title="Панель навигации" Width="200" ID="panelWest" Collapsed="true" Collapsible="true" Stateful="False">
                        <LayoutConfig>
                            <ext:AccordionLayoutConfig OriginalHeader="true" Animate="true" CollapseFirst="true"/>
                        </LayoutConfig>
                    </ext:Panel>
                </West>
                <Center>
                    <ext:Panel runat="server" Title="Center" Header="false" ID="ctl342">
                        <Content>
                            <ext:FitLayout runat="server">
                                <Items>
                                    <ext:TabPanel ID="tpMain" runat="server" ActiveTabIndex="0" Border="false" Title="Center" EnableTabScroll="true">
                                        <Items>
                                            <ext:Panel runat="server" Title="Главная">
                                                <AutoLoad Url="/Home/Dashboard/" Mode="IFrame" ShowMask="true" MaskMsg="Loading 'Dashboard'..." />
                                            </ext:Panel>
                                        </Items>
                                        <Plugins>
                                            <ext:TabCloseMenu ID="TabCloseMenu1" runat="server" />
                                            <ext:TabScrollerMenu runat="server"/>
                                            <ext:GenericPlugin InstanceName="Ext.ux.SlidingTabs" />
                                        </Plugins>
                                    </ext:TabPanel>
                                </Items>
                            </ext:FitLayout>
                        </Content>
                    </ext:Panel>
                </Center>
                <South>
                    <ext:Panel ID="southPanel" runat="server" AutoHeight="true" Header="false" Border="false" Title="Title">
                        <Content>
                            <ext:StatusBar 
                                ID="StatusBar1" 
                                runat="server"
                                DefaultText="">
                                <Items>
                                    <ext:ToolbarSeparator ID="ToolbarSeparator3" runat="server" />
                                    <ext:ToolbarTextItem ID="tbiServer" runat="server" Text='<%# ((HomeIndexViewModel) Model).ServerName %>' AutoDataBind="true"/>
                                    <ext:ToolbarSeparator ID="ToolbarSeparator2" runat="server" />
                                    <ext:ToolbarTextItem ID="tbiScheme" runat="server" Text='<%# ((HomeIndexViewModel) Model).SchemeName %>' AutoDataBind="true"/>
                                </Items>
                            </ext:StatusBar>
                        </Content>
                    </ext:Panel>
                </South>
            </ext:BorderLayout>
        </Content>
    </ext:Viewport>
    </form>
    <ext:TaskManager ID="taskManager" runat="server">
        <Tasks>
            <ext:Task TaskID="keepAliveTask" Interval="250000" AutoRun="true">
                <Listeners>
                    <Update
                        Handler="
Ext.net.DirectEvent.request({
    method: 'POST',
    url: '/Session/KeepAlive/',
    userFailure: function(response, result, el, type, action, extraParams){
        Ext.Msg.show({
            title:'Ошибка',
            msg: result.errorMessage == 'NORESPONSE' ? 'Потеряно соединение с веб-сервером, попробуйте подключиться позже.' : result.errorMessage,
            buttons: Ext.Msg.OK,
            icon: Ext.Msg.ERROR,
            closable: false,
            fn: function() {
                window.location = window.location.protocol + '//' + window.location.host + '/Account/LogOn/';
            }
        });
    },
    userSuccess: function(response, result, el, type, action, extraParams){
        lblTitle.el.frame();
    }
    ,control:vpMain});
                    " />
                </Listeners>
            </ext:Task>
            <ext:Task AutoRun="true" Interval="180000" TaskID="updateNewMessageCount">
                <Listeners>
                    <Update
                        Handler="
Ext.net.DirectEvent.request({
    method: 'GET',
    url: '/Message/NewMessageCount/',
    userFailure: function(response, result, el, type, action, extraParams){
        },
    userSuccess: function(response, result, el, type, action, extraParams){
        if (parent.acMessages) {
            if (result.extraParamsResponse.messageCount != 0){
                parent.acMessages.setTitle('Сообщения (' + result.extraParamsResponse.messageCount + ')');
                parent.acMessages.setIconClass('icon-emailstar');
             }
              else {
                parent.acMessages.setTitle('Сообщения');
                parent.acMessages.setIconClass('icon-email');
                        }
        }
    },
    control:vpMain
    });
                    " />
                </Listeners>
            </ext:Task>
        </Tasks>
    </ext:TaskManager>
</body>
</html>
