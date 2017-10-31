using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO
{
    public class HMAORequestView : View
    {
        /// <summary>
        /// Идентификатор Store для показателей
        /// </summary>
        public static readonly string IndicatorsStoreId = "IndicatorsStore";

        private readonly IFO41Extension extension;

        /// <summary>
        /// Репозиторий с заявками от налогоплательщиков
        /// </summary>
        private readonly IAppPrivilegeService requestsRepository;

        private readonly ICategoryTaxpayerService categoryRepository;

        /// <summary>
        ///  Идентификатор налогоплательщика заявки
        /// </summary>
        private readonly int taxPayerId;

        /// <summary>
        /// Идентификатор типа налога
        /// </summary>
        private readonly int taxTypeId;

        /// <summary>
        /// Идентификатор заявки от налогоплательщика
        /// </summary>
        private readonly int applicationId;

        /// <summary>
        /// Идентификатор активной вкладки
        /// </summary>
        private readonly string activeTab;

        /// <summary>
        /// Идентификатор категории
        /// </summary>
        private readonly int? categoryId;

        /// <summary>
        /// Идентификатор панели с заявкой
        /// </summary>
        private readonly string requestsTabPanelId;

        /// <summary>
        /// Признак, что заявка была создана и вкладка еще не закрывалась (необходимо для корректного получения идентификатора вкладки)
        /// </summary>
        private readonly bool? wasNew;

        /// <summary>
        /// Идентификатор комбобокса с заявками из прошлых периодов
        /// </summary>
        private const string ComboPrevReqId = "cbPrevRequests";

        /// <summary>
        /// Идентификатор Store для заявок из прошлых периодов
        /// </summary>
        private const string StorePrevReqId = "dsPrevRequests";

        /// <summary>
        /// Скрипт - проверка, заполнены ли показатели, предупреждающее сообщение. 
        /// Обработчик - сохранение заявки
        /// </summary>
        private const string CheckIndicatorsSave = @"
            // проверить корректность   
            var isCorrect = true;    
            var recCnt = IndicatorsGrid.store.getCount();
            for (var i = 0; i < recCnt; i++){{
                var data = IndicatorsGrid.store.getAt(i);
                if ((data.get('RefName') != 'Справочно') && (data.get('RefName') == '' || data.get('RefName') == null ||
                    (data.get('Fact') != 0 && (data.get('Fact') == '' || data.get('Fact') == null)) ||
                    (data.get('PreviousFact') != 0 && (data.get('PreviousFact') == '' || data.get('PreviousFact') == null))))
                        
                isCorrect = false;
            }}

            if (!isCorrect) {{
                Ext.Msg.show({{
                    title: 'Анализ и планирование',
                    msg: 'Не все показатели заполнены. Продолжить сохранение заявки?',
                    buttons: Ext.MessageBox.YESNO,
                    multiline: false,
                    animEl: 'ApplicPanel',
                    icon: Ext.MessageBox.WARNING,
                    fn: function (btn) {{
                        if (btn == 'yes') {{
                            {0}
                        }}
                    }}
                }});
            }}
            else {{
                {0}
            }}";

        /// <summary>
        /// Скрипт - сообщение, нужно ли передавать заявку на рассмотрение ОГВ. 
        /// Параметр - обработчик нажатия кнопок
        /// </summary>
        private const string SendToOGVQuestion = @"
            Ext.Msg.show({{    
                title: 'Анализ и планирование',
                msg: 'Вы действительно хотите отправить заявку на {1}?',
                buttons: Ext.MessageBox.YESNO,
                multiline: false,
                animEl: 'ApplicPanel',
                icon: Ext.MessageBox.WARNING,
                fn: function (btn) {{
                   {0}
                }}
            }});";

        /// <summary>
        /// Идентификатор Store для доументов
        /// </summary>
        private string docstoreId;

        /// <summary>
        /// Модель с реквизитами заявки
        /// </summary>
        private HMAODetailViewModel detailsViewModel;

        /// <summary>
        /// Признак, что ОГВ создает заявку от налогоплательщика
        /// </summary>
        private bool isNewAppByOGV;

        /// <summary>
        /// Признак, рдактируема ли заявка
        /// </summary>
        private bool editable;

        /// <summary>
        /// Признак, редактируемы ли документы и комменарии
        /// </summary>
        private bool docsAndCommentsEditable;

        /// <summary>
        /// Идентификатор закладки с заявкой
        /// </summary>
        private string reqTabId;

        public HMAORequestView(
            IFO41Extension extension, 
            IAppPrivilegeService requestsRepository,
            ICategoryTaxpayerService categoryRepository,
            int taxPayerId,
            int taxTypeId,
            int applicationId, 
            string activeTab,
            bool? wasNew,
            int? categoryId)
        {
            this.extension = extension;
            this.categoryRepository = categoryRepository;
            this.taxPayerId = taxPayerId;
            this.taxTypeId = taxTypeId;
            this.applicationId = applicationId;
            this.activeTab = activeTab ?? "Реквизиты";
            this.requestsRepository = requestsRepository;
            this.categoryId = categoryId;
            this.wasNew = wasNew;
            requestsTabPanelId = "HMAORequestTabPanel";
            IsPrevRequestsVisisble = true;
        }

        public bool IsPrevRequestsVisisble { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            detailsViewModel = requestsRepository.GetHMAODetailsViewModel(applicationId, taxTypeId);
            if (applicationId < 1 && categoryId != null && categoryId > 0)
            {
                detailsViewModel.CategoryId = (int)categoryId;
                detailsViewModel.CategoryName = categoryRepository.GetQueryOne((int)categoryId).Name;
            }

           if (extension.UserGroup != FO41Extension.GroupOGV && 
               extension.UserGroup != FO41Extension.GroupTaxpayer)
            {
                ResourceManager.GetInstance(page).RegisterOnReadyScript(
                    ExtNet.Msg.Alert(
                        "Ошибка", 
                        "Текущему пользователю не сопоставлен налогоплательщик или экономический орган.")
                        .ToScript());

                return new List<Component>();
            }

           isNewAppByOGV = detailsViewModel.StateId == 1 && detailsViewModel.Id < 1
                    && extension.UserGroup == FO41Extension.GroupOGV;

           editable = (detailsViewModel.StateId == 1 && extension.UserGroup == FO41Extension.GroupTaxpayer)
               || isNewAppByOGV;

            docsAndCommentsEditable = detailsViewModel.Id > 0 && 
                (editable || (detailsViewModel.StateId == 2 && extension.UserGroup == FO41Extension.GroupOGV));

            var store = CreatePrevRequestsStore();
            page.Controls.Add(store);

            // TODO ХМАО новая заявка от EO закрытие вкладки, идентификатор заявки неверный.
            var categoryIdForReqId = wasNew == true && extension.UserGroup == FO41Extension.GroupTaxpayer ? 0 : detailsViewModel.CategoryId;
            reqTabId = extension.GetReqTabId(categoryIdForReqId, detailsViewModel.PeriodId, wasNew == true ? -1 : detailsViewModel.Id);

            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterClientScriptBlock("RequestViewJs", Resources.Resource.RequestViewJs);

            var panel = new Panel
                            {
                                ID = "ApplicPanel",
                                Items =
                                    {
                                        new BorderLayout
                                            {
                                                North = { Items = { CreateHeaderTools() } },
                                                Center = { Items = { CreateRequestTabPanel(page) } }
                                            }
                                    }
                            };

            var saveHandler = "if (formValidate()) {" +
                              CheckIndicatorsSave.FormatWith(
                                  @"
                DetailsForm.form.submit({{
                    waitMsg: 'Сохранение...', 
                    success: function(form, action) {{
                        successSaveHandler(form, action);
                        currentTab.forceClose = true; 
                        currentTab.ownerCt.closeTab(currentTab);
                    }}, 
                    failure: failureSaveHandler, 
                    params: {{ 
                        'state': {0}.getAt(0) == null ? 1 : {0}.getAt(0).data.StateId 
                    }}
                }});".FormatWith(store.ID)) 
                 + "}";

            var mdiTabOrCategoryTabPanel = extension.UserGroup == FO41Extension.GroupTaxpayer
            ? "MdiTab"
            : "Category{0}TabPanel".FormatWith(detailsViewModel.CategoryId);

            resourceManager.AddScript(
                @"
            // обработчик в случае удачного сохранения заявки с полным обновлением интерфейса с заявкой
            successSaveHandlerReload = function(form, action) {{
                successSaveHandler(form, action);
                var tab = parent.{3}.getComponent({2});
                tab.autoLoad.url = 
                    '/FO41HMAORequestsList/HMAORequestView?applicationId='+action.result.message +
                        '&taxTypeId={0}'+ '{4}&activeTab=' + {1}.activeTab.id;
                tab.reload();
            }}".FormatWith(taxTypeId, requestsTabPanelId, reqTabId, mdiTabOrCategoryTabPanel, wasNew == true ? "&wasNew=true" : string.Empty));

            resourceManager.AddScript(@"
            successSaveHandler = function(form, action){{
                if (action.result.script)
                    eval(action.result.script);    
                // сохранить коммментарии
                if (action.result && action.result.message)
                    CommentsPanel_0.store.baseParams.applicationId = action.result.message;
                CommentsPanel_0.store.save();
                // обновить номер заявки
                if (action.result && action.result.message) {{
                    {0}.baseParams.applicationId = action.result.message;
                    {0}.updateProxy.conn.url = '/FO41Indicators/Save?applicationId=' + action.result.message;
                }}
                // сохранить показатели
                {0}.save();
                // сохранить документы
                {1}.save();
                DetailsForm.form.url = '/FO41HMAORequests/Save?applicaionId='+action.result.message+
                    '&taxPayerId={2}&taxTypeId={3}';
                {0}.commitChanges();
                /*обновляем показатели*/
            }}"
                        .FormatWith(
                            IndicatorsStoreId, 
                            docstoreId, 
                            detailsViewModel.OrgId, 
                            detailsViewModel.TypeTax.ID));

            var getTabScript = extension.UserGroup == FO41Extension.GroupTaxpayer
                ? @"
                    var tab = parent.MdiTab.getComponent({0});
                    ".FormatWith(reqTabId)
                : @"var tab = parent.Category{0}TabPanel.getComponent({1});"
                    .FormatWith(detailsViewModel.CategoryId, reqTabId);

                 // Регистрируем событие на закрытие вкладки
                // переопределение функций
                resourceManager.AddScript(@"
            saveApplication = function(currentTab) {{
                // сохранить заявку
                {1}
            }};

            isDirty = function() {{
                return true;
            }};

            {0}
            if (typeof (tab.events.beforeclose) == 'object')
            {{
                   tab.events.beforeclose.clearListeners();
            }}

            ".FormatWith(getTabScript, saveHandler));

            if ((editable || docsAndCommentsEditable) && extension.IsReqLastPeriod(detailsViewModel.PeriodId))
            {
                resourceManager.AddScript(
                    @"
                tab.addListener('beforeclose', beforeCloseTab);");
            }

            return new List<Component> 
            {
                new Viewport
                {
                    ID = "viewportRequestHMAO",
                    Items = { new BorderLayout { Center = { Items = { panel } } } }
                } 
            };
        }

        private Store CreatePrevRequestsStore()
        {
            var ds = new Store { ID = StorePrevReqId, AutoLoad = true };

            if (applicationId > 0)
            {
                var req = requestsRepository.Get(applicationId);
                ds.BaseParams.Add(new Parameter("orgId", req.RefOrgPrivilege.ID.ToString()));
                ds.BaseParams.Add(new Parameter("periodId", req.RefYearDayUNV.ID.ToString()));
            }
            else
            {
                ds.BaseParams.Add(new Parameter("periodId", extension.GetPrevPeriod().ToString()));
                ds.BaseParams.Add(new Parameter("orgId", taxPayerId.ToString()));
            }

            ds.SetHttpProxy("/FO41Requests/ReadPrevPeriods")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name");

            return ds;
        }

        private IEnumerable<Component> CreateHeaderTools()
        {
            var buttonOk = new Button
            {
                ID = "btnOk",
                ToolTip = @"Сохранить",
                Icon = Icon.TableSave,
                StyleSpec = "padding-right: 5px",
                Hidden = !editable && !docsAndCommentsEditable && extension.IsReqLastPeriod(detailsViewModel.PeriodId)
            };

            buttonOk.Listeners.Click.Handler = "if (formValidate()) {" 
                + CheckIndicatorsSave.FormatWith("saveApp();") + "}";
            var msgText = string.Empty;
            switch (detailsViewModel.StateId)
            {
                case 1:
                    {
                        msgText = (extension.UserGroup == FO41Extension.GroupTaxpayer)
                                      ? string.Empty
                                      : (isNewAppByOGV
                                        ? string.Empty 
                                        : "Заявка находится на редактировании у налогоплательщика, ее редактирование невозможно");
                        break;
                    }

                case 2:
                    {
                        msgText = (extension.UserGroup == FO41Extension.GroupTaxpayer)
                                      ? "Заявка находится на рассмотрении у ОГВ, ее редактирование невозможно"
                                      : string.Empty;
                        break;
                    }

                case 3:
                    {
                        msgText = "Заявка находится на оценке, ее редактирование невозможно"; 
                        break;
                    }

                case 4:
                    {
                        msgText = "Заявка принята, ее редактирование невозможно"; 
                        break;
                    }

                case 5:
                    {
                        msgText = "Заявка отклонена, ее редактирование невозможно"; 
                        break;
                    }
            }

            var headerPanel = new Panel
            {
                Border = false,
                Height = 30,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                StyleSpec = " padding-top: 5px;",
                Layout = "ColumnLayout",
                Items =
                    {
                        new Button
                        {
                            ID = "toOGV",
                            ToolTip = @"Отправить на рассмотрение ОГВ",
                            Hidden = (!(editable && extension.UserGroup == FO41Extension.GroupTaxpayer) && 
                                !(detailsViewModel.StateId == 1 && extension.UserGroup == FO41Extension.GroupOGV)) ||
                                !extension.IsReqLastPeriod(detailsViewModel.PeriodId),
                            Icon = Icon.Tick,
                            CommandName = "toOGVRequest",
                            StyleSpec = "padding-left: 5px; padding-right: 5px",
                            Listeners =
                                {
                                    Click =
                                        {
                                            Handler = SendToOGVQuestion.FormatWith(
                                                GetScriptChangeState(2), 
                                                "рассмотрение ОГВ")
                                        }
                                }
                        },
                        new Button
                        {
                            ID = "toEdit",
                            ToolTip = @"Отправить на доработку МО",
                            Hidden = !(docsAndCommentsEditable && extension.UserGroup == FO41Extension.GroupOGV) ||
                                !extension.IsReqLastPeriod(detailsViewModel.PeriodId),
                            Icon = Icon.PageBack,
                            CommandName = "toEditRequest",
                            StyleSpec = "padding-left: 5px; padding-right: 5px",
                            Listeners =
                                {
                                    Click =
                                        {
                                            Handler = SendToOGVQuestion.FormatWith(
                                                GetScriptChangeState(1), 
                                                "доработку МО")
                                        }
                                }
                        },
                        buttonOk,
                        new ComboBox
                            { 
                                ID = ComboPrevReqId,
                                EmptyText = @"Открыть заявки за прошлые периоды",
                                StyleSpec = "padding-left: 5px; padding-right: 5px;",
                                Width = 400,
                                TriggerAction = TriggerAction.All,
                                StoreID = StorePrevReqId,
                                ValueField = "ID",
                                DisplayField = "Name",
                                SelectOnFocus = true,
                                TypeAhead = true,
                                Hidden = !IsPrevRequestsVisisble,
                                Enabled = applicationId > 0
                            },
                        new Button
                            {
                                Text = @"Открыть",
                               StyleSpec = "padding-left: 5px; padding-right: 5px;",
                               Hidden = !IsPrevRequestsVisisble,
                               Listeners =
                                   {
                                       Click =
                                           {
                                               Handler = extension.UserGroup == FO41Extension.GroupTaxpayer 
                                                ? @"
                                                    if ({0}.value != '' && 
                                                        {0}.value != null && 
                                                        {0}.value != undefined) 
                                                        parent.MdiTab.addTab({{ 
                                                            title: 'Просмотр заявки', 
                                                            url: '/FO41HMAORequestsList/HMAORequestView?applicationId=' + {0}.value + '&taxTypeId={1}&isPrevRVisible=false', 
                                                            icon: 'icon-report'}});"
                                                    .FormatWith(ComboPrevReqId, taxTypeId)
                                                : @"if ({3}.value != '' && 
                                                        {3}.value != null && 
                                                        {3}.value != undefined) 
                                                        parent.parent.MdiTab.addTabTo(parent.Category{1}TabPanel, 
                                                        {{ 
                                                            id: 'req_new_{1}_{2}',
                                                            title: 'Просмотр заявки', 
                                                            url: '/FO41HMAORequestsList/HMAORequestView?applicationId=' + {3}.value + '&taxTypeId={0}&isPrevRVisible=false'
                                                         }});".FormatWith(taxTypeId, detailsViewModel.CategoryId, detailsViewModel.PeriodId, ComboPrevReqId)
                                           }
                                   }
                            },
                       new Button
                            {
                                ID = "exportReportButton",
                                Icon = Icon.PageExcel,
                                ToolTip = @"Выгрузка в Excel",
                                Listeners = 
                                { 
                                    Click = 
                                    { 
                                                  Handler = @"
                                        var orgName = '{0}';
                                        Ext.net.DirectMethod.request({{
                                            url: '/FO41Export/ReportTaxpayer',
                                            isUpload: true,
                                            formProxyArg: 'DetailsForm',
                                            cleanRequest: true,
                                            params: {{
                                                id: {1}, name: orgName
                                            }},
                                            success:function (response, options) {{
                                                            }},
                                                            failure: function (response, options) {{
                                                            }}
                                        }});".FormatWith(detailsViewModel.OrgName, applicationId)
                                              } 
                                    }
                            },

                        new Label
                              {
                                  StyleSpec = "padding-left: 5px; padding-right: 5px; padding-top: 5px; color: red;",
                                  Text = msgText
                              }
                    }
            };

            return new List<Component> { headerPanel };
        }
        
        private string GetScriptChangeState(int newState)
        {
            var closeTab = 
                extension.UserGroup == FO41Extension.GroupTaxpayer
                    ? @"var idTab = parent.MdiTab.hashCode('/FO41HMAORequestsList/HMAORequestView?applicationId='
                            +action.result.message+'&taxTypeId={0}');
                        var tab = parent.MdiTab.getComponent(idTab);
                        tab.forceClose = true; 
                        tab.ownerCt.closeTab(tab);".FormatWith(taxTypeId)
                    : @"parent.Category{0}TabPanel.activeTab.forceClose = true; 
                        parent.Category{0}TabPanel.closeTab(parent.Category{0}TabPanel.activeTab);"
                        .FormatWith(detailsViewModel.CategoryId);
            return @"if (btn == 'yes') {{ {0} }}".FormatWith(
                CheckIndicatorsSave.FormatWith(
                    @"                              
                    DetailsForm.form.submit({{
                        waitMsg:'Сохранение...', 
                        success: function(form, action) {{
                                    successSaveHandler(form, action);
                                    {3}
                                    }},
                        failure: failureSaveHandler, 
                        params: {{'state': {2}}}
                    }});
                    toOGV.hidden = true;
                    toOGV.hide();"
                        .FormatWith(taxTypeId, requestsTabPanelId, newState, closeTab)));
        }

        private IEnumerable<Component> CreateRequestTabPanel(ViewPage page)
        {
            var tabs = new TabPanel
            {
                ID = requestsTabPanelId,
                Border = false,
                ActiveTabIndex = 0
            };

            var detailView = new HMAODetailView(extension, detailsViewModel)
            {
                Editable = editable,
                DocsAndCommentsEditabe = docsAndCommentsEditable
            };

            tabs.Add(detailView.Build(page));
            
            docstoreId = detailView.DocsStoreId;

            var indicatorsView = new HMAOIndicatorsView(
                extension, 
                detailsViewModel.Id, 
                detailsViewModel.TypeTax.ID, 
                detailsViewModel.PeriodId)
            {
                ApplicationId = applicationId,
                IndicatorsStoreId = IndicatorsStoreId,
                Editable = editable
            };

            tabs.Add(indicatorsView.Build(page));

            tabs.ActiveIndex = activeTab.Equals("Показатели") ? 1 : 0;
            return new List<Component> { tabs };
        }
    }
}
