using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Progress;
using Krista.FM.RIA.Extensions.Consolidation.Data;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Consolidation.Presentation.Views
{
    public class CollectingTasksView : View
    {
        private readonly ICollectingPeriodRepository periodRepository;
        private readonly IUserSessionState userState;

        public CollectingTasksView(ICollectingPeriodRepository periodRepository, IUserSessionState userState)
        {
            this.periodRepository = periodRepository;
            this.userState = userState;
        }

        public override List<Component> Build(ViewPage page)
        {
            if (!ExtNet.IsAjaxRequest)
            {
                var resourceManager = ResourceManager.GetInstance(page);
                resourceManager.RegisterClientScriptBlock("CollectingTasksView", Resource.CollectingTasksView);
            }

            page.Controls.Add(CreateStore());
            page.Controls.Add(CreateNewTaskWindow());

            Window uploadWindow = GetFileListPanel().GetFileUploadWindow();
            page.Controls.Add(uploadWindow);

            return new List<Component>
            {
                new Viewport
                {
                    ID = "viewportMain",
                    Items = 
                    {
                        new BorderLayout
                        {
                            Center = { Items = { CreateGridPanel(page) } }
                        }
                    }
                }
            };
        }

        private Store CreateStore()
        {
            var ds = new Store
            {
                ID = "dsTasks",
                Restful = true,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None
            };

            ds.SetHttpProxy("/ConsCollectingTasks/Load")
                .SetJsonReader()
                .AddField("Id")
                .AddField("Date")
                .AddField("ProvideDate")
                .AddField("PeriodId")
                .AddField("AuthorId")
                .AddField("PeriodName")
                .AddField("AuthorName");

            ds.Listeners.Exception.Handler =
                @"
                    Ext.net.Notification.show({
                        iconCls    : 'icon-exclamation', 
                        html       : e && e.message ? e.message : response.message || response.statusText, 
                        title      : 'EXCEPTION', 
                        hideDelay  : 5000
                    });";

            ds.Listeners.Save.Handler =
                @" Ext.net.Notification.show({
                        iconCls    : 'icon-information', 
                        html       : arg.message, 
                        title      : 'Success', 
                        hideDelay  : 5000
                    });";

            return ds;
        }

        private IEnumerable<Component> CreateGridPanel(ViewPage page)
        {
            var gp = new GridPanel
            {
                ID = "gpTasks",
                Icon = Icon.Table,
                StoreID = "dsTasks",
                Border = false,
                View = { new Ext.Net.GridView { ForceFit = false } },
            };

            gp.ColumnModel.AddColumn("Date", "Конец периода сбора", DataAttributeTypes.dtDate);

            gp.ColumnModel.AddColumn("ProvideDate", "Дата сдачи", DataAttributeTypes.dtDate);

            gp.ColumnModel.AddColumn("PeriodName", "За период", DataAttributeTypes.dtString)
                .SetWidth(250);

            gp.ColumnModel.AddColumn("AuthorName", "Автор", DataAttributeTypes.dtString)
                .SetWidth(200);

            gp.AddColumnsWrapStylesToPage(page);

            gp.AddRefreshButton();

            gp.Toolbar()
                .AddIconButton("btnCreate", Icon.Add, "Новая задача сбора отчетности", "window.newTaskWindow.show();");

            gp.Toolbar()
                .AddIconButton("btnDoPump", Icon.DatabaseYellowStart, "Передать данные для многомерного анализа", "doPump();")
                .Text = "Передать данные для многомерного анализа";

            var btn = gp.Toolbar()
                .AddIconButton("btnExport", Icon.DiskDownload, "Экспорт утвержденных отчетов", String.Empty);
            btn.DirectEvents.Click.Url = "/ConsCollectingTasks/Export";
            btn.DirectEvents.Click.IsUpload = true;
            btn.DirectEvents.Click.CleanRequest = true;
            btn.DirectEvents.Click.Timeout = 60 * 60 * 1000;
            btn.DirectEvents.Click.FormID = "Form1";
            btn.DirectEvents.Click.ExtraParams.Add(new Parameter("taskId", "gpTasks.getSelectionModel().getSelectedCell() == null ? null : dsTasks.getAt(gpTasks.getSelectionModel().getSelectedCell()[0]).get('Id')", ParameterMode.Raw));
            btn.DirectEvents.Click.Before = "var sel = gpTasks.getSelectionModel().getSelectedCell(); if (!sel) { Ext.Msg.alert('Уведомление', 'Необходимо выбрать задачу.'); } return sel != null;";
            btn.DirectEvents.Click.ExtraParams.Add(new ProgressConfig("Экспорт утвержденных отчетов..."));

            gp.Toolbar()
                .AddIconButton("btnImport", Icon.DiskDownload, "Импорт отчетов", "wUploadFileDialog.documentParameters = {fileName: 'Форма'}; wUploadFileDialog.show();");

            /*gp.Toolbar()
                .AddIconButton("btnDelete", Icon.Delete, "Удалить задачу сбора отчетности", "window.newTaskWindow.show();");*/

            return new Collection<Component> { gp };
        }

        private Window CreateNewTaskWindow()
        {
            var periodComboBox = new ComboBox
            {
                ID = "periodId1",
                ValueField = "value",
                HiddenName = "periodId",
                FieldLabel = "За период",
                AutoWidth = true,
                Editable = false,
                EmptyText = "Выберите период...",
            };
            periodComboBox.Items.AddRange(FillPeriods());

            var subjectComboBox = new ComboBox
            {
                ID = "authorId1",
                ValueField = "value",
                HiddenName = "authorId",
                FieldLabel = "Для субъекта",
                AutoWidth = true,
                Editable = false,
                EmptyText = "Выберите субъект...",
            };
            subjectComboBox.Items.AddRange(FillSubjects());

            var wnd = new Window
            {
                ID = "newTaskWindow",
                AutoShow = true,
                Hidden = true,
                Title = "Создание задачи сбора отчетности",
                Width = 500,
                Height = 250,
                Items =
                {
                    new FormPanel
                    {
                        ID = "newTaskForm",
                        Url = "/ConsCollectingTasks/Create",
                        Method = HttpMethod.POST,
                        Border = false,
                        Padding = 5,
                        BodyStyle = "background-color:transparent",
                        Layout = "form",
                        Timeout = 10 * 60 * 1000,
                        Items =
                        {
                            new DateField
                            {
                                ID = "date",
                                FieldLabel = "За период"
                            },
                            new DateField
                            {
                                ID = "provideDate",
                                FieldLabel = "Дата сдачи"
                            },
                            periodComboBox, 
                            subjectComboBox
                        },
                        BaseParams = { new ProgressConfig("Создание задач...") }
                    }
                },
                Buttons =
                {
                    new Button
                    {
                        Text = "Создать",
                        Listeners =
                        {
                            Click =
                            {
                                Handler = "newTaskWindowSubmit();"
                            }
                        }
                    },
                    new Button
                    {
                        Text = "Отмена",
                        Listeners =
                        {
                            Click =
                            {
                                Handler = "newTaskForm.getForm().reset();newTaskWindow.hide();"
                            }
                        }
                    }
                }
            };
            
            return wnd;
        }

        private IEnumerable<ListItem> FillPeriods()
        {
            var items = new Collection<ListItem>();
            foreach (var period in periodRepository.GetAllPeriods())
            {
                items.Add(new ListItem(period.Kind + ", " + period.Name, Convert.ToString(period.Id)));
            }

            return items;
        }

        private IEnumerable<ListItem> FillSubjects()
        {
            var items = new Collection<ListItem>();
            foreach (var subjet in userState.Subjects)
            {
                items.Add(new ListItem(subjet.Name, Convert.ToString(subjet.ID)));
            }

            return items;
        }

        private FileListPanel GetFileListPanel()
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
                ParameterCollection = new ParameterCollection()
            };

            UrlWithParameters fileUploadController = new UrlWithParameters
            {
                Url = "/ConsCollectingTasks/Import",
                ParameterCollection = new ParameterCollection
                {
                    new Parameter("taskId", "dsTasks.getAt(gpTasks.getSelectionModel().getSelectedCell()[0]).get('Id')", ParameterMode.Raw),
                    new ProgressConfig("Импорт отчетов...")
                }
            };

            UrlWithParameters fileDownloadController = new UrlWithParameters
            {
                Url = "/ConsCollectingTasks/Export",
            };

            var fileListPanel = new FileListPanel(panelColumns, false, storeFields, loadController, null, fileUploadController, fileDownloadController, "taskForm");
            return fileListPanel;
        }
    }
}
