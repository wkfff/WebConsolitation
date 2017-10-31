using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Helpers;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Services;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Presentation.Views
{
    public class DetailTargetRatingsListView : View
    {
        private const string DatasourceID = "dsRatings";
        private const string GridpanelID = "gpRatings";
        private const string LookupUnitsDatasource = "dsUnits";
        private const string LookupTasksDatasource = "dsTasks";
        private const string LookupRateTypeDatasource = "dsRateTypes";
        
        private IProgramService programService;

        public DetailTargetRatingsListView(IProgramService programService)
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
            var yearColumns = programService.GetYearsWithPreviousAndFollowing(ProgramId);

            var view = new Viewport
            {
                ID = "viewportMain",
                Items = 
                {
                   new FitLayout { Items = { GetTargetRatingsGridPanel(editable, yearColumns, page) } }
                }
            };

            return new List<Component> { view };
        }

        private GridPanel GetTargetRatingsGridPanel(bool editable, IList<int> yearColumns, ViewPage page)
        {
            var gp = new GridPanel
            {
                ID = GridpanelID,
                Icon = Icon.DatabaseTable,
                Store = { GetTargetRatingsStore(yearColumns) },
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
                                ColumnID = "TaskName", 
                                Header = "Задача", 
                                DataIndex = "TaskName", 
                                Width = 200, 
                                Editable = editable, 
                                Editor = 
                                {
                                    new ComboBox
                                        {
                                            TriggerAction = TriggerAction.All,
                                            Store = { GetTasksLookupStore() },
                                            AutoShow = true,
                                            MaxHeight = 60,
                                            Editable = false,
                                            AllowBlank = false,
                                            ValueField = "Name",
                                            DisplayField = "Name",
                                            Listeners = { Select = { Handler = "{0}.getSelectionModel().getSelected().set('TaskId', record.get('ID'));".FormatWith(GridpanelID) } }
                                        } 
                                },
                                Wrap = true
                            },
                        new Column { ColumnID = "TaskId", Hidden = true, DataIndex = "TaskId" },
                        new Column
                            {
                                ColumnID = "RateName", 
                                Header = "Наименование показателя", 
                                DataIndex = "RateName", 
                                Width = 150, 
                                Editable = editable, 
                                Editor = { new TextField() },
                                Wrap = true
                            },
                       new Column
                            {
                                ColumnID = "RateTypeName", 
                                Header = "Тип показателя", 
                                DataIndex = "RateTypeName", 
                                Width = 150, 
                                Editable = editable, 
                                Editor = 
                                {
                                    new ComboBox
                                        {
                                            TriggerAction = TriggerAction.All,
                                            Store = { GetAllRateTypeLookupStore() },
                                            AutoShow = true,
                                            Height = 70,
                                            Resizable = true,
                                            Editable = false,
                                            AllowBlank = false,
                                            ValueField = "Name",
                                            DisplayField = "Name",
                                            Listeners = { Select = { Handler = "{0}.getSelectionModel().getSelected().set('RateTypeId', record.get('ID'));".FormatWith(GridpanelID) } }
                                        } 
                                },
                            },
                       new Column { Hidden = true, DataIndex = "RateTypeId" },

                       new Column
                            {
                                ColumnID = "UnitName", 
                                Header = "Ед.изм.", 
                                DataIndex = "UnitName", 
                                Width = 60, 
                                Editable = editable, 
                                Editor = 
                                {
                                    new ComboBox
                                        {
                                            TriggerAction = TriggerAction.All,
                                            Store = { GetAllUnitLookupStore() },
                                            AutoShow = true,
                                            Height = 70,
                                            Resizable = true,
                                            Editable = false,
                                            AllowBlank = false,
                                            ValueField = "Designation",
                                            DisplayField = "Name",
                                            Listeners = { Select = { Handler = "{0}.getSelectionModel().getSelected().set('UnitId', record.get('ID'));".FormatWith(GridpanelID) } }
                                        } 
                                },
                            },
                        new Column { Hidden = true, DataIndex = "UnitId" },
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

            gp.Listeners.BeforeEdit.Handler = @"
if ( (e.field == 'ParamUnitName' && ({0}.loaded === undefined || !{0}.loaded))
    || (e.field == 'TaskName' && ({1}.loaded === undefined || !{1}.loaded))
    || (e.field == 'RateTypeName' && ({2}.loaded === undefined || !{2}.loaded))
    ){{
  var f = e.grid.getColumnModel().getCellEditor(e.column, e.row).field;
  f.store.load();
  f.lastQuery = '';
}}
return true;
".FormatWith(LookupUnitsDatasource, LookupTasksDatasource, LookupRateTypeDatasource);

            // Динамический набор колонок))
            foreach (int year in yearColumns)
            {
                gp.ColumnModel.Columns.Add(new NumberColumn
                {
                    Header = "Год - {0}".FormatWith(year),
                    DataIndex = "Year{0}".FormatWith(year),
                    Width = 150,
                    Format = "0,0.00",
                    Align = Alignment.Center,
                    Editable = editable,
                    Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                });   
            }

            var columnPreviousYear = gp.ColumnModel.Columns.First(x => x.Header == "Год - {0}".FormatWith(yearColumns.First()));
            columnPreviousYear.Header = "На начало реализации программы";

            var columnFollowingYear = gp.ColumnModel.Columns.First(x => x.Header == "Год - {0}".FormatWith(yearColumns.Last()));
            columnFollowingYear.Header = "На момент окончания действия программы";

            ////gp.AddColumnsWrapStylesToPage(page);
            var style = new StringBuilder();
            foreach (ColumnBase column in gp.ColumnModel.Columns)
            {
                if (column.Wrap)
                {
                    style.Append(".x-grid3-col-")
                        .Append(column.ColumnID)
                        .AppendLine("{white-space: normal;}");
                }
            }

            ResourceManager.GetInstance(page).RegisterClientStyleBlock("RefColumnStyles", style.ToString());

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
                Listeners = { Click = { Handler = "{0}.reload(); {1}.loaded = false; {2}.loaded = false;".FormatWith(DatasourceID, LookupTasksDatasource, LookupUnitsDatasource) } }
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

        private Store GetTargetRatingsStore(IList<int> yearColumns)
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

            store.Proxy.Add(new HttpProxy { Url = "/DetailTargetRatingsList/GetRatingsTable", Method = HttpMethod.GET });
            store.BaseParams.Add(new Parameter("programId", ProgramId.ToString(), ParameterMode.Value));
            
            store.UpdateProxy.Add(new HttpWriteProxy { Url = "/DetailTargetRatingsList/SaveRatingsTable", Method = HttpMethod.POST, Timeout = 500000 });
            store.WriteBaseParams.Add(new Parameter("programId", ProgramId.ToString(), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("storeChangedData", "{0}.store.getChangedData()".FormatWith(GridpanelID), ParameterMode.Raw));

            var reader = new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                {
                    new RecordField("ID"),
                    new RecordField("TaskName"),
                    new RecordField("TaskId"),
                    new RecordField("RateName"),
                    new RecordField("RateTypeName"),
                    new RecordField("RateTypeId"),
                    new RecordField("UnitName"),
                    new RecordField("UnitId"),
                }
            };
            
            foreach (int year in yearColumns)
            {
                reader.Fields.Add(new RecordField("Year{0}".FormatWith(year)));
            }

            store.Reader.Add(reader);
            store.Listeners.Save.Handler = "Ext.net.Notification.show({iconCls: 'icon-information', html: 'Изменения сохранены.', title: 'Уведомление', hideDelay: 2500});";
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке целеывых показателей', e.message || response.statusText);";
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

        private Store GetAllUnitLookupStore()
        {
            var store = new Store { ID = LookupUnitsDatasource, AutoLoad = false };
            store.SetHttpProxy("/DetailTargetRatingsList/GetAllUnitListForLookup");
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Name"),
                    new RecordField("Designation"),
                }
            });
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка показателей', response.responseText);";

            store.Listeners.Load.Handler = "this.loaded = true;";
            ////store.Listeners.Load.Delay = 100;
            return store;
        }

        private Store GetTasksLookupStore()
        {
            var store = new Store { ID = LookupTasksDatasource, AutoLoad = false };
            store.SetHttpProxy("/DetailTargetRatingsList/GetTasksListForLookup");
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
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка целей', response.responseText);";

            store.Listeners.Load.Handler = "this.loaded = true;";
            store.Listeners.Load.Delay = 100;
            return store;
        }

        private Store GetAllRateTypeLookupStore()
        {
            var store = new Store { ID = LookupRateTypeDatasource, AutoLoad = false };
            store.SetHttpProxy("/DetailTargetRatingsList/GetAllRateTypeListForLookup");
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
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка типов показателей', response.responseText);";

            store.Listeners.Load.Handler = "this.loaded = true;";
            ////store.Listeners.Load.Delay = 100;
            return store;
        }
    }
}
