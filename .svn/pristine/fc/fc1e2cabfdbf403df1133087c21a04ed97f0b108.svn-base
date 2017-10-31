using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.DocumentsRegisterModel;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.Documents;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public sealed class DocumentsView : View
    {
        private const string GridID = "Documents";
        private const string Scope = "E86n.View.Documents.Grid";
        private readonly IAuthService auth;
        private readonly IDocuments documents;
        private readonly DocumentsRegisterViewModel viewModel = new DocumentsRegisterViewModel();

        public DocumentsView()
        {
            auth = Resolver.Get<IAuthService>();
            documents = Resolver.Get<IDocuments>();
        }

        private ViewPage Page { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;
            ResourceManager.GetInstance(page).RegisterClientStyleBlock("DocumentsStyle", Resource.DocumentsViewStyle);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("DocumentsView.js", Resource.DocumentsView);
            ResourceManager.GetInstance(page).RegisterClientScriptBlock("StateToolBarControl.js", Resource.StateToolBarControl);

            ResourceManager.GetInstance(page).Listeners.DocumentReady.Handler += @"
                Ext.getCmp('Documents').view.hmenu.on('show', function() {

                                    var filters =  Ext.getCmp('Documents').view.hmenu.items.get('filters');
                                    if (!filters.checked && filters.menu.type == 'list')
                                        filters.menu.items.each(function (e) {
                                            e.setChecked(false);
                                        });

                });
                ";

            var restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.PUT;
            restActions.Destroy = HttpMethod.DELETE;

            Page.Controls.Add(
                UpLoadGmuBtnControl.CreateUploadGmuWindow(
                    string.Concat(@"#{wndUploadGMU}.hide(); dsDocuments.reload();", UpLoadGmuBtnControl.SucHndler), 
                    string.Concat(@"#{wndUploadGMU}.hide();", UpLoadGmuBtnControl.FailHndler),
                    UiBuilders.GetUrl<UpLoadGmuController>("Index"),
                    "window.wndUploadGMU.docs"));

            var store = new MultiGroupingStore
                {
                    ID = string.Concat("ds", GridID), 
                    Restful = true, 
                    AutoLoad = true, 
                };
            store
                .SetRestController(UiBuilders.GetControllerID<DocumentsController>())
                .SetJsonReader()
                .AddFieldsByClass(viewModel)
                .SortInfo.Field = viewModel.NameOf(() => viewModel.ID);

            Page.Controls.Add(store);
            store.BaseParams.Add(new Parameter("limit", "pageSizeDocs.getValue()", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("start", "0", ParameterMode.Raw));

            var panel = new MultiGroupingPanel
                {
                    StateID = "DocumentsRegistryGrid", 
                    ID = GridID, 
                    StoreID = store.ID, 
                    LoadMask = { ShowMask = true }, 
                    BottomBar =
                        {
                            new PagingToolbar
                                {
                                    ID = "paggingDocs", 
                                    StoreID = store.ID, 
                                    PageSize = 200, 
                                    Items =
                                        {
                                            new NumberField
                                                {
                                                    ID = "pageSizeDocs", 
                                                    FieldLabel = @"Документов на страницу", 
                                                    LabelWidth = 150, 
                                                    Width = 200, 
                                                    Number = 200, 
                                                    Listeners =
                                                        {
                                                            Change =
                                                                {
                                                                    Handler = "#{paggingDocs}.pageSize = parseInt(this.getValue());"
                                                                }
                                                        }
                                                }, 
                                        }, 
                                }, 
                        }, 
                    Plugins =
                        {
                            new GridFilters
                                {
                                    Local = false, 
                                    Filters =
                                        {
                                            new NumericFilter { DataIndex = viewModel.NameOf(() => viewModel.ID) }, 
                                            new BooleanFilter { DataIndex = viewModel.NameOf(() => viewModel.Closed) }, 
                                            new BooleanFilter { DataIndex = viewModel.NameOf(() => viewModel.ClosedOrg) }, 
                                            new ListFilter { DataIndex = viewModel.NameOf(() => viewModel.State), Options = documents.GetStates(), }, 
                                            new ListFilter { DataIndex = viewModel.NameOf(() => viewModel.Type), Options = documents.GetDocTypes(), }, 
                                            new ListFilter { DataIndex = viewModel.NameOf(() => viewModel.Year), Options = documents.GetYears() }, 
                                            new StringFilter { DataIndex = viewModel.NameOf(() => viewModel.Note), EmptyText = "Примечание", }, 
                                            new StringFilter { DataIndex = viewModel.NameOf(() => viewModel.StructureName), EmptyText = "Полное или сокращенное название, ИНН, КПП", }, 
                                            new StringFilter { DataIndex = viewModel.NameOf(() => viewModel.StructurePpo), EmptyText = "Название ППО", }, 
                                            new StringFilter { DataIndex = viewModel.NameOf(() => viewModel.StructureGrbs), EmptyText = "Название ГРБС, Код", }, 
                                            new DateFilter { DataIndex = viewModel.NameOf(() => viewModel.StructureCloseDate) }, 
                                        }, 
                                }, 
                        }, 
                };

            panel.ColumnModel.Columns.Add(
                new ImageCommandColumn
                    {
                        DataIndex = "Action", 
                        Width = 22, 
                        Commands =
                            {
                                new ImageCommand
                                    {
                                        Icon = Icon.PencilGo, 
                                        CommandName = "OpenDocument", 
                                        ToolTip = { Text = "Редактировать документ" }, 
                                    }, 
                                new ImageCommand
                                    {
                                        Icon = Icon.NoteEdit, 
                                        CommandName = "EditNote", 
                                        ToolTip = { Text = "Редактировать примечание" }, 
                                        Hidden = !auth.IsAdmin()
                                    }, 
                                new ImageCommand
                                    {
                                        Icon = Icon.UserGreen, 
                                        CommandName = "ExportDocument", 
                                        ToolTip = { Text = "Экспортировать документ" }, 
                                        Hidden = !auth.IsAdmin()
                                    }
                            }, 
                        PrepareCommand = { Fn = string.Concat(Scope, ".prepareCommand") }
                    });

            panel.ColumnModel.Columns.Add(
                new ImageCommandColumn
                    {
                        DataIndex = "Action", 
                        Width = 18, 
                        Commands =
                            {
                                new ImageCommand
                                    {
                                        Icon = Icon.LockOpen, 
                                        CommandName = "OpenDoc", 
                                        ToolTip = { Text = "Открыть документ" }, 
                                        Hidden = !auth.IsAdmin()
                                    }, 
                                new ImageCommand
                                    {
                                        Icon = Icon.Lock, 
                                        CommandName = "CloseDoc", 
                                        ToolTip = { Text = "Закрыть документ" }, 
                                        Hidden = !auth.IsAdmin()
                                    }, 
                                new ImageCommand
                                    {
                                        Icon = Icon.Delete, 
                                        CommandName = "DeleteDoc", 
                                        ToolTip = { Text = "Удалить документ" }, 
                                        Hidden = !auth.IsAdmin()
                                    }
                            }, 
                        PrepareCommand = { Fn = string.Concat(Scope, ".prepareCommand") }
                    });

            panel.ColumnModel.Columns.Add(
                new ImageCommandColumn
                    {
                        DataIndex = "Action", 
                        Width = 18, 
                        Commands =
                            {
                                new ImageCommand
                                    {
                                        Icon = Icon.Lock, 
                                        CommandName = "ClosedDocument", 
                                        ToolTip = { Text = "Признак закрытости документа" }, 
                                    }, 
                                new ImageCommand
                                    {
                                        Icon = Icon.Decline, 
                                        CommandName = "ClosedOrg", 
                                        ToolTip = { Text = "Признак закрытости учреждения" }, 
                                    }
                            }, 
                        PrepareCommand = { Fn = string.Concat(Scope, ".prepareCommand") }
                    });

            panel.ColumnModel.AddColumn(() => viewModel.ID, DataAttributeTypes.dtInteger)
                .SetWidth(20).SetHidden(true);

            panel.ColumnModel.AddColumn(() => viewModel.StructureName, DataAttributeTypes.dtString)
                .SetWidth(150);

            panel.ColumnModel.AddColumn(() => viewModel.StructureGrbs, DataAttributeTypes.dtString)
                .SetWidth(80).SetHidden(false);

            panel.ColumnModel.AddColumn(() => viewModel.StructurePpo, DataAttributeTypes.dtString)
                .SetWidth(80).SetHidden(false);

            panel.ColumnModel.AddColumn(() => viewModel.State, DataAttributeTypes.dtString)
                .SetWidth(50).SetHidden(false);

            panel.ColumnModel.AddColumn(() => viewModel.Type, DataAttributeTypes.dtString)
                .SetWidth(50);

            panel.ColumnModel.AddColumn(() => viewModel.Note, DataAttributeTypes.dtString)
                .SetHidden(true);

            panel.ColumnModel.AddColumn(() => viewModel.Year, DataAttributeTypes.dtInteger)
                .SetWidth(40).SetHidden(true);

            panel.ColumnModel.AddColumn(() => viewModel.StructureCloseDate, DataAttributeTypes.dtDateTime)
                .SetWidth(50).SetHidden(true);

            panel.Listeners.CellClick.Fn = string.Concat(Scope, ".actionHandler1");

            panel.AddColumnsWrapStylesToPage(page);
            panel.View.Add(
                new MultiGroupingView
                    {
                        GroupTextTpl = "{text}: {gvalue}({[values.rs.length]})", 
                        HideGroupedColumn = true, 
                        ForceFit = true, 
                        GetRowClass = { Fn = string.Concat(Scope, ".getRowClass") }, 
                    });

            return new List<Component>
                {
                    new Viewport
                        {
                            ID = "viewportMain", 
                            Items =
                                {
                                    new BorderLayout
                                        {
                                            North = { Items = { CreateTopPanel() } }, 
                                            Center = { Items = { panel }, Margins = { Bottom = 3, }, }, 
                                        }
                                }
                        }, 
                };
        }

        private Panel CreateTopPanel()
        {
            var toolbar = new Toolbar();

            var excelBtn = new Button
                {
                    ID = "excel", 
                    Icon = Icon.PageExcel, 
                    ToolTip = @"Выгрузить реестр документов в excel", 
                };
            excelBtn.DirectEvents.Click.Url = "/Reports/ImportDocumentsGrid";
            excelBtn.DirectEvents.Click.CleanRequest = true;
            excelBtn.DirectEvents.Click.Before = string.Concat(
                @"var grid = ",
                GridID,
                @".getState();if (grid.group && grid.group.length != 0){Ext.Msg.alert('Предупреждение', 'Невозможно выполнить операцию на сгруппированных данных');return false;}");
            excelBtn.DirectEvents.Click.IsUpload = true;
            excelBtn.DirectEvents.Click.FormID = "Form1";
            excelBtn.DirectEvents.Click.ExtraParams.Add(new Parameter("state", string.Concat(GridID, ".getState()"), ParameterMode.Raw));
            excelBtn.DirectEvents.Click.ExtraParams.Add(new Parameter("gridFilters", string.Format("{0}.filters.buildQuery({0}.filters.getFilterData())", GridID), ParameterMode.Raw));
            excelBtn.DirectEvents.Click.Failure = @"Ext.Msg.show({
                                                           title:'Ошибка',
                                                           msg: result.responseText,
                                                           buttons: Ext.Msg.OK,
                                                           icon: Ext.MessageBox.ERROR,
                                                           maxWidth: 1000
                                                        });";

            var filterNotClosed = new Button
                {
                    ID = "filterNotClosed", 
                    Icon = Icon.LockBreak, 
                    ToolTip = @"Отобрать открытые документы", 
                    EnableToggle = true, 
                };

            filterNotClosed.SetHandler("E86n.View.Documents.Grid.toggleFilter", Scope);

            var filterClosed = new Button
                {
                    ID = "filterClosed", 
                    Icon = Icon.Lock, 
                    ToolTip = @"Отобрать закрытые документы", 
                    EnableToggle = true, 
                };

            filterClosed.SetHandler("E86n.View.Documents.Grid.toggleFilter", Scope);

            var filterNotClosedOrg = new Button
                {
                    ID = "filterNotClosedOrg", 
                    Icon = Icon.Accept, 
                    ToolTip = @"Отобрать документы открытых учреждений", 
                    EnableToggle = true
                };

            filterNotClosedOrg.SetHandler("E86n.View.Documents.Grid.toggleFilter", Scope);
            filterNotClosedOrg.Listeners.BeforeRender.Handler = "E86n.View.Documents.Grid.InitView()";

            var filterClosedOrg = new Button
                {
                    ID = "filterClosedOrg", 
                    Icon = Icon.Decline, 
                    ToolTip = @"Отобрать документы закрытых учреждений", 
                    EnableToggle = true
                };

            filterClosedOrg.SetHandler("E86n.View.Documents.Grid.toggleFilter", Scope);

            toolbar.Add(filterNotClosed);
            toolbar.Add(filterClosed);
            toolbar.Add(filterNotClosedOrg);
            toolbar.Add(filterClosedOrg);

            toolbar.Add(excelBtn);
            toolbar.Add(new ReportControl().Build(Page));
            toolbar.Add(new AnalReportControl().Build(Page));
            toolbar.Add(new NewAnalReportControl().Build(Page));

            if (auth.IsAdmin())
            {
                var import = new UpLoadFileBtnControl
                    {
                        Id = "btnImport",
                        Name = "Импорт из XML",
                        Icon = Icon.DiskDownload,
                        Upload = true,
                        UploadController = UiBuilders.GetUrl<ImportsController>("ImportsFromGmuXml")
                    };

                toolbar.Add(import.Build(Page));
            }

            return new Panel
                {
                    ID = "topPanel", 
                    Height = 27, 
                    Border = false, 
                    TopBar = { toolbar }, 
                };
        }
    }
}
