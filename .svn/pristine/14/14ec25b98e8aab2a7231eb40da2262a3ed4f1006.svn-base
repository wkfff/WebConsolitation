using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.FO41.Presentation.Controls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO
{
    public class HMAOReqListForOGVView : GridPanel
    {
        private readonly IFO41Extension extension;
        private readonly string gridId = "reqestsListGrid";
        private readonly int categoryId;
        private readonly int periodId;
        private readonly int taxTypeId;
        private readonly string titleTemplate;

        /// <summary>
        /// Функция: как отображать статус заявки
        /// </summary>
        private const string RendererFn =
            @"function (v, p, r, rowIndex, colIndex, ds) { 
                var tpl = '<img title=\'{1}\' src=\'{0}\' width=\'16\' height=\'16\' />';
                return String.format(tpl, 
                    Ext.getCmp(String.format('UrlIconStatus{0}', r.data.StateID)).getValue(), r.data.StateName);
            }";

        public HMAOReqListForOGVView(
            IFO41Extension extension,
            int categoryId, 
            int periodId, 
            int taxTypeId, 
            string titleTemplate)
        {
            this.extension = extension;
            this.categoryId = categoryId;
            this.periodId = periodId;
            this.taxTypeId = taxTypeId;
            this.titleTemplate = titleTemplate;
            gridId = "reqestsListGrid{0}".FormatWith(categoryId);
        }

        public void Build(ViewPage page)
        {
            var fields = new List<string>
                             {
                                 "ID", 
                                 "RequestDate", 
                                 "TaxTypeName", 
                                 "TaxTypeID", 
                                 "CategoryShort", 
                                 "StateName", 
                                 "StateID",
                                 "OrgName"
                             };
            var storeDefects = CreateGridStore(
                "requestStore{0}".FormatWith(categoryId), 
                "/FO41HMAORequestsList/ReadForOGV?categoryId={0}&periodId={1}".FormatWith(categoryId, periodId), 
                fields);
            page.Controls.Add(storeDefects);
            InitGrid(storeDefects.ID, page);
        }

        private Store CreateGridStore(string id, string url, IEnumerable<string> fields)
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

            ds.Listeners.Load.AddAfter("taxCnt{0}.setValue(String.format('{1}', {2}.data.items.length));"
.FormatWith(categoryId, titleTemplate, ds.ID));

            return ds;
        }

        private void InitGrid(string storeId, ViewPage page)
        {
            ID = gridId;
            LoadMask.ShowMask = true;
            SaveMask.ShowMask = true;
            StoreID = storeId;
            EnableColumnMove = false;
            ColumnLines = true;
            AutoExpandColumn = "OrgName";
            AutoExpandMin = 150;

            AutoScroll = true;
            
            var command = new GridCommand { Icon = Icon.ApplicationGo, CommandName = "OpenRequest" };
            command.ToolTip.Text = "Открыть заявку";
            var columnOpen = new CommandColumn
                                 {
                                     Header = string.Empty, 
                                     Width = 30, 
                                     ButtonAlign = Alignment.Center
                                 };
            columnOpen.Commands.Add(command);
            ColumnModel.Columns.Add(columnOpen);

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

            ColumnModel.Columns.Add(columnState);

            ColumnModel.AddColumn("ID", "Номер заявки", DataAttributeTypes.dtString).SetWidth(60);
            ColumnModel.AddColumn("RequestDate", "Дата создания заявки", DataAttributeTypes.dtString);
            ColumnModel.AddColumn("OrgName", "Наименование налогоплательщика", DataAttributeTypes.dtString);
            
            var excelHandler =
                @"var s = {0}.getSelectionModel().selection;
                                    
                                        Ext.net.DirectMethod.request({{
                                            url: '/FO41Export/ReportTaxpayer',
                                            isUpload: true,
                                            formProxyArg: '{1}',
                                            cleanRequest: true,
                                            params: {{
                                                id: s.record.data.ID, name: s.record.data.OrgName
                                            }},
                                            success:function (response, options) {{
                                                            }},
                                                            failure: function (response, options) {{
                                                            }}
                                        }});
                                    ".FormatWith(gridId, "HMAOTaxForm{0}".FormatWith(taxTypeId));

            Listeners.Command.Handler = @"
            if (command == 'OpenRequest') {
                parent.MdiTab.addTabTo(Category" + categoryId + @"TabPanel, 
                    { 
                        id: 'req_' + record.id,
                        title: 'Просмотр заявки', 
                        url: '/FO41HMAORequestsList/HMAORequestView?applicationId=' + record.id + 
                            '&taxTypeId=' + record.data.TaxTypeID, 
                        icon: 'icon-report'
                    });
            }";

            this.AddColumnsWrapStylesToPage(page);

            var createBtn = this.Toolbar().AddIconButton(
                "{0}AddBtn".FormatWith(ID), 
                Icon.Add, 
                "Создать заявку",
                GetCreateRequestHandler());

            createBtn.Hidden = !extension.IsReqLastPeriod(periodId);

            if (extension.IsReqLastPeriod(periodId) &&
        (extension.UserGroup == FO41Extension.GroupTaxpayer || extension.UserGroup == FO41Extension.GroupOGV))
            {
                var handler = @"
                var s = {0}.getSelectionModel().selection;".FormatWith(gridId) +
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
                                        {0}.reload();".FormatWith(gridId) +
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
                this.Toolbar().Add(new Button
                {
                    ID = "deleteReqBt_{0}".FormatWith(gridId),
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

            this.AddRefreshButton();

            this.Toolbar().AddIconButton(
                "{0}exportReportBtn".FormatWith(ID), 
                Icon.PageExcel, 
                "Выгрузка в Excel", 
                excelHandler);

            AddChangeStateButton(
                2,
                @"Отправить на рассмотрение",
                @"Вы действительно хотите отправить заявки по данной категории на рассмотрение?",
                Icon.Tick);

            AddChangeStateButton(
                3,
                @"Отправить на оценку",
                @"Вы действительно хотите отправить заявки по данной категории на оценку?",
                Icon.BookNext);

            AddChangeStateButton(
                4,
                @"Принять",
                @"Вы действительно хотите принять заявки по данной категории?",
                Icon.Accept);

            AddChangeStateButton(
                5,
                @"Отклонить",
                @"Вы действительно хотите отклонить заявки по данной категории?",
                Icon.Cancel);

            this.Toolbar().Add(new DisplayField
            {
                Text = @"Срок подачи заявок по выбранному периоду истек",
                StyleSpec = "color: red; font-size: 12px;",
                Hidden = !createBtn.Hidden
            });
        }

        private void AddChangeStateButton(int newState, string name, string askQuestion, Icon icon)
        {
            var buttonId = "changeStateTo_{1}_{0}".FormatWith(categoryId, newState);

            var handler = @"
                    Ext.Ajax.request({{
                        url: '/FO41HMAORequestsList/ChangeState?categoryId={0}&periodId={1}&newState={2}',
                        success: function (response, options) {{
                            {3}.reload();
                            var fi = response.responseText.indexOf('message:') + 9;
                            var li = response.responseText.lastIndexOf('{5}')
                            var msg = response.responseText.substring(li, fi);
                            Ext.net.Notification.show({{
                                iconCls    : 'icon-information', 
                                html       : msg, 
                                title      : 'Анализ и планирование', 
                                autoScroll : true,
                                hideDelay  : 2500
                            }});
                        }},
                        failure: function (response, options) {{
                        }}
                    }});"
                .FormatWith(categoryId, periodId, newState, gridId, buttonId, '"');

            this.Toolbar().Add(new Button
                    {
                        ID = buttonId,
                        ToolTip = name,
                        Icon = icon,
                        CommandName = buttonId,
                        Listeners =
                            {
                                Click =
                                    {
                                        Handler = RequestsListToolbar.CreateAskHandler(askQuestion, handler)
                                    }
                            },
                        Hidden = !extension.IsReqLastPeriod(periodId) 
                    });
        }

        private string GetCreateRequestHandler()
        {
            var addCreateReqTabScript =
                @"parent.MdiTab.addTabTo(Category" + categoryId + @"TabPanel, 
                        {{ 
                            id: 'req_new_{1}_{2}',
                            title: 'Новая заявка', 
                            url: '/FO41HMAORequestsList/HMAORequestView?applicationId=-1&taxTypeId={0}&categoryId={1}&periodId={2}'
                        }});"
                    .FormatWith(taxTypeId, categoryId, periodId);

            return @"
            Ext.Ajax.request({
                url: '/FO41HMAORequestsList/CheckCanCreateRequestHMAO?periodID=" + periodId + @"&categoryId=" + categoryId + @"',
                success: function (response, options) {
                    if (response.responseText.indexOf('success:true') > -1) {
                        " + addCreateReqTabScript + @"
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
