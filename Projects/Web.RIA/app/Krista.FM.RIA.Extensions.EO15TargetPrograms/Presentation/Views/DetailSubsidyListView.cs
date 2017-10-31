using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Helpers;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Services;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Presentation.Views
{
    public class DetailSubsidyListView : View
    {
        private const string FormId = "formSubsidyList";
        
        private IProgramService programService;

        public DetailSubsidyListView(IProgramService programService)
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
            
            FileListPanel fileListPanel = GetFileListPanel(editable);
            page.Controls.Add(fileListPanel.GetFileListStore());

            var view = new Viewport
            {
                ID = "viewportMain",
                Items = 
                {
                   new FormPanel { ID = FormId, Layout = LayoutType.Fit.ToString(), Items = { fileListPanel }, Border = false, AutoHeight = true }
                }
            };

            return new List<Component> { view, fileListPanel.GetFileUploadWindow() };
        }

        private FileListPanel GetFileListPanel(bool editable)
        {
            List<FileListPanelColumn> panelColumns = new List<FileListPanelColumn>
            {
                new FileListPanelColumn 
                    { 
                        Column = new Column
                                     {
                                         ColumnID = "SubsidyName",
                                         Header = "Наименование субсидии",
                                         DataIndex = "SubsidyName",
                                         Width = 300,
                                         MenuDisabled = true,
                                         Editable = true
                                     },
                        AllowBlank = false,
                        VisibleInDialogWindow = true
                    },
            };

            List<RecordField> storeFields = new List<RecordField>
                                                {
                                                    new RecordField("ID"),
                                                    new RecordField("SubsidyName"),
                                                };

            UrlWithParameters loadController = new UrlWithParameters
            {
                Url = "/DetailSubsidyList/GetSubsidyListTable",
                ParameterCollection = new ParameterCollection { new Parameter("programId", ProgramId.ToString(), ParameterMode.Value) }
            };

            UrlWithParameters fileUploadController = new UrlWithParameters
            {
                Url = "/DetailSubsidyList/CreateOrUpdateFileWithUploadBody",
                ParameterCollection = new ParameterCollection { new Parameter("programId", ProgramId.ToString(), ParameterMode.Value) }
            };

            UrlWithParameters fileDownloadController = new UrlWithParameters
            {
                Url = "/DetailSubsidyList/DownloadFile"
            };

            UrlWithParameters updateController = new UrlWithParameters
            {
                Url = "/DetailSubsidyList/Save",
                ParameterCollection = new ParameterCollection { new Parameter("programId", ProgramId.ToString(), ParameterMode.Value) }
            };

            var fileListPanel = new FileListPanel(
                panelColumns,
                editable,
                storeFields,
                loadController,
                updateController,
                fileUploadController,
                fileDownloadController,
                FormId);

            fileListPanel.Title = null;
            fileListPanel.Collapsible = false;
            fileListPanel.Border = false;

            var toolbar = fileListPanel.TopBar[0];

            // Допиливаем существующий функционал у Toolbar-а
            Button btnAdd = (Button)toolbar.Items[0];
            btnAdd.Text = null;

            var btnRefresh = new Button
            {
                ID = "btnRefresh",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Visible = true,
                Listeners = { Click = { Handler = "{0}.reload();".FormatWith(FileListPanel.StoreId) } }
            };
            toolbar.Items.Insert(0, btnRefresh);

            toolbar.Add(new Button
            {
                ID = "btnSave",
                Icon = Icon.TableSave,
                ToolTip = "Сохранить",
                Visible = editable,
                Listeners =
                {
                    Click =
                    {
                        Handler = "{0}.save();".FormatWith(FileListPanel.StoreId)
                    }
                }
            });

            return fileListPanel;
        }
    }
}
