using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views
{
    public class RequestView : View
    {
        /// <summary>
        /// Идентификатор Store для показателей
        /// </summary>
        public static readonly string IndicatorsStoreId = "IndicatorsStore";

        /// <summary>
        /// Идентификатор заявки от налогоплательщика
        /// </summary>
        private readonly int applicationId;

        /// <summary>
        /// Идентификатор категории
        /// </summary>
        private readonly int categoryId;

        /// <summary>
        /// Признак, является ли заявка копией
        /// </summary>
        private readonly bool isCopy;

        /// <summary>
        /// Репозиторий с заявками от налогоплательщиков
        /// </summary>
        private readonly IAppPrivilegeService requestsRepository;

        private readonly IAppFromOGVService appFromOGVRepository;

        /// <summary>
        /// Глобальные параметры
        /// </summary>
        private readonly IFO41Extension extension;

        /// <summary>
        /// Идентификатор состояния заявки от налогоплательщика
        /// </summary>
        private readonly int state;

        /// <summary>
        /// Cостояниt заявки от налогоплательщика
        /// </summary>
        private readonly string stateName;

        private readonly bool? wasNew;

        /// <summary>
        /// Идентификатор активной вкладки
        /// </summary>
        private readonly string activeTab;

        private readonly DetailsViewModel detailsViewModel;
        
        /// <summary>
        /// Признак, принадлежит ли заявка тому же ОГВ или для пользователя не ОГВ true
        /// </summary>
        private readonly bool sameOgv;

        /// <summary>
        /// Признак, заявка создана в текущем году
        /// </summary>
        private readonly bool reqLastPeriod;

        /// <summary>
        /// Признак, можно ли редактировать и создавать заявки от налогопл. по состоянию заявки от ОГВ
        /// </summary>
        private readonly bool appOGVStateIsCreatedOrReEdited;
        
        /// <summary>
        /// Идентификатор вкладки с заявкой
        /// </summary>
        private readonly string reqTabId;

        /// <summary>
        /// Идентификатор комбобокса с заявками из прошлых периодов
        /// </summary>
        private const string ComboPrevReqId = "cbPrevRequests";

        /// <summary>
        /// Идентификатор Store для заявок из прошлых периодов
        /// </summary>
        private const string StorePrevReqId = "dsPrevRequests";

        /// <summary>
        /// Скрипт - сообщение, нужно ли передавать заявку на рассмотрение ОГВ. 
        /// Параметр - обработчик нажатия кнопок
        /// </summary>
        private const string SendToOGVQuestion = @"
            Ext.Msg.show({{    
                title: 'Анализ и планирование',
                msg: 'Вы действительно хотите отправить заявку на рассмотрение ОГВ?',
                buttons: Ext.MessageBox.YESNO,
                multiline: false,
                animEl: 'ApplicPanel',
                icon: Ext.MessageBox.WARNING,
                fn: function (btn) {{
                   {0}
                }}
            }});";

        /// <summary>
        /// Скрипт - проверка, заполнены ли показатели, предупреждающее сообщение. 
        /// Обработчик - сохранение заявки
        /// </summary>
        private const string CheckIndicatorsSave = @"
            // проверить корректность   
            if (!isValid(IndicatorsGrid.store)) {{
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
        /// Идентификатор Store для доументов
        /// </summary>
        private string docstoreId;

        public RequestView(
            IFO41Extension extension,
            IAppPrivilegeService requestsRepository,
            IAppFromOGVService appFromOGVRepository,
            ICategoryTaxpayerService categoryRepository,
            int applicationId, 
            int? categoryId, 
            bool? isCopy,
            bool? wasNew,
            string activeTab)
        {
            state = (applicationId > 0 && !(isCopy ?? false))
                 ? requestsRepository.Get(applicationId).RefStateOrg.ID
                 : 1;
            stateName = (applicationId > 0 && !(isCopy ?? false))
                 ? requestsRepository.Get(applicationId).RefStateOrg.Name
                 : "Создана"; 

            this.extension = extension;
            this.requestsRepository = requestsRepository;
            this.appFromOGVRepository = appFromOGVRepository;
            this.applicationId = applicationId;
            this.categoryId = (categoryId == null) 
                ? requestsRepository.Get(applicationId).RefOrgCategory.ID 
                : (int)categoryId;
            this.isCopy = isCopy == null ? false : (bool)isCopy;
            this.wasNew = wasNew;
            this.activeTab = activeTab;

            IsPrevRequestsVisisble = true;

            detailsViewModel = requestsRepository.GetDetailsViewModel(applicationId);
            
            detailsViewModel.StateId = state;
            detailsViewModel.StateName = stateName;
            
            if (categoryId != null && categoryId > 0)
            {
                detailsViewModel.CategoryId = (int)categoryId;
            }

            sameOgv = (applicationId < 1 || isCopy == true) ||
                (extension.ResponsOIV != null && extension.ResponsOIV.ID == categoryRepository.GetQueryOne(detailsViewModel.CategoryId).RefOGV.ID) ||
                extension.ResponsOrg != null;

            var periodId = detailsViewModel.PeriodId;
            reqLastPeriod = extension.IsReqLastPeriod(periodId);

            appOGVStateIsCreatedOrReEdited = CheckAppOgvState(detailsViewModel.PeriodId);

            var categoryIdForReqId = wasNew == true && extension.UserGroup == FO41Extension.GroupTaxpayer
                                         ? 0
                                         : (categoryId ?? detailsViewModel.CategoryId);
            reqTabId = extension.GetReqTabId(categoryIdForReqId, detailsViewModel.PeriodId, wasNew == true ? -1 : detailsViewModel.Id);
        }

        public bool IsPrevRequestsVisisble { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreatePrevRequestsStore());

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

            var saveHandler = @"
            if (formValidate()) {" +
                              CheckIndicatorsSave.FormatWith(
                                  @"
                DetailsForm.form.submit({ 
                    waitMsg: 'Сохранение...', 
                    success: function(form, action) {
                        successSaveHandler(form, action);
                        currentTab.forceClose = true; 
                        currentTab.ownerCt.closeTab(currentTab);
                    }, 
                    failure: failureSaveHandler, 
                    params: { 'state': dsDetail.getAt(0).data.StateId} });") +
                              @"
            }";

            var ogvCanEdit = extension.UserGroup == FO41Extension.GroupTaxpayer || !reqLastPeriod
                ? false
                : detailsViewModel.StateId == 2 && extension.ResponsOIV.Role.Equals("ОГВ");

            if (((state == 1 && appOGVStateIsCreatedOrReEdited) || ogvCanEdit) && sameOgv && reqLastPeriod)
            {
                // Регистрируем событие на закрытие вкладки
                // переопределение функций
                resourceManager.AddScript(@"
            saveApplication = function(currentTab) {{
                // сохранить заявку
                {2}
            }};

            isDirty = function() {{
                return  DetailsForm.isDirty() || 
                        !DetailsForm.isValid() || 
                        IndicatorsGrid.isDirty() || 
                        CommentsPanel_{0}.isDirty() || 
                        {1}.isDirty();
            }};

            var tab = parent.MdiTab.getComponent({5});
            if (typeof(tab.events.beforeclose) == 'object'){{
                tab.events.beforeclose.clearListeners();
            }}
            tab.addListener('beforeclose', beforeCloseTab);"
                        .FormatWith(
                            detailsViewModel.CategoryId,
                            docstoreId,
                            saveHandler,
                            wasNew != true ? applicationId : -1,
                            isCopy ? "&isCopy=true" : (applicationId > 0 && wasNew != true ? string.Empty : "&categoryId={0}".FormatWith(detailsViewModel.CategoryId)),
                            reqTabId));

                resourceManager.AddScript(
                    @"
            // обработчик в случае удачного сохранения заявки с полным обновлением интерфейса с заявкой
            successSaveHandlerReload = function(form, action) {{
                successSaveHandler(form, action);
                var currentTab = parent.MdiTab.getComponent({5});
                if ({4}) {{
                    currentTab.forceClose = true;
                    currentTab.ownerCt.closeTab(currentTab);    
                }}
                else {{
                    currentTab.autoLoad.url = '/FO41Requests/ShowRequest?applicationId=' + action.result.message + '{3}&activeTab=' + RequestTabPanel.activeTab.id;
                    currentTab.reload();
                }}
            }}

            successSaveHandler = function(form, action){{
                if (action.result.script)
                    eval(action.result.script);    
                // сохранить коммментарии
                if (action.result && action.result.message)
                    CommentsPanel_{2}.store.baseParams.applicationId = action.result.message;
                CommentsPanel_{2}.store.save();
                // обновить номер заявки
                if (action.result && action.result.message) {{
                    {0}.baseParams.applicationId = action.result.message;
                    {0}.updateProxy.conn.url = '/FO41Indicators/Save?applicationId=' + action.result.message;
                    dsDetail.getAt(0).data.Id.value = action.result.message;
                    DetailsForm.getForm().items.items[1].value = action.result.message;
                    DetailsForm.get('requestNumberValue').update();
                }}
                // сохранить показатели
                {0}.save();
                // сохранить документы
                {1}.save();
                DetailsForm.form.url = '/FO41Requests/Save?applicaionId='+action.result.message+'&copyApplicId=-1';
                {0}.commitChanges();
                /*обновляем показатели*/
            }}"
            .FormatWith(
                IndicatorsStoreId, 
                docstoreId, 
                detailsViewModel.CategoryId,
                isCopy ? "&wasNew=true" : (applicationId > 0 && (wasNew != true) ? string.Empty : "&categoryId={0}&wasNew=true".FormatWith(detailsViewModel.CategoryId)),
                isCopy ? "true" : "false",
                reqTabId));
            }

            return new List<Component>
            {
                new Viewport
                {
                    ID = "viewportRequest",
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
            }

            ds.Listeners.BeforeLoad.AddBefore(@"
            {0}.baseParams.orgId = requestOrgPrivilege.value; 
            if ({0}.lastOptions.params != null) 
                {0}.lastOptions.params.orgId = requestOrgPrivilege.value;".FormatWith(ds.ID));

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
                ToolTip = @"Сохранить и рассчитать показатели",
                Icon = Icon.TableSave,
                StyleSpec = "padding-right: 5px",
                Hidden = true
            };

            // если заявка в состоянии "создана" или это копия, т.е. может редактироваться, 
            // добавляем кнопки сохранить/отмена
            // TODO extension.ResponsOrg!= null && та же организация должна быть тогда, хотя мы только ее заявки и должны отображать
            if (reqLastPeriod && 
                (state == 1 || isCopy || 
                    (state == 2 && 
                        (extension.ResponsOIV != null && extension.ResponsOIV.Role.Equals("ОГВ")))) 
                && (sameOgv || extension.ResponsOrg != null))
            {
                buttonOk.Hidden = false;
                buttonOk.Listeners.Click.Handler = "if (formValidate()) {" + 
                    CheckIndicatorsSave.FormatWith("saveApp();") + "}";
            }

            var toEditButton = new Button
                                 {
                                     ID = "toEdit",
                                     ToolTip = @"Вернуть на редактирование",
                                     Icon = Icon.PageBack,
                                     CommandName = "toOGVRequest",
                                     StyleSpec = "padding-left: 5px; padding-right: 5px",
                                     Hidden = state != 2 || !sameOgv,
                                     Visible = extension.ResponsOIV != null && extension.ResponsOIV.Role.Equals("ОГВ") && reqLastPeriod,
                                     Listeners =
                                         {
                                             Click =
                                                 {
                                                     Handler = @"
                                                        Ext.Msg.show({
                                                            title: 'Анализ и планирование',
                                                            msg: 'Вы действительно хотите вернуть заявку на редактирование?',
                                                            buttons: Ext.MessageBox.YESNO,
                                                            multiline: false,
                                                            animEl: 'ApplicPanel',
                                                            icon: Ext.MessageBox.WARNING,
                                                            fn: function (btn) {
                                                                if (btn == 'yes') {
                                                                    if (dsDetail.getAt(0).data.Id > 0) {
                                                                        dsDetail.getAt(0).data.StateId = 1;
                                                                        DetailsForm.form.submit({
                                                                            waitMsg:'Сохранение...', 
                                                                            success: successSaveHandlerReload, 
                                                                            failure: failureSaveHandler, 
                                                                            params: {'state': 1}
                                                                        });
                                                                        toOGV.hidden = false;
                                                                        toOGV.show();
                                                                        var toEditCmp = Ext.getCmp('toEdit');
                                                                        if (toEditCmp)
                                                                        {{
                                                                            toEdit.hidden = true;
                                                                            toEditCmp.hide();
                                                                        }}
                                                                    }
                                                                }
                                                            }
                                                        });"
                                                 }
                                         }
                                 };

            var toOGVButton = new Button
                                 {
                                     ID = "toOGV",
                                     ToolTip = @"Отправить на рассмотрение ОГВ",
                                     Hidden = state != 1 || !sameOgv,
                                     Icon = Icon.Tick,
                                     CommandName = "toEditRequest",
                                     StyleSpec = "padding-left: 5px; padding-right: 5px",
                                     Visible = reqLastPeriod && ((extension.ResponsOIV != null && extension.ResponsOIV.Role.Equals("ОГВ")) || extension.ResponsOrg != null),
                                     Listeners =
                                         {
                                             Click =
                                                 {
                                                    Handler = SendToOGVQuestion.FormatWith(@"if (btn == 'yes') {{ {0} }}".FormatWith(
                                                        @"
                                            if (formValidate()) {" + CheckIndicatorsSave.FormatWith(
                                                            @"
                                                dsDetail.getAt(0).data.StateId = 2;
                                                var idTab = parent.MdiTab.hashCode('/FO41Requests/ShowRequest?applicationId={0}{1}');
                                                var tab = parent.MdiTab.getComponent(idTab);
                                                DetailsForm.form.submit({{
                                                    waitMsg:'Сохранение...', 
                                                    success: function(form, action) {{
                                                                successSaveHandler(form, action);
                                                                tab.forceClose = true; 
                                                                tab.ownerCt.closeTab(tab);
                                                                }},
                                                    failure: failureSaveHandler, 
                                                    params: {{'state': 2}}
                                                }});
                                                toOGV.hidden = true;
                                                toOGV.hide();
                                                var toEditCmp = Ext.getCmp(toEdit);
                                                if (toEditCmp)
                                                {{
                                                    toEdit.hidden = false;
                                                    toEditCmp.show();
                                                }}"
                                                                .FormatWith(
                                                                    wasNew != true ? applicationId : -1, 
                                                                    isCopy ? "&isCopy=true" : (applicationId > 0 && wasNew != true ? string.Empty : "&categoryId={0}".FormatWith(categoryId))))
                                                                    + @"
                                            }"))
                                                 }
                                         }
                                 };
            var prevReqsCombo = new ComboBox
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
                               };
            var openPrevReqButton = new Button
                                 {
                                     Text = @"Открыть",
                                     StyleSpec = "padding-left: 5px; padding-right: 5px;",
                                     Hidden = !IsPrevRequestsVisisble,
                                     Listeners =
                                         {
                                             Click =
                                                 {
                                                     Handler = "if (cbPrevRequests.value != '' && cbPrevRequests.value != null && cbPrevRequests.value != undefined) parent.MdiTab.addTab({ title: 'Редактирование заявки', url: '/FO41Requests/ShowRequest?applicationId=' + cbPrevRequests.value + '&isPrevRVisible=false', icon: 'icon-report'});"
                                                 }
                                         }
                                 };
            var excelButton = new Button
                                 {
                                     ID = "exportReportButton",
                                     Icon = Icon.PageExcel,
                                     ToolTip = @"Выгрузка в Excel",
                                     Hidden = IsPrevRequestsVisisble,
                                     Listeners = 
                                         { 
                                             Click = 
                                                 { 
                                                     Handler = @"
                                        var orgName = '{0}';
                                        Ext.net.DirectMethod.request({{
                                            url: '/FO41Export/ReportTaxpayer',
                                            isUpload: true,
                                            formProxyArg: 'RequestsListFrom',
                                            cleanRequest: true,
                                            params: {{
                                                id: {1}, name: 'orgName'
                                            }},
                                            success:function (response, options) {{
                                                            }},
                                                            failure: function (response, options) {{
                                                            }}
                                        }});".FormatWith(detailsViewModel.OrgName, applicationId)
                                                 } 
                                         }
                                 };
            var infoMsgLabel = new Label
                                 {
                                     StyleSpec = "padding-left: 5px; padding-right: 5px; padding-top: 5px; color: red;",
                                     Text = reqLastPeriod
                                         ? "Заявка не может быть отредактирована, поскольку она находится в сосотоянии \"{0}\""
                                         .FormatWith(stateName)
                                         : "Заявки за прошлые года не могут быть отредактированы",
                                     Hidden = state == 1 || (extension.ResponsOIV != null && extension.ResponsOIV.Role == "ДФ")
                                 };
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
                        toEditButton,
                        toOGVButton,
                        buttonOk,
                        prevReqsCombo,
                        openPrevReqButton,
                        excelButton,
                        infoMsgLabel
                    }
            };

            return new List<Component> { headerPanel };
        }

        private IEnumerable<Component> CreateRequestTabPanel(ViewPage page)
        {
            var tabs = new TabPanel
                           {
                               ID = "RequestTabPanel",
                               Border = false,
                               ActiveTabIndex = 0
                           };
            
            var detailView = new DetailView(extension, requestsRepository)
                                {
                                     ApplicationId = applicationId, 
                                     CategoryId = categoryId, 
                                     CopyApplicId = isCopy ? applicationId : -1,
                                     OrgControl = ComboPrevReqId,
                                     Editable = state == 1 && appOGVStateIsCreatedOrReEdited && sameOgv && reqLastPeriod
                                 };

            // если создаем копию, устанавливаем значения
            if (isCopy)
            {
                // устанавливаем реквизиты как у оригинальной заявки, 
                // очищаем значение категории, идентификатора, дата - текущая дата
                detailsViewModel.Id = -1;
                detailsViewModel.CategoryId = -1;
                detailsViewModel.CategoryName = string.Empty;
                detailsViewModel.RequestDate = DateTime.Today
                    .ToString("d", CultureInfo.CreateSpecificCulture("de-DE"));
                detailView.DetailsViewModel = detailsViewModel;
            }

            tabs.Add(detailView.Build(page));

            docstoreId = detailView.DocsStoreId;

            var indicatorsView = new IndicatorsView 
            { 
                ApplicationId = applicationId, 
                IndicatorsStoreId = IndicatorsStoreId,
                RequestYear = detailsViewModel.PeriodId / 10000,
                IsCopy = isCopy,
                Editable = state == 1 && appOGVStateIsCreatedOrReEdited && sameOgv && reqLastPeriod 
            };

            tabs.Add(indicatorsView.Build(page));

            if (activeTab.IsNotNullOrEmpty())
            {
                tabs.SetActiveTab(activeTab);
            }

            return new List<Component> { tabs };
        }

        /// <summary>
        /// Проверяет, можно и редактировать зи создавать заявки от налогопл. по состоянию заявки от ОГВ
        /// </summary>
        /// <param name="periodId">Идентификатор периода</param>
        /// <returns>true, если заявка не создана или в состоянии "Создана", иначе - false</returns>
        private bool CheckAppOgvState(int periodId)
        {
            var app = appFromOGVRepository.FindAll()
                .FirstOrDefault(x => x.RefOrgCategory.ID == categoryId && x.RefYearDayUNV.ID == periodId);
            if (app == null)
            {
                return true;
            }

            if (app.RefStateOGV.ID == 1 || app.RefStateOGV.ID == 3)
            {
                return true;
            }

            return false;
        }
    }
}
