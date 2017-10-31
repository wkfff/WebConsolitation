using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class FormsView : View
    {
        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreateStore());

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

        private static Store CreateStore()
        {
            var ds = new Store
                         {
                             ID = "dsForms",
                             Restful = true,
                             ShowWarningOnFailure = false,
                             SkipIdForNewRecords = false,
                             RefreshAfterSaving = RefreshAfterSavingMode.None
                         };

            ds.SetRestController("ConsForms")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name")
                .AddField("ShortName")
                .AddField("NameCD")
                .AddField("Code")
                .AddField("InternalName")
                .AddField("FormVersion", new RecordField.Config { DefaultValue = "1" })
                .AddField("Status");

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

        private static IEnumerable<Component> CreateGridPanel(ViewPage page)
        {
            var gp = new GridPanel
            {
                ID = "gpForms",
                Icon = Icon.Table,
                Frame = true,
                Title = "Шаблоны форм",
                StoreID = "dsForms",
                SelectionModel = { new ExcelLikeSelectionModel() },
                View = { new Ext.Net.GridView { ForceFit = false } },
            };

            gp.ColumnModel.AddColumn("Name", "Наименование", DataAttributeTypes.dtString)
                .SetWidth(250).SetEditableString();

            gp.ColumnModel.AddColumn("ShortName", "Краткое наименование", DataAttributeTypes.dtString)
                .SetEditableString();

            gp.ColumnModel.AddColumn("NameCD", "Наименование сбора", DataAttributeTypes.dtString)
                .SetWidth(200).SetEditableString();

            gp.ColumnModel.AddColumn("Code", "Код", DataAttributeTypes.dtString)
                .SetWidth(100).SetEditableString();

            gp.ColumnModel.AddColumn("InternalName", "Внутреннее имя", DataAttributeTypes.dtString)
                .SetWidth(100).SetEditableString();

            gp.ColumnModel.AddColumn("FormVersion", "Версия", DataAttributeTypes.dtInteger)
                .SetEditable(false).SetWidth(50);

            gp.ColumnModel.AddColumn("Status", "Статус", DataAttributeTypes.dtInteger)
                .SetEditable(false).SetWidth(50);

            gp.AddColumnsWrapStylesToPage(page);

            gp.AddRefreshButton();
            gp.AddSaveButton();
            gp.AddNewRecordButton();
            gp.AddRemoveRecordButton();

            var btn = gp.Toolbar()
                .AddIconButton("btnExport", Icon.DiskDownload, "Экспорт формы", String.Empty);
            btn.DirectEvents.Click.Url = "/ConsForm/Export";
            btn.DirectEvents.Click.IsUpload = true;
            btn.DirectEvents.Click.CleanRequest = true;
            btn.DirectEvents.Click.FormID = "Form1";
            btn.DirectEvents.Click.ExtraParams.Add(new Parameter("formId", "gpForms.getSelectionModel().selection == null ? null : gpForms.getSelectionModel().selection.record.id", ParameterMode.Raw));
            btn.DirectEvents.Click.Before = "var sel = gpForms.getSelectionModel().selection; if (!sel) { alert('234234134'); } return sel != null;";

            gp.Toolbar()
                .AddIconButton("btnImport", Icon.DiskDownload, "Импорт формы", "wUploadFileDialog.documentParameters = {fileName: 'Форма'}; wUploadFileDialog.show();");

            btn = gp.Toolbar()
                .AddIconButton("btnValidate", Icon.ScriptError, "Проверить форму на наличие ошибок", String.Empty);
            btn.DirectEvents.Click.Url = "/ConsFormActivator/Validate";
            btn.DirectEvents.Click.CleanRequest = true;
            btn.DirectEvents.Click.FormID = "Form1";
            btn.DirectEvents.Click.ExtraParams.Add(new Parameter("formId", "gpForms.getSelectionModel().selection == null ? null : gpForms.getSelectionModel().selection.record.id", ParameterMode.Raw));
            btn.DirectEvents.Click.Before = "var sel = gpForms.getSelectionModel().selection; if (!sel) { alert('234234134'); } return sel != null;";
            btn.DirectEvents.Click.EventMask.ShowMask = true;
            btn.DirectEvents.Click.EventMask.Msg = "Проверка формы...";

            btn = gp.Toolbar()
                .AddIconButton("btnActivate", Icon.ScriptLightning, "Активировать форму", String.Empty);
            btn.DirectEvents.Click.Url = "/ConsFormActivator/Activate";
            btn.DirectEvents.Click.CleanRequest = true;
            btn.DirectEvents.Click.FormID = "Form1";
            btn.DirectEvents.Click.ExtraParams.Add(new Parameter("formId", "gpForms.getSelectionModel().selection == null ? null : gpForms.getSelectionModel().selection.record.id", ParameterMode.Raw));
            btn.DirectEvents.Click.Before = "var sel = gpForms.getSelectionModel().selection; if (!sel) { alert('234234134'); } return sel != null;";
            btn.DirectEvents.Click.EventMask.ShowMask = true;
            btn.DirectEvents.Click.EventMask.Msg = "Активация формы...";
            btn.DirectEvents.Click.Timeout = 3 * 60 * 1000;

            return new List<Component> { gp };
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
                Url = "/ConsForm/Import",
                ParameterCollection = new ParameterCollection()
            };

            UrlWithParameters fileDownloadController = new UrlWithParameters
            {
                Url = "/ConsForm/Export",
            };

            return new FileListPanel(
                panelColumns,
                false,
                storeFields,
                loadController,
                null,
                fileUploadController,
                fileDownloadController,
                "taskForm");
        }
    }
}
