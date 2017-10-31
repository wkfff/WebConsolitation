using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.FO41.Presentation.Controls;
using Krista.FM.RIA.Extensions.FO41.Services;
using Krista.FM.ServerLibrary;
using Button = Ext.Net.Button;
using Control = System.Web.UI.Control;
using GridView = Ext.Net.GridView;
using Panel = Ext.Net.Panel;
using View = Krista.FM.RIA.Core.Gui.View;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views
{
    public class RequestsListView : View
    {
        private readonly CategoryTaxpayerService categoryRepository;

        private readonly IAppPrivilegeService requestsRepository;

        private readonly IFO41Extension extension;
        private readonly int periodId;

        /// <summary>
        /// Функция: как отображать статус заявки
        /// </summary>
        private const string RendererFn =
            @"function (v, p, r, rowIndex, colIndex, ds) { 
                var tpl = '<img title=\'{1}\' src=\'{0}\' width=\'16\' height=\'16\' />';
                return String.format(
                    tpl, 
                    Ext.getCmp(String.format('UrlIconStatus{0}', r.data.RefStateID)).getValue(), r.data.RefState);
            }";

        public RequestsListView(
            CategoryTaxpayerService categoryRepository, 
            IAppPrivilegeService requestsRepository,
            IFO41Extension extension,
            int periodId)
        {
            this.requestsRepository = requestsRepository;
            this.categoryRepository = categoryRepository;
            this.extension = extension;
            this.periodId = periodId;
        }

        public override List<Component> Build(ViewPage page)
        {
            var restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            page.Controls.Add(CreateCategoriesStore());

            return new List<Component>
            {
                new Viewport
                {
                    ID = "viewportRequestsList",
                    Items = 
                    {
                        new BorderLayout
                        {
                            Center = { Items = { CreateRequestsGridPanel(page) } }
                        }
                    }
                }
            };
        }

        private static Control CreateCategoriesStore()
        {
            var ds = new Store
                         {
                             ID = "dsCategories",
                             Restful = true,
                             ShowWarningOnFailure = false,
                             SkipIdForNewRecords = false,
                             RefreshAfterSaving = RefreshAfterSavingMode.None
                         };

            ds.SetRestController("FO41Categories")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Code")
                .AddField("Name")
                .AddField("ShortName")
                .AddField("CorrectIndex")
                .AddField("RowType");

            return ds;
        }

        private static RowExpander CreateRowExpander(string gridId)
        {
            var rowExpander = new RowExpander { ExpandOnDblClick = false };
            rowExpander.Template.Html = @"<div id=""row-{ID}"" style=""background-color:White;""></div>";

            rowExpander.DirectEvents.BeforeExpand.Url = "/FO41Requests/Expand";
            rowExpander.DirectEvents.BeforeExpand.CleanRequest = true;
            rowExpander.DirectEvents.BeforeExpand.IsUpload = false;
            rowExpander.DirectEvents.BeforeExpand.Before = "return !body.rendered;";
            rowExpander.DirectEvents.BeforeExpand.Success = "body.rendered=true;";
            rowExpander.DirectEvents.BeforeExpand.EventMask.ShowMask = true;
            rowExpander.DirectEvents.BeforeExpand.EventMask.Target = MaskTarget.CustomTarget;
            rowExpander.DirectEvents.BeforeExpand.EventMask.CustomTarget = gridId;
            rowExpander.DirectEvents.BeforeExpand.ExtraParams.Add(
                new Parameter("applicationId", "record.id", ParameterMode.Raw));

            return rowExpander;
        }

        private static string GetRequestText(int requestCnt)
        {
            var ost = requestCnt % 10;
            var reqText = (ost == 0) ? "заявок" : ((ost == 1) ? "заявка" : (ost < 5 ? "заявки" : "заявок"));
            if (requestCnt % 100 > 10 && requestCnt % 100 < 20)
            {
                reqText = "заявок";
            }

            return reqText;
        }

        private Control CreateRequestsStore(int categoryId, string categoryName, int periodId)
        {
            var ds = new Store
            {
                ID = "dsRequests_{0}".FormatWith(categoryId),
                Restful = true,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None
            };

            ds.SetRestController("FO41Requests")
                .SetJsonReader()
                .AddField("ID")
                .AddField("RowType")
                .AddField("Executor")
                .AddField("RequestDate")
                .AddField("RefCategory")
                .AddField("RefCategoryShort")
                .AddField("RefOrgName")
                .AddField("RefStateID")
                .AddField("CopiesCnt")
                .AddField("RefState");

            ds.BaseParams.Add(new Parameter("CategoryId", categoryId.ToString(), ParameterMode.Raw));
            ds.BaseParams.Add(new Parameter("periodId", periodId.ToString(), ParameterMode.Raw));
            ds.BaseParams.Add(new Parameter(
                "filter", 
                extension.UserGroup == FO41Extension.GroupTaxpayer ? "getTrueStateFilter_{0}()".FormatWith(categoryId) : "getStateFilter_{0}()".FormatWith(categoryId), 
                ParameterMode.Raw)); 
            
            ds.AddScript(
                @"
                function toggleFilter_{1}(button, state) {{
                    {0}.load();
                }}

                function getTrueStateFilter_{1}() {{
                    return [true, true, true, true, true];
                }}

                function getStateFilter_{1}() {{
                    var filter = [true, true, true, true, true];
                    filter[0] = filterCreate_{1}.pressed;
                    filter[1] = filterMagnify_{1}.pressed;
                    filter[2] = filterEstimate_{1}.pressed;
                    filter[3] = filterAccept_{1}.pressed;
                    filter[4] = filterReject_{1}.pressed;

                    return filter;
                }}
                ".FormatWith(ds.ID, categoryId));

            ds.Listeners.Load.AddAfter(@"
                var requestCnt = dsRequests_{0}.data.items.length;
                var ost = requestCnt % 10;
                var reqText = (ost == 0) ? 'заявок' : ((ost == 1) ? 'заявка' : (ost < 5 ? 'заявки' : 'заявок'));
                if (requestCnt % 100 > 10 && requestCnt % 100 < 20)
                    reqText = 'заявок';
                gpRequests_{0}.setTitle('{1} (' + requestCnt + ' ' + reqText + ')');"
                .FormatWith(categoryId, categoryName));

            return ds;
        }
        
        private IEnumerable<Component> CreateToolBar(int categoryId)
        {
            var addButton = new Button
            {
                ID = "addButton{0}".FormatWith(categoryId),
                Icon = Icon.Add,
                ToolTip = @"Создать заявку от налогоплательщика",
                Text = @"Создать заявку от налогоплательщика",
                CommandName = "NewRequest",
                StyleSpec = "padding-left: 5px; padding-top: 5px;"
            };

            addButton.Listeners.Click.Handler = GetCreateRequestHandler(
                categoryId,
                "Новая заявка",
                "'/FO41Requests/ShowRequest?applicationId=-1&categoryId={0}'".FormatWith(categoryId));

            var infoPanel = new Panel
            {
                Border = false,
                Height = 30,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Layout = "ColumnLayout",
                Items =
                    {
                        addButton
                    }
            };

            return extension.IsReqLastPeriod(periodId) 
                ? new List<Component> { infoPanel }
                : new List<Component> { new DisplayField { Text = @"Срок подачи заявок по выбранному периоду истек", StyleSpec = "color: red; font-size: 12px;" } };
        }

        private IEnumerable<Component> CreateRequestsGridPanel(ViewPage page)
        {
            var form = new FormPanel
                                 {
                                     ID = "RequestsListFrom",
                                     Border = false,
                                     CssClass = "x-window-mc",
                                     BodyCssClass = "x-window-mc",
                                     Layout = "RowLayout"
                                 };

            var categoriesTabPanel = new TabPanel
                                            {
                                                ID = "reqByCatTabPanel", 
                                                Border = false, 
                                                ActiveTabIndex = 0, 
                                                HeaderAsText = extension.UserGroup == FO41Extension.GroupTaxpayer
                                            };

            if (extension.UserGroup != FO41Extension.GroupTaxpayer)
            {
                var list = extension.ResponsOIV == null
                ? null
                : (extension.ResponsOIV.Role.Equals("ДФ") 
                    ? categoryRepository.FindAll()
                    : categoryRepository.FindAll().Where(x => x.RefOGV.ID == extension.ResponsOIV.ID));

                if (list != null)
                {
                    CreateTabRequestsByCategory(list, page, categoriesTabPanel);
                }
            }

            return new List<Component> 
            { 
                new Hidden { ID = "UrlIconStatus1", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserEdit) },
                new Hidden { ID = "UrlIconStatus2", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserMagnify) },
                new Hidden { ID = "UrlIconStatus3", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserTick) },
                new Hidden { ID = "UrlIconStatus4", Value = ResourceManager.GetInstance().GetIconUrl(Icon.Accept) },
                new Hidden { ID = "UrlIconStatus5", Value = ResourceManager.GetInstance().GetIconUrl(Icon.Cancel) },
                form, 
                extension.UserGroup == FO41Extension.GroupTaxpayer
                    ? (Component)CreateRequestsGridByTaxPayer(page)
                    : categoriesTabPanel 
            };
        }

        private void CreateTabRequestsByCategory(IEnumerable<D_Org_CategoryTaxpayer> list, ViewPage page, TabPanel categoriesTabPanel)
        {
            foreach (var orgCategoryTaxpayer in list.Where(dOrgCategoryTaxpayer => dOrgCategoryTaxpayer.ID > -1))
            {
                var store = CreateRequestsStore(orgCategoryTaxpayer.ID, orgCategoryTaxpayer.Name, periodId);
                page.Controls.Add(store);
                var gp = GetGridByCategory(page, orgCategoryTaxpayer.ID, orgCategoryTaxpayer.Name, store.ID);
                var title = orgCategoryTaxpayer.ShortName ?? orgCategoryTaxpayer.Name;
                var panel = new Panel
                                {
                                    Border = false,
                                    Height = 30,
                                    Title = title,
                                    CssClass = "x-window-mc",
                                    Items =
                                        {
                                            new BorderLayout
                                                {
                                                    North = { Items = { CreateToolBar(orgCategoryTaxpayer.ID) } },
                                                    Center = { Items = { gp } }
                                                }
                                        }
                                };
                panel.Attributes.Add("categoryId", orgCategoryTaxpayer.ID.ToString());
                panel.Attributes.Add("categoryName", orgCategoryTaxpayer.Name);
                    
                categoriesTabPanel.Items.Add(panel);
            }
        }

        private Panel CreateRequestsGridByTaxPayer(ViewPage page)
        {
            var store = CreateRequestsStore(0, string.Empty, periodId);
            page.Controls.Add(store);
            var gp = GetGridByCategory(page, 0, string.Empty, store.ID);
            var title = string.Empty;
            var panel = new Panel
            {
                Border = false,
                Height = 30,
                Title = title,
                CssClass = "x-window-mc",
                Items =
                                        {
                                            new BorderLayout
                                                {
                                                    North = { Items = { CreateToolBar(0) } },
                                                    Center = { Items = { gp } }
                                                }
                                        }
            };
            panel.Attributes.Add("categoryId", 0.ToString());
            panel.Attributes.Add("categoryName", string.Empty);

            return panel;
        }

        private GridPanel GetGridByCategory(ViewPage page, int categoryId, string categoryName, string storeId)
        {
            var curPeriodId = extension.CurrentYearUNV.ID;
            var cnt = extension.UserGroup == FO41Extension.GroupTaxpayer
                          ? requestsRepository.FindAll().Count(x =>
                                                               x.RefOrgPrivilege.ID == extension.ResponsOrg.ID &&
                                                               x.RefYearDayUNV.ID == curPeriodId)
                          : requestsRepository.FindAll().Count(x =>
                                                               x.RefOrgCategory.ID == categoryId &&
                                                               x.RefYearDayUNV.ID == curPeriodId);
            
            var gp = new GridPanel
                         {
                             ID = "gpRequests_{0}".FormatWith(categoryId),
                             Icon = Icon.Table,
                             Closable = false,
                             Frame = true,
                             StoreID = storeId,
                             SelectionModel = { new RowSelectionModel() },
                             Title = "{0} ({1} {2})".FormatWith(categoryName, cnt, GetRequestText(cnt)),
                             View = { new GridView() },
                         };

            var topBar = new RequestsListToolbar(extension, false);
            topBar.Build(categoryId, curPeriodId);
            gp.TopBar.Add(topBar);

            var command = new GridCommand { Icon = Icon.ApplicationGo, CommandName = "OpenRequest" };
            command.ToolTip.Text = "Открыть заявку"; 
            var column = new CommandColumn { Header = string.Empty, Width = 30, ButtonAlign = Alignment.Center };
            column.Commands.Add(command);

            gp.ColumnModel.Columns.Add(column);

            var columnState = new Column
                                  {
                                      ColumnID = "RefState",
                                      Header = string.Empty,
                                      Width = 40,
                                      Hideable = false,
                                      Renderer =
                                          {
                                              Fn = RendererFn
                                          }
                                  };
            
            gp.ColumnModel.Columns.Add(columnState);
            
            gp.ColumnModel.AddColumn("ID", "Номер заявки", DataAttributeTypes.dtString).SetWidth(90);
            if (extension.UserGroup == FO41Extension.GroupTaxpayer)
            {
                gp.ColumnModel
                    .AddColumn("RefCategoryShort", "Наименование категории", DataAttributeTypes.dtString)
                    .SetWidth(200);
            }
            else 
            {
                gp.ColumnModel
                    .AddColumn("RefOrgName", "Наименование налогоплательщика", DataAttributeTypes.dtString)
                    .SetWidth(200);
            }

            gp.ColumnModel.AddColumn("RequestDate", "Дата создания заявки", DataAttributeTypes.dtString).SetWidth(120);
            gp.ColumnModel.AddColumn("CopiesCnt", "Число копий", DataAttributeTypes.dtString).SetWidth(80);

            if (extension.IsReqLastPeriod(curPeriodId))
            {
                var commandCopy = new GridCommand { Icon = Icon.PageCopy, CommandName = "CopyRequest" };
                commandCopy.ToolTip.Text = "Дублировать заявку по другой категории";
                var columnCopy = new CommandColumn
                                     {
                                         Header = "Скопировать заявку",
                                         Tooltip = "Дублировать заявку по другой категории",
                                         Width = 120,
                                         ButtonAlign = Alignment.Center
                                     };
                columnCopy.Commands.Add(commandCopy);
                gp.ColumnModel.Columns.Add(columnCopy);
            }

            gp.AutoExpandColumn = extension.UserGroup == FO41Extension.GroupTaxpayer 
                ? "RefCategoryShort" 
                : "RefOrgName";
            if (extension.IsReqLastPeriod(curPeriodId) && ((extension.ResponsOIV != null && extension.ResponsOIV.Role.Equals("ОГВ")) || extension.ResponsOrg != null))
            {
                gp.Listeners.RowClick.Handler = @"
                // если в состоянии создана - доступна кнопка - на рассмотрение
                {0}.hidden = !(this.store.getAt(rowIndex).get('RefStateID') == 1);
                if ({0}.hidden) {0}.hide();
                else {0}.show();
                // если в состоянии на рассмотрение - доступна кнопка - вернуть на редактирование
                var toEditCmp = Ext.getCmp('{1}');
                if (toEditCmp != null && toEditCmp != undefined) {{
                    {1}.hidden = !(this.store.getAt(rowIndex).get('RefStateID') == 2);
                    if ({1}.hidden) 
                        {1}.hide();
                    else 
                        {1}.show();
                }}"
                    .FormatWith("toOGV_{0}".FormatWith(categoryId), "toEdit_{0}".FormatWith(categoryId));

                gp.Listeners.KeyPress.AddAfter(@"
                // если в состоянии создана - доступна кнопка - на рассмотрение
                if(this.selModel.getSelected()){{
                    var k = e.charCode || e.keyCode;
                    var rowIndex = this.selModel.last;
                    if (k == e.PAGE_UP && rowInd > 0)
                        rowIndex--;
                    if (k == e.PAGE_DOWN && rowInd < this.store.data.items.length - 1)
                        rowIndex++;
                    {0}
                }}"
                    .FormatWith(gp.Listeners.RowClick.Handler));
            }

            // на открытие существующей заявки
            gp.Listeners.Command.Handler = @"
            if (command == 'OpenRequest') {{
                parent.MdiTab.addTab({{ 
                    id: 'req_' + record.id,
                    title: 'Редактирование заявки', 
                    url: '/FO41Requests/ShowRequest?applicationId=' + record.id, 
                    icon: 'icon-report'}});
            }}
            else{{
                if (command == 'CopyRequest') {{
                    parent.MdiTab.addTab({{
                            title: '{0}',
                            url: {1},
                            icon: 'icon-report'
                        }});
                }}    
            }}".FormatWith("Создание копии заявки", "'/FO41Requests/ShowRequest?applicationId=' + record.id + '&isCopy=true'");

            gp.AddColumnsWrapStylesToPage(page);

            // Если есть копии - в расширении строки выводить информацию о них
            gp.Plugins.Add(CreateRowExpander(gp.ID));

            return gp;
        }

        private string GetCreateRequestHandler(int categoryId, string title, string url)
        {
            return @"
            Ext.Ajax.request({
                url: '/FO41Requests/CheckCanCreateRequest?categoryId=' + " + categoryId + @",
                success: function (response, options) {
                    if (response.responseText.indexOf('success:true') > -1)
                        parent.MdiTab.addTab({
                            id: 'req_new_" + categoryId + "_" + periodId + @"',
                            title: '" + title + @"',
                            url: " + url + @",
                            icon: 'icon-report'
                        });
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
