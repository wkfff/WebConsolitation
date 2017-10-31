using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Services;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Presentation.Views
{
    public class TargetRatingsView : View
    {
        private const string DatasourceRatingsListID = "dsRatings";
        private const string GridpanelRatingsListID = "gpRatings";
        private readonly IProjectService projectService;

        public TargetRatingsView(IProjectService projectService)
        {
            this.projectService = projectService;
        }

        public int ProjId
        {
            get { return Params["projId"] == "null" ? 0 : Convert.ToInt32(Params["projId"]); }
        }

        public override List<Component> Build(ViewPage page)
        {
            var projectStatus = projectService.GetProjectStatus(ProjId);

            var view = new Viewport
            {
                ID = "viewportMain",
                Items = 
                                {
                                    new BorderLayout
                                    {   
                                        North = { Items = { CreateTopPanel(projectStatus == InvProjStatus.Edit) }, Margins = { Bottom = 5 } },
                                        Center = { Items = { CreateTargetRatingsPanel(projectStatus == InvProjStatus.Edit) }, Margins = { Bottom = 3 } }
                                    }
                                }
            };

            return new List<Component> { view };
        }

        private Panel CreateTopPanel(bool editable)
        {
            var toolbar = new Toolbar();

            toolbar.Add(new Button
            {
                ID = "btnRefresh",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Listeners = { Click = { Handler = "{0}.reload();".FormatWith(DatasourceRatingsListID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnAdd",
                Icon = Icon.Add,
                ToolTip = "Добавить",
                Hidden = !editable,
                Listeners = { Click = { Handler = "{0}.insertRecord();{0}.getSelectionModel().selectFirstRow();{0}.startEditing(0,1);".FormatWith(GridpanelRatingsListID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnDelete",
                Icon = Icon.Delete,
                Disabled = true,
                ToolTip = "Удалить",
                Hidden = !editable,
                Listeners = { Click = { Handler = "{0}.deleteSelected();".FormatWith(GridpanelRatingsListID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnSave",
                Icon = Icon.TableSave,
                ToolTip = "Сохранить",
                Hidden = !editable,
                Listeners =
                {
                    Click =
                    {
                        Handler = "{0}.save();".FormatWith(DatasourceRatingsListID)
                    }
                }
            });
            
            toolbar.Add(new ToolbarSpacer(25));

            toolbar.Add(new ComboBox
            {
                ID = "cmbPeriod",
                FieldLabel = "Период реализации",
                Store = { GetQuarterLookupStore() },
                AutoShow = true,
                Editable = false,
                AllowBlank = false,
                TriggerAction = TriggerAction.All,
                Mode = DataLoadMode.Local,
                ValueField = "ID",
                DisplayField = "Value",
                Width = 250,
                Listeners = { Select = { Handler = "{0}.reload();".FormatWith(DatasourceRatingsListID) } }
            });

            return new Panel
            {
                ID = "topPanel",
                Height = 27,
                Border = false,
                TopBar = { toolbar }
            };
        }

        private List<Component> CreateTargetRatingsPanel(bool editable)
        {
            GridPanel table = new GridPanel
            {
                ID = GridpanelRatingsListID,
                Store = { GetRatingsListStore() },
                Border = false,
                TrackMouseOver = true,
                Icon = Icon.DatabaseTable,
                AutoScroll = true,
                Layout = LayoutType.Fit.ToString(),
                LoadMask = { ShowMask = true, Msg = "Загрузка..." },
                ClearEditorFilter = false,
                SelectionModel =
                {
                    new RowSelectionModel
                        {
                            ID = "RowSelectionModel1", 
                            SingleSelect = true,
                            Listeners =
                                {
                                    RowSelect = { Handler = "if(Ext.getCmp('btnDelete')!=undefined){btnDelete.enable();}" },
                                    RowDeselect = { Handler = "if (!gpRatings.hasSelection() && Ext.getCmp('btnDelete')!=undefined){btnDelete.disable();}" }
                                }
                        }
                }
            };

            table.ColumnModel.Columns.Add(new Column { ColumnID = "ID", Hidden = true, DataIndex = "ID" });
            table.ColumnModel.Columns.Add(new Column { ColumnID = "IndicatorId", Hidden = true, DataIndex = "IndicatorId" });
            
            table.ColumnModel.Columns.Add(new Column
            {
                ColumnID = "Indicator",
                Header = "Наименование показателя",
                DataIndex = "Indicator",
                Width = 500,
                Align = Alignment.Left,
                Editable = editable,
                Editor =
                {
                    new ComboBox
                        {
                            TriggerAction = TriggerAction.All,
                            Store = { GetIndicatorLookupStore() },
                            AutoShow = true,
                            MaxHeight = 60,
                            Editable = false,
                            AllowBlank = false,
                            ValueField = "Name",
                            DisplayField = "Name",
                            ClearFilterOnReset = false,
                            Listeners = 
                            {
                                Select = 
                                {
                                    Handler = @"
var row = {0}.getSelectionModel().getSelected();
row.set('IndicatorId', record.get('ID'));
row.set('Unit', record.get('Unit'));
row.set('Note', record.get('Note'));
".FormatWith(GridpanelRatingsListID) 
                                } 
                            }
                        }
                }
            });

            ////Запрещаем редактировать колонку у существующих строк! Инициализируем store
            table.Listeners.BeforeEdit.Handler = @"
if (e.field == 'Indicator' && !e.record.phantom){{
   return false;
}}
else if (e.field == 'Indicator' && dsRefIndicator.loaded === undefined){{
   {0}.getColumnModel().getCellEditor(e.column, e.row).field.store.load();
}}
return true;
".FormatWith(GridpanelRatingsListID);

            ////Фильтруем допустимые значения
            table.Listeners.AfterEdit.Handler = "if (e.field == 'Indicator'){ filterIndicators(e); }";

            table.ColumnModel.Columns.Add(new Column
            {
                ColumnID = "Unit",
                Header = "Ед.изм.",
                DataIndex = "Unit",
                Width = 70,
                Align = Alignment.Left,
                Editable = false
            });

            table.ColumnModel.Columns.Add(new Column
            {
                ColumnID = "Note",
                Header = "Примечание",
                DataIndex = "Note",
                Width = 100,
                Align = Alignment.Left
            });

            table.ColumnModel.Columns.Add(new NumberColumn
            {
                Header = "За отчетный период",
                DataIndex = "Value",
                Width = 180,
                Format = "0,0.00",
                Align = Alignment.Center,
                Editable = editable,
                Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
            });

            table.ColumnModel.Columns.Add(new NumberColumn
            {
                Header = "Всего",
                DataIndex = "SumValue",
                Width = 180,
                Format = "0,0.00",
                Align = Alignment.Center
            });

            return new List<Component> { table };
        }

        private Store GetRatingsListStore()
        {
            var store = new Store
            {
                ID = DatasourceRatingsListID,
                WarningOnDirty = true,
                DirtyWarningTitle = "Несохраненные изменения",
                DirtyWarningText = "Есть несохраненные изменения. Перезагрузить данные?",
                ShowWarningOnFailure = false,
                RefreshAfterSaving = RefreshAfterSavingMode.Always,
                AutoLoad = false ////Загружается только после инициализации combobox-а с кварталами
            };

            store.Proxy.Add(new HttpProxy { Url = "/TargetRatings/GetRatingsTable", Method = HttpMethod.GET });
            store.BaseParams.Add(new Parameter("refProjId", ProjId.ToString(), ParameterMode.Value));
            store.BaseParams.Add(new Parameter("yearQuarter", "cmbPeriod.getValue()", ParameterMode.Raw));
            
            store.UpdateProxy.Add(new HttpWriteProxy { Url = "/TargetRatings/SaveRatingsTable", Method = HttpMethod.POST, Timeout = 500000 });
            store.WriteBaseParams.Add(new Parameter("refProjId", ProjId.ToString(), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("yearQuarter", "cmbPeriod.getValue()", ParameterMode.Raw));
            store.WriteBaseParams.Add(new Parameter("storeChangedData", "#{{{0}}}.store.getChangedData()".FormatWith(GridpanelRatingsListID), ParameterMode.Raw));

            var reader = new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                {
                    new RecordField("ID"),
                    new RecordField("IndicatorId"),
                    new RecordField("Indicator"),
                    new RecordField("Unit"),
                    new RecordField("Note"),
                    new RecordField("Value"),
                    new RecordField("SumValue"),
                }
            };
            
            store.Reader.Add(reader);
            store.Listeners.Save.Handler = "Ext.net.Notification.show({iconCls: 'icon-information', html: 'Изменения сохранены.', title: 'Уведомление', hideDelay: 2500});";
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка показателей', response.statusText);";
            store.Listeners.SaveException.Handler = @"
if (response.extraParams != undefined && response.extraParams.responseText != undefined) {
    Ext.Msg.alert('Ошибка', response.extraParams.responseText);
} else {
    var responseParams = Ext.decode(response.responseText);
    if (responseParams != undefined && responseParams.extraParams != undefined && responseParams.extraParams.responseText != undefined) {
        Ext.Msg.alert('Ошибка', responseParams.extraParams.responseText);
    } else {
        Ext.Msg.alert('Ошибка', 'Server failed');
    }
};
";
            return store;
        }

        private Store GetIndicatorLookupStore()
        {
            var store = new Store { ID = "dsRefIndicator", AutoLoad = false };
            store.SetHttpProxy("/TargetRatings/GetIndicatorList");
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Name"),
                    new RecordField("Note"),
                    new RecordField("Unit")
                }
            });
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка показателей', response.responseText);";

            store.Listeners.Load.Handler = "this.loaded = true; filterIndicators({{field:'Indicator', grid: {0}, column: 2, row:0}});".FormatWith(GridpanelRatingsListID);
            store.Listeners.Load.Delay = 100;
            store.AddScript(@"
function filterIndicators(o){
   var editor = o.grid.getColumnModel().getCellEditor(o.column, o.row);
   editor.field.store.filterBy(function(record){
                                    var value = record.data.ID;
                                    var store = o.grid.store;
                                    ind = store.findExact('IndicatorId', value);
                                    return ind === -1;
                               });
   editor.field.lastQuery = '';
};
");

            return store;
        }

        private Store GetQuarterLookupStore()
        {
            var store = new Store { ID = "dsRefQuarter", AutoLoad = true };
            store.SetHttpProxy("/TargetRatings/GetQuarterList");
            store.BaseParams.Add(new Parameter("refProjId", ProjId.ToString(), ParameterMode.Value));
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Value"),
                }
            });
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка кварталов', response.responseText);";
            store.Listeners.DataChanged.Handler = "cmbPeriod.setValue(dsRefQuarter.data.items[0].data.ID);{0}.load();".FormatWith(DatasourceRatingsListID);
            return store;
        }
    }
}
