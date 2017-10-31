using System;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO51PassportMO
{
    public class FO51ExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        private FO51Extension extension;

        public FO51ExtensionInstaller()
            : base(typeof(FO51ExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.FO51PassportMO.Config.xml")
        {
        }

        public string Identifier
        {
            get { return "Krista.FM.RIA.Extensions.FO51PassportMO.FO51ExtensionInstaller, Krista.FM.RIA.Extensions.FO51PassportMO"; }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            var navigation = base.ConfigureNavigation(parametersService);
            if (navigation != null)
            {
                switch (extension.UserGroup)
                {
                    case FO51Extension.GroupMo:
                        navigation.DefaultItemId = "nidbeEditPassport";
                        break;
                    case FO51Extension.GroupOGV:
                        navigation.DefaultItemId = "nidbeCheckPassport";
                        break;
                    default:
                        navigation.DefaultItemId = String.Empty;
                        break;
                }

                navigation.Action = @"
                    if (menuItem.id == 'nidbeEditPassport'){
                        var win = Ext.getCmp('FO51ChoosePeriodWindow');
                        win.show();
                    }
                    else
                        MdiTab.addTab({
                            title: menuItem.text, 
                            url: menuItem.url, 
                            icon: menuItem.iconCls, 
                            passParentSize: menuItem.passParentSize
                        });";
            }

            navigationService.AddNavigation(navigation);
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            base.ConfigureClientExtension(clientExtensionService);

            extension = new FO51Extension(
                Resolver.Get<IScheme>(),
                Resolver.Get<ILinqRepository<Users>>(),
                Resolver.Get<ILinqRepository<D_OMSU_ResponsOIV>>(),
                Resolver.Get<ILinqRepository<D_Marks_PassportMO>>(),
                Resolver.Get<ILinqRepository<DataSources>>(),
                Resolver.Get<ILinqRepository<D_Regions_Analysis>>());
            extension.Initialize();
            Resolver.RegisterInstance<IFO51Extension>(extension, LifetimeManagerType.Session);
        }

        public override void ConfigureWindows(WindowService windowService)
        {
            windowService.AddWindow(GetChoosePeriodWindow());
        }

        /// <summary>
        /// Создание модального окна для выбора периода
        /// </summary>
        /// <returns>Модальное окно</returns>
        private Window GetChoosePeriodWindow()
        {
            var win = new Window
            {
                ID = "FO51ChoosePeriodWindow",
                Width = 400,
                Height = 200,
                Title = @"Выбор периода",
                AutoLoad = { Url = "/FO51Periods/ShowPeriodView" },
                Icon = Icon.ApplicationFormEdit,
                Hidden = true,
                Modal = true,
                Constrain = true
            };

            win.AutoLoad.Mode = LoadMode.IFrame;
            win.AutoLoad.TriggerEvent = "show";
            win.AutoLoad.ReloadOnEvent = true;
            win.AutoLoad.ShowMask = true;
            win.AutoLoad.MaskMsg = @"Открытие формы с выбором периода...";
            win.AutoLoad.Params.Add(new Parameter("id", String.Empty, ParameterMode.Value));

            var nextHandler = @"
var year = Extension.PeriodView.year;
if (year > 0) {{
    {0}.hide();
    var periodText = year + ' год';
    FO51btnOk.ownerCt.ownerCt.iframe.dom.contentWindow.parent.parent.MdiTab.addTab({{
        title: 'Сбор данных с МО по антикризисному паспорту ' + periodText, 
        url: '/FO51FormSbor/ShowPassportMo?year=' + year
    }});
}}".FormatWith(win.ID);
            var buttonSave = new Button
            {
                ID = "FO51btnOk",
                Text = @"Далее",
                Icon = Icon.Accept,
                Listeners =
                {
                    Click =
                    {
                        Handler = nextHandler
                    }
                }
            };

            var buttonCancel = new Button
            {
                ID = "FO51btnCancel",
                Text = @"Отмена",
                Icon = Icon.Cancel,
                Listeners =
                {
                    Click =
                    {
                        Handler = @"{0}.hide()".FormatWith(win.ID)
                    }
                }
            };

            win.Buttons.Add(buttonCancel);
            win.Buttons.Add(buttonSave);

            win.AddScript(@"
Extension = {
    PeriodView: {
        year: 0
    }
};
");

            return win;
        }
    }
}
