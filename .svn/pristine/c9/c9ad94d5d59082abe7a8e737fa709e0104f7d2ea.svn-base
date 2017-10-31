using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot
{
    public class FormView : View
    {
        private readonly ITaskService taskService;
        private readonly IUserSessionState sessionState;

        public FormView(ITaskService taskService, IUserSessionState sessionState)
        {
            this.taskService = taskService;
            this.sessionState = sessionState;
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
            TaskViewModel taskViewModel = taskService.GetTaskViewModel(TaskId);

            // Регистрируем событие на закрытие вкладки
            ResourceManager.GetInstance(page).AddScript(@"
 var tab = parent.MdiTab.getComponent('consReport_' + {0});
 tab.addListener('beforeclose', onTabCloseEventHandler);".FormatWith(TaskId));

            TaskPermisionSettings taskPermisionSettings = new TaskPermisionSettings(sessionState, taskViewModel);

            if (!ExtNet.IsAjaxRequest)
            {
                RegisterResources(page);
            }

            FormPanel infoPanel = new FormPanel
            {
                Border = false,
                Layout = "form",
                Height = 100,
                LabelWidth = 1,
                Padding = 5,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
            };

            infoPanel.TopBar.Add(CreateToolbar(taskViewModel, taskPermisionSettings));

            CreateTopInfoPanel(infoPanel);

            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };
            layout.North.Items.Add(infoPanel);
            
            Panel centerPanel = new Panel { Border = false, AutoScroll = true };
            centerPanel.Items.Add(CreateGrid(page, taskPermisionSettings));
            centerPanel.Items.Add(CreateGridJobTitle(page, taskPermisionSettings));

            layout.Center.Items.Add(centerPanel);

            Viewport viewport = new Viewport { ID = "viewportMain", Layout = "Center" };
            viewport.Items.Add(layout);

            var toolTip = new ToolTip
            {
                ID = "RowTip",
                Target = "={grid.getView().mainBody}",
                Delegate = ".exclamation-cell",
                TrackMouse = true,
                Listeners = { Show = { Fn = "showCellErrorTip" } }
            };

            return new List<Component> { viewport, toolTip };
        }

        private static void RegisterResources(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);

            resourceManager.RegisterClientStyleBlock("CustomStyle", Resource.FormViewCss);

            resourceManager.RegisterScript("Hack", "/Content/js/Ext.util.Format.number.Hack.js");

            resourceManager.RegisterClientScriptBlock("MofoWebFormSogl", Resource.FormViewJs);
        }

        private Button CreateReportButton(string reportName, string reportCaption)
        {
            return new Button
            {
                ID = String.Format("export{0}Button", reportName),
                Icon = Icon.PageExcel,
                ToolTip = reportCaption,
                DirectEvents =
                {
                    Click =
                    {
                        Url = String.Format("/ExportMrot/{0}", reportName),
                        IsUpload = true,
                        CleanRequest = true,
                        ExtraParams = { new Parameter("taskId", Convert.ToString(TaskId), ParameterMode.Value) }
                    }
                }
            };            
        }

        private Toolbar CreateToolbar(TaskViewModel taskViewModel, TaskPermisionSettings taskPermisionSettings)
        {
            Toolbar toolbar = new Toolbar { ID = "toolbar" };

            Button refreshButton = new Button
            {
                ID = "refreshButton",
                Icon = Icon.PageRefresh,
                ToolTip = "Обновить"
            };
            refreshButton.Listeners.Click.Handler = "store.reload();";
            toolbar.Add(refreshButton);
            
            var exportFormCollection = CreateReportButton(
                "FormCollection", "Выгрузка в Excel формы сбора с данными");

            toolbar.Add(exportFormCollection);

            toolbar.Add(new ToolbarSeparator());
            
            toolbar.Add(new Button
            {
                ID = "btnSave",
                Icon = Icon.Disk,
                Text = "Сохранить изменения",
                Listeners = { Click = { Handler = "saveReport();" } },
                Hidden = !taskPermisionSettings.CanEditTask
            });
            
            // Кнопки управления состоянием
            toolbar.Add(new ToolbarSeparator());
            toolbar.Add(new Button
            {
                ID = "btnToTest",
                Icon = Icon.TimeGo,
                Text = "Отправить на рассмотрение",
                Listeners = { Click = { Handler = "changeState({0});".FormatWith((int)TaskViewModel.TaskStatus.OnTest) } },
                Hidden = !(taskViewModel.RefStatus == (int)TaskViewModel.TaskStatus.Edit && taskPermisionSettings.CanEditTask)
            });
            toolbar.Add(new Button
            {
                ID = "btnReject",
                Icon = Icon.Stop,
                Text = "Отправить на доработку",
                Listeners = { Click = { Handler = "changeState({0});".FormatWith((int)TaskViewModel.TaskStatus.Edit) } },
                Hidden = !(taskPermisionSettings.CanSetVise && taskViewModel.RefStatus == (int)TaskViewModel.TaskStatus.OnTest)
            });
            toolbar.Add(new Button
            {
                ID = "btnAccept",
                Icon = Icon.Tick,
                Text = "Утвердить",
                Listeners = { Click = { Handler = "changeState({0});".FormatWith((int)TaskViewModel.TaskStatus.Accepted) } },
                Hidden = !(taskPermisionSettings.CanSetVise && taskViewModel.RefStatus == (int)TaskViewModel.TaskStatus.OnTest)
            });

            return toolbar;
        }

        private Store CreateStore()
        {
            Store store = new Store
            {
                ID = "store",
                DirtyWarningTitle = "Несохраненные изменения",
                DirtyWarningText = "Есть несохраненные данные. Вы уверены, что хотите обновить?",
            };

            JsonReader reader = new JsonReader { IDProperty = "Code", Root = "data", TotalProperty = "total" };
            reader.Fields.Add("Code");
            reader.Fields.Add("CodeStr");
            reader.Fields.Add("Name");
            reader.Fields.Add("OrgType1");
            reader.Fields.Add("OrgType2");
            reader.Fields.Add("OrgType3");
            reader.Fields.Add("IsEditable");
            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/MrotForm/Load",
                Method = HttpMethod.POST,
            });

            store.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/MrotForm/Save",
                Method = HttpMethod.POST,
                Timeout = 500000
            });

            store.BaseParams.Add(new Parameter("taskId", Convert.ToString(TaskId), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("taskId", Convert.ToString(TaskId), ParameterMode.Value));

            store.Listeners.BeforeSave.Fn = "validateForm";
            store.Listeners.Load.Handler = "validateStore(store, {});";

            return store;
        }

        private GridPanel CreateGrid(ViewPage page, TaskPermisionSettings taskPermisionSettings)
        {
            page.Controls.Add(CreateStore());

            GridPanel gp = new GridPanel
            {
                ID = "grid",
                StoreID = "store",
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                AutoHeight = true,
                ColumnLines = true,
                LoadMask = { ShowMask = true },
                SaveMask = { ShowMask = true }
            };

            var groupRow = new HeaderGroupRow();
            groupRow.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 3 });
            groupRow.Columns.Add(new HeaderGroupColumn { Header = "Из них:", ColSpan = 2, Align = Alignment.Center });

            gp.View.Add(new Ext.Net.GridView { HeaderGroupRows = { groupRow } });

            gp.ColumnModel.AddColumn("CodeStr", "№ п.п.", DataAttributeTypes.dtString)
                .SetWidth(35).Renderer.Fn = "columnNameRender()";

            gp.ColumnModel.AddColumn("Name", "Наименование", DataAttributeTypes.dtString)
                .SetWidth(300).Renderer.Fn = "columnNameRender()";

            gp.ColumnModel.AddColumn("OrgType1", "Всего по муниципальному образованию", DataAttributeTypes.dtString)
                .SetEditableDouble(2).SetNullable().SetWrap(true).Renderer.Fn = "columnOrgType1Render()";

            gp.ColumnModel.AddColumn("OrgType2", "Крупные и средние организации", DataAttributeTypes.dtString)
                .SetEditableDouble(2).SetNullable().SetWrap(true).Renderer.Fn = "columnOrgType2Render()";

            gp.ColumnModel.AddColumn("OrgType3", "Субъекты малого предпринимательства", DataAttributeTypes.dtString)
                .SetEditableDouble(2).SetNullable().SetWrap(true).Renderer.Fn = "columnOrgType2Render()";

            gp.AddColumnsWrapStylesToPage(page);

            gp.SelectionModel.Add(new ExcelLikeSelectionModel());

            gp.Listeners.BeforeEdit.Fn = "beforeEditCell";
            gp.Listeners.AfterEdit.Fn = "afterEditCell";

            gp.SetReadonly(!taskPermisionSettings.CanEditTask);

            return gp;
        }

        private Store CreateStoreJobTitle()
        {
            Store store = new Store
            {
                ID = "dsJobTitle",
                Restful = true,
                DirtyWarningTitle = "Несохраненные изменения",
                DirtyWarningText = "Есть несохраненные данные. Вы уверены, что хотите обновить?",
            };

            store.SetRestController("JobTitle")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name")
                .AddField("Office")
                .AddField("Phone")
                .AddField("RefReport");

            store.BaseParams.Add(new Parameter("taskId", Convert.ToString(TaskId), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("taskId", Convert.ToString(TaskId), ParameterMode.Value));
            store.Listeners.Exception.Fn = "exceptionHandler";

            return store;
        }

        private GridPanel CreateGridJobTitle(ViewPage page, TaskPermisionSettings taskPermisionSettings)
        {
            page.Controls.Add(CreateStoreJobTitle());

            GridPanel gp = new GridPanel
            {
                ID = "gpJobTitle",
                StoreID = "dsJobTitle",
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                AutoHeight = true,
                ColumnLines = true,
                LoadMask = { ShowMask = true },
                SaveMask = { ShowMask = true }
            };

            gp.ColumnModel.AddColumn("Name", "ФИО", DataAttributeTypes.dtString).SetWidth(150).SetEditableString();
            gp.ColumnModel.AddColumn("Office", "Должность", DataAttributeTypes.dtString).SetWidth(150).SetEditableString();
            gp.ColumnModel.AddColumn("Phone", "Телефон", DataAttributeTypes.dtString).SetWidth(100).SetEditableString();

            gp.AddColumnsWrapStylesToPage(page);

            gp.SelectionModel.Add(new ExcelLikeSelectionModel());

            gp.SetReadonly(!taskPermisionSettings.CanEditTask);

            if (taskPermisionSettings.CanEditTask)
            {
                gp.AddNewRecordButton();
                gp.AddRemoveRecordButton();
            }

            return gp;
        }

        private void CreateTopInfoPanel(FormPanel panel)
        {
            var task = taskService.GetTaskViewModel(TaskId);

            panel.Items.Add(new TextField { ID = "taskId", Hidden = true, Value = task.ID });
    
            Label label = new Label();
            label.Html = "<b>{0}</b>.".FormatWith(task.TemplateName);
            panel.Items.Add(label);

            label = new Label();
            label.Html = "по муниципальному образованию: {0}".FormatWith(task.Region.Name);
            panel.Items.Add(label);

            label = new Label();
            label.Html = "за: {0} квартал {1} г.".FormatWith(Convert.ToInt32(Math.Round(task.BeginDate.Month / (float)4, MidpointRounding.AwayFromZero) + 1), task.BeginDate.Year);
            panel.Items.Add(label);
        }
    }
}
