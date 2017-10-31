using System.Collections.Generic;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controls
{
    public class DetailFileListControl
    {
        public static FileListPanel Get(bool editable, int applicationId)
        {
            var panelColumns = new List<FileListPanelColumn>
            {
                new FileListPanelColumn 
                    { 
                        Column = new Column
                                     {
                                         ColumnID = "fileName",
                                         Header = "Имя файла",
                                         DataIndex = "Name",
                                         MenuDisabled = true,
                                         Editable = editable
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
                                         DataIndex = "Note",
                                         MenuDisabled = true,
                                         Editable = editable
                                     },
                        AllowBlank = false,
                        VisibleInDialogWindow = true
                    },

                new FileListPanelColumn
                    {
                        Column = new Column
                                     {
                                         ColumnID = "changeUser",
                                         Header = "Кто добавил",
                                         DataIndex = "Executor",
                                         MenuDisabled = true,
                                         Editable = false
                                     },
                        AllowBlank = false,
                        VisibleInDialogWindow = false
                    },

                new FileListPanelColumn
                    {
                        Column = new Column
                                     {
                                         ColumnID = "fileDate",
                                         Header = "Дата",
                                         DataIndex = "DateDoc",
                                         MenuDisabled = true,
                                         Editable = false
                                     },
                        AllowBlank = true,
                        VisibleInDialogWindow = false
                    }
            };

            var storeFields = new List<RecordField>
                                                {
                                                    new RecordField("Name"),
                                                    new RecordField("Note"),
                                                    new RecordField("DateDoc"),
                                                    new RecordField("Executor")
                                                };
            var loadController = new UrlWithParameters
            {
                Url = "/UploadFiles/GetFiles",
                ParameterCollection = new ParameterCollection
                                          {
                                              new Parameter(
                                                  "applicationId", 
                                                  applicationId.ToString(), 
                                                  ParameterMode.Value)
                                          }
            };

            var fileUploadController = new UrlWithParameters
            {
                Url = "/UploadFiles/UploadFile",
                ParameterCollection = new ParameterCollection 
                                            {
                                                new Parameter(
                                                    "taskId", 
                                                    applicationId.ToString(), 
                                                    ParameterMode.Value),
                                            }
            };

            var fileDownloadController = new UrlWithParameters
            {
                Url = "/UploadFiles/DownloadFile"
            };

            var fileUpdateController = new UrlWithParameters
            {
                Url = "/UploadFiles/SaveFiles",
                ParameterCollection = new ParameterCollection 
                                            {
                                                new Parameter(
                                                    "taskId", 
                                                    applicationId.ToString(), 
                                                    ParameterMode.Value),
                                            }
            };

            return new FileListPanel(
                panelColumns,
                editable,
                storeFields,
                loadController,
                fileUpdateController,
                fileUploadController,
                fileDownloadController,
                "DetailsForm",
                true);
        }
    }
}
