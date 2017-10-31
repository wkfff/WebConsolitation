using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Helpers;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Services;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Presentation.Views
{
    public class DetailFinancesListView : View
    {
        private const string DatasourceID = "dsFinances";
        private const string GridpanelID = "gpFinances";
 
        private IProgramService programService;

        public DetailFinancesListView(IProgramService programService)
        {
            this.programService = programService;
        }

        public int ProgramId
        {
            get { return String.IsNullOrEmpty(Params["programId"]) ? 0 : Convert.ToInt32(Params["programId"]); }
        }

        public override List<Component> Build(ViewPage page)
        {
            var program = programService.GetProgram(ProgramId);
            var editable = new PermissionSettings(User, program).CanEditDetail;
            var yearColumns = programService.GetYears(ProgramId);

            var view = new Viewport
            {
                ID = "viewportMain",
                Items = 
                {
                   new FitLayout { Items = { GetFinancesGridPanel(editable, yearColumns, page) } }
                }
            };

            return new List<Component> { view };
        }

        private GridPanel GetFinancesGridPanel(bool editable, IList<int> yearColumns, ViewPage page)
        {
            var gp = new GridPanel
            {
                ID = GridpanelID,
                Icon = Icon.DatabaseTable,
                Store = { GetActionsStore(yearColumns) },
                TopBar = { GetToolbar(editable) },
                Layout = LayoutType.Fit.ToString(),
                AutoScroll = true,
                Border = false,
                TrackMouseOver = true,
                LoadMask = { ShowMask = true },
                ColumnModel =
                {
                    Columns = 
                    {
                        new Column { Hidden = true, DataIndex = "ID" },
                        new Column
                            {
                                ColumnID = "ActionName", 
                                Header = "Мероприятие", 
                                DataIndex = "ActionName", 
                                Width = 200, 
                                Editable = editable, 
                                Editor = 
                                {
                                    new ComboBox
                                        {
                                            TriggerAction = TriggerAction.All,
                                            Store = { GetActionsLookupStore() },
                                            AutoShow = true,
                                            Height = 70,
                                            Resizable = true,
                                            Editable = false,
                                            AllowBlank = false,
                                            ValueField = "Name",
                                            DisplayField = "Name",
                                            Listeners = { Select = { Handler = "{0}.getSelectionModel().getSelected().set('ActionId', record.get('ID'));".FormatWith(GridpanelID) } }
                                        } 
                                },
                                Wrap = true
                            },
                        new Column { ColumnID = "ActionId", Hidden = true, DataIndex = "ActionId" },
                        new Column { ColumnID = "ActionIdOld", Hidden = true, DataIndex = "ActionId", Editable = false },

                        new Column
                            {
                                ColumnID = "FinSourceName", 
                                Header = "Источник финансирования",
                                DataIndex = "FinSourceName", 
                                Width = 200, 
                                Editable = editable, 
                                Editor = 
                                {
                                    new ComboBox
                                        {
                                            TriggerAction = TriggerAction.All,
                                            Store = { GetFinSourcesLookupStore() },
                                            AutoShow = true,
                                            Height = 70,
                                            Resizable = true,
                                            Editable = false,
                                            AllowBlank = false,
                                            ValueField = "Name",
                                            DisplayField = "Name",
                                            Listeners = { Select = { Handler = "{0}.getSelectionModel().getSelected().set('FinSourceId', record.get('ID'));".FormatWith(GridpanelID) } }
                                        }
                                }
                            },
                        new Column { ColumnID = "FinSourceId", Hidden = true, DataIndex = "FinSourceId" },
                        new Column { ColumnID = "FinSourceIdOld", Hidden = true, DataIndex = "FinSourceId", Editable = false }
                    },
                },
                SelectionModel = 
                {
                    new RowSelectionModel
                        {
                            ID = "RowSelectionModel1", 
                            Listeners = 
                            { 
                                RowSelect = { Handler = "btnDelete.enable();" },
                                RowDeselect = { Handler = "if(!{0}.hasSelection()){{btnDelete.disable();}}".FormatWith(GridpanelID) }
                            }
                        }
                }
            };

            ////Запрещаем редактировать существующие показатели
            gp.Listeners.BeforeEdit.Handler = @"
if ( (e.field == 'ActionName' || e.field == 'FinSourceName') && !e.record.phantom){
   Ext.net.Notification.show({iconCls: 'icon-information', html: 'заведенный показатель изменять нельзя!', title: 'Уведомление', hideDelay: 2500}); 
   return false;
}
else if ( (e.field == 'ActionName' && (dsActions.loaded === undefined || !dsActions.loaded))
    ||(e.field == 'FinSourceName' && (dsFinSources.loaded === undefined || !dsFinSources.loaded) )
    ){
  var f = e.grid.getColumnModel().getCellEditor(e.column, e.row).field;
  f.store.load();
  f.lastQuery = '';
}
return true;
";

            // Динамический набор колонок))
            if (yearColumns.Count == 1)
            {
                gp.ColumnModel.Columns.Add(new NumberColumn
                {
                    Header = "Год - {0}".FormatWith(yearColumns[0]),
                    DataIndex = "Year{0}".FormatWith(yearColumns[0]),
                    Width = 150,
                    Format = "0,0.00 руб.",
                    Align = Alignment.Center,
                    Editable = editable,
                    Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                });
            }
            else
            {
                foreach (int year in yearColumns)
                {
                    gp.ColumnModel.Columns.Add(new NumberColumn
                    {
                        Header = "Год - {0}".FormatWith(year),
                        DataIndex = "Year{0}".FormatWith(year),
                        Width = 150,
                        Format = "0,0.00 руб.",
                        Align = Alignment.Center,
                        Editable = editable,
                        Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                    });   
                }
            }

            gp.ColumnModel.Columns.Add(new NumberColumn
                                           {
                                               ColumnID = "Total", 
                                               Header = "Всего", 
                                               DataIndex = "Total", 
                                               Format = "0,0.00 руб.", 
                                               Align = Alignment.Center, 
                                               Width = 100, 
                                               Editable = false
                                           });
            
            gp.AddColumnsWrapStylesToPage(page);

            return gp;
        }

        private Toolbar GetToolbar(bool editable)
        {
            var toolbar = new Toolbar();

            toolbar.Add(new Button
            {
                ID = "btnRefresh",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Listeners = { Click = { Handler = "{0}.reload(); dsActions.loaded = false; dsFinSources.loaded = false;".FormatWith(DatasourceID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnAdd",
                Icon = Icon.Add,
                ToolTip = "Добавить",
                Hidden = !editable,
                Listeners = { Click = { Handler = "{0}.insertRecord();{0}.getSelectionModel().selectFirstRow();{0}.startEditing(0,1);".FormatWith(GridpanelID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnDelete",
                Icon = Icon.Delete,
                Disabled = true,
                Hidden = !editable,
                ToolTip = "Удалить",
                Listeners = { Click = { Handler = "{0}.deleteSelected(); if(!{0}.hasSelection()){{btnDelete.disable();}}".FormatWith(GridpanelID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnSave",
                Icon = Icon.TableSave,
                ToolTip = "Сохранить",
                Hidden = !editable,
                Listeners = { Click = { Handler = "{0}.save();".FormatWith(DatasourceID) } }
            });

            return toolbar;
        }

        private Store GetActionsStore(IList<int> yearColumns)
        {
            var store = new Store
            {
                ID = DatasourceID,
                AutoLoad = true,
                WarningOnDirty = true,
                DirtyWarningTitle = "Несохраненные изменения",
                DirtyWarningText = "Есть несохраненные изменения. Перезагрузить данные?",
                ShowWarningOnFailure = false,
                RefreshAfterSaving = RefreshAfterSavingMode.Always
            };

            store.Proxy.Add(new HttpProxy { Url = "/DetailFinancesList/GetFinancesTable", Method = HttpMethod.GET });
            store.BaseParams.Add(new Parameter("programId", ProgramId.ToString(), ParameterMode.Value));

            store.UpdateProxy.Add(new HttpWriteProxy { Url = "/DetailFinancesList/SaveFinancesTable", Method = HttpMethod.POST, Timeout = 500000 });
            store.WriteBaseParams.Add(new Parameter("programId", ProgramId.ToString(), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("storeChangedData", "{0}.store.getChangedData()".FormatWith(GridpanelID), ParameterMode.Raw));

            var reader = new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                {
                    new RecordField("ID"),
                    new RecordField("ActionName"),
                    new RecordField("ActionId"),
                    new RecordField("ActionIdOld"),
                    new RecordField("FinSourceName"),
                    new RecordField("FinSourceId"),
                    new RecordField("FinSourceIdOld"),
                    new RecordField("Total")
                }
            };

            foreach (int year in yearColumns)
            {
                reader.Fields.Add(new RecordField("Year{0}".FormatWith(year)));
            }

            store.Reader.Add(reader);
            store.Listeners.Save.Handler = "Ext.net.Notification.show({iconCls: 'icon-information', html: 'Изменения сохранены.', title: 'Уведомление', hideDelay: 2500});";
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка мероприятий', response.responseText);";
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

        private Store GetActionsLookupStore()
        {
            var store = new Store { ID = "dsActions", AutoLoad = false };
            store.SetHttpProxy("/DetailFinancesList/GetActionsListForLookup");
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Name"),
                }
            });
            store.BaseParams.Add(new Parameter("programId", ProgramId.ToString(), ParameterMode.Value));
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка мероприятий', response.responseText);";

            store.Listeners.Load.Handler = "this.loaded = true;";
            store.Listeners.Load.Delay = 100;
            return store;
        }

        private Store GetFinSourcesLookupStore()
        {
            var store = new Store { ID = "dsFinSources", AutoLoad = false };
            store.SetHttpProxy("/DetailFinancesList/GetFinSourcesListForLookup");
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Name"),
                }
            });
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка источников финансирования', response.responseText);";

            store.Listeners.Load.Handler = "this.loaded = true;";
            store.Listeners.Load.Delay = 100;
            return store;
        }
    }
}
