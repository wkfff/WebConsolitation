using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controls
{
    public class RequestsListToolbar : Toolbar
    {
        private readonly IFO41Extension extension;

        private readonly AppFromOGVService requestsRepository;

        /// <summary>
        /// Тулбар для грида с заявками от налогоплательщиков (false) или от ОГВ (true)
        /// </summary>
        private readonly bool isEstimateList;

        /// <summary>
        /// Скрипт - диалоговое окно с вопросом
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
        /// Окончание идентификаторов компонентов
        /// </summary>
        private string ends;

        public RequestsListToolbar(IFO41Extension extension, bool isEstimateList)
        {
            this.extension = extension;
            this.isEstimateList = isEstimateList;
            requestsRepository = new AppFromOGVService();
        }

        /// <summary>
        /// Формирует скрипт для обработчика handler с диалоговым окном с вопросом msg
        /// </summary>
        /// <param name="msg">Текст для диалогового окна</param>
        /// <param name="handler">Скрипт обрабочика</param>
        /// <returns>Скрипт обрабочика с диалоговм окном</returns>
        public static string CreateAskHandler(string msg, string handler)
        {
            return AskChangeState.FormatWith(msg, handler);
        }

        public void Build(int categoryId, int periodId)
        {
            // окончание идентификаторов компонентов
            ends = categoryId > -1 ? "_{0}".FormatWith(categoryId) : string.Empty;
            if (extension.IsReqLastPeriod(periodId))
            {
                if (!isEstimateList && 
                    extension.IsReqLastPeriod(periodId) &&
                    (extension.UserGroup == FO41Extension.GroupTaxpayer || extension.UserGroup == FO41Extension.GroupOGV))
                {
                    var handler = @"
                    var s = gpRequests{0}.getSelectionModel().getSelections();".FormatWith(ends) +
                                                  @"
                    for (i = 0; i < s.length; i++) {
                        Ext.Ajax.request({
                            url: '/FO41Requests/RemoveRequest?requestId=' + s[i].get('ID'),
                            success: function (response, options) {
                                if (response.responseText.indexOf('success:true') > -1) {
                                    var fi = response.responseText.indexOf('message:') + 9;
                                    var li = response.responseText.lastIndexOf('" +
                                                  '"' +
                                                  @"')
                                    var msg = response.responseText.substring(li, fi);
                                    Ext.net.Notification.show({
                                        iconCls    : 'icon-information', 
                                        html       : msg, 
                                        title      : 'Анализ и планирование', 
                                        hideDelay  : 2500
                                    });" +
                                                  @"
                                    gpRequests{0}.reload();".FormatWith(ends) +
                                                  @"
                                }
                                else {
                                    var fi = response.responseText.indexOf('message:') + 9;
                                    var li = response.responseText.lastIndexOf('" +
                                                  '"' +
                                                  @"')
                                    var msg = response.responseText.substring(li, fi);
                                    Ext.net.Notification.show({
                                        iconCls    : 'icon-information', 
                                        html       : msg, 
                                        title      : 'Анализ и планирование', 
                                        hideDelay  : 2500
                                    });
                                }
                            },
                            failure: function (response, options) {
                            }
                        });
                    }";
                    Add(new Button
                            {
                                ID = "deleteReqBt{0}".FormatWith(ends),
                                ToolTip = @"Удалить заявку",
                                Icon = Icon.Delete,
                                CommandName = "DeleteRequest",
                                Listeners =
                                    {
                                        Click =
                                            {
                                                Handler = CreateAskHandler(@"Вы действительно хотите удалить выделенные заявки?", handler)
                                            }
                                    },
                            });
                }

                // для ОГВ доступкна кнопка подачи заявки на оценку
                if (extension.ResponsOIV != null && extension.ResponsOIV.Role.Equals("ОГВ") && !isEstimateList)
                {
                    var estReqId = requestsRepository.GetAppIdByCategory(categoryId, periodId);
                    var estReqTabId = extension.GetEstReqTabId(categoryId, periodId, estReqId);
                    Add(new Button
                            {
                                ID = "sendReqBt{0}".FormatWith(ends),
                                ToolTip = @"Подать заявку на оценку по данной категории",
                                Icon = Icon.BookNext,
                                CommandName = "RequestToEstimate",
                                Listeners =
                                    {
                                        Click =
                                            {
                                                Handler = @"
                parent.MdiTab.addTab({{ 
                    id: {3},
                    title: 'Заявка для оценки', 
                    url: '/FO41Estimate/ShowRequest?appFromOGVId={0}&categoryId={1}&periodId={2}', 
                    icon: 'icon-report'
                }});"
                                                    .FormatWith(
                                                        estReqId,
                                                        categoryId, 
                                                        periodId,
                                                        estReqTabId)
                                            }
                                    },
                            });
                }
            }

            Add(new Button
                    {
                        ID = "updateBt{0}".FormatWith(ends),
                        ToolTip = @"Обновить",
                        Icon = Icon.PageRefresh,
                        CommandName = "updateRequest",
                        Disabled = false,
                        Listeners = { Click = { Handler = "dsRequests{0}.reload();".FormatWith(ends) } }
                    });

            Add(new ToolbarSeparator());

            if (extension.IsReqLastPeriod(periodId))
            {
                // Кнопки перехода состояний
                CreateChangeStateButtons();
                
                Add(new ToolbarSeparator());
            }

            // Кнопки для фильтрации заявок
            if (extension.UserGroup != FO41Extension.GroupTaxpayer)
            {
                CreateFilterButtons();
                Add(new ToolbarSeparator());
            }

            Add(new Button
                    {
                        ID = "exportReportButton{0}".FormatWith(ends),
                        Icon = Icon.PageExcel,
                        ToolTip = @"Выгрузка в Excel",
                        Listeners = { Click = { Handler = GetExportHandler() } }
                    });
        }

        /// <summary>
        /// Формирует скрипт обработчика по созданию отчета
        /// </summary>
        /// <returns>Скрипт обработчика по созданию отчета</returns>
        private string GetExportHandler()
        {
            return isEstimateList

                       // для интерфейса "Список заявок на оценку"
                       ? @"
                                    var s = gpRequests{0}.getSelectionModel().getSelections();
                                    if (s.length == 1) {{
                                        var orgName = s[0].get('RefOGVName');
                                        Ext.net.DirectMethod.request({{
                                            url: '/FO41Export/ReportOGV',
                                            isUpload: true,
                                            formProxyArg: 'RequestsOGVListFrom',
                                            cleanRequest: true,
                                            params: {{
                                                id: s[0].get('ID'), name: orgName
                                            }},
                                            success:function (response, options) {{
                                                            }},
                                                            failure: function (response, options) {{
                                                            }}
                                        }});
                                    }}".FormatWith(ends)

                       // для интерфейса "Список заявок от налогоплательщика" 
                       : @"
                                    var s = gpRequests{0}.getSelectionModel().getSelections();
                                    if (s.length == 1) {{
                                        var orgName = s[0].get('RefOrgName');
                                        Ext.net.DirectMethod.request({{
                                            url: '/FO41Export/ReportTaxpayer',
                                            isUpload: true,
                                            formProxyArg: 'RequestsListFrom',
                                            cleanRequest: true,
                                            params: {{
                                                id: s[0].get('ID'), name: orgName
                                            }},
                                            success:function (response, options) {{
                                                            }},
                                                            failure: function (response, options) {{
                                                            }}
                                        }});
                                    }}".FormatWith(ends);
        }

        /// <summary>
        /// Создает кнопки для фильтрации заявок
        /// </summary>
        private void CreateFilterButtons()
        {
            Add(new Label { Text = @"Фильтры:" });
            Add(new Button
                    {
                        ID = "filterCreate{0}".FormatWith(ends),
                        ToolTip = @"Заявка создана",
                        Icon = Icon.UserEdit,
                        EnableToggle = true,
                        Pressed = true,
                        ToggleHandler = "toggleFilter{0}".FormatWith(ends)
                    });
            var buttonMagnify = new Button
                                    {
                                        ID = "filterMagnify{0}".FormatWith(ends),
                                        ToolTip =
                                            isEstimateList
                                                ? "Заявка на доработке у ОГВ"
                                                : "Заявка на рассмотрении у ОГВ",
                                        Icon = Icon.UserMagnify,
                                        EnableToggle = true,
                                        Pressed = true,
                                        ToggleHandler = "toggleFilter{0}".FormatWith(ends)
                                    };
            var buttonEstimate = new Button
                                     {
                                         ID = "filterEstimate{0}".FormatWith(ends),
                                         ToolTip = @"Заявка на оценке",
                                         Icon = Icon.UserTick,
                                         EnableToggle = true,
                                         Pressed = true,
                                         ToggleHandler = "toggleFilter{0}".FormatWith(ends)
                                     };
            if (isEstimateList)
            {
                Add(buttonEstimate);
                Add(buttonMagnify);
            }
            else
            {
                Add(buttonMagnify);
                Add(buttonEstimate);
            }

            Add(new Button
                    {
                        ID = "filterAccept{0}".FormatWith(ends),
                        ToolTip = @"Заявка принята",
                        Icon = Icon.Accept,
                        EnableToggle = true,
                        Pressed = true,
                        ToggleHandler = "toggleFilter{0}".FormatWith(ends)
                    });
            Add(new Button
                    {
                        ID = "filterReject{0}".FormatWith(ends),
                        ToolTip = @"Заявка отклонена",
                        Icon = Icon.Cancel,
                        EnableToggle = true,
                        Pressed = true,
                        ToggleHandler = "toggleFilter{0}".FormatWith(ends)
                    });
        }

        /// <summary>
        /// Создает кнопки для изменения состояния заявки
        /// </summary>
        private void CreateChangeStateButtons()
        {
            // если грид с заявками от налогоплательщиков
            if (!isEstimateList)
            {
                CreateButtonsReqList();
            }
            else
            {
                if (extension.ResponsOIV.Role.Equals("ОГВ"))
                {
                    // Создаем кнопки для интерфейса "Список заявок на оценку" для пользователя ОГВ
                    CreateButtonsEstReqListOGV();
                }
                else
                if (extension.ResponsOIV.Role.Equals("ДФ"))
                {
                    // Создаем кнопки для интерфейса "Список заявок на оценку" для пользователя ДФ
                    CreateButtonsEstReqListDF();
                }
            }
        }

        /// <summary>
        /// Создает кнопки для интерфейса "Список заявок на оценку" для пользователя ДФ
        /// </summary>
        private void CreateButtonsEstReqListDF()
        {
            var toReEditHandler = @"
                    for (i = 0; i < s.length; i++) {{
                        if (s[i].get('RefStateID') == 2) {{
                            s[i].set('RefStateID', 3);
                        }}
                    }}
                    dsRequests{0}.save();
                    toReEdit{0}.hide();
                    toAccept{0}.hide();
                    toReject{0}.hide();".FormatWith(ends);
            Add(new Button
                    {
                        ID = "toReEdit{0}".FormatWith(ends),
                        ToolTip = @"На доработку ОГВ",
                        Icon = Icon.PageBack,
                        CommandName = "toReEdit",
                        Listeners =
                            {
                                Click =
                                    {
                                        Handler = @"
    var s = gpRequests{0}.getSelectionModel().getSelections();
    if (s.length > 0 ) {{
        {1}
    }}".FormatWith(
        ends, 
        CreateAskHandler(@"Вы действительно хотите вернуть заявку по данной категории на доработку ОГВ?", toReEditHandler))
                                    }
                            },
                        Hidden = true
                    });

            var toAcceptHandler = @"
                    for (i = 0; i < s.length; i++) {{
                        if (s[i].get('RefStateID') == 2) {{
                            s[i].set('RefStateID', 4);
                        }}
                    }}
                    dsRequests{0}.save();
                    toReEdit{0}.hide();
                    toAccept{0}.hide();
                    toReject{0}.hide();".FormatWith(ends);

            Add(new Button
                    {
                        ID = "toAccept{0}".FormatWith(ends),
                        ToolTip = @"Принять",
                        Icon = Icon.Accept,
                        CommandName = "toAccept",
                        Listeners =
                            {
                                Click =
                                    {
                                        Handler = @"
    var s = gpRequests{0}.getSelectionModel().getSelections();
    if (s.length > 0 ) {{
        {1}
    }}"
    .FormatWith(
        ends, 
        CreateAskHandler(@"Вы действительно хотите утвердить заявку по данной категории?", toAcceptHandler))
                                    }
                            },
                        Hidden = true
                    });
            var toRejectHandler = @"
                    for (i = 0; i < s.length; i++) {{
                        if (s[i].get('RefStateID') == 2) {{
                            s[i].set('RefStateID', 5);
                        }}
                    }}
                    dsRequests{0}.save();
                    toReEdit{0}.hide();
                    toAccept{0}.hide();
                    toReject{0}.hide();".FormatWith(ends);

            Add(new Button
                    {
                        ID = "toReject{0}".FormatWith(ends),
                        ToolTip = @"Отклонить",
                        Icon = Icon.Cancel,
                        CommandName = "toReject",
                        Listeners =
                            {
                                Click =
                                    {
                                        Handler = @"
    var s = gpRequests{0}.getSelectionModel().getSelections();
    if (s.length > 0 ) {{
        {1}
    }}".FormatWith(
       ends, 
       CreateAskHandler(@"Вы действительно хотите отклонить заявку по данной категории?", toRejectHandler))
                                    }
                            },
                        Hidden = true
                    });
        }

        /// <summary>
        /// Создает кнопки для интерфейса "Список заявок на оценку" для пользователя ОГВ
        /// </summary>
        private void CreateButtonsEstReqListOGV()
        {
            var toEstimateHandler =
                @"
                    for (i = 0; i < s.length; i++) {{
                        if (s[i].get('RefStateID') == 3 || s[i].get('RefStateID') == 1) {{
                            s[i].set('RefStateID', 2);
                        }}
                    }}
                    dsRequests{0}.save();
                    toEstimate{0}.hide();"
                    .FormatWith(ends);
            Add(new Button
                    {
                        ID = "toEstimate{0}".FormatWith(ends),
                        ToolTip = @"Отправить на оценку",
                        Icon = Icon.Tick,
                        CommandName = "toEstimate",
                        Listeners =
                            {
                                Click =
                                    {
                                        Handler = @"
    var s = gpRequests{0}.getSelectionModel().getSelections();
    if (s.length > 0 ) {{
        {1}
    }}".FormatWith(
       ends, 
       CreateAskHandler(@"Вы действительно хотите отправить заявку на оценку?", toEstimateHandler))
                                    }
                            },
                        Hidden = true
                    });
        }

        /// <summary>
        /// Создает кнопки для интерфейса "Список заявок от налогоплательщиков"
        /// </summary>
        private void CreateButtonsReqList()
        {
            if (extension.ResponsOIV != null && extension.ResponsOIV.Role.Equals("ОГВ"))
            {
                var toEditHandler =
                    @"
                    for (i = 0; i < s.length; i++) {{
                        if (s[i].get('RefStateID') == 2) {{
                            s[i].set('RefStateID', 1);
                        }}
                    }}
                    dsRequests{0}.save();
                    toEdit{0}.hide();
                    toOGV{0}.show();"
                        .FormatWith(ends);
                Add(new Button
                        {
                            ID = "toEdit{0}".FormatWith(ends),
                            ToolTip = @"Вернуть на редактирование",
                            Icon = Icon.PageBack,
                            CommandName = "toOGVRequest",
                            Listeners =
                                {
                                    Click =
                                        {
                                            Handler =
                                                @"
    var s = gpRequests{0}.getSelectionModel().getSelections();
    if (s.length > 0 ) {{
        {1}
    }}"
    .FormatWith(
        ends,
        CreateAskHandler(@"Вы действительно хотите вернуть заявку на редактирование?", toEditHandler))
                                        }
                                },
                            Hidden = true
                        });
            }

            if ((extension.ResponsOIV != null && extension.ResponsOIV.Role.Equals("ОГВ")) || extension.ResponsOrg != null)
            {
            var toOGVHandler = @"
                    for (i = 0; i < s.length; i++) {{
                        if (s[i].get('RefStateID') == 1) {{
                            s[i].set('RefStateID', 2);
                        }}
                    }}
                    dsRequests{0}.save();
                    if (Ext.getCmp('toEdit{0}')) {{
                        toEdit{0}.show();
                    }}
                    toOGV{0}.hide();".FormatWith(ends);

                Add(new Button
                        {
                            ID = "toOGV{0}".FormatWith(ends),
                            ToolTip = @"Отправить на рассмотрение ОГВ",
                            Icon = Icon.Tick,
                            CommandName = "toEditRequest",
                            Listeners =
                                {
                                    Click =
                                        {
                                            Handler = @"
    var s = gpRequests{0}.getSelectionModel().getSelections();
    if (s.length > 0 ) {{
        {1}
    }}".FormatWith(
       ends, 
       CreateAskHandler(@"Вы действительно хотите отправить заявку на рассмотрение ОГВ?", toOGVHandler))
                                        }
                                },
                            Hidden = true
                        });
            }
        }
    }
}
