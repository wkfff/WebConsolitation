using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Org3PricesAndTariffs
{
    public class PricesAndTariffsView : View
    {
        internal const string GridpanelID = "gpPrices";
        internal const string DatasourceID = "dsPrices";
        
        private readonly GoodType goodType;
        
        private readonly ITaskService taskService;
        private readonly IUserSessionState userSessionState;

        public PricesAndTariffsView(GoodType goodType, ITaskService taskService, IUserSessionState userSessionState)
        {
            this.taskService = taskService;
            this.userSessionState = userSessionState;
            this.goodType = goodType;
        }

        private int TaskId
        {
            get
            {
                return Convert.ToInt32(Params["taskId"]);
            }
        }

        public override List<Component> Build(ViewPage page)
        {
            var task = taskService.GetTaskViewModel(TaskId);
            var taskPermisionSettings = new TaskPermisionSettings(userSessionState, task);
            
            bool editable = taskPermisionSettings.CanEditTask && task.RefStatus == (int)TaskViewModel.TaskStatus.Edit;

            if (!ExtNet.IsAjaxRequest)
            {
                RegisterResources(page);
            }

            var view = new Viewport
            {
                ID = "viewportMain",
                Items = 
                {
                   new FitLayout { Items = { CreateGridPanel(editable, task) } }
                }
            };

            return new List<Component> { view, GetWindowOrganization(TaskId), GetWindowCopyReport(TaskId) };
        }

        private Toolbar GetToolbar(bool editable, TaskViewModel task)
        {
            var toolbar = new Toolbar();

            toolbar.Add(new TextField { ID = "taskId", Hidden = true, Value = task.ID });

            toolbar.Add(new Button
                            {
                                ID = "btnRefresh",
                                Icon = Icon.ArrowRefresh,
                                ToolTip = "Обновить",
                                Listeners = { Click = { Handler = "{0}.reload();".FormatWith(DatasourceID) } }
                            });

            toolbar.Add(new Button
            {
                ID = "btnChangeGrouping",
                Icon = Icon.ShapeRotateClockwise,
                ToolTip = "Сменить группировку",
                Listeners =
                    {
                        Click =
                            {
                                Handler = @"
var store = {0};
if (store.groupField == 'NameGood'){{
  store.groupBy('NameOrg',true);
}}else{{
  store.groupBy('NameGood',true);
}};".FormatWith(DatasourceID)
                            }
                    }
            });

            toolbar.Add(new Button
            {
                ID = "btnSave",
                Icon = Icon.Disk,
                Visible = editable,
                ToolTip = "Сохранить изменения",
                Listeners = { Click = { Handler = "saveReport({0});".FormatWith(DatasourceID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnAdd",
                Icon = Icon.Add,
                Visible = editable,
                ToolTip = "Добавить организацию",
                Listeners = { Click = { Handler = "addOrganization({0});".FormatWith(DatasourceID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnDelete",
                Icon = Icon.Delete,
                Visible = editable,
                ToolTip = "Удалить организацию",
                Listeners = { Click = { Handler = "deleteOrganization({0});".FormatWith(GridpanelID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnCopy",
                Icon = Icon.DatabaseCopy,
                Visible = editable,
                ToolTip = "Скопировать с другого отчета",
                Listeners = { Click = { Handler = "copyFromOtherReport();" } }
            });

            toolbar.Add(new Button
            {
                ID = "btnExcel",
                Icon = Icon.PageExcel,
                ToolTip = "Выгрузить в Excel",
                DirectEvents =
                    {
                        Click =
                            {
                                Url = "/Org3PriceAndTariffs/ExportToExcel",
                                ExtraParams = 
                                { 
                                    new Parameter("taskId", task.ID.ToString(), ParameterMode.Value),
                                    new Parameter("goodType", ((int)goodType).ToString(), ParameterMode.Value) 
                                },

                                CleanRequest = true,
                                IsUpload = true,
                                FormID = "Form1"
                            }
                    }
            });
            
            toolbar.Add(new ToolbarFill());
            var label = new ToolbarTextItem();
            label.Text = "На дату: {0}".FormatWith(task.BeginDate.ToString("dd.MM.yyyy"));
            label.StyleSpec = "font-weight:bold;";
            toolbar.Add(label);

            return toolbar;
        }

        private List<Component> CreateGridPanel(bool editable, TaskViewModel task)
        {
            var table = new GridPanel
            {
                ID = GridpanelID,
                Icon = Icon.DatabaseTable,
                Store = { GetStore(task) },
                TopBar = { GetToolbar(editable, task) },
                Layout = LayoutType.Fit.ToString(),
                Border = false,
                AutoScroll = true,
                TrackMouseOver = true,
                LoadMask = { ShowMask = true },
                ColumnModel =
                {
                    Columns = 
                    {
                        new Column { Hidden = true, DataIndex = "ID", Groupable = false },
                        new Column { Hidden = true, DataIndex = "Ordering", Groupable = false },
                        new Column { ColumnID = "NameOrg", Header = "Наименование предприятия торговли", DataIndex = "NameOrg", Width = 300, Editable = false, GroupName = "Предприятие" },
                        new Column { ColumnID = "NameGood", Header = "Наименование товара", DataIndex = "NameGood", Width = 300, Editable = false, GroupName = "Товар" },
                        new Column
                            {
                                Header = "Цена", 
                                DataIndex = "Price", 
                                Width = 100, 
                                Align = Alignment.Right, 
                                Editable = editable, 
                                Groupable = false, 
                                Editor =
                                    {
                                        new NumberField { AllowDecimals = true, DecimalPrecision = 2, AllowNegative = false, DecimalSeparator = "," }
                                    }
                            }
                    },
                },
                SelectionModel = 
                { 
                    new ExcelLikeSelectionModel
                        {
                            ID = "ExcelLikeSelectionModel1"
                        }
                },
                View = 
                { 
                    new GroupingView
                        {
                            ID = "GroupingView",
                            HideGroupedColumn = true, 
                            ForceFit = false,
                            EnableNoGroups = true,
                            StartCollapsed = false
                        }
                }
            };

            return new List<Component>
                       {
                           table
                       };
        }

        private Store GetStore(TaskViewModel task)
        {
            var store = new Store
            {
                ID = DatasourceID,
                AutoLoad = true,
                WarningOnDirty = true,
                DirtyWarningTitle = "Несохраненные изменения",
                DirtyWarningText = "Есть несохраненные изменения. Перезагрузить данные?",
                ShowWarningOnFailure = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                GroupField = "NameOrg",
            };
            store.Sort("Ordering", SortDirection.ASC);

            store.Proxy.Add(new HttpProxy { Url = "/Org3PriceAndTariffs/Load", Method = HttpMethod.GET });
            store.BaseParams.Add(new Parameter("taskId", task.ID.ToString(), ParameterMode.Value));
            store.BaseParams.Add(new Parameter("goodType", ((int)goodType).ToString(), ParameterMode.Value));

            store.UpdateProxy.Add(new HttpWriteProxy { Url = "/Org3PriceAndTariffs/Save", Method = HttpMethod.POST, Timeout = 500000 });
            store.WriteBaseParams.Add(new Parameter("taskId", task.ID.ToString(), ParameterMode.Value));
            
            var reader = new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                {
                    new RecordField("ID"),
                    new RecordField("Ordering"),
                    new RecordField("NameOrg"),
                    new RecordField("RefRegistrOrg"),
                    new RecordField("NameGood"),
                    new RecordField("RefGood"),
                    new RecordField("Price", RecordFieldType.Float),
                }
            };

            store.Reader.Add(reader);
            store.Listeners.Load.AddAfter("eval(store.reader.jsonData.extraParams); store.reader.jsonData.extraParams=null;");

            store.Listeners.Save.Handler = "Ext.net.Notification.show({iconCls: 'icon-information', html: 'Изменения сохранены.', title: 'Уведомление', hideDelay: 2500});";
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке', response.responseText);";
            store.Listeners.SaveException.Handler = @"
if (response.extraParams != undefined && response.extraParams.responseText != undefined) {
    Ext.Msg.alert('Ошибка', response.extraParams.responseText);
} else {
    var responseParams = Ext.decode(response.responseText);
    if (responseParams != undefined && responseParams.message != undefined) {
        Ext.Msg.alert('Ошибка', responseParams.message);
    } else {
        Ext.Msg.alert('Ошибка', 'Server failed');
    }
};
";
            return store;
        }

        private void RegisterResources(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterEmbeddedScript<PricesAndTariffsView>("PricesAndTariffsView.js");
        }

        private Window GetWindowOrganization(int taskId)
        {
            Window win = new Window
            {
                ID = "wOrganization",
                Hidden = true,
                Title = "Добавление организации",
                Width = 400,
                MinWidth = 400,
                Height = 150,
                MinHeight = 150,
                Modal = true,
                Layout = LayoutType.Fit.ToString(),
                MonitorResize = true,
                CloseAction = CloseAction.Hide
            };

            var panel = new Panel
                            {
                                ID = "organizationPanel",
                                Frame = true,
                                Layout = LayoutType.Form.ToString(),
                                Padding = 5,
                                LabelWidth = 75,
                                BodyCssClass = "x-window-mc",
                            };
            win.Items.Add(panel);

            panel.Items.Add(new TextField { ID = "orgId", Hidden = true });
            panel.Items.Add(new ComboBox
                                {
                                    ID = "orgNameCombo", 
                                    FieldLabel = "Организация", 
                                    AllowBlank = false, 
                                    TypeAhead = false,
                                    ForceSelection = false,
                                    TriggerAction = TriggerAction.All,
                                    AnchorHorizontal = "100%",
                                    Store = { GetStoreForOrgCombo(taskId) },
                                    MinChars = 1,
                                    ValueField = "ID",
                                    DisplayField = "Name",
                                    LoadingText = "Поиск...",
                                    AutoShow = true,
                                    EnableKeyEvents = true,
                                    Triggers = { new FieldTrigger { HideTrigger = true, Icon = TriggerIcon.Clear } },
                                    Listeners =
                                    {
                                        TriggerClick = { Handler = "if(index == 0) { this.focus().clearValue(); trigger.hide(); deselectOrganization(); }" },
                                        BeforeQuery = { Handler = "this.triggers[0][this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" },
                                        Select = { Handler = "this.triggers[0][this.getRawValue().toString().length == 0 ? 'hide' : 'show'](); onSelectOrganization(this.getSelectedItem(), {0});".FormatWith(DatasourceID) },
                                        Blur = { Handler = "if(Ext.isEmpty(this.getText())) { this.setValue(''); this.triggers[0].hide(); deselectOrganization();};" },
                                        KeyPress = { Handler = "deselectOrganization();" }
                                    }
                                });

            panel.Items.Add(new Checkbox
                                {
                                    ID = "orgIsMarketGrid",
                                    FieldLabel = "Торговая сеть",
                                    Listeners = { Check = { Handler = "deselectOrganization();" } }
                                });
            
            win.Buttons.Add(new Button { ID = "btnCreateOrganization", Text = "Создать новую", Listeners = { Click = { Handler = "createAndAddOrganization({0},{1});".FormatWith(DatasourceID, (int)goodType) } } });
            win.Buttons.Add(new Button { ID = "btnChooseOrganization", Text = "Выбрать", Disabled = true, Listeners = { Click = { Handler = "addSelectedOrganization({0});".FormatWith(DatasourceID) } } });

            win.Listeners.Hide.Handler = "initializeOrgWindow();";
            return win; 
        }

        private Store GetStoreForOrgCombo(int taskId)
        {
            var store = new Store { ID = "dsOrganization", AutoLoad = false };
            store.Proxy.Add(new HttpProxy { Url = "/Org3PriceAndTariffs/GetOrganizations", Method = HttpMethod.POST });
            
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Name"),
                    new RecordField("IsMarketGrid", RecordFieldType.Boolean)
                }
            });

            store.BaseParams.Add(new Parameter("filter", "orgNameCombo.getText()", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("taskId", taskId.ToString(), ParameterMode.Value));
            store.BaseParams.Add(new Parameter("goodType", ((int)goodType).ToString(), ParameterMode.Value));

            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка организаций', response.responseText);";
            store.Listeners.Load.Handler = "eval(store.reader.jsonData.extraParams);";
            return store;
        }

        private Window GetWindowCopyReport(int taskId)
        {
            Window win = new Window
            {
                ID = "wCopyReport",
                Hidden = true,
                Title = "Копирование данных с другого отчета",
                Width = 280,
                MinWidth = 280,
                Height = 300,
                MinHeight = 200,
                Modal = true,
                Layout = LayoutType.Fit.ToString(),
                MonitorResize = true,
                CloseAction = CloseAction.Hide
            };

            var grid = new GridPanel
                           {
                               ID = "gpCopyReport",
                               Store = { GetStoreOldReports(taskId) },
                               TopBar =
                                   {
                                       new Toolbar
                                           {
                                               Items = 
                                                {
                                                    new Button
                                                        {
                                                            Icon = Icon.ArrowRefresh,
                                                            ToolTip = "Обновить",
                                                            Listeners = { Click = { Handler = "dsCopyReport.reload();" } }
                                                        }
                                                }
                                           }
                                   },
                               Layout = LayoutType.Fit.ToString(),
                               Border = false,
                               AutoScroll = true,
                               TrackMouseOver = true,
                               LoadMask = { ShowMask = true },
                               ColumnModel =
                                   {
                                       Columns =
                                           {
                                               new Column { Hidden = true, DataIndex = "ID", Editable = false },
                                               new Column { Header = "Отчетная дата", DataIndex = "ReportDate", Editable = false },
                                           },
                                   },
                               SelectionModel = 
                               { 
                                   new RowSelectionModel
                                   {
                                       ID = "RowSelectionModel1",
                                       SingleSelect = true,
                                       Listeners = 
                                        { 
                                            RowSelect = { Handler = "btnCopyFromReport.enable();" },
                                            RowDeselect = { Handler = "if(!gpCopyReport.hasSelection()){btnCopyFromReport.disable();}" }
                                        }
                                   }
                               },
                               AutoExpandColumn = "ReportDate"
                           };

            win.Items.Add(grid);

            win.Buttons.Add(new Button { ID = "btnCopyFromReport", Text = "Скопировать", Disabled = true, Listeners = { Click = { Handler = "copyFromReport({0});".FormatWith(DatasourceID) } } });
            
            win.Listeners.Hide.Handler = "initializeCopyWindow();";
            win.Listeners.Show.Handler = "if (dsCopyReport.loaded==undefined || dsCopyReport.loaded == false){dsCopyReport.reload();dsCopyReport.loaded=true;}";
            return win;
        }

        private Store GetStoreOldReports(int taskId)
        {
            var store = new Store { ID = "dsCopyReport", AutoLoad = false };
            store.Proxy.Add(new HttpProxy { Url = "/Org3PriceAndTariffs/GetOldReportDates", Method = HttpMethod.POST });
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("ReportDate"),
                }
            });

            store.BaseParams.Add(new Parameter("taskId", taskId.ToString(), ParameterMode.Value));

            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка отчетов', response.responseText);";
            store.Listeners.Load.Handler = "eval(store.reader.jsonData.extraParams);";
            return store;
        }
    }
}