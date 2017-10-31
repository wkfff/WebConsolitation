using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Presentation.Controls;
using Krista.FM.RIA.Extensions.FO41.Presentation.Models;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views
{
    /// <summary>
    /// Интерфейс заявки от ОГВ
    /// </summary>
    public class EstimateRequestView : View
    {
        /// <summary>
        /// Идентификатор категории
        /// </summary>
        private readonly int categoryId;

        /// <summary>
        /// Идентификатор категории
        /// </summary>
        private readonly int periodId;

        /// <summary>
        /// глобальные параметры
        /// </summary>
        private readonly IFO41Extension extension;

        /// <summary>
        /// Репозиторий показателей (в т.ч. величина прожиточного минимума)
        /// </summary>
        private readonly IRepository<D_Marks_NormPrivilege> normRepository;

        /// <summary>
        /// Модель заявки от ОГВ
        /// </summary>
        private readonly DetailsEstimateModel detailsViewModel;

        /// <summary>
        /// Ответственный ОГВ
        /// </summary>
        private readonly D_OMSU_ResponsOIV responsOIV;

        /// <summary>
        /// Краткое наименование категории
        /// </summary>
        private readonly string categoryShortName;

        /// <summary>
        /// Признак, можно ли редактировать заявку от ОГВ
        /// </summary>
        private readonly bool editable;

        /// <summary>
        /// Признак, можно ли редактировать комментарии и документы, если editable = false
        /// </summary>
        private readonly bool canAddCommentsAndDocs;

        /// <summary>
        /// Идентификатор активной вкладки
        /// </summary>
        private readonly string activeTab;

        /// <summary>
        /// Признак, текущего ли года заявка
        /// </summary>
        private readonly bool reqThisPeriod;

        /// <summary>
        /// Идентификатор закладки с заявкой
        /// </summary>
        private readonly string estReqTabId;

        /// <summary>
        /// Скрипт - диалоговое окно с вопросом (Да/Нет) Параметры - текст, обработчик
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

        /// <summary>
        /// Вкладка "Реквизиты" заявки от ОГВ
        /// </summary>
        private EstimateDetailView estimateDetailView;

        /// <summary>
        /// Вкладка "Исходные данные" заявки от ОГВ
        /// </summary>
        private EstimateInputDataView estimateInputDataView;

        /// <summary>
        /// Вкладка "Итоговые данные" заявки от ОГВ
        /// </summary>
        private EstimateResultDataView estimateResultDataView;

        /// <summary>
        /// Вкладка "Оценка" (для ДФ)
        /// </summary>
        private EstimateView estimateView;

        private AppFromOGVService requestsRepository;

        /// <summary>
        /// Initializes a new instance of the EstimateRequestView class (Интерфейс заявки от ОГВ)
        /// </summary>
        /// <param name="extension">Глобальные параметры</param>
        /// <param name="appFromOGVId">Идентификатор заявки от ОГВ</param>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="responsOIV">Ответственный ОГВ</param>
        /// <param name="requestsRepository">Репозиторий заявок от ОГВ</param>
        /// <param name="normRepository">Репозиторий показателей</param>
        /// <param name="categoryShortName">Краткое наименование категории</param>
        /// <param name="activeTab">Имя активной вкладки</param>
        public EstimateRequestView(
            IFO41Extension extension, 
            int appFromOGVId,
            int categoryId,
            int periodId, 
            D_OMSU_ResponsOIV responsOIV, 
            AppFromOGVService requestsRepository, 
            IRepository<D_Marks_NormPrivilege> normRepository,
            string categoryShortName,
            string activeTab)
        {
            this.normRepository = normRepository;
            this.requestsRepository = requestsRepository;
            this.extension = extension;
            this.responsOIV = responsOIV;
            this.categoryShortName = categoryShortName;
            this.periodId = periodId;

            if (appFromOGVId > 0)
            {
                // получаем заявку от ОГВ по выбранной категории или пустую заявку, 
                // если по этой категории еще не создана
                detailsViewModel = requestsRepository.GetDetailsViewModelByID(appFromOGVId);
            }
            else
            {
                // получаем заявку от ОГВ по выбранной категории или пустую заявку, 
                // если по этой категории еще не создана
                detailsViewModel = requestsRepository.GetDetailsViewModelByCategory(categoryId, periodId);

                // если новая заявка, прописываем ОГВ
                if (detailsViewModel.ID < 1)
                {
                    detailsViewModel.OGVID = responsOIV.ID;
                    detailsViewModel.OGVName = responsOIV.Name;
                }
            }

            this.categoryId = detailsViewModel.CategoryID;

            reqThisPeriod = extension.IsReqLastPeriod(detailsViewModel.PeriodId);

            // ОГВ может редактировать заявку полностью в состояниях "создана" и "на редактировании у ОГВ"
            editable = (detailsViewModel.StateOGV == 1 || detailsViewModel.StateOGV == 3) &&
                extension.ResponsOIV.Role.Equals("ОГВ") && reqThisPeriod;

            // ДФ может добавлять комментарии и документы к заявке в состоянии "на оценке"
            canAddCommentsAndDocs = (detailsViewModel.StateOGV == 2) && extension.ResponsOIV.Role.Equals("ДФ") && reqThisPeriod;

            this.activeTab = activeTab;

            // Идентификатор закладки с заявкой
            estReqTabId = extension.GetEstReqTabId(this.categoryId, this.periodId, detailsViewModel.ID);
        }

        /// <summary>
        /// Исполнитель (пользователь)
        /// </summary>
        public string Executor { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterClientScriptBlock("RequestViewJs", Resources.Resource.RequestViewJs);
            if (reqThisPeriod && (editable || canAddCommentsAndDocs))
            {
                resourceManager.AddScript(@"
            saveApplication = function() {{
                // сохранить заявку
                {0}
            }};

            isDirty = function() {{
                return true;
            }};

            var tab = parent.MdiTab.getComponent({1});
            if (typeof(tab.events.beforeclose) == 'object'){{
                tab.events.beforeclose.clearListeners();
            }}
            tab.addListener('beforeclose', beforeCloseTab);"
            .FormatWith(GetSaveHandler(true), estReqTabId));
            }

            if (detailsViewModel == null)
            {
                return new List<Component>
                {
                    new Viewport
                    {
                        ID = "viewportEstimateRequest",
                        Items = { new BorderLayout { Center = { Items = { new Label("Заявка не найдена") } } } }
                    }
                };  
            }

            var panel = new Panel
                {
                    ID = "ApplicPanel",
                    Items =
                        {
                            new BorderLayout
                                {
                                    Center = { Items = { CreateRequestTabPanel(page) } },
                                    North = { Items = { CreateHeaderTools() } }
                                }
                        }
                };

            return new List<Component>
            {
                new Viewport
                {
                    ID = "viewportEstimateRequest",
                    Items = 
                    {
                        new BorderLayout { Center = { Items = { panel } } }
                    }
                }
            };
        }

        /// <summary>
        /// Формирует текст скрипта для обработчика handler с диалоговым окном с вопросом msg
        /// </summary>
        /// <param name="msg">Текст для диалогового окна</param>
        /// <param name="handler">Скрипт обрабочика</param>
        /// <returns>Скрипт обрабочика с диалоговм окном</returns>
        private static string CreateAskHandler(string msg, string handler)
        {
            return AskChangeState.FormatWith(msg, handler);
        }

        /// <summary>
        /// Формирует скрипт для изменения состояния заявки по идентификатору нового состояния
        /// </summary>
        /// <param name="newState">Идентификатор нового состояния заявки</param>
        /// <param name="needClose">Признак, закрывать ли вкладку</param>
        /// <returns>Скрипт для изменения состояния заявки</returns>
        private string GetChangeStateHandler(int newState, bool needClose)
        {
            var tab = needClose ? "parent.MdiTab.getComponent({0})".FormatWith(estReqTabId) : "null";
            return (newState == 2)
                       ? @"
            if (dsDetail.getAt(0).data.ID > 0 && 
                    (dsDetail.getAt(0).data.StateOGV == 3 || dsDetail.getAt(0).data.StateOGV == 1)) {{
                dsDetail.getAt(0).data.StateOGV = 2;
                SaveAppFromOGV(2, {0});
            }}".FormatWith(tab)
                       : @"if (dsDetail.getAt(0).data.ID > 0 && dsDetail.getAt(0).data.StateOGV == 2) {{
                dsDetail.getAt(0).data.StateOGV = {0};
                    SaveAppFromOGV({0}, {1});
            }}".FormatWith(newState, tab);
        }

        private Panel CreateHeaderTools()
        {
            var buttonOk = new Button
                               {
                                   ID = "btnOk",
                                   ToolTip = @"Сохранить",
                                   Icon = Icon.TableSave,
                                   Hidden = !(editable || canAddCommentsAndDocs),
                                   StyleSpec = "padding-left: 5px; padding-top: 5px;"
                               };

            if (editable || canAddCommentsAndDocs)
            {
                buttonOk.Listeners.Click.Handler = GetSaveHandler(false);
            }

            var toEstimateButton = new Button
                {
                    ID = "toEstimate",
                    ToolTip = @"Отправить на оценку",
                    Icon = Icon.Tick,
                    CommandName = "toEstimate",
                    StyleSpec = "padding-left: 5px; padding-top: 5px;",
                    Visible = extension.ResponsOIV.Role.Equals("ОГВ"),
                    Hidden = (detailsViewModel.ID < 1) || (detailsViewModel.StateOGV != 3 && detailsViewModel.StateOGV != 1),
                    Listeners =
                        {
                            Click =
                                {
                                    Handler = CreateAskHandler(
                                        @"Вы действительно хотите отправить на оценку заявку по данной категории?", 
                                        GetChangeStateHandler(2, true))
                                }
                        }
                };

            var toReeditOgvButton = new Button
                {
                    ID = "toReEdit",
                    ToolTip = @"На доработку ОГВ",
                    Icon = Icon.PageBack,
                    CommandName = "toReEdit",
                    StyleSpec = "padding-left: 5px; padding-top: 5px;",
                    Visible = extension.ResponsOIV.Role.Equals("ДФ"),
                    Hidden = (detailsViewModel.ID < 1) || (detailsViewModel.StateOGV != 2),
                    Listeners =
                        {
                            Click =
                                {
                                    Handler = CreateAskHandler(
                                        @"Вы действительно хотите вернуть заявку по данной категории на доработку ОГВ?", 
                                        GetChangeStateHandler(3, true))
                                }
                        }
                };
            var acceptButton = new Button
                {
                    ID = "toAccept",
                    ToolTip = @"Принять",
                    Icon = Icon.Accept,
                    CommandName = "toAccept",
                    StyleSpec = "padding-left: 5px; padding-top: 5px;",
                    Visible = extension.ResponsOIV.Role.Equals("ДФ"),
                    Hidden = (detailsViewModel.ID < 1) || (detailsViewModel.StateOGV != 2),
                    Listeners =
                        {
                            Click =
                                {
                                    Handler = CreateAskHandler(
                                        @"Вы действительно хотите утвердить заявку по данной категории?", 
                                        GetChangeStateHandler(4, true))
                                }
                        }
                };
            var rejectButton = new Button
                                 {
                                     ID = "toReject",
                                     ToolTip = @"Отклонить",
                                     Icon = Icon.Cancel,
                                     CommandName = "toReject",
                                     StyleSpec = "padding-left: 5px; padding-top: 5px;",
                                     Visible = extension.ResponsOIV.Role.Equals("ДФ"),
                                     Listeners =
                                         {
                                             Click =
                                                 {
                                                     Handler = CreateAskHandler(@"Вы действительно хотите отклонить заявку по данной категории?", GetChangeStateHandler(5, true))
                                                 }
                                         },
                                     Hidden = (detailsViewModel.ID < 1) || (detailsViewModel.StateOGV != 2)
                                 };
            var infoLabel = new Label
                                 {
                                     StyleSpec =
                                         "padding-left: 5px; padding-right: 5px; padding-top: 10px; color: red;",
                                     Text = reqThisPeriod 
                                         ? "Заявка не может быть отредактирована, поскольку она находится в сосотоянии \"{0}\""
                                         .FormatWith(detailsViewModel.StateNameOGV)
                                         : "Заявки за прошлые года не могут быть отредактированы",
                                     Hidden = editable || canAddCommentsAndDocs
                                 };
            var tb = new Panel
                         {
                             CssClass = "x-window-mc",
                             AutoHeight = true,
                             StyleSpec = " padding-top: 5px;",
                             Layout = "ColumnLayout",
                             Border = false,
                             BodyCssClass = "x-window-mc",
                             Items =
                                 {
                                     toEstimateButton,
                                     toReeditOgvButton,
                                     acceptButton,
                                     rejectButton,
                                     buttonOk,
                                     infoLabel
                                 }
                         };

            if (editable || canAddCommentsAndDocs)
            {
                tb.AddScript(
                    @"
function successSaveHandler(form, action){{
    if (action.result.script) {{
        eval(action.result.script);    
    }}
    
    // сохранить коммментарии
    if (action.result && action.result.message) {{
        CommentsPanel_{0}.store.baseParams.applicationId = action.result.message;
    }}


    CommentsPanel_{0}.store.save();

    // настроить панель документов
    gpTaskFiles.disabled = false;
    btnAddFile.disabled = false;
    btnAddFile.enabled = true;
    dsFiles.baseParams.applicaionId = action.result.message;
    dsFiles.beforeSaveParams = function(store,options){{
        if (!options.params) {{
            options.params = {{}};
        }};
        Ext.apply(options.params,{{'storeChangedData':gpTaskFiles.store.getChangedData(),'taskId':'1'}});
        Ext.applyIf(options.params,{{}});
    }};

    // сохранить документы
    {1}.save();
    Ext.net.Notification.show({{
        iconCls    : 'icon-information', 
        html       : 'Изменения успешно сохранены', 
        title      : 'Анализ и планирование', 
        hideDelay  : 2500
    }});

    // сохранить включение заявок на оценку
    saveIncluded(action.result.message);

    // обновить таблицу показателей
    {3}.reload();

    DetailsForm{2}.form.url = '/FO41Estimate/Save?applicaionId='+action.result.message;

    var estReqTab = parent.MdiTab.getComponent({4});
    var urlTemp = estReqTab.autoLoad.url;
    urlTemp = urlTemp.replace('-1', action.result.message) + '&activeTab';
    var strToReplace = urlTemp.substring(urlTemp.indexOf('activeTab'),urlTemp.length);
    urlTemp = urlTemp.replace(strToReplace, 'activeTab=' + RequestTabPanel.activeTab.id)
    estReqTab.autoLoad.url = urlTemp;
    estReqTab.reload();
    ".FormatWith(
        categoryId,
        estimateDetailView.DocsStoreId,
        detailsViewModel.ID,
        estimateResultDataView.IndicatorsStoreId,
        estReqTabId)
+
                    @"
}

function failureSaveHandler(form, action){
    switch (action.failureType) {
        case Ext.form.Action.CLIENT_INVALID:
            Ext.Msg.alert('Failure', 'Form fields may not be submitted with invalid values');
            break;
        case Ext.form.Action.CONNECT_FAILURE:
            Ext.Msg.alert('Ошибка', 'Ajax communication failed');
            break;
        case Ext.form.Action.SERVER_INVALID:
            if (action.result != undefined) {
                if (action.result.msg != undefined) {
                    Ext.Msg.alert('Ошибка', action.result.msg);
                } else {
                    if (action.result.errors != undefined) {
                        Ext.Msg.alert('Ошибка', action.result.errors[0].msg);
                    }
                }
            } else {
                Ext.Msg.alert('Ошибка', 'Server failed');
            }
            break;
    }
}");
            }

            return tb;
        }

        private string GetSaveHandler(bool needClose)
        {
            if (!editable && canAddCommentsAndDocs)
            {
                return "SaveAppFromOGV(null, {0});".FormatWith(needClose ? "parent.MdiTab.getComponent({0})".FormatWith(estReqTabId) : "null");
            }

            return @"    
                // проверить корректность   
                if (!DetailsForm{0}.isValid()) {{
                        Ext.Msg.show({{
                            title: 'Анализ и планирование',
                            msg: 'Не все реквизиты заполнены корректно.',
                            buttons: Ext.MessageBox.OK,
                            multiline: false,
                            animEl: 'ApplicPanel',
                            icon: Ext.MessageBox.WARNING
                        }});
                    return;
                }}
                SaveAppFromOGV(null, {1});"
                    .FormatWith(detailsViewModel.ID, needClose ? "parent.MdiTab.getComponent({0})".FormatWith(estReqTabId) : "null");
        }

        private IEnumerable<Component> CreateRequestTabPanel(ViewPage page)
        {
            // Панель закладок
            var tabs = new TabPanel
                           {
                               ID = "RequestTabPanel",
                               Border = false,
                               ActiveTabIndex = 0
                           };

            // реквизиты заявки от ОГВ
            estimateDetailView = new EstimateDetailView(extension)
                                     {
                                         DetailsViewModel = detailsViewModel, 
                                         Executor = Executor, 
                                         Editable = editable,
                                         CanAddCommentsAndDocs = canAddCommentsAndDocs
                                     };
            tabs.Add(estimateDetailView.Build(page));

            // Исходные данные (Заявки от налогоплательщиков)
            estimateInputDataView = new EstimateInputDataView(detailsViewModel.CategoryID, detailsViewModel.PeriodId)
                { 
                    Editable = editable
                };
            tabs.Add(estimateInputDataView.Build(page));

            // Итоговые данные
            estimateResultDataView = new EstimateResultDataView(detailsViewModel.ID, detailsViewModel.PeriodId / 10000);
            tabs.Add(estimateResultDataView.Build(page));

            // если пользователь - ДФ
            if (responsOIV.Role.Equals("ДФ"))
            {
                // Оценка
                estimateView = new EstimateView(extension, normRepository, requestsRepository, detailsViewModel.ID, categoryShortName)
                    {
                        Editable = (detailsViewModel.StateOGV == 2) && extension.ResponsOIV.Role.Equals("ДФ") && reqThisPeriod
                    };

                tabs.Add(estimateView.Build(page));
            }

            if (activeTab.IsNotNullOrEmpty())
            {
                tabs.SetActiveTab(activeTab);
            }

            var form = new FormPanel
            {
                ID = "RequestsEstimateFrom",
                Border = false,
                CssClass = "x-window-mc",
                BodyCssClass = "x-window-mc",
                Layout = "RowLayout"
            };

            return new List<Component> { form, tabs };
        }
    }
}
