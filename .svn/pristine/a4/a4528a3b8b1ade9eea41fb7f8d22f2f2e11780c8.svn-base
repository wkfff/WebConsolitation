using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.FO41.Presentation.Controls;
using Krista.FM.RIA.Extensions.FO41.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO
{
    public class HMAOReqListForTaxPayerView : View
    {
        private readonly IFO41Extension extension;

        private readonly string GridId = "reqestsListGrid";

        private readonly IAppPrivilegeService requestsRepository;

        private readonly int periodId;

        /// <summary>
        /// Функция: как отображать статус заявки
        /// </summary>
        private const string RendererFn =
            @"function (v, p, r, rowIndex, colIndex, ds) { 
                var tpl = '<img title=\'{1}\' src=\'{0}\' width=\'16\' height=\'16\' />';
                return String.format(
                    tpl, 
                    Ext.getCmp(String.format('UrlIconStatus{0}', r.data.StateID)).getValue(), r.data.StateName);
            }";

        public HMAOReqListForTaxPayerView(
            IFO41Extension extension,
            IAppPrivilegeService requestsRepository, 
            int periodId)
        {
            this.periodId = periodId;
            this.extension = extension;
            this.requestsRepository = requestsRepository;
        }

        public override List<Component> Build(ViewPage page)
        {
            var fields = new List<string>
                             {
                                 "ID", 
                                 "RequestDate", 
                                 "TaxTypeName", 
                                 "TaxTypeID", 
                                 "CategoryShort", 
                                 "StateName", 
                                 "StateID"
                             };
            var store = CreateGridStore(
                "requestStore",
                "/FO41HMAORequestsList/Read?taxPayerId={0}&periodId={1}".FormatWith(extension.ResponsOrg.ID, periodId),
                "/FO41HMAORequestsList/Save", 
                fields);
            page.Controls.Add(store);

            return new List<Component>
                {
                    new Viewport
                    {
                        ID = "viewportRequestsView",
                        Items = 
                        { 
                            new BorderLayout
                                      {
                                          Center = { Items = { CreateCenterPanel(store.ID, page) } },
                                      } 
                        }
                    }
                };  
        }

        private static Store CreateGridStore(string id, string url, string udateUrl, IEnumerable<string> fields)
        {
            var ds = new Store
            {
                ID = id,
                AutoSave = true
            };

            ds.SetHttpProxy(url)
                .SetJsonReader();
            foreach (var field in fields)
            {
                ds.AddField(field);
            }

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = udateUrl,
                Method = HttpMethod.POST
            });

            return ds;
        }
        
        /// <summary>
        /// Создание модального окна для формы с новой организацией
        /// </summary>
        /// <param name="gridId">Идентификатор грида, который нужно обновить после добавление организации</param>
        /// <param name="title">Заголовок модального окна</param>
        /// <returns>Модальное окно</returns>
        private Window GetBookWindow(string gridId, string title)
        {
            var win = new Window
            {
                ID = "{0}BookWindow".FormatWith(gridId),
                Width = 400,
                Height = 200,
                Title = title,
                Icon = Icon.ApplicationFormEdit,
                Hidden = false,
                Modal = true,
                Constrain = true
            };
            win.AutoLoad.Url = "/";
            win.AutoLoad.Mode = LoadMode.IFrame;
            win.AutoLoad.TriggerEvent = "show";
            win.AutoLoad.ReloadOnEvent = true;
            win.AutoLoad.ShowMask = true;
            win.AutoLoad.MaskMsg = @"Открытие формы с выбором налога для создания заявки...";
            win.AutoLoad.Params.Add(new Parameter("id", String.Empty, ParameterMode.Value));

            var buttonSave = new Button
            {
                ID = "btnOk",
                Text = @"Далее",
                Icon = Icon.Accept,
                Listeners =
                {
                    Click =
                    {
                        Handler = @"if (true) {{
                                        {0}.hide();
                                        btnOk.ownerCt.ownerCt.iframe.dom.contentWindow.parent.parent.MdiTab.addTab({{ 
                                            id : {3},
                                            title: 'Новая заявка', 
                                            url: '/FO41HMAORequestsList/HMAORequestView?applicationId=-1&taxTypeId=' + btnOk.ownerCt.ownerCt.iframe.dom.contentWindow.taxTypeCombo.value}});
                                    }}
                                    else {{
                                        Ext.net.Notification.show({{
                                            iconCls    : 'icon-information', 
                                            html       : 'Для продолжения необходимо выбрать налог.', 
                                            title      : 'Внимание', 
                                            hideDelay  : 2500
                                        }});
                                    }}".FormatWith(win.ID, gridId, '"', extension.GetReqTabId(0, periodId, -1))
                    }
                }
            };

            var buttonCancel = new Button
            {
                ID = "btnCancel",
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

            return win;
        }

        private GridPanel CreatePlanGrid(string storeId, ViewPage page, string addRequestHandler)
        {
            var gp = new GridPanel
            {
                ID = GridId,
                LoadMask = { ShowMask = true },
                SaveMask = { ShowMask = true },
                StoreID = storeId,
                EnableColumnMove = false,
                ColumnLines = true,
                AutoExpandColumn = "TaxTypeName",
                AutoExpandMin = 150,
                StyleSpec = "margin: 5px 0px 0px 5px; padding-right: 30px; padding-bottom: 5px;",
                AutoScroll = true
            };

            var command = new GridCommand { Icon = Icon.ApplicationGo, CommandName = "OpenRequest" };
            command.ToolTip.Text = "Открыть заявку";
            var columnOpen = new CommandColumn { Header = string.Empty, Width = 30, ButtonAlign = Alignment.Center };
            columnOpen.Commands.Add(command);
            gp.ColumnModel.Columns.Add(columnOpen);

            var columnState = new Column
            {
                ColumnID = "StateID",
                Header = string.Empty,
                Width = 40,
                Hideable = false,
                Renderer =
                {
                    Fn = RendererFn
                }
            };

            gp.ColumnModel.Columns.Add(columnState);

            gp.ColumnModel.AddColumn("ID", "Номер заявки", DataAttributeTypes.dtString).SetWidth(60);
            gp.ColumnModel.AddColumn("RequestDate", "Дата создания заявки", DataAttributeTypes.dtString);
            gp.ColumnModel.AddColumn("TaxTypeName", "Вид налога", DataAttributeTypes.dtString);
            gp.ColumnModel.AddColumn("CategoryShort", "Категория", DataAttributeTypes.dtString);

            var excelHandler =
                @"var s = {0}.getSelectionModel().selection;
                                    
                                        Ext.net.DirectMethod.request({{
                                            url: '/FO41Export/ReportTaxpayer',
                                            isUpload: true,
                                            formProxyArg: 'HMAOReqListForTaxPayer',
                                            cleanRequest: true,
                                            params: {{
                                                id: s.record.data.ID, name: '{1}'
                                            }},
                                            success:function (response, options) {{
                                                            }},
                                                            failure: function (response, options) {{
                                                            }}
                                        }});
                                    ".FormatWith(GridId, extension.ResponsOrg.Name);

            gp.Listeners.Command.Handler = @"
            if (command == 'OpenRequest') {
                parent.MdiTab.addTab({
                    id: 'req_' + record.id,
                    title: 'Просмотр заявки', 
                    url: '/FO41HMAORequestsList/HMAORequestView?applicationId=' + record.id + '&taxTypeId=' + record.data.TaxTypeID, icon: 'icon-report'
                });
            }";

            gp.AddColumnsWrapStylesToPage(page);

            var canAddRequest = !requestsRepository.FindAll().Any(x =>
                x.RefYearDayUNV.ID > DateTime.Today.Year * 10000 &&
                x.RefYearDayUNV.ID < (DateTime.Today.Year + 1) * 10000 &&
                x.RefStateOrg.ID >= 3);

            if (canAddRequest && extension.IsReqLastPeriod(periodId))
            {
                gp.Toolbar().AddIconButton(
                    "{0}AddBtn".FormatWith(gp.ID),
                    Icon.Add,
                    "Создать заявку",
                    GetCreateRequestHandler(addRequestHandler));
            }

            if (extension.IsReqLastPeriod(periodId) &&
                    (extension.UserGroup == FO41Extension.GroupTaxpayer || extension.UserGroup == FO41Extension.GroupOGV))
            {
                var handler = @"
                var s = {0}.getSelectionModel().selection;".FormatWith(GridId) +
                                          @"
                if  (s != null && s != undefined) {                       
                    Ext.Ajax.request({
                        url: '/FO41Requests/RemoveRequest?requestId=' + s.record.get('ID'),
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
                                {0}.reload();".FormatWith(GridId) +
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

                gp.Toolbar().Add(new Button
                {
                    ID = "deleteReqBt",
                    ToolTip = @"Удалить заявку",
                    Icon = Icon.Delete,
                    CommandName = "DeleteRequest",
                    Listeners =
                    {
                        Click =
                        {
                            Handler = RequestsListToolbar.CreateAskHandler(@"Вы действительно хотите удалить выделенные заявки?", handler)
                        }
                    },
                });
            }

            gp.AddRefreshButton();

            if (extension.IsReqLastPeriod(periodId))
            {
                AddToOGVButton(gp);
            }

            gp.Toolbar().AddIconButton("{0}exportReportBtn".FormatWith(gp.ID), Icon.PageExcel, "Выгрузка в Excel", excelHandler);

            gp.Toolbar().Add(new DisplayField
            {
                Text = @"Срок подачи заявок по выбранному периоду истек",
                StyleSpec = "color: red; font-size: 12px;",
                Hidden = extension.IsReqLastPeriod(periodId)
            });

            return gp;
        }

        private IEnumerable<Component> CreateCenterPanel(string storeId, ViewPage page)
        {
            var bookWindow = GetBookWindow(GridId, @"Выбор налога");
            var addRequestHandler = @"
                {0}
                {1}.autoLoad.url = '/FO41HMAORequestsList/HMAOChooseTaxView';
                {1}.show();"
                .FormatWith(bookWindow.ToScript(), bookWindow.ID, extension.ResponsOrg.ID);

             return new List<Component> 
            { 
                new Hidden { ID = "UrlIconStatus1", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserEdit) },
                new Hidden { ID = "UrlIconStatus2", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserMagnify) },
                new Hidden { ID = "UrlIconStatus3", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserTick) },
                new Hidden { ID = "UrlIconStatus4", Value = ResourceManager.GetInstance().GetIconUrl(Icon.Accept) },
                new Hidden { ID = "UrlIconStatus5", Value = ResourceManager.GetInstance().GetIconUrl(Icon.Cancel) },
                new FormPanel { ID = "HMAOReqListForTaxPayer" },
                CreatePlanGrid(storeId, page, addRequestHandler)
            };
        }

        private void AddToOGVButton(GridPanel gp)
        {
            var buttonId = "changeStateTo_2";

            var handler = @"
                    var record = {0}.getSelectionModel().selection.record;
                    if (record.data.StateID == 1) {{
                        record.set('StateID', 2);
                        record.set('StateName', 'На рассмотрении у ОГВ');
                        {0}.store.updateProxy.conn.url = '/FO41HMAORequestsList/Save?applicationId='+record.data.ID+
                            '&stateId=2';
                        {0}.store.save(); 
                    }}"
                .FormatWith(GridId);

            gp.Toolbar().Add(new Button
            {
                ID = buttonId,
                ToolTip = @"Отправить на рассмотрение ОГВ",
                Icon = Icon.Tick,
                CommandName = buttonId,
                Listeners =
                {
                    Click =
                    {
                        Handler = RequestsListToolbar.CreateAskHandler(@"Вы действительно хотите отправить заявку на рассмотрение ОГВ?", handler)
                    }
                },
                Hidden = false
            });
        }

        private string GetCreateRequestHandler(string addRequestHandler)
        {
            return @"
            Ext.Ajax.request({
                url: '/FO41HMAORequestsList/CheckCanCreateRequestHMAO?periodID=" + periodId + @"&categoryId=-1',
                success: function (response, options) {
                    if (response.responseText.indexOf('success:true') > -1) {
                        " + addRequestHandler + @"
                    }
                    else {
                        var fi = response.responseText.indexOf('message:') + 9;
                        var li = response.responseText.lastIndexOf('" + '"' + @"')
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
            })";
        }
    }
}
