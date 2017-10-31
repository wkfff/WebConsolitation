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
    public class DetailActionListView : View
    {
        private const string DatasourceID = "dsActions";
        private const string GridpanelID = "gpActions";
 
        private IProgramService programService;

        public DetailActionListView(IProgramService programService)
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
                   new FitLayout { Items = { GetActionsGridPanel(editable, page) } }
                }
            };

            return new List<Component> { view };
        }

        private GridPanel GetActionsGridPanel(bool editable, ViewPage page)
        {
            var gp = new GridPanel
            {
                ID = GridpanelID,
                Icon = Icon.DatabaseTable,
                Store = { GetActionsStore() },
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

                        new Column { ColumnID = "ActionName", Header = "Мероприятия программы", DataIndex = "ActionName", Wrap = true, Width = 200, Editable = editable, Editor = { new TextField() } },
                        
                        new Column
                            {
                                ColumnID = "ActionOwnerName", 
                                Header = "Заказчик",
                                DataIndex = "ActionOwnerName", 
                                Width = 200, 
                                Editable = editable, 
                                Editor = 
                                {
                                    new ComboBox
                                        {
                                            TriggerAction = TriggerAction.All,
                                            Store = { GetOwnersLookupStore() },
                                            AutoShow = true,
                                            MaxHeight = 60,
                                            Editable = false,
                                            AllowBlank = false,
                                            ValueField = "Name",
                                            DisplayField = "Name",
                                            Listeners = { Select = { Handler = "{0}.getSelectionModel().getSelected().set('ActionOwnerId', record.get('ID'));".FormatWith(GridpanelID) } }
                                        }
                                },
                                Wrap = true
                            },
                        new Column { ColumnID = "ActionOwnerId", Hidden = true, DataIndex = "ActionOwnerId" },

                        new Column
                            {
                                ColumnID = "ActionResults", 
                                Header = "Эффект от реализации программы", 
                                DataIndex = "ActionResults", 
                                Width = 300, 
                                Editable = editable, 
                                Editor = { new TextField() },
                                Wrap = true
                            },
                        new Column { ColumnID = "ActionNote", Header = "Примечание", DataIndex = "ActionNote", Wrap = true, Width = 300, Editable = editable, Editor = { new TextField() } },
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
if ( (e.field == 'TaskName' && (dsRefTasks.loaded === undefined || !dsRefTasks.loaded))
    ||(e.field == 'ActionOwnerName' && (dsRefOwners.loaded === undefined || !dsRefOwners.loaded) )
    ){{
  var f = e.grid.getColumnModel().getCellEditor(e.column, e.row).field;
  f.store.load();
  f.lastQuery = '';
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
                Listeners = { Click = { Handler = "{0}.reload(); dsRefTasks.loaded = false; dsRefOwners.loaded = false;".FormatWith(DatasourceID) } }
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

        private Store GetActionsStore()
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

            store.Proxy.Add(new HttpProxy { Url = "/DetailActionList/GetActionsTable", Method = HttpMethod.GET });
            store.BaseParams.Add(new Parameter("programId", ProgramId.ToString(), ParameterMode.Value));

            store.UpdateProxy.Add(new HttpWriteProxy { Url = "/DetailActionList/SaveActionsTable", Method = HttpMethod.POST, Timeout = 500000 });
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
                    new RecordField("ActionName"),
                    new RecordField("ActionOwnerName"),
                    new RecordField("ActionOwnerId"),
                    new RecordField("ActionResults"),
                    new RecordField("ActionNote")
                }
            };

            store.Reader.Add(reader);
            store.Listeners.Save.Handler = "Ext.net.Notification.show({iconCls: 'icon-information', html: 'Изменения сохранены.', title: 'Уведомление', hideDelay: 2500});";
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка мероприятий', e.message || response.statusText);";
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

        private Store GetTasksLookupStore()
        {
            var store = new Store { ID = "dsRefTasks", AutoLoad = false };
            store.SetHttpProxy("/DetailActionList/GetTasksListForLookup");
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

        private Store GetOwnersLookupStore()
        {
            var store = new Store { ID = "dsRefOwners", AutoLoad = false };
            store.SetHttpProxy("/DetailActionList/GetOwnersListForLookup");
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
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка заказчиков', response.responseText);";

            store.Listeners.Load.Handler = "this.loaded = true;";
            store.Listeners.Load.Delay = 100;
            return store;
        }
    }
}
