using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Control = System.Web.UI.Control;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Views
{
    public class EditPassportMOView : View
    {
        private readonly IFO51Extension extension;

        private readonly ILinqRepository<D_Regions_Analysis> regionRepository;

        private readonly ILinqRepository<D_Marks_PassportMO> marksPassportRepository;

        private readonly int yearToShow;

        /// <summary>
        /// Скрипт - диалоговое окно с вопросом о изменении состояния заявки.
        /// </summary>
        private const string AskChangeState =
            @"
            Ext.Msg.show({{
                title: 'Анализ и планирование',
                msg: '{0}',
                buttons: Ext.MessageBox.YESNO,
                multiline: false,
                animEl: 'ApplicPanel',
                icon: Ext.MessageBox.WARNING,
                fn: function (btn) {{
                    if (btn == 'yes') {{
                        {1}
                    }}
                }}
            }});";

        private string inEditState;
        private string inOGVState;
        private string isOGV;

        public EditPassportMOView(
            IFO51Extension extension,
            ILinqRepository<D_Regions_Analysis> regionRepository,
            ILinqRepository<D_Marks_PassportMO> marksPassportRepository,
            int yearToShow)
        {
            this.extension = extension;
            this.regionRepository = regionRepository;
            this.marksPassportRepository = marksPassportRepository;
            var month = DateTime.Today.Month;
            this.yearToShow = yearToShow;
            isOGV = extension.UserGroup == FO51Extension.GroupOGV ? "true" : "false";
            PeriodId = month > 1
                           ? (yearToShow * 10000) + ((month - 1) * 100)
                           : ((yearToShow - 1) * 10000) + 1200;
        }

        protected D_Regions_Analysis Region { get; set; }

        protected int PeriodId { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            if (extension.UserGroup == FO51Extension.GroupOther)
            {
                ResourceManager.GetInstance(page).RegisterOnReadyScript(
                    ExtNet.Msg.Alert("Ошибка", "Текущему пользователю не сопоставлен МО или ОГВ.").
                        ToScript());

                return new List<Component>();
            }

            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterClientScriptBlock("Fo51ToolBar", Resources.Resource.Fo51ToolBar);

            return CreateForm(page);
        }

        /// <summary>
        /// Создает Store для периодов по месяцам.
        /// </summary>
        /// <returns>Store для периодов.</returns>
        private Store CreatePeriodsMonthStore()
        {
            var ds = new Store
            {
                ID = "periodsMonthStore",
                AutoLoad = true,
            };

            ds.SetHttpProxy("/FO51Periods/LookupMonthPeriod?year={0}".FormatWith(yearToShow))
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name");

            return ds;
        }

        private List<Component> CreateForm(ViewPage page)
        {
            try
            {
                if ((Region == null && extension.UserGroup == FO51Extension.GroupOGV) ||
                    (extension.User.RefRegion == null && extension.UserGroup == FO51Extension.GroupMo))
                {
                    throw new Exception("Текущему пользователю не сопоставлен район.");
                }

                if (extension.UserGroup == FO51Extension.GroupMo)
                {
                    Region = regionRepository.FindOne(extension.User.RefRegion.Value);
                }

                if (Region == null)
                {
                    throw new Exception("Район с идентификатором {0} не найден.".FormatWith(extension.User.RefRegion.Value));
                }

                var year = PeriodId / 10000;
                var source = extension.DataSourcesFO51.FirstOrDefault(x => x.Year.Equals(year.ToString()));
                if (source == null)
                {
                    throw new Exception("Источник ФО 51 Финансовый паспорт МО на {0} год не найден. Обратитесь к администратору"
                                             .FormatWith(year));
                }

                const string StyleAll = " padding-left: 5px; font-size: 12px;";
                var storeMonth = CreatePeriodsMonthStore();
                page.Controls.Add(storeMonth);

                StringBuilder reloadHandler;
                string setStateForStoresHandler;
                StringBuilder saveHandler;
                StringBuilder disableEditHandler;
                StringBuilder enableEditHandler;
                var tabs = GetTabs(
                    page, 
                    source, 
                    out reloadHandler, 
                    out setStateForStoresHandler, 
                    out saveHandler, 
                    out disableEditHandler, 
                    out enableEditHandler);

                ResourceManager.GetInstance().AddScript(@"
                reloadHandler = function() {{
                    {0}
                }};
                saveHandler = function() {{
                    {1}
                }};".FormatWith(reloadHandler, saveHandler));

                var monthOn = ((PeriodId / 100) % 100) + 1;
                var yearOn = year;
                if (monthOn > 12)
                {
                    monthOn = 1;
                    yearOn = year + 1;
                }

                var dateOn = "{0}.{1}.{2}".FormatWith("01", monthOn, yearOn);
                var periodCombo = GetPeriodCombo(storeMonth, reloadHandler.ToString());
                var headerPanel = GetHeaderPanel(setStateForStoresHandler, StyleAll, periodCombo, dateOn);
                var mainPanel = GetMainPanel(tabs, headerPanel, StyleAll);
                var topToolbar = GetTopToolbar(saveHandler.ToString(), reloadHandler.ToString(), Region, PeriodId);
                return new List<Component>
                           {
                               new Viewport
                                   {
                                       ID = "viewportMeasures",
                                       AutoScroll = true,
                                       CssClass = "x-window-mc",
                                       StyleSpec = " padding-top: 5px;",
                                       Items =
                                           {
                                               new BorderLayout
                                                   {
                                                       Center = { Items = { mainPanel } },
                                                       North = { Items = { topToolbar } }
                                                   }
                                           }
                                   }
                           };
            }
            catch (Exception e)
            {
                ResourceManager.GetInstance(page)
                    .RegisterOnReadyScript(ExtNet.Msg.Alert("Ошибка", e.Message).ToScript());

                return new List<Component>();
            }
        }

        private TabPanel GetTabs(
            ViewPage page, 
            DataSources source,
            out StringBuilder reloadHandler, 
            out string setStateForStores, 
            out StringBuilder saveHandler,
            out StringBuilder disableEditHandler,
            out StringBuilder enableEditHandler)
        {
            var tabs = new TabPanel
                       {
                           ID = "PassportTabPanel",
                           Border = false,
                           LabelWidth = 300
                       };

            var marksTop = marksPassportRepository.FindAll()
                .Where(x => x.ParentID == null && x.RefTypeMark.ID == 1 && x.SourceID == source.ID).OrderBy(x => x.Code);

            saveHandler = new StringBuilder(@"var dataState = 1;");
            var storeState = new StringBuilder();
            disableEditHandler = new StringBuilder();
            enableEditHandler = new StringBuilder();
            reloadHandler = new StringBuilder(@"
                var activeTab = PassportTabPanel.activeTab.id;");

            var showMarksCopyBtns = new StringBuilder();
            var hideMarksCopyBtns = new StringBuilder();

            var tabList = new List<FO51FormSborView>();
            foreach (var mark in marksTop)
            {
                var month = (PeriodId / 100) % 100;
                var hidden = month != 1 && mark.Name.Equals("Справочно");

                var tab = new FO51FormSborView(mark, PeriodId, Region, extension.UserGroup);
                tabList.Add(tab);

                saveHandler.Append("{0}.store.save(); {0}.store.commitChanges();".FormatWith(tab.GridId));
                storeState.Append(@"
                    {0}.store.dataState = response.responseText.substring(li, fi);".FormatWith(tab.GridId));

                if (mark.Name.Equals("Доходы") || mark.Name.Equals("Источники финансирования дефицита бюджета") ||
                             mark.Name.Equals("Расходы"))
                {
                    if (month != 1)
                    {
                        showMarksCopyBtns.Append("{0}CopyPrevEstimate.show();".FormatWith(tab.GridId));
                        hideMarksCopyBtns.Append("{0}CopyPrevEstimate.hide();".FormatWith(tab.GridId));
                    }
                }
                
                disableEditHandler.Append(@"
                    for (var col in {0}.colModel.columns)
                        {0}.colModel.setEditable(col, false);    
                    ".FormatWith(tab.GridId));
                enableEditHandler.Append(@"
                    for (var col in {0}.colModel.columns)
                        if (col != 0)
                            {0}.colModel.setEditable(col, true);    
                    ".FormatWith(tab.GridId));

                tabs.Add(tab.Build(page));

                reloadHandler.Append(@"
                    CheckHidden_{1}({0}); 
                    {0}.store.load();
                    var columns{0} = {0}.colModel.columns; 
                    {0}.reconfigureColumns(columns{0});
                    ".FormatWith(tabs.Items.Last().ID, mark.ID));

                if (hidden)
                {
                    tabs.Items.Last().Disabled = true;
                }
            }

            if (extension.UserGroup == FO51Extension.GroupMo)
            {
                inEditState = "SaveBtn.show();SendDFBtn.show();" + showMarksCopyBtns;
                inOGVState = "SaveBtn.hide(); SendDFBtn.hide();" + hideMarksCopyBtns;
            }
            else if (Region.ID < 0)
            {
                inEditState = "SaveBtn.hide();SendDFBtn.hide();" + hideMarksCopyBtns;
                inOGVState = "SaveBtn.hide(); SendDFBtn.hide();" + hideMarksCopyBtns;
            }
            else
            {
                inEditState = "SaveBtn.hide();SendDFBtn.hide();" + hideMarksCopyBtns;
                inOGVState = "SaveBtn.show(); SendDFBtn.hide();" + showMarksCopyBtns;
            }

            setStateForStores = GetStateForStoresScript(storeState.ToString(), disableEditHandler.ToString(), enableEditHandler.ToString());

            reloadHandler.Append(@"
                PassportTabPanel.setActiveTab(activeTab);");
            reloadHandler.Insert(0, setStateForStores);

            if (tabList.FirstOrDefault() != null)
            {
                tabs.Listeners.AfterRender.Handler = setStateForStores;
                tabs.ActiveTabIndex = 0;
            }

            return tabs;
        }

        private Panel GetHeaderPanel(string setStateForStores, string styleAll, Component periodCombo, string dateOn)
        {
            return new Panel
            {
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false,
                Height = extension.UserGroup == FO51Extension.GroupMo ? 150 : 130,
                Items =
                    {
                        new DisplayField
                            {
                                StyleSpec = "padding-top: 10px;  padding-bottom: 10px; font-size: 14px; font-weight: bold;",
                                Text = @"Показатели для формирования антикризисного паспорта МО"
                            },
                        new DisplayField
                            {
                                StyleSpec = styleAll,
                                LabelWidth = 120,
                                FieldLabel = @"Наименование МО",
                                Text = "{0}".FormatWith(Region.Name)
                            },
                        periodCombo,
                        new DisplayField
                            {
                                ID = "DateOn",
                                StyleSpec = styleAll,
                                LabelWidth = 120,
                                FieldLabel = @"Данные на",
                                Text = "{0}".FormatWith(dateOn)
                            },

                        new DisplayField
                            {
                                ID = "StateField",
                                StyleSpec = styleAll,
                                LabelWidth = 120,
                                FieldLabel = @"Состояние данных",
                                Text = String.Empty,
                                Listeners = { BeforeShow = { Handler = setStateForStores } }
                            },
                        new DisplayField
                            {
                                ID = "InfoField",
                                StyleSpec = styleAll + " color: red;",
                                Text = String.Empty
                            }
                    }
            };
        }

        private Toolbar GetTopToolbar(string saveHandler, string reloadHandler, D_Regions_Analysis region, int periodId)
        {
            var changeStateHandler = AskChangeState.FormatWith(
                @"Вы действительно хотите отправить форму на рассмотрение?",
                "changeStateReloadHandler({0}, 2, 'Отправка на рассмотрение', 'Cверка с ежемесячной отчетностью...', {1});".FormatWith(Region.ID, isOGV));

            var toEditHandler = AskChangeState.FormatWith(
                @"Вы действительно хотите отправить форму на доработку МО?",
                "changeStateReloadHandler({0}, 1, 'Отправка на доработку МО', 'Выполняется', {1});".FormatWith(Region.ID, isOGV));

            var acceptHandler = AskChangeState.FormatWith(
                @"Вы действительно хотите утвердить форму?",
                "changeStateReloadHandler({0}, 3, 'Утверждение', 'Выполняется', {1});".FormatWith(Region.ID, isOGV));
            
            var reportUrl = extension.UserGroup == FO51Extension.GroupMo ? extension.GetReportForRegionsUrl() : extension.GetSvodReportUrl();

            var checkHandler =
                @"  
        parent.MdiTab.addTab({{ 
            title: 'Контроль {0} (' + {1} + '.' + {2} + ')', 
            url: '/FO51FormSbor/Control?periodId=' + {3} + '&regionId={4}', 
            icon: 'icon-report'
        }});".FormatWith(region.Name, (periodId / 100) % 100, periodId / 10000, periodId, region.ID);
            return new Toolbar
            {
                Height = 30,
                Items =
                    {
                        new Button
                            {
                                ID = "RefreshBtn",
                                ToolTip = @"Обновить",
                                Icon = Icon.TableRefresh,
                                Listeners = { Click = { Handler = reloadHandler } }
                            },

                        new Button
                            {
                                ID = "SaveBtn",
                                ToolTip = @"Сохранить",
                                Icon = Icon.TableSave,
                                Hidden = true,
                                Listeners = { Click = { Handler = saveHandler } }
                            },

                        new Button
                            {
                                ID = "SendDFBtn",
                                Text = @"Отправить на рассмотрение",
                                Icon = Icon.Tick,
                                Hidden = true,
                                Listeners = { Click = { Handler = changeStateHandler } }
                            },
                        new Button
                            {
                                ID = "SendEditBtn",
                                Text = @"Отправить на доработку МО",
                                Icon = Icon.PageBack,
                                Hidden = true,
                                Listeners = { Click = { Handler = toEditHandler } }
                            },
                        new Button
                            {
                                ID = "AcceptBtn",
                                Text = @"Утвердить",
                                Icon = Icon.Accept,
                                Hidden = true,
                                Listeners = { Click = { Handler = acceptHandler } }
                            },
                       new Button
                            {
                                ToolTip = @"Проверка на соответствие занесенных сумм ежемесячному отчету", 
                                Text = @"Контроль", Icon = Icon.Calculator,
                                Hidden = extension.UserGroup != FO51Extension.GroupOGV || region.ID < 0,
                                Listeners = { Click = { Handler = checkHandler } }
                            },
                        new ToolbarSeparator(),
                        new HyperLink 
                        {
                            ID = "LinkToSiteReport",
                            Text = @"Сводный отчет",
                            NavigateUrl = reportUrl,
                            Hidden = reportUrl == null,
                            Target = "_blank"
                        }
                    }
            };
        }

        private Panel GetMainPanel(Component tabs, Component header, string styleAll)
        {
            return new Panel
            {
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false,
                StyleSpec = styleAll,
                Items =
                    {
                        new BorderLayout
                            {
                                North = { Items = { header } },
                                Center = { Items = { tabs } }
                            }
                    }
            };
        }

        private ComboBox GetPeriodCombo(Control storeMonth, string reloadHandler)
        {
            return new ComboBox
            {
                EmptyText = @"период",
                StoreID = storeMonth.ID,
                StyleSpec = "pagging-left: 5px",
                ID = "cbPeriodMonth",
                AllowBlank = false,
                TriggerAction = TriggerAction.All,
                ValueField = "ID",
                Width = 400,
                Disabled = false,
                DisplayField = "Name",
                FieldLabel = @"Отчетный период",
                LabelWidth = 120,
                Value = PeriodId,
                Hidden = extension.UserGroup != FO51Extension.GroupMo,
                Listeners =
                    {
                        Select =
                            {
                                Handler = 
                                    @"
                                        var month = (cbPeriodMonth.value / 100) % 100 + 1;
                                        var year = (cbPeriodMonth.value - cbPeriodMonth.value % 10000) / 10000;
                                        if (month > 12) {{
                                            month = 1;
                                            year = year + 1;
                                        }}    
                                        var monthText = '' + month;
                                        if (month < 10)
                                            monthText = '0' + month;
                                        DateOn.setValue('01.' + monthText + '.' + year); DateOn.show(); 
                                        {0}
                                        ".FormatWith(reloadHandler)
                            }
                    }
            };
        }

        private string GetStateForStoresScript(string storeState, string disableEditHandler, string enableEditHandler)
        {
            var isFictRegion = Region.RefBridgeRegions == null
                                   ? "false"
                                   : (FO51Extension.RegionFictID == Region.RefBridgeRegions.ID ? "true" : "false");
            return @"
            var curState = 1;
            Ext.Ajax.request({{
                url: '/FO51FormSbor/GetState?periodId=' + cbPeriodMonth.value + '&regionId={0}',
                timeout: 600000,
                success: function (response, options) {{
                        var fi = response.responseText.indexOf('message:') + 9;
                        var li = response.responseText.lastIndexOf('{2}');
                        var stateId = response.responseText.substring(li, fi);
                        updateButtons{7}(stateId);
                        if (stateId == 1) {{
                            StateField.setValue('Данные находятся на редактировании у МО');
                            {3}
                            InfoField.setValue();
                            // TODO РАСКОММЕНТИРОВАТЬ!!!
                            /*if (!{8} && !{9}) {{
                                InfoField.setValue('Проверка наличия данных по месячному отчету...');
                                // проверить, есть ли данные по ежемесячному отчету
                                Ext.Ajax.request({{
                                    url: '/FO51FormSbor/ReportDataExists?periodId=' + cbPeriodMonth.value + '&regionId={0}',
                                    success: function (response, options) {{
                                            var fi = response.responseText.indexOf('message:') + 9;
                                            var li = response.responseText.lastIndexOf('{2}');
                                            if (response.responseText.substring(li, fi) == 1) {{
                                                {5}
                                                InfoField.setValue();
                                            }}
                                            else {{
                                                {4}
                                                {6}
                                                InfoField.setValue('Ввод данных возможен только после заполнения ежемесячного отчета');
                                                Ext.net.Notification.show({{
                                                    iconCls    : 'icon-information', 
                                                    html       : 'Ежемесячный отчет об исполнении бюджета муниципального образования за выбранный период не заполнен. Ввод данных возможен только после заполнения отчета.', 
                                                    title      : 'Внимание', 
                                                    height     : 130,
                                                    hideDelay  : 10000
                                                }});
                                            }}
                                    }},
                                    failure: function (response, options) {{}}
                                }});
                            }}*/
                        }}
                        else {{
                            if (stateId == 2) {{
                                StateField.setValue('Данные находятся на рассмотрении у ОГВ');
                                {4}
                            }}
                            if (stateId == 3)
                                StateField.setValue('Данные утверждены');
                            
                        }}
                        {1}
                }},
                failure: function (response, options) {{}}
            }});
            ".FormatWith(Region.ID, storeState, '"', inEditState, inOGVState, enableEditHandler, disableEditHandler, isOGV.Equals("true") ? "OGV" : "MO", isOGV, isFictRegion);
        }
    }
}
