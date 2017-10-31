using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Services;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Presentation.Views
{
    public class VisualizationView : View
    {
        private const string PanelId = "fileListPanelId";
        private readonly IProjectService projectService;

        public VisualizationView(IProjectService projectService)
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

            FileListPanel fileListPanel = GetFileListPanel(projectStatus == InvProjStatus.Edit);

            page.Controls.Add(fileListPanel.GetFileListStore());

            var view = new Viewport
            {
                ID = "viewportMain",
                Items = 
                {
                    new FitLayout
                    {   
                        Items = { new FormPanel { ID = PanelId, Items = { fileListPanel }, Border = false } }
                    }
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
                                         ColumnID = "fileType",
                                         Header = "Тип файла",
                                         DataIndex = "FileType",
                                         Width = 200,
                                         MenuDisabled = true,
                                         Editable = true
                                     },
                        AllowBlank = false,
                        VisibleInDialogWindow = false
                    },
                new FileListPanelColumn 
                    { 
                        Column = new Column
                                     {
                                         ColumnID = "fileName",
                                         Header = "Имя файла",
                                         DataIndex = "FileName",
                                         Width = 200,
                                         MenuDisabled = true,
                                         Editable = true
                                     },
                        AllowBlank = false,
                        VisibleInDialogWindow = false
                    }
            };

            List<RecordField> storeFields = new List<RecordField>
                                                {
                                                    new RecordField("ID"),
                                                    new RecordField("FileType"),
                                                    new RecordField("FileName")
                                                };

            UrlWithParameters loadController = new UrlWithParameters
            {
                Url = "/Visualization/GetFileTable",
                ParameterCollection = new ParameterCollection { new Parameter("refProjId", ProjId.ToString(), ParameterMode.Value) }
            };

            UrlWithParameters fileUploadController = new UrlWithParameters
            {
                Url = "/Visualization/CreateOrUpdateFileWithUploadBody",
                ParameterCollection = new ParameterCollection { new Parameter("refProjId", ProjId.ToString(), ParameterMode.Value) }
            };

            UrlWithParameters fileDownloadController = new UrlWithParameters
            {
                Url = "/Visualization/DownloadFile"
            };

            UrlWithParameters updateController = new UrlWithParameters
            {
                Url = "/Visualization/Save",
                ParameterCollection = new ParameterCollection { new Parameter("refProjId", ProjId.ToString(), ParameterMode.Value) }
            };
            
            var fileListPanel = new FileListPanel(
                panelColumns,
                editable,
                storeFields,
                loadController,
                updateController,
                fileUploadController,
                fileDownloadController,
                PanelId);

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
                             Visible = editable,
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
