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
    public class DetailTaskListView : View
    {
        private const string DatasourceID = "dsTasks";
        private const string GridpanelID = "gpTasks";
 
        private IProgramService programService;

        public DetailTaskListView(IProgramService programService)
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
            
            var view = new Viewport
            {
                ID = "viewportMain",
                Items = 
                {
                   new FitLayout { Items = { GetTasksGridPanel(editable, page) } }
                }
            };

            return new List<Component> { view };
        }

        private GridPanel GetTasksGridPanel(bool editable, ViewPage page)
        {
            var gp = new GridPanel
            {
                ID = GridpanelID,
                Icon = Icon.DatabaseTable,
                Store = { GetTasksStore() },
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
                                Header = "Название задачи", 
                                DataIndex = "TaskName", 
                                Wrap = true, 
                                Width = 300, 
                                Editable = editable, 
                                Editor = { new TextField { AllowBlank = false } }
                            },
                        new Column
                            {
                                ColumnID = "TargetName", 
                                Header = "Цель", 
                                DataIndex = "TargetName", 
                                Width = 350, 
                                Editable = editable, 
                                Editor = 
                                {
                                    new ComboBox
                                        {
                                            TriggerAction = TriggerAction.All,
                                            Store = { GetTargetsLookupStore() },
                                            AutoShow = true,
                                            MaxHeight = 60,
                                            Editable = false,
                                            AllowBlank = false,
                                            ValueField = "Name",
                                            DisplayField = "Name",
                                            Listeners = { Select = { Handler = "{0}.getSelectionModel().getSelected().set('TargetId', record.get('ID'));".FormatWith(GridpanelID) } }
                                        }
                                },
                                Wrap = true
                            },
                        new Column { ColumnID = "TargetId", Hidden = true, DataIndex = "TargetId" },
                        new Column { ColumnID = "TaskNote", Header = "Примечание", DataIndex = "TaskNote", Width = 300, Editable = editable, Editor = { new TextField() } },
                    },
                },
                AutoExpandColumn = "TaskNote",
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
if (e.field == 'TargetName' && (dsRefTargets.loaded === undefined || !dsRefTargets.loaded)){{
  {0}.getColumnModel().getCellEditor(e.column, e.row).field.store.load();
  e.grid.getColumnModel().getCellEditor(e.column, e.row).field.lastQuery = '';
}}
return true;
".FormatWith(GridpanelID);

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
                Listeners = { Click = { Handler = "{0}.reload(); dsRefTargets.loaded = false;".FormatWith(DatasourceID) } }
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

        private Store GetTasksStore()
        {
            var store = new Store
            {
                ID = DatasourceID,
                AutoLoad = true,
                WarningOnDirty = true,
                DirtyWarningTitle = "Несохраненные изменения",
                DirtyWarningText = "Есть несохраненные изменения. Перезагрузить данные?",
                ShowWarningOnFailure = false,
                RefreshAfterSaving = RefreshAfterSavingMode.Auto
            };

            store.Proxy.Add(new HttpProxy { Url = "/DetailTaskList/GetTasksTable", Method = HttpMethod.GET });
            store.BaseParams.Add(new Parameter("programId", ProgramId.ToString(), ParameterMode.Value));

            store.UpdateProxy.Add(new HttpWriteProxy { Url = "/DetailTaskList/SaveTasksTable", Method = HttpMethod.POST, Timeout = 500000 });
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
                    new RecordField("TargetName"),
                    new RecordField("TargetId"),
                    new RecordField("TaskNote")
                }
            };

            store.Reader.Add(reader);
            store.Listeners.Save.Handler = "Ext.net.Notification.show({iconCls: 'icon-information', html: 'Изменения сохранены.', title: 'Уведомление', hideDelay: 2500});";
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка задач', e.message || response.statusText);";
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

        private Store GetTargetsLookupStore()
        {
            var store = new Store { ID = "dsRefTargets", AutoLoad = false };
            store.SetHttpProxy("/DetailTaskList/GetTargetsListForLookup");
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
    }
}
