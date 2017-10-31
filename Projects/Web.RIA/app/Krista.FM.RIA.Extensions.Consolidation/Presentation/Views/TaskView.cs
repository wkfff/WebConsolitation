using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.Consolidation.Presentation.Views;
using Button = Ext.Net.Button;
using GridView = Ext.Net.GridView;
using Menu = Ext.Net.Menu;
using MenuItem = Ext.Net.MenuItem;
using Panel = Ext.Net.Panel;
using Parameter = Ext.Net.Parameter;
using ParameterCollection = Ext.Net.ParameterCollection;
using View = Krista.FM.RIA.Core.Gui.View;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class TaskView : View
    {
        private readonly TaskService taskService;
        private readonly IUserSessionState sessionState;

        public TaskView(
            TaskService taskService,
            IUserSessionState sessionState)
        {
            this.taskService = taskService;
            this.sessionState = sessionState;
        }

        public int TaskId
        {
            get { return Convert.ToInt32(Params["id"]); }
        }

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);

            if (!ExtNet.IsAjaxRequest)
            {
                resourceManager.RegisterClientScriptBlock("TaskView", Resource.TaskView);
            }

            // Регистрируем событие на закрытие вкладки
            resourceManager.AddScript(@"
var tab = parent.MdiTab.getComponent('consTask_' + {0});
if (typeof(tab.events.beforeclose) == 'object'){{
    tab.events.beforeclose.clearListeners();
}}
tab.addListener('beforeclose', onTabCloseEventHandler);".FormatWith(TaskId));

            TaskViewModel taskViewModel = taskService.GetTaskViewModel(TaskId);
            TaskPermisionSettings taskPermisionSettings = new TaskPermisionSettings(sessionState, taskViewModel);

            page.Controls.Add(GetTaskStore(taskViewModel));
            page.Controls.Add(GetProtocolStore());

            FileListPanel fileListPanel = GetFileListPanel(taskPermisionSettings);

            page.Controls.Add(fileListPanel.GetFileListStore());

            Window fileUploadWindow = fileListPanel.GetFileUploadWindow();
            
            // Допиливаем - вешаем свой обработчик для редактировния имени файла
            FormPanel form = (FormPanel)fileUploadWindow.Items[0];
            FileUploadField field = (FileUploadField)form.Items.Find(delegate(Component cmp) { return cmp.ClientID == FileListPanel.FileUploadFieldId; });
            field.Listeners.FileSelected.Handler = "if (!fileName.getValue()){fileName.setValue(value);}";

            return new List<Component>
            {
                new Viewport
                {
                    ID = "viewportMain",
                    Items = 
                    {
                        new BorderLayout
                        {
                            North = { Items = { CreateTopPanel(taskPermisionSettings, taskViewModel) }, Margins = { Bottom = 5 } },
                            Center = { Items = { CreateTaskPanel(taskPermisionSettings, fileListPanel) }, Margins = { Bottom = 3 } }
                        }
                    }
                },
                fileUploadWindow
            };
        }

        private static Panel CreateTopPanel(TaskPermisionSettings taskPermisionSettings, TaskViewModel taskViewModel)
        {
            var toolbar = new Toolbar();

            toolbar.Add(new Button
            {
                Text = "Открыть форму сбора",
                Icon = Icon.TableGo,
                Listeners =
                {
                    Click =
                    {
                        Handler = "parent.MdiTab.addTab({{id: 'consReport_{1}', title:parent.MdiTab.getActiveTab().title, url:'/ConsReport/{0}?taskId={1}', icon:'icon-report'}});".FormatWith(
                                taskViewModel.TemplateClass, 
                                taskViewModel.ID)
                    }
                }
            });

            toolbar.Add(new ToolbarSpacer { Width = 100 });
            toolbar.Add(new ToolbarSeparator { Hidden = !taskPermisionSettings.CanEditTask });

            toolbar.Add(new Button
            {
                ID = "btnSave",
                Icon = Icon.Disk,
                Text = "Сохранить изменения",
                Listeners = { Click = { Handler = "saveForm(null);" } },
                Hidden = !taskPermisionSettings.CanEditTask
            });
            toolbar.Add(new ToolbarSeparator());
            toolbar.Add(new Button
            {
                ID = "btnToTest",
                Icon = Icon.TimeGo,
                Text = "Отправить на рассмотрение",
                Listeners = { Click = { Handler = "saveForm({0});".FormatWith((int)TaskViewModel.TaskStatus.OnTest) } },
                Hidden = !(taskViewModel.RefStatus == (int)TaskViewModel.TaskStatus.Edit && taskPermisionSettings.CanEditTask)
            });
            toolbar.Add(new Button
            {
                ID = "btnReject",
                Icon = Icon.Stop,
                Text = "Отправить на доработку",
                Listeners = { Click = { Handler = "saveForm({0});".FormatWith((int)TaskViewModel.TaskStatus.Edit) } },
                Hidden = !(taskPermisionSettings.CanSetVise && taskViewModel.RefStatus == (int)TaskViewModel.TaskStatus.OnTest)
            });
            toolbar.Add(new Button
            {
                ID = "btnAccept",
                Icon = Icon.Tick,
                Text = "Утвердить",
                Listeners = { Click = { Handler = "saveForm({0});".FormatWith((int)TaskViewModel.TaskStatus.Accepted) } },
                Hidden = !(taskPermisionSettings.CanSetVise && taskViewModel.RefStatus == (int)TaskViewModel.TaskStatus.OnTest)
            });

            toolbar.Add(new ToolbarSeparator());

            toolbar.Add(new Button
            {
                ID = "btnPump",
                Icon = Icon.DatabaseYellowStart,
                Text = "Передать данные для многомерного анализа",
                Listeners = { Click = { Handler = "doPump();" } },
                Hidden = !(taskPermisionSettings.CanPumpReport && taskViewModel.RefStatus == (int)TaskViewModel.TaskStatus.Accepted)
            });

            toolbar.Add(new ToolbarFill());

            toolbar.Add(new Button
            {
                ID = "btnRefresh",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Listeners = { Click = { Handler = "parent.MdiTab.getActiveTab().reload();" } }
            });

            return new Panel
            {
                ID = "topPanel",
                Height = 27,
                Border = false,
                TopBar = { toolbar }
            };
        }

        #region Store Initialization

        private Store GetTaskStore(TaskViewModel taskViewModel)
        {
            Store store = new Store { ID = "dsTask", AutoLoad = true };

            store.Reader.Add(new JsonReader
            {
                Fields =
                {
                    new RecordField("ID"),
                    new RecordField("TemplateName"),
                    new RecordField("SubjectShortName"),
                    new RecordField("BeginDate", RecordFieldType.Date, "Y-m-dTH:i:s.u"),
                    new RecordField("EndDate", RecordFieldType.Date, "Y-m-dTH:i:s.u"),
                    new RecordField("Deadline", RecordFieldType.Date, "Y-m-dTH:i:s.u"),
                    new RecordField("Status"),
                    new RecordField("RefStatus")
                }
            });
            store.DataSource = new List<TaskViewModel> { taskViewModel };
            store.DataBind();
            store.AddScript("taskForm.getForm().loadRecord(dsTask.getAt(0));");

            // Сброс признака isDirty() после инициализации полей
            store.AddScript("resetDirtyAttributeOnFormItems();");
            return store;
        }

        private Store GetProtocolStore()
        {
            Store store = new Store { ID = "dsProtocol", AutoLoad = true };
            store.SetHttpProxy("/ConsTask/GetProtocolTable");
            store.BaseParams.Add(new Parameter("taskId", TaskId.ToString(), ParameterMode.Value));
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("RowType"),
                    new RecordField("ChangeDate", RecordFieldType.Date, "Y-m-dTH:i:s.u"),
                    new RecordField("ChangeUser"),
                    new RecordField("Commentary"),
                    new RecordField { Name = "ProtocolDetail", IsComplex = true },
                }
            });

            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке протокола', e.message || response.statusText);";

            store.AddScript("taskForm.getForm().loadRecord(dsProtocol);");
            return store;
        }

        #endregion Store Initialization

        private IEnumerable<Component> CreateTaskPanel(TaskPermisionSettings taskPermisionSettings, FileListPanel fileListPanel)
        {
            var form = new FormPanel
            {
                ID = "taskForm",
                Width = 400,
                LoadMask = { ShowMask = true },
                Padding = 5
            };

            form.Items.Add(new TextField { ID = "taskId", DataIndex = "ID", Hidden = true, Value = TaskId });
            form.Items.Add(new TextField { ID = "refStatusId", DataIndex = "RefStatus", Hidden = true });
            form.Items.Add(new DisplayField { DataIndex = "SubjectShortName", FieldLabel = "Субъект", ReadOnly = true });
            form.Items.Add(new DisplayField { DataIndex = "TemplateName", FieldLabel = "Имя шаблона", ReadOnly = true });
            form.Items.Add(new DisplayField { ID = "StatusName", DataIndex = "Status", FieldLabel = "Состояние", ReadOnly = true });
            form.Items.Add(new DateField { ID = "beginDate", DataIndex = "BeginDate", FieldLabel = "Начало периода", ReadOnly = true });
            form.Items.Add(new DateField { ID = "endDate", DataIndex = "EndDate", FieldLabel = "Конец периода", ReadOnly = true });
            form.Items.Add(new DateField { ID = "deadline", DataIndex = "Deadline", FieldLabel = "Дата сдачи", ReadOnly = !taskPermisionSettings.CanChangeDeadline });

            form.Items.Add(new Container
            {
                LabelAlign = LabelAlign.Top,
                Items = 
                {
                    new TextArea
                    {
                        ID = "newComment",
                        FieldLabel = "Комментарий",
                        MaxLength = 255,
                        Width = 300,
                        Height = 100
                    }
                }
            });

            Container container = new Container { Layout = "HBoxLayout", Flex = 1, LayoutConfig = { new HBoxLayoutConfig { Align = HBoxAlign.StretchMax } } };
            form.Items.Add(container);

            container.Items.Add(CreateProtocolPanel(taskPermisionSettings));

            fileListPanel.Plugins.Add(
                new RowExpander
                {
                    Template =
                    {
                        Html =
                            @"
<div style='margin: 5px;color: Gray'>
  Дата создания: {[values.CreateDate.format('d.m.Y H:i:s')]}<br />
  Дата изменения: <tpl if=""values.ChangeDate != null "">{[values.ChangeDate.format('d.m.Y H:i:s')]}</tpl><br />
  Автор: {ChangeUser}
</div>",
                    },
                    ExpandOnDblClick = false
                });
            
            container.Items.Add(fileListPanel);

            return new List<Component> { form };
        }

        private Component CreateProtocolPanel(TaskPermisionSettings taskPermisionSettings)
        {
            GridPanel gridPanelProtocol = new GridPanel
            {
                ID = "gpTaskChangeProtocol",
                StoreID = "dsProtocol",
                Title = "Протокол изменений",
                Height = 400,
                Flex = 1,
                AutoScroll = true,
                Collapsible = true,
                AnimCollapse = true,
                TrackMouseOver = true,
                Icon = Icon.DatabaseTable,
                LoadMask = { ShowMask = true },
                ColumnModel =
                {
                    Columns =
                    {   
                        new Column { Hidden = true, DataIndex = "ID", MenuDisabled = true },
                        new DateColumn { Header = "Дата изменения", DataIndex = "ChangeDate", Width = 100, Format = "d-m-Y H:mm:ss", MenuDisabled = true },
                        new Column { Header = "Автор изменений", DataIndex = "ChangeUser", Width = 100, MenuDisabled = true }
                    }
                },
                SelectionModel = { new RowSelectionModel { ID = "RowSelectionModel1", SingleSelect = true } },
                Plugins = 
                { 
                    new RowExpander
                        {
                            Template = 
                            {
                                Html = @"
<div class='ul' style='margin: 5px;color: Gray'>
    <tpl for=""values.ProtocolDetail"">
        <li style='margin-left: 15px;'>
            <tpl if=""values.AttributeType!=1 "">
               Параметер <b>{Attribute}</b> изменился с <i>{OldValue}</i> на <i>{NewValue}</i>
            </tpl>
            <tpl if=""values.AttributeType == 1 "">
              <tpl if=""values.OldValue == null "">
                Присоединен файл: <i>{NewValue}</i>
              </tpl>
              <tpl if=""values.NewValue == null "">
                Удален файл: <i>{OldValue}</i>
              </tpl>
              <tpl if=""values.NewValue != null && values.OldValue != null "">
                Изменен файл с <i>{OldValue}</i> на <i>{NewValue}</i>
              </tpl>
            </tpl>
        </li>
    </tpl>
</div>
<div style='margin: 5px'>{[this.convertNewLineToBr(values.Commentary)]}</div>
",
                                Functions =
                                {
                                    new JFunction
                                        {
                                            Name = "convertNewLineToBr",
                                            Args = new[] { "param" },
                                            Handler = "if (param===null){return '';}else{return param.replace(/\\n/g, '<br />');}"
                                        }
                                }
                            },
                        }
                },
                ContextMenuID = "menuTaskChangeProtocol"
            };

            gridPanelProtocol.Listeners.RowContextMenu.Handler = "e.preventDefault(); #{menuTaskChangeProtocol}.dataRecord = this.store.getAt(rowIndex);#{menuTaskChangeProtocol}.showAt(e.getXY());";

            GridView gridViewProtocol = new GridView { ID = "GridViewProtocol", ForceFit = true };
            gridViewProtocol.Listeners.Refresh.Handler = "this.grid.getRowExpander().expandAll();";
            gridViewProtocol.Listeners.Refresh.Delay = 500;
            gridPanelProtocol.View.Add(gridViewProtocol);

            // Popup-Меню для удаления записей протокола
            MenuItem menuDelete = new MenuItem { Text = "Удалить запись", Icon = Icon.Delete };
            menuDelete.DirectEvents.Click.Url = "/ConsTask/DeleteProtocolRow";
            menuDelete.DirectEvents.Click.CleanRequest = true;
            menuDelete.DirectEvents.Click.Method = HttpMethod.POST;
            menuDelete.DirectEvents.Click.ExtraParams.Add(new Parameter("protocolId", "#{menuTaskChangeProtocol}.dataRecord.data.ID", ParameterMode.Raw));
            menuDelete.DirectEvents.Click.Success = "successSaveHandler";
            menuDelete.DirectEvents.Click.Failure = "failureSaveHandler";
            gridPanelProtocol.Items.Add(new Menu
            {
                ID = "menuTaskChangeProtocol",
                Items = { menuDelete },
                Width = new Unit(200),
                Visible = taskPermisionSettings.CanEditTask,
            });

            return gridPanelProtocol;
        }

        private FileListPanel GetFileListPanel(TaskPermisionSettings taskPermisionSettings)
        {
            List<FileListPanelColumn> panelColumns = new List<FileListPanelColumn>
            {
                new FileListPanelColumn 
                    { 
                        Column = new Column
                                     {
                                         ColumnID = "fileName",
                                         Header = "Имя файла",
                                         DataIndex = "FileName",
                                         MenuDisabled = true,
                                         Editable = true
                                     },
                        AllowBlank = false,
                        VisibleInDialogWindow = true
                    },
                new FileListPanelColumn
                    {
                        Column = new Column
                                     {
                                         ColumnID = "fileDescription",
                                         Header = "Описание",
                                         DataIndex = "FileDescription",
                                         MenuDisabled = true,
                                         Editable = true
                                     },
                        AllowBlank = true,
                        VisibleInDialogWindow = true
                    }
            };

            List<RecordField> storeFields = new List<RecordField>
                                                {
                                                    new RecordField("FileName"),
                                                    new RecordField("FileDescription"),
                                                    new RecordField("CreateDate", RecordFieldType.Date, "Y-m-dTH:i:s"),
                                                    new RecordField("ChangeDate", RecordFieldType.Date, "Y-m-dTH:i:s"),
                                                    new RecordField("ChangeUser")
                                                };

            UrlWithParameters loadController = new UrlWithParameters
            {
                Url = "/ConsTask/GetFileTable",
                ParameterCollection = new ParameterCollection { new Parameter("taskId", TaskId.ToString(), ParameterMode.Value) }
            };

            UrlWithParameters fileUploadController = new UrlWithParameters
            {
                Url = "/ConsTask/CreateOrUpdateFileWithUploadBody",
                ParameterCollection = new ParameterCollection 
                                            {
                                                new Parameter("taskId", TaskId.ToString(), ParameterMode.Value),
                                                new Parameter("fileName", "fileName.getValue()", ParameterMode.Raw),
                                                new Parameter("fileDescription", "fileDescription.getValue()", ParameterMode.Raw)
                                            }
            };

            UrlWithParameters fileDownloadController = new UrlWithParameters
            {
                Url = "/ConsTask/DownloadFile"
            };

            return new FileListPanel(
                panelColumns,
                taskPermisionSettings.CanEditTask,
                storeFields,
                loadController,
                null,
                fileUploadController,
                fileDownloadController,
                "taskForm");
        }
    }
}
