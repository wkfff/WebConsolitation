using System.Collections.Generic;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controls
{
    public class DetailFileListControl
    {
        public static FileListPanel Get(bool editable, bool allowDelete, int objId)
        {
            var panelColumns = new List<FileListPanelColumn>
            {
                new FileListPanelColumn 
                    { 
                        Column = new Column
                                     {
                                         ColumnID = "fileName",
                                         Header = "Документ",
                                         DataIndex = "Name",
                                         MenuDisabled = true,
                                         Editable = editable
                                     },
                        AllowBlank = false,
                        VisibleInDialogWindow = true
                    },
            };

            var storeFields = new List<RecordField>
                                                {
                                                    new RecordField("Name"),
                                                    new RecordField("CObjectId")
                                                };
            var loadController = new UrlWithParameters
            {
                Url = "/EO15AIPUploadFiles/GetFiles",
                ParameterCollection = new ParameterCollection
                                          {
                                              new Parameter(
                                                  "objId", 
                                                  objId.ToString(), 
                                                  ParameterMode.Value)
                                          }
            };

            var fileUploadController = new UrlWithParameters
            {
                Url = "/EO15AIPUploadFiles/UploadFile",
                ParameterCollection = new ParameterCollection 
                                            {
                                                new Parameter(
                                                    "objId", 
                                                    objId.ToString(), 
                                                    ParameterMode.Value),
                                            }
            };

            var fileDownloadController = new UrlWithParameters
            {
                Url = "/EO15AIPUploadFiles/DownloadFile"
            };

            var fileUpdateController = new UrlWithParameters
            {
                Url = "/EO15AIPUploadFiles/SaveFiles",
                ParameterCollection = new ParameterCollection 
                                            {
                                                new Parameter(
                                                    "objId", 
                                                    objId.ToString(), 
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
                "CObjectCardForm",
                true,
                allowDelete);
        }
    }
}
