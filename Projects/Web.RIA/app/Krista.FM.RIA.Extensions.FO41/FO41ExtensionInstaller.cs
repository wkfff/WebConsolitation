using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtensionModule.Services;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO41
{
    public class FO41ExtensionInstaller : ExtensionInstallerBase, IExtensionInstaller
    {
        private readonly string winReqList = "_RL";
        private readonly string winReqToEstimateList = "_REL";

        private FO41Extension extension;

        public FO41ExtensionInstaller()
            : base(typeof(FO41ExtensionInstaller).Assembly, "Krista.FM.RIA.Extensions.FO41.Config.xml")
        {
        }

        public string Identifier
        {
            get { return "Krista.FM.RIA.Extensions.FO41.FO41ExtensionInstaller, Krista.FM.RIA.Extensions.FO41"; }
        }

        public void ConfigureNavigation(NavigationService navigationService, IParametersService parametersService)
        {
            // для пользователя в роли ДФ по умолчанию открывается интерфейс список заявок на оценку
            var navigation = base.ConfigureNavigation(parametersService);
            if (navigation != null)
            {
                switch (extension.UserGroup)
                {
                    case FO41Extension.GroupDF:
                        {
                            if (extension.OKTMO.Equals(FO41Extension.OKTMOYar))
                            {
                                navigation.DefaultItemId = "nidbeEstimate";
                            }

                            break;
                        }

                    case FO41Extension.GroupOGV:
                        {
                            break;
                        }

                    case FO41Extension.GroupTaxpayer:
                        {
                            navigation.DefaultItemId = extension.OKTMO.Equals(FO41Extension.OKTMOHMAO) 
                                ? "nidbeRequestListFO41HMAO"
                                : "nidbeRequestListForTaxPayer";

                            break;
                        }
                }

                navigation.Action = extension.OKTMO.Equals(FO41Extension.OKTMOHMAO)
                    ? @"
if (menuItem.id == 'nidbeFO41TaxOnProfit')
    ChoosePeriodWindow4.show();
else if (menuItem.id == 'nidbeFO4PropertyTax')
    ChoosePeriodWindow9.show();
else if (menuItem.id == 'nidbeFO41MotorVehicleTax')
    ChoosePeriodWindow11.show();
else
    /*MdiTab.addTab({ 
        title: menuItem.text, 
        url: menuItem.url, 
        icon: menuItem.iconCls, 
        passParentSize: menuItem.passParentSize
    });*/
    ChoosePeriodWindow.show();"
                    : @"
if (menuItem.id == 'nidbeRequestList' || menuItem.id == 'nidbeRequestListForTaxPayer'){{
    var win = Ext.getCmp('ChoosePeriodWindow{0}');
    win.show();
}}
else
if (menuItem.id == 'nidbeEstimate'){{
    var win = Ext.getCmp('ChoosePeriodWindow{1}');
    win.show();
}}
else
    MdiTab.addTab({{ 
        title: menuItem.text, 
        url: menuItem.url, 
        icon: menuItem.iconCls, 
        passParentSize: menuItem.passParentSize
    }});".FormatWith(winReqList, winReqToEstimateList);
            }

            navigationService.AddNavigation(navigation);
        }

        public override void ConfigureClientExtension(ClientExtensionService clientExtensionService)
        {
            base.ConfigureClientExtension(clientExtensionService);

            extension = new FO41Extension(
                Resolver.Get<IScheme>(),
                Resolver.Get<IRepository<Users>>(),
                Resolver.Get<IRepository<FX_Date_YearDayUNV>>(),
                Resolver.Get<ILinqRepository<DataSources>>(),
                Resolver.Get<ILinqRepository<D_OMSU_ResponsOIV>>(),
                Resolver.Get<ILinqRepository<D_Org_Privilege>>());
            extension.Initialize();
            Resolver.RegisterInstance<IFO41Extension>(extension, LifetimeManagerType.Session);
        }

        public override void ConfigureWindows(WindowService windowService)
        {
            if (extension.OKTMO.Equals(FO41Extension.OKTMOHMAO))
            {
                windowService.AddWindow(GetChoosePeriodWindow(4, "/FO41HMAORequests/ShowTaxView", string.Empty));
                windowService.AddWindow(GetChoosePeriodWindow(9, "/FO41HMAORequests/ShowTaxView", string.Empty));
                windowService.AddWindow(GetChoosePeriodWindow(11, "/FO41HMAORequests/ShowTaxView", string.Empty));
                windowService.AddWindow(GetChoosePeriodWindow(-1, "/FO41HMAORequests/ShowReqListForTaxPayerView", string.Empty));
            }
            else
            {
                if (extension.UserGroup != FO41Extension.GroupDF)
                {
                    windowService.AddWindow(GetChoosePeriodWindow(-1, "/FO41Requests/ShowRequestsListView", winReqList));
                }

                if (extension.UserGroup != FO41Extension.GroupTaxpayer)
                {
                    windowService.AddWindow(GetChoosePeriodWindow(-1, "/FO41Requests/ShowReqToEstimateListView", winReqToEstimateList));
                }
            }
        }

        /// <summary>
        /// Создание модального окна для выбора периода перед открытием интерфейса "По типу налога"
        /// </summary>
        /// <param name="taxId">Идентификатор типа налога или -1 (для Ярославля)</param>
        /// <param name="urlToGo">Url для перехода</param>
        /// <param name="winIdEnds">Заключительная часть идентификатора окна</param>
        /// <returns>Модальное окно</returns>
        private Window GetChoosePeriodWindow(int taxId, string urlToGo, string winIdEnds)
        {
            var url = extension.OKTMO == FO41Extension.OKTMOHMAO
                          ? "/FO41HMAORequests/ShowPeriodView?taxId={0}".FormatWith(taxId)
                          : "/FO41Requests/ShowPeriodView";

            var win = new Window
            {
                ID = "ChoosePeriodWindow{0}".FormatWith(taxId > -1 ? taxId.ToString() : winIdEnds),
                Width = 400,
                Height = 200,
                Title = @"Выбор периода",
                AutoLoad = { Url = url },
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
            win.AutoLoad.Params.Add(new Parameter("id", string.Empty, ParameterMode.Value));

            var tabTitleForHMAO = taxId == -1 
                ? "Создание и просмотр заявок" 
                : (taxId == 4 ? "Налог на прибыль организаций" : (taxId == 9 ? "Налог на имущество организаций" : "Транспортный налог"));
            var nextHandlerHMAO = @"
{0}.hide();
var periodId = Extension.PeriodView.periodId;
var notYear = periodId % 10000;
var year = (periodId - notYear) / 10000;
var periodText = (notYear  == 9991) 
    ? '1 квартал ' + year + ' года' 
    : ((notYear  == 9992) 
        ? '2 квартал ' + year + ' года'  
        : ((notYear  == 9993) 
            ? '3 квартал ' + year + ' года'  
            : (notYear  == 9994) 
                ? '4 квартал ' + year + ' года'  
                : year + ' год'))
btnOk{1}.ownerCt.ownerCt.iframe.dom.contentWindow.parent.parent.MdiTab.addTab({{
    title: '{2} ' + periodText, 
    url: '{3}?taxId={1}&periodId=' + Extension.PeriodView.periodId
}});".FormatWith(win.ID, taxId > -1 ? taxId.ToString() : string.Empty, tabTitleForHMAO, urlToGo);

            var tabTitle = winIdEnds.Equals(winReqList) 
                ? extension.UserGroup == FO41Extension.GroupTaxpayer ? "Создание и просмотр заявок" : "Список заявок от налогоплательщиков"  
                : "Список заявок на оценку";
            var nextHandlerYar = @"
{0}.hide();
var periodId = Extension.PeriodView.periodId;
var notYear = periodId % 10000;
var year = (periodId - notYear) / 10000;
var periodText = year + ' год';
btnOk{3}.ownerCt.ownerCt.iframe.dom.contentWindow.parent.parent.MdiTab.addTab({{
    title: '{1} ' + periodText, 
    url: '{2}?periodId=' + Extension.PeriodView.periodId
}});".FormatWith(win.ID, tabTitle, urlToGo, winIdEnds);
            var buttonSave = new Button
            {
                ID = "btnOk{0}".FormatWith(taxId > -1 ? taxId.ToString() : winIdEnds),
                Text = @"Далее",
                Icon = Icon.Accept,
                Listeners =
                {
                    Click =
                    {
                        Handler = extension.OKTMO == FO41Extension.OKTMOHMAO ? nextHandlerHMAO : nextHandlerYar
                    }
                }
            };

            var buttonCancel = new Button
            {
                ID = "btnCancel{0}".FormatWith(taxId > -1 ? taxId.ToString() : winIdEnds),
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
        periodId: 0
    }
};
");

            return win;
        }
    }
}
